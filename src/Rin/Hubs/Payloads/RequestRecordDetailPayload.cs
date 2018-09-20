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
        public string ParentId { get; private set; }
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
        public DateTimeOffset RequestReceivedAt { get; private set; }
        public DateTimeOffset TransferringCompletedAt { get; private set; }

        public ExceptionData Exception { get; private set; }

        public TimelineData Timeline { get; private set; }

        public RequestRecordDetailPayload(HttpRequestRecord record)
        {
            Id = record.Id;
            ParentId = record.ParentId;
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
            Timeline = new TimelineData(record.Timeline);
        }

        public class TimelineData
        {
            public string EventType { get; }
            public DateTimeOffset Timestamp { get; }
            public long Duration { get; }
            public string Category { get; }
            public string Name { get; }
            public string Data { get; }
            public TimelineData[] Children { get; }

            public TimelineData(ITimelineEvent timelineEvent)
            {
                EventType = timelineEvent.EventType;
                Timestamp = timelineEvent.Timestamp;
                Category = timelineEvent.Category;
                Name = timelineEvent.Name;
                Data = timelineEvent.Data;

                if (timelineEvent is ITimelineScope timelineScope)
                {
                    Duration = (long)timelineScope.Duration.TotalMilliseconds;
                    Children = timelineScope.Children.Select(x => new TimelineData(x)).ToArray();
                }
            }
        }
    }
}
