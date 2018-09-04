using Rin.Core.Event;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Rin.Core.Record
{
    public interface IRecordStorage : IMessageSubscriber<RequestEventMessage>, IDisposable
    {
        Task AddAsync(HttpRequestRecord entry);
        Task UpdateAsync(HttpRequestRecord entry);
        Task<HttpRequestRecordInfo[]> GetAllAsync();
        Task<RecordStorageTryGetResult> TryGetByIdAsync(string id);
    }

    public struct RecordStorageTryGetResult
    {
        public bool Succeed { get; }
        public HttpRequestRecord Value { get; }

        public RecordStorageTryGetResult(bool succeed, HttpRequestRecord value)
        {
            Succeed = succeed;
            Value = value;
        }
    }
}
