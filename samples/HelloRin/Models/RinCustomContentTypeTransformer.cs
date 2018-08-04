using Microsoft.Extensions.Primitives;
using Rin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HelloRin.Controllers.MyApiController;

namespace HelloRin.Models
{
    public class RinCustomContentTypeTransformer : BodyDataTransformer
    {
        public override bool CanTransform(StringValues contentTypeHeaderValues)
        {
            return contentTypeHeaderValues.Any(x => x == "application/x-rin-custom");
        }

        public override BodyDataTransformResult Transform(byte[] body, StringValues contentTypeHeaderValues)
        {
            var data = MessagePack.LZ4MessagePackSerializer.Deserialize<MyClass>(body, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
            var json = MessagePack.MessagePackSerializer.ToJson<MyClass>(data, MessagePack.Resolvers.ContractlessStandardResolver.Instance);

            return new BodyDataTransformResult(new UTF8Encoding(false).GetBytes(json), contentTypeHeaderValues.ToString(), "application/json");
        }
    }
}
