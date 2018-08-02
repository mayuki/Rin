using Rin.Channel;
using Rin.Core;
using Rin.Core.Storage;
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
        private IMessageStorage<HttpRequestRecord> _storage;
        private RinChannel _rinChannel;

        public RinCoreHub(IMessageStorage<HttpRequestRecord> storage, RinChannel rinChannel)
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
    }
}
