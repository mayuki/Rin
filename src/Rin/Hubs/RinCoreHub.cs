using Rin.Channel;
using Rin.Core;
using Rin.Core.Event;
using Rin.Core.Storage;
using Rin.Hubs.HubClients;
using Rin.Hubs.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rin.Hubs
{
    public class RinCoreHub : IHub
    {
        private IRecordStorage _storage;
        private RinChannel _rinChannel;

        public RinCoreHub(IRecordStorage storage, RinChannel rinChannel)
        {
            _storage = storage;
            _rinChannel = rinChannel;
        }

        public RequestEventPayload[] GetRecordingList()
        {
            return _storage.GetAll().Select(x => new RequestEventPayload(x)).Reverse().ToArray();
        }

        public RequestRecordDetailPayload GetDetailById(string id)
        {
            return (_storage.TryGetById(id, out var value))
                ? new RequestRecordDetailPayload(value)
                : null;
        }

        public byte[] GetRequestBody(string id)
        {
            return (_storage.TryGetById(id, out var value))
                ? value.RequestBody
                : null;
        }

        public byte[] GetResponseBody(string id)
        {
            return (_storage.TryGetById(id, out var value))
                ? value.ResponseBody
                : null;
        }

        public bool Ping()
        {
            return true;
        }

        public class MessageSubscriber : IMessageSubscriber<RequestEventMessage>
        {
            private IRinCoreHubClient _client;
            public MessageSubscriber(RinChannel channel)
            {
                _client = channel.GetClient<RinCoreHub, IRinCoreHubClient>();
            }

            public void Publish(RequestEventMessage message)
            {
                switch (message.Event)
                {
                    case RequestEvent.BeginRequest:
                        _client.RequestBegin(new RequestEventPayload(message.Value));
                        break;
                    case RequestEvent.CompleteRequest:
                        _client.RequestEnd(new RequestEventPayload(message.Value));
                        break;
                }
            }
        }
    }
}
