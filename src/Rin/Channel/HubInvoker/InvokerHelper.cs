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

        public static Func<object, InvokeMessage, ValueTask<InvocationResult>> GetInvoker(HubMethodDefinition methodDefinition)
        {
            var func = GetInvokerCore(methodDefinition);
            return (instance, invokeMessage) => func((T)instance, invokeMessage);
        }

        private static Func<T, InvokeMessage, ValueTask<InvocationResult>> GetInvokerCore(HubMethodDefinition methodDefinition)
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
                    Expression<Func<T, InvokeMessage, ValueTask<InvocationResult>>> expression = (instance, invokeMessage) => default;

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

        private static ValueTask<InvocationResult> InvokeCore(T instance, InvokeMessage invokeMessage)
        {
            var result = invokeMessage.MethodDefinition.Method.Invoke(instance, invokeMessage.Arguments);
            return new ValueTask<InvocationResult>(new InvocationResult(result));
        }

        private static async ValueTask<InvocationResult> InvokeCoreValueTaskAsync<TResult>(T instance, InvokeMessage invokeMessage)
        {
            var task = (ValueTask<TResult>)invokeMessage.MethodDefinition.Method.Invoke(instance, invokeMessage.Arguments)!;
            return new InvocationResult(await task);
        }

        private static async ValueTask<InvocationResult> InvokeCoreValueTaskAsync(T instance, InvokeMessage invokeMessage)
        {
            await ((ValueTask)invokeMessage.MethodDefinition.Method.Invoke(instance, invokeMessage.Arguments)!);
            return new InvocationResult();
        }

        private static async ValueTask<InvocationResult> InvokeCoreTaskAsync<TResult>(T instance, InvokeMessage invokeMessage)
        {
            var task = ((Task<TResult>)invokeMessage.MethodDefinition.Method.Invoke(instance, invokeMessage.Arguments)!);
            return new InvocationResult(await task);
        }

        private static async ValueTask<InvocationResult> InvokeCoreTaskAsync(T instance, InvokeMessage invokeMessage)
        {
            await ((Task)invokeMessage.MethodDefinition.Method.Invoke(instance, invokeMessage.Arguments)!);
            return new InvocationResult();
        }
    }
}
