using Microsoft.Extensions.Primitives;
using Rin.Core;
using Rin.Core.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HelloRin.Controllers.MyApiController;

namespace HelloRin.Models
{
    public class RinCustomContentTypeTransformer : IBodyDataTransformer
    {
        public bool CanTransform(HttpRequestRecord record, StringValues contentTypeHeaderValues)
        {
            return contentTypeHeaderValues.Any(x => x == "application/x-msgpack");
        }

        public BodyDataTransformResult Transform(HttpRequestRecord record, byte[] body, StringValues contentTypeHeaderValues)
        {
            var data = MessagePack.LZ4MessagePackSerializer.Deserialize<MyClass>(body, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
            var json = MessagePack.MessagePackSerializer.ToJson<MyClass>(data, MessagePack.Resolvers.ContractlessStandardResolver.Instance);

            return new BodyDataTransformResult(new UTF8Encoding(false).GetBytes(json), contentTypeHeaderValues.ToString(), "application/json");
        }
    }
}
