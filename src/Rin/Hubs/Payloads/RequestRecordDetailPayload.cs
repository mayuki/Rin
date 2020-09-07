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
        public string Id { get; }
        public string? ParentId { get; }
        public bool IsCompleted { get; }
        public string Method { get; }
        public bool IsHttps { get; }
        public string Host { get; }
        public string Path { get; }
        public string QueryString { get; }
        public int ResponseStatusCode { get; }
        public string RemoteIpAddress { get; }

        public IDictionary<string, StringValues> RequestHeaders { get; }
        public IDictionary<string, StringValues>? RequestTrailers { get; }
        public IDictionary<string, StringValues>? ResponseHeaders { get; }
        public IDictionary<string, StringValues>? ResponseTrailers { get; }
        public DateTimeOffset RequestReceivedAt { get; }
        public DateTimeOffset TransferringCompletedAt { get; }

        public ExceptionData? Exception { get; }

        public TimelineData Timeline { get; }

        public RequestRecordDetailPayload(HttpRequestRecord record)
        {
            Id = record.Id;
            ParentId = record.ParentId;
            IsCompleted = record.IsCompleted;
            IsHttps = record.IsHttps;
            Host = record.Host;
            Method = record.Method;
            Path = record.Path;
            QueryString = record.QueryString;
            ResponseStatusCode = record.ResponseStatusCode;

            RemoteIpAddress = record.RemoteIpAddress.ToString();
            RequestHeaders = record.RequestHeaders;
            RequestTrailers = record.RequestTrailers;
            ResponseHeaders = record.ResponseHeaders;
            ResponseTrailers = record.ResponseTrailers;
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
            public string? Data { get; }
            public TimelineData[]? Children { get; }

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
