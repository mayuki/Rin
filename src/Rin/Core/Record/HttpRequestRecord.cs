using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Rin.Core.Record
{
    public class HttpRequestRecord
    {
        public string Id { get; internal set; }
        public bool IsHttps { get; internal set; }
        public HostString Host { get; internal set; }
        public QueryString QueryString { get; internal set; }
        public PathString Path { get; internal set; }
        public string Method { get; internal set; }
        public int ResponseStatusCode { get; internal set; }
        public IPAddress RemoteIpAddress { get; internal set; }
        public byte[] RequestBody { get; internal set; }
        public IDictionary<string, StringValues> RequestHeaders { get; internal set; }
        public byte[] ResponseBody { get; internal set; }
        public IDictionary<string, StringValues> ResponseHeaders { get; internal set; }
        public DateTime RequestReceivedAt { get; internal set; }
        public DateTime ProcessingStartedAt { get; internal set; }
        public DateTime ProcessingCompletedAt { get; internal set; }
        public DateTime TransferringStartedAt { get; internal set; }
        public DateTime TransferringCompletedAt { get; internal set; }
        public Exception Exception { get; internal set; }

        public ConcurrentQueue<TraceLogRecord> Traces { get; internal set; }

        public bool IsCompleted => TransferringCompletedAt != default(DateTime);
    }
}
