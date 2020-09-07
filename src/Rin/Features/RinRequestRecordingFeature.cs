using Rin.Core;
using Rin.Core.Record;
using Rin.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rin.Features
{
    public class RinRequestRecordingFeature : IRinRequestRecordingFeature
    {
        public HttpRequestRecord Record { get; }
        public DataCaptureStream? ResponseDataStream { get; set; }

        public RinRequestRecordingFeature(HttpRequestRecord record)
        {
            Record = record;
        }
    }
}
