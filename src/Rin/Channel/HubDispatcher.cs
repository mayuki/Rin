using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Rin.Channel
{
    internal class HubDispatcher<T> where T: IHub
    {
        private static Dictionary<string, Func<object, JToken[], Task<object>>> _methodMap = new Dictionary<string, Func<object, JToken[], Task<object>>>();

        static HubDispatcher()
        {
            var type = typeof(T);

            foreach (var methodInfo in type.GetMethods().Where(x => x.DeclaringType == type && x.DeclaringType != typeof(object)))
            {
                _methodMap[methodInfo.Name] = CreateFunctionProxyFromInstanceMethod(methodInfo);
            }
        }

        public static Task<object> InvokeAsync(string name, T thisArg, JToken[] args)
        {
            return _methodMap[name](thisArg, args);
        }

        public static bool CanInvoke(string name)
        {
            return _methodMap.ContainsKey(name);
        }

        private static Func<object, JToken[], Task<object>> CreateFunctionProxyFromInstanceMethod(MethodInfo methodInfo)
        {
            var methodInfoOfAsTask = (methodInfo.ReturnType.BaseType == typeof(Task))
                ? typeof(HubDispatcher<IHub>).GetMethod(nameof(AsTaskOfObject), BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(methodInfo.ReturnType.GenericTypeArguments[0])
                : typeof(HubDispatcher<IHub>).GetMethod(nameof(AsTaskFromResult), BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(methodInfo.ReturnType);
            var methodInfoToObject = typeof(JObject).GetMethod(nameof(JObject.ToObject), new[] { typeof(Type) });

            var thisType = typeof(T);
            var methodParams = methodInfo.GetParameters();
            var thisArg = Expression.Parameter(typeof(object));
            var args = Expression.Parameter(typeof(JToken[]));

            var lambdaParams = methodParams.Select((x, i) =>
                Expression.Convert(Expression.Call(Expression.ArrayIndex(args, Expression.Constant(i)), methodInfoToObject, Expression.Constant(x.ParameterType)), x.ParameterType)
            );

            var expression = Expression.Lambda<Func<object, JToken[], Task<Object>>>(
                Expression.Call(methodInfoOfAsTask, Expression.Call(Expression.Convert(thisArg, thisType), methodInfo, lambdaParams)),
                thisArg, args
            );

            return expression.Compile();
        }

        private static Task<object> AsTaskFromResult<TValue>(TValue value)
        {
            return Task.FromResult((object)value);
        }

        private static Task<object> AsTaskOfObject<TValue>(Task<TValue> task)
        {
            return task.ContinueWith<object>(x => x.Result);
        }
    }
}
