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
    public interface IRinRequestRecordingFeature
    {
        HttpRequestRecord Record { get; }
        DataCaptureStream? ResponseDataStream { get; set; }
    }
}
