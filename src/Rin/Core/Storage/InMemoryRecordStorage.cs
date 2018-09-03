using Rin.Core.Event;
using Rin.Core.Record;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rin.Core.Storage
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

        public Task AddAsync(HttpRequestRecord record)
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
            return Task.CompletedTask;
        }

        public Task UpdateAsync(HttpRequestRecord record)
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
            return Task.CompletedTask;
        }

        public Task<HttpRequestRecord[]> GetAllAsync()
        {
            _lock.EnterReadLock();
            try
            {
                return Task.FromResult(_entryIds.Select(x => _entries[x]).ToArray());
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public Task<RecordStorageTryGetResult> TryGetByIdAsync(string id)
        {
            _lock.EnterReadLock();
            try
            {
                var succeed = _entries.TryGetValue(id, out var value);
                return Task.FromResult(new RecordStorageTryGetResult(succeed, value));
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        void IMessageSubscriber<RequestEventMessage>.Publish(RequestEventMessage message)
        {
            switch (message.Event)
            {
                case RequestEvent.BeginRequest:
                    AddAsync(message.Value);
                    break;
                case RequestEvent.CompleteRequest:
                    UpdateAsync(message.Value);
                    break;
            }
        }
    }
}
