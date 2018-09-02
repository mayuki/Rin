using Microsoft.Extensions.Primitives;
using Rin.Channel;
using Rin.Core;
using Rin.Core.Event;
using Rin.Core.Record;
using Rin.Hubs.HubClients;
using Rin.Hubs.Payloads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rin.Hubs
{
    public class RinCoreHub : IHub
    {
        private IRecordStorage _storage;
        private RinChannel _rinChannel;
        private BodyDataTransformerSet _bodyDataTransformerSet;

        public RinCoreHub(IRecordStorage storage, RinChannel rinChannel, BodyDataTransformerSet bodyDataTransformerSet)
        {
            _storage = storage;
            _rinChannel = rinChannel;
            _bodyDataTransformerSet = bodyDataTransformerSet;
        }

        public async Task<RequestEventPayload[]> GetRecordingList()
        {
            return (await _storage.GetAllAsync()).Select(x => new RequestEventPayload(x)).Reverse().ToArray();
        }

        public async Task<RequestRecordDetailPayload> GetDetailById(string id)
        {
            var result = await _storage.TryGetByIdAsync(id);

            return (result.Succeed)
                ? new RequestRecordDetailPayload(result.Value)
                : null;
        }

        public async Task<BodyDataPayload> GetRequestBody(string id)
        {
            var result = await _storage.TryGetByIdAsync(id);

            return (result.Succeed)
                ? BodyDataPayload.CreateFromRecord(result.Value, result.Value.RequestHeaders, result.Value.RequestBody, _bodyDataTransformerSet.Request)
                : null;
        }

        public async Task<BodyDataPayload> GetResponseBody(string id)
        {
            var result = await _storage.TryGetByIdAsync(id);

            return (result.Succeed)
                ? BodyDataPayload.CreateFromRecord(result.Value, result.Value.ResponseHeaders, result.Value.ResponseBody, _bodyDataTransformerSet.Response)
                : null;
        }

        public RinServerInfoPayload GetServerInfo()
        {
            return new RinServerInfoPayload
            {
                Version = typeof(RinCoreHub).Assembly.GetName().Version.ToString(),
                BuildDate = new FileInfo(typeof(RinCoreHub).Assembly.Location).LastWriteTimeUtc,
                FeatureFlags = Array.Empty<string>(),
            };
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
