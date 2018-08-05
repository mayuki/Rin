using System;
using System.Collections.Generic;
using System.Text;

namespace Rin.Core
{
    public class InspectorOptions
    {
        public string MountPath { get; set; } = "/rin";
        public List<IBodyDataTransformer> RequestBodyDataTransformers { get; } = new List<IBodyDataTransformer>();
        public List<IBodyDataTransformer> ResponseBodyDataTransformers { get; } = new List<IBodyDataTransformer>();
    }
}
