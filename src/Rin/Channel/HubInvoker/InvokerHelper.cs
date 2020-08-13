using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Rin.Channel.HubInvoker
{
    internal static class InvokerHelper<T>
    {
        private static readonly MethodInfo _methodInfoInvokeCoreTaskAsync = typeof(InvokerHelper<T>).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).First(x => x.Name == "InvokeCoreTaskAsync" && x.GetGenericArguments().Length == 1)!;
        private static readonly MethodInfo _methodInfoInvokeCoreValueTaskAsync = typeof(InvokerHelper<T>).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).First(x => x.Name == "InvokeCoreValueTaskAsync" && x.GetGenericArguments().Length == 1)!;

        public static Func<object, HubInvokeMessage, ValueTask<HubInvocationResult>> GetInvoker(HubMethodDefinition methodDefinition)
        {
            var func = GetInvokerCore(methodDefinition);
            return (instance, invokeMessage) => func((T)instance, invokeMessage);
        }

        private static Func<T, HubInvokeMessage, ValueTask<HubInvocationResult>> GetInvokerCore(HubMethodDefinition methodDefinition)
        {
            var returnType = methodDefinition.ReturnType;
            if (returnType == typeof(Task))
            {
                return InvokeCoreTaskAsync;
            }
            else if (returnType == typeof(ValueTask))
            {
                return InvokeCoreValueTaskAsync;
            }
            else if (returnType.IsGenericType)
            {
                var openGenericType = returnType.GetGenericTypeDefinition();
                var actualReturnType = returnType.GetGenericArguments()[0];
                if (openGenericType == typeof(Task<>) || openGenericType == typeof(ValueTask<>))
                {
                    Expression<Func<T, HubInvokeMessage, ValueTask<HubInvocationResult>>> expression = (instance, invokeMessage) => default;

                    var methodCore = openGenericType == typeof(Task<>) ? _methodInfoInvokeCoreTaskAsync : _methodInfoInvokeCoreValueTaskAsync;

                    expression = expression.Update(
                        Expression.Call(null, methodCore.MakeGenericMethod(actualReturnType), expression.Parameters),
                        expression.Parameters
                    );

                    return expression.Compile();
                }
            }

            return InvokeCore;
        }

        private static ValueTask<HubInvocationResult> InvokeCore(T instance, HubInvokeMessage invokeMessage)
        {
            var result = invokeMessage.MethodDefinition.Method.Invoke(instance, invokeMessage.Arguments);
            return new ValueTask<HubInvocationResult>(new HubInvocationResult(result));
        }

        private static async ValueTask<HubInvocationResult> InvokeCoreValueTaskAsync<TResult>(T instance, HubInvokeMessage invokeMessage)
        {
            var task = (ValueTask<TResult>)invokeMessage.MethodDefinition.Method.Invoke(instance, invokeMessage.Arguments)!;
            return new HubInvocationResult(await task);
        }

        private static async ValueTask<HubInvocationResult> InvokeCoreValueTaskAsync(T instance, HubInvokeMessage invokeMessage)
        {
            await ((ValueTask)invokeMessage.MethodDefinition.Method.Invoke(instance, invokeMessage.Arguments)!);
            return new HubInvocationResult();
        }

        private static async ValueTask<HubInvocationResult> InvokeCoreTaskAsync<TResult>(T instance, HubInvokeMessage invokeMessage)
        {
            var task = ((Task<TResult>)invokeMessage.MethodDefinition.Method.Invoke(instance, invokeMessage.Arguments)!);
            return new HubInvocationResult(await task);
        }

        private static async ValueTask<HubInvocationResult> InvokeCoreTaskAsync(T instance, HubInvokeMessage invokeMessage)
        {
            await ((Task)invokeMessage.MethodDefinition.Method.Invoke(instance, invokeMessage.Arguments)!);
            return new HubInvocationResult();
        }
    }
}
