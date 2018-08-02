using System;
using System.Threading.Tasks;

namespace Rin.Core.Event
{
    public interface IMessageEventBus<T> : IDisposable where T : IMessage
    {
        Task PostAsync(T value);
    }
}
