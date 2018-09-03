using Rin.Core.Record;
using Rin.Core.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rin.Core
{
    public class RinOptions
    {
        public RequestRecorderOptions RequestRecorder { get; set; } = new RequestRecorderOptions();
        public InspectorOptions Inspector { get; set; } = new InspectorOptions();
    }
}
