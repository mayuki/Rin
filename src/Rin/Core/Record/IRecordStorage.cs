using Rin.Core.Event;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Rin.Core.Record
{
    public interface IRecordStorage : IMessageSubscriber<RequestEventMessage>, IMessageSubscriber<StoreBodyEventMessage>, IDisposable
    {
        Task AddAsync(HttpRequestRecord entry);
        Task UpdateAsync(HttpRequestRecord entry);
        Task<HttpRequestRecordInfo[]> GetAllAsync();
        Task<RecordStorageTryGetResult<HttpRequestRecord?>> TryGetDetailByIdAsync(string id);
        Task<RecordStorageTryGetResult<byte[]?>> TryGetResponseBodyByIdAsync(string id);
        Task<RecordStorageTryGetResult<byte[]?>> TryGetRequestBodyByIdAsync(string id);
    }

    public readonly struct RecordStorageTryGetResult<T>
    {
        public bool Succeed { get; }
        public T Value { get; }

        public RecordStorageTryGetResult(bool succeed, T value)
        {
            Succeed = succeed;
            Value = value;
        }
    }

    public static class RecordStorageTryGetResult
    {
        public static RecordStorageTryGetResult<T> Create<T>(bool succeed, T value)
        {
            return new RecordStorageTryGetResult<T>(succeed, value);
        }
    }
}
