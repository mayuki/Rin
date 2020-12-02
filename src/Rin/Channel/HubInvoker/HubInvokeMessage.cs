using System;

namespace Rin.Channel.HubInvoker
{
    public class HubInvokeMessage
    {
        public string OperationId { get; }
        public string Method { get; }
        public HubMethodDefinition MethodDefinition { get; }
        public object?[] Arguments { get; }

        public HubInvokeMessage(string operationId, string method, HubMethodDefinition methodDefinition, object?[] arguments)
        {
            OperationId = operationId ?? throw new ArgumentNullException(nameof(operationId));
            Method = method ?? throw new ArgumentNullException(nameof(method));
            MethodDefinition = methodDefinition ?? throw new ArgumentNullException(nameof(methodDefinition));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }
    }
}
