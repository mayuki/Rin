using Microsoft.Extensions.DependencyInjection;
using Rin.Core.Event;
using Rin.Core.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rin.Core.Storage
{
    /// <summary>
    /// In-Memory record storage. This storage doesn't persist/share any records.
    /// </summary>
    public class InMemoryRecordStorage : IRecordStorage
    {
        private Dictionary<string, RecordEntry> _entries = new Dictionary<string, RecordEntry>();
        private Queue<string> _entryIds = new Queue<string>();
        private int _retentionMaxRequests = 100;
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public static readonly Func<IServiceProvider, IRecordStorage> Factory = (services) =>
        {
            return new InMemoryRecordStorage(services.GetService<RinOptions>().RequestRecorder.RetentionMaxRequests);
        };

        public InMemoryRecordStorage(int retentionMaxRequests)
        {
            _retentionMaxRequests = retentionMaxRequests;
        }

        public Task AddAsync(HttpRequestRecord record)
        {
            _lock.EnterWriteLock();
            try
            {
                _entries[record.Id] = new RecordEntry { Record = record };
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
                if (_entries.TryGetValue(record.Id, out var entry))
                {
                    entry.Record = record;
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            return Task.CompletedTask;
        }

        public Task<HttpRequestRecordInfo[]> GetAllAsync()
        {
            _lock.EnterReadLock();
            try
            {
                return Task.FromResult(_entryIds.Reverse().Select(x => _entries[x].Record).ToArray() as HttpRequestRecordInfo[]);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public Task<RecordStorageTryGetResult<HttpRequestRecord?>> TryGetDetailByIdAsync(string id)
        {
            _lock.EnterReadLock();
            try
            {
                var succeed = _entries.TryGetValue(id, out var value);
                return Task.FromResult(RecordStorageTryGetResult.Create(succeed, value?.Record));
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public Task<RecordStorageTryGetResult<byte[]?>> TryGetResponseBodyByIdAsync(string id)
        {
            _lock.EnterReadLock();
            try
            {
                var succeed = _entries.TryGetValue(id, out var value) && value.ResponseBody != null;
                return Task.FromResult(RecordStorageTryGetResult.Create(succeed, value?.ResponseBody));
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public Task<RecordStorageTryGetResult<byte[]?>> TryGetRequestBodyByIdAsync(string id)
        {
            _lock.EnterReadLock();
            try
            {
                var succeed = _entries.TryGetValue(id, out var value) && value.RequestBody != null;
                return Task.FromResult(RecordStorageTryGetResult.Create(succeed, value?.RequestBody));
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        Task IMessageSubscriber<RequestEventMessage>.Publish(RequestEventMessage message)
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

            return Task.CompletedTask;
        }

        Task IMessageSubscriber<StoreBodyEventMessage>.Publish(StoreBodyEventMessage message)
        {
            _lock.EnterWriteLock();
            try
            {
                if (_entries.TryGetValue(message.Id, out var entry))
                {
                    switch (message.Event)
                    {
                        case StoreBodyEvent.Request:
                            entry.RequestBody = message.Body;
                            break;
                        case StoreBodyEvent.Response:
                            entry.ResponseBody = message.Body;
                            break;
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }


        private class RecordEntry
        {
            public HttpRequestRecord Record { get; set; } = default!;
            public byte[]? RequestBody { get; set; }
            public byte[]? ResponseBody { get; set; }
        }
    }
}
