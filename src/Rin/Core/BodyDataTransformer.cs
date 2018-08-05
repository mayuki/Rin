using Microsoft.Extensions.Primitives;
using Rin.Core.Record;
using System.Linq;

namespace Rin.Core
{
    public interface IBodyDataTransformer
    {
        bool CanTransform(HttpRequestRecord record, StringValues contentTypeHeaderValues);
        BodyDataTransformResult Transform(HttpRequestRecord record, byte[] body, StringValues contentTypeHeaderValues);
    }

    public abstract class BodyDataTransformer : IBodyDataTransformer
    {
        public abstract bool CanTransform(HttpRequestRecord record, StringValues contentTypeHeaderValues);
        public abstract BodyDataTransformResult Transform(HttpRequestRecord record, byte[] body, StringValues contentTypeHeaderValues);
    }
}
