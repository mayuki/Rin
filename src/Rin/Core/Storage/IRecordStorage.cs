using Rin.Core.Event;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Rin.Core.Storage
{
    public interface IRecordStorage : IMessageSubscriber<RequestEventMessage>
    {
        void Add(HttpRequestRecord entry);
        HttpRequestRecord[] GetAll();
        bool TryGetById(string id, out HttpRequestRecord value);
    }
}
