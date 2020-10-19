using System;
using Microsoft.Extensions.Primitives;
using Rin.Core.Record;
using System.Collections.Generic;
using System.Linq;

namespace Rin.Core
{
    public class BodyDataTransformerPipeline : IBodyDataTransformer
    {
        private readonly IEnumerable<IBodyDataTransformer> _transformers;

        public BodyDataTransformerPipeline(IEnumerable<IBodyDataTransformer> transformers)
        {
            _transformers = transformers;
        }

        public bool CanTransform(HttpRequestRecord record, StringValues contentTypeHeaderValues)
        {
            return true;
        }

        public bool TryTransform(HttpRequestRecord record, ReadOnlySpan<byte> body, StringValues contentTypeHeaderValues, out BodyDataTransformResult result)
        {
            var transformer = _transformers.FirstOrDefault(x => x.CanTransform(record, contentTypeHeaderValues));
            if (transformer != null)
            {
                return transformer.TryTransform(record, body, contentTypeHeaderValues, out result);
            }

            result = default;
            return false;
        }
    }
}
