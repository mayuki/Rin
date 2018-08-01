using Rin.Channel;
using Rin.Core;
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
        private RequestRecordStorage _requestEventStorage;
        private RinChannel _rinChannel;

        public RinCoreHub(RequestRecordStorage requestEventStorage, RinChannel rinChannel)
        {
            _requestEventStorage = requestEventStorage;
            _rinChannel = rinChannel;
        }

        public RequestEventPayload[] GetRecordingList()
        {
            return _requestEventStorage.Records.Select(x => new RequestEventPayload(x)).Reverse().ToArray();
        }

        public RequestRecordDetailPayload GetDetailById(string id)
        {
            return _requestEventStorage.Records
                .Where(x => x.Id == id)
                .Select(x => new RequestRecordDetailPayload(x))
                .FirstOrDefault();
        }

        public byte[] GetRequestBody(string id)
        {
            return _requestEventStorage.Records
                .Where(x => x.Id == id)
                .Select(x => x.RequestBody)
                .FirstOrDefault();
        }

        public byte[] GetResponseBody(string id)
        {
            return _requestEventStorage.Records
                .Where(x => x.Id == id)
                .Select(x => x.ResponseBody)
                .FirstOrDefault();
        }

        public bool Ping()
        {
            return true;
        }
    }
}
