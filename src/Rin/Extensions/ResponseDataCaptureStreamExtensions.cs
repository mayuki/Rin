using Microsoft.AspNetCore.Http;
using Rin.Features;
using Rin.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rin.Extensions
{
    public static class ResponseDataCaptureStreamExtensions
    {
        public static void EnableResponseDataCapturing(this HttpContext context)
        {
            var captureDataStream = new DataCaptureStream(context.Response.Body);
            context.Response.Body = captureDataStream;

            context.Features.Get<IRinRequestRecordingFeature>().ResponseDataStream = captureDataStream;
        }
    }
}
