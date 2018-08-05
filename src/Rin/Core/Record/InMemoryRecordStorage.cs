using Rin.Core.Event;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Rin.Core.Record
{
    public class InMemoryRecordStorage : IRecordStorage
    {
        private Dictionary<string, HttpRequestRecord> _entries = new Dictionary<string, HttpRequestRecord>();
        private Queue<string> _entryIds = new Queue<string>();
        private int _retentionMaxRequests = 100;
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public InMemoryRecordStorage(int retentionMaxRequests)
        {
            _retentionMaxRequests = retentionMaxRequests;
        }

        public void Add(HttpRequestRecord record)
        {
            _lock.EnterWriteLock();
            try
            {
                _entries[record.Id] = record;
                _entryIds.Enqueue(record.Id);

                if (_entryIds.Count > _retentionMaxRequests)
                {
                    var deletedKey = _entryIds.Dequeue();
                    _entries.Remove(deletedKey);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Update(HttpRequestRecord record)
        {
            _lock.EnterWriteLock();
            try
            {
                _entries[record.Id] = record;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public HttpRequestRecord[] GetAll()
        {
            _lock.EnterReadLock();
            try
            {
                return _entryIds.Select(x => _entries[x]).ToArray();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void Publish(RequestEventMessage message)
        {
            switch (message.Event)
            {
                case RequestEvent.BeginRequest:
                    Add(message.Value);
                    break;
                case RequestEvent.CompleteRequest:
                    Update(message.Value);
                    break;
            }
        }

        public bool TryGetById(string id, out HttpRequestRecord value)
        {
            _lock.EnterReadLock();
            try
            {
                return _entries.TryGetValue(id, out value);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
