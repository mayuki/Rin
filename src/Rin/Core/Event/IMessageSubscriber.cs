using System.Threading.Tasks;

namespace Rin.Core.Event
{
    public interface IMessageSubscriber<T>
    {
        Task Publish(T message);
    }
}
