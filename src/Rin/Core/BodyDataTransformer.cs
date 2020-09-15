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

    public interface IRequestBodyDataTransformer : IBodyDataTransformer
    {
    }

    public interface IResponseBodyDataTransformer : IBodyDataTransformer
    {
    }
}
