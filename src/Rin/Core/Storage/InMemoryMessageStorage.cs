using System.Collections.Generic;
using System.Linq;

namespace Rin.Core.Storage
{
    public class InMemoryMessageStorage<T> : IMessageStorage<T> where T: IMessage
    {
        private Dictionary<string, T> _entries = new Dictionary<string, T>();
        private Queue<string> _entryIds = new Queue<string>();
        private int _retentionMaxRequests = 100;

        public InMemoryMessageStorage(int retentionMaxRequests)
        {
            _retentionMaxRequests = retentionMaxRequests;
        }

        public void Add(T entry)
        {
            lock (_entries)
            {
                _entries[entry.Id] = entry;
                _entryIds.Enqueue(entry.Id);

                if (_entryIds.Count > _retentionMaxRequests)
                {
                    var deletedKey = _entryIds.Dequeue();
                    _entries.Remove(deletedKey);
                }
            }
        }

        public T[] GetAll()
        {
            lock (_entries)
            {
                return _entryIds.Select(x => _entries[x]).ToArray();
            }
        }

        public void Publish(T value)
        {
            Add(value);
        }

        public bool TryGetById(string id, out T value)
        {
            lock (_entryIds)
            {
                return _entries.TryGetValue(id, out value);
            }
        }
    }
}
