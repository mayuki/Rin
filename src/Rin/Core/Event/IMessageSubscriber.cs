namespace Rin.Core.Event
{
    public interface IMessageSubscriber<T>
    {
        void Publish(T message);
    }
}
