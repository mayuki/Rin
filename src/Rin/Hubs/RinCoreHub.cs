using Microsoft.Extensions.Primitives;
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
        private IBodyDataTransformer _bodyDataTransformer;

        public RinCoreHub(IRecordStorage storage, RinChannel rinChannel, IBodyDataTransformer bodyDataTransformer)
        {
            _storage = storage;
            _rinChannel = rinChannel;
            _bodyDataTransformer = bodyDataTransformer;
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

        public BodyDataPayload GetRequestBody(string id)
        {
            return (_storage.TryGetById(id, out var value))
                ? CreateFromRecord(value.RequestHeaders, value.RequestBody)
                : null;
        }

        public BodyDataPayload GetResponseBody(string id)
        {
            return (_storage.TryGetById(id, out var value))
                ? CreateFromRecord(value.ResponseHeaders, value.ResponseBody)
                : null;
        }

        private BodyDataPayload CreateFromRecord(IDictionary<string, StringValues> headers, byte[] body)
        {
            if (headers.TryGetValue("Content-Type", out var contentType))
            {
                var result = _bodyDataTransformer.Transform(body, contentType);

                if (result.ContentType.StartsWith("text/") || result.ContentType.StartsWith("application/json") || result.ContentType.StartsWith("text/json"))
                {
                    return new BodyDataPayload(new UTF8Encoding(false).GetString(result.Body), false, result.TransformedContentType ?? "");
                }
                else
                {
                    return new BodyDataPayload(Convert.ToBase64String(result.Body), true, result.TransformedContentType ?? "");
                }
            }

            return new BodyDataPayload(Convert.ToBase64String(body), true, "");
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
