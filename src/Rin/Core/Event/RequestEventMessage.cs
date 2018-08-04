using Rin.Core.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rin.Core.Event
{
    public struct RequestEventMessage
    {
        public HttpRequestRecord Value { get; }
        public RequestEvent Event { get; }

        public RequestEventMessage(HttpRequestRecord value, RequestEvent requestEvent)
        {
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
