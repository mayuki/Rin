using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rin.Core
{
    public class RequestRecordStorage
    {
        public ConcurrentQueue<HttpRequestRecord> Records { get; } = new ConcurrentQueue<HttpRequestRecord>();

        public static RequestRecordStorage Instance { get; } = new RequestRecordStorage();

        public void Add(HttpRequestRecord record)
        {
            Records.Enqueue(record);
        }
    }
}
