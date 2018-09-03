using Rin.Core.Record;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rin.Core.Event
{
    public struct RequestEventMessage
    {
        public string EventSource { get; }
        public HttpRequestRecord Value { get; }
        public RequestEvent Event { get; }

        public RequestEventMessage(string eventSource, HttpRequestRecord value, RequestEvent requestEvent)
        {
            EventSource = eventSource;
            Value = value;
            Event = requestEvent;
        }
    }

    public enum RequestEvent
    {
        BeginRequest,
        CompleteRequest
    }
}
