using Microsoft.AspNetCore.Http;
using Rin.Features;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Features;

namespace Rin.Extensions
{
    public static class ResponseDataCaptureStreamExtensions
    {
        public static void EnableResponseDataCapturing(this HttpContext context)
        {
            var originalResponseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>();
            if (originalResponseBodyFeature is null) return;

            var rinRecordingFeature = context.Features.Get<IRinRequestRecordingFeature>();
            if (rinRecordingFeature is null) return;

            var captureFeature = new CaptureHttpResponseBodyFeature(originalResponseBodyFeature);
            context.Features.Set<IHttpResponseBodyFeature>(captureFeature);

            rinRecordingFeature.ResponseDataStream = captureFeature.CaptureStream;
        }
    }
}
