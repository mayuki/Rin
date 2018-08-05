using Microsoft.Extensions.Primitives;
using Rin.Core.Record;
using System.Linq;

namespace Rin.Core
{
    public interface IBodyDataTransformer
    {
        bool CanTransform(StringValues contentTypeHeaderValues);
        BodyDataTransformResult Transform(byte[] body, StringValues contentTypeHeaderValues);
    }

    public abstract class BodyDataTransformer : IBodyDataTransformer
    {
        public abstract bool CanTransform(StringValues contentTypeHeaderValues);
        public abstract BodyDataTransformResult Transform(byte[] body, StringValues contentTypeHeaderValues);
    }
}
