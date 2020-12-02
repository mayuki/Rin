using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rin.Channel.HubInvoker
{
    public interface IHubInvoker
    {
        IReadOnlyDictionary<string, HubMethodDefinition> MethodMap { get; }
        ValueTask<HubInvocationResult> InvokeAsync(object instance, HubInvokeMessage invokeMessage);
        bool TryCreateMessage(string json, [NotNullWhen(true)] out HubInvokeMessage? invokeMessage);
    }

    /// <summary>
    /// Provides a mechanism for invoking Hub methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HubInvoker<T> : IHubInvoker
        where T : notnull
    {
        private static readonly Dictionary<string, HubMethodDefinition> _methodMap;
        private static readonly Dictionary<string, Func<object, HubInvokeMessage, ValueTask<HubInvocationResult>>> _methodInvoker;

        public IReadOnlyDictionary<string, HubMethodDefinition> MethodMap => _methodMap;

        static HubInvoker()
        {
            _methodMap = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.DeclaringType == typeof(T))
                .Select(x => new HubMethodDefinition(x))
                .ToDictionary(x => x.Method.Name);

            _methodInvoker = _methodMap.ToDictionary(k => k.Key, v => InvokerHelper<T>.GetInvoker(v.Value));
        }

        ValueTask<HubInvocationResult> IHubInvoker.InvokeAsync(object instance, HubInvokeMessage invokeMessage)
        {
            return InvokeAsync((T)instance, invokeMessage);
        }

        public async ValueTask<HubInvocationResult> InvokeAsync(T instance, HubInvokeMessage invokeMessage)
        {
            return await _methodInvoker[invokeMessage.Method](instance, invokeMessage);
        }

        /// <summary>
        /// Create a message for invoking method from a JSON string.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="invokeMessage"></param>
        /// <returns></returns>
        public bool TryCreateMessage(string json, [NotNullWhen(true)] out HubInvokeMessage? invokeMessage)
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));

            var operationId = default(string);
            var arguments = Array.Empty<object?>();
            var method = default(string);
            var methodDef = default(HubMethodDefinition);

            var currentPropName = default(string);
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        currentPropName = reader.GetString();
                        break;
                    case JsonTokenType.String:
                        if (currentPropName == "operationId" || currentPropName == "O")
                        {
                            operationId = reader.GetString();
                        }
                        else if (currentPropName == "method" || currentPropName == "M")
                        {
                            method = reader.GetString();
                            methodDef = _methodMap[method!];
                        }

                        currentPropName = null;
                        break;
                    case JsonTokenType.StartArray:
                        if (currentPropName == "arguments" || currentPropName == "A")
                        {
                            if (methodDef == null) throw new InvalidOperationException();

                            arguments = new object[methodDef.ParameterTypes.Count];
                            for (int i = 0; i < methodDef.ParameterTypes.Count; i++)
                            {
                                arguments[i] = ReadObject(ref reader, methodDef.ParameterTypes[i]);
                            }

                            reader.Read();
                            if (reader.TokenType != JsonTokenType.EndArray)
                            {
                                throw new InvalidOperationException();
                            }
                        }
                        break;
                }
            }

            if (methodDef == null || method == null)
            {
                invokeMessage = null;
                return false;
            }

            invokeMessage = new HubInvokeMessage(
                operationId ?? throw new InvalidOperationException("OperationId must not be null or empty"),
                method,
                methodDef,
                arguments
            );
            return true;
        }

        private static object? ReadObject(ref Utf8JsonReader reader, Type type)
        {
            reader.Read();

            return JsonSerializer.Deserialize(ref reader, type);
        }
    }
}
