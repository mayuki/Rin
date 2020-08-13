using System.Diagnostics;

namespace Rin.Channel.HubInvoker
{
    [DebuggerDisplay("HubInvocationResult: Value={Value,nq}; HasResult={HasResult,nq}")]
    public readonly struct HubInvocationResult
    {
        public object? Value { get; }
        public bool HasResult { get; }

        public HubInvocationResult(object? value)
        {
            Value = value;
            HasResult = true;
        }
    }
}
