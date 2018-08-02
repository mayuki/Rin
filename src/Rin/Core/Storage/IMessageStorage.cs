using Rin.Core.Event;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Rin.Core.Storage
{
    public interface IMessageStorage<T> : IMessageSubscriber<T> where T : IMessage
    {
        void Add(T entry);
        T[] GetAll();
        bool TryGetById(string id, out T value);
    }
}
