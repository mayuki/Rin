using System;
using Microsoft.Extensions.Primitives;
using Rin.Core.Record;
using System.Linq;

namespace Rin.Core
{
    public interface IBodyDataTransformer
    {
        bool CanTransform(HttpRequestRecord record, StringValues contentTypeHeaderValues);
        bool TryTransform(HttpRequestRecord record, ReadOnlySpan<byte> body, StringValues contentTypeHeaderValues, out BodyDataTransformResult result);
    }

    public interface IRequestBodyDataTransformer : IBodyDataTransformer
    {
    }

    public interface IResponseBodyDataTransformer : IBodyDataTransformer
    {
    }
}
