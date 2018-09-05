using System;
using System.Collections.Generic;
using System.Text;

namespace Rin.Core.Event
{
    public struct StoreBodyEventMessage
    {
        public StoreBodyEvent Event { get; set; }
        public string Id { get; set; }
        public byte[] Body { get; set; }

        public StoreBodyEventMessage(StoreBodyEvent storeBodyEvent, string id, byte[] body)
        {
            Event = storeBodyEvent;
            Id = id;
            Body = body;
        }
    }

    public enum StoreBodyEvent
    {
        Request,
        Response,
    }
}
