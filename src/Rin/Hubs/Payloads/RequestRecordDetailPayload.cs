using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.Primitives;
using Rin.Core;
using Rin.Core.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Rin.Hubs.Payloads
{
    public class RequestRecordDetailPayload
    {
        public string Id { get; private set; }
        public bool IsCompleted { get; private set; }
        public string Method { get; private set; }
        public bool IsHttps { get; private set; }
        public string Host { get; private set; }
        public string Path { get; private set; }
        public string QueryString { get; private set; }
        public int ResponseStatusCode { get; private set; }
        public string RemoteIpAddress { get; private set; }

        public IDictionary<string, StringValues> RequestHeaders { get; private set; }
        public IDictionary<string, StringValues> ResponseHeaders { get; private set; }
        public DateTime RequestReceivedAt { get; private set; }
        public DateTime ProcessingStartedAt { get; private set; }
        public DateTime ProcessingCompletedAt { get; private set; }
        public DateTime TransferringStartedAt { get; private set; }
        public DateTime TransferringCompletedAt { get; private set; }

        public Exception Exception { get; private set; }

        public TraceLogRecord[] Traces { get; private set; }
        public TimelineData Timeline { get; private set; }

        public RequestRecordDetailPayload(HttpRequestRecord record)
        {
            Id = record.Id;
            IsCompleted = record.IsCompleted;
            IsHttps = record.IsHttps;
            Host = record.Host.Value;
            Method = record.Method;
            Path = record.Path;
            QueryString = record.QueryString.Value;
            ResponseStatusCode = record.ResponseStatusCode;

            RemoteIpAddress = record.RemoteIpAddress.ToString();
            RequestHeaders = record.RequestHeaders;
            ResponseHeaders = record.ResponseHeaders;
            RequestReceivedAt = record.RequestReceivedAt;
            TransferringCompletedAt = record.TransferringCompletedAt;

            Exception = record.Exception;
            Traces = record.Traces.ToArray();
            Timeline = new TimelineData(record.Timeline);
        }

        public class TimelineData
        {
            public DateTime BeginTime { get; }
            public long Duration { get; }
            public string Category { get; }
            public string Name { get; }
            public string Data { get; }
            public TimelineData[] Children { get; }

            public TimelineData(TimelineScope scope)
            {
                BeginTime = scope.BeginTime;
                Duration = (long)scope.Duration.TotalMilliseconds;
                Category = scope.Category;
                Name = scope.Name;
                Data = scope.Data;
                Children = scope.Children.Select(x => new TimelineData(x)).ToArray();
            }
        }
    }
}
