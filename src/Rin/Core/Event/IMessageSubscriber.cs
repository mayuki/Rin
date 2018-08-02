namespace Rin.Core.Event
{
    public interface IMessageSubscriber<T> where T: IMessage
    {
        void Publish(T value);
    }
}
