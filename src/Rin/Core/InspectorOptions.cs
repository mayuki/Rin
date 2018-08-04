using System;
using System.Collections.Generic;
using System.Text;

namespace Rin.Core
{
    public class InspectorOptions
    {
        public string MountPath { get; set; } = "/rin";
        public List<IBodyDataTransformer> BodyDataTransformers { get; } = new List<IBodyDataTransformer>();
    }
}
