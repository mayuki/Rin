using System;
using System.Threading.Tasks;

namespace Rin.Core.Event
{
    public interface IMessageEventBus<T> : IDisposable
    {
        ValueTask PostAsync(T value);
    }
}
