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
        public string ParentId { get; set; }

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

        public DateTimeOffset RequestReceivedAt { get; internal set; }
        public DateTimeOffset TransferringCompletedAt { get; internal set; }

        public Exception Exception { get; internal set; }

        public ITimelineScope Timeline { get; internal set; }

        internal ITimelineScope Processing { get; set; }
        internal ITimelineScope Transferring { get; set; }

        public bool IsCompleted => TransferringCompletedAt != default(DateTimeOffset);
    }
}
