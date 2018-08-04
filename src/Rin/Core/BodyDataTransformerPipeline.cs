using Microsoft.Extensions.Primitives;
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

        public bool CanTransform(StringValues contentTypeHeaderValues)
        {
            return true;
        }

        public BodyDataTransformResult Transform(byte[] body, StringValues contentTypeHeaderValues)
        {
            var transformer = _transformers.FirstOrDefault(x => x.CanTransform(contentTypeHeaderValues));
            if (transformer != null)
            {
                return transformer.Transform(body, contentTypeHeaderValues);
            }

            return new BodyDataTransformResult(body, contentTypeHeaderValues.ToString(), null);
        }
    }
}
