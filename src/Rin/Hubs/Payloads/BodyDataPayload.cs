using Microsoft.Extensions.Primitives;
using Rin.Core;
using Rin.Core.Record;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rin.Hubs.Payloads
{
    public class BodyDataPayload
    {
        public string Body { get; }
        public bool IsBase64Encoded { get; }
        public string PresentationContentType { get; }
        
        public BodyDataPayload(string body, bool isBase64Encoded, string presentationContentType)
        {
            Body = body;
            IsBase64Encoded = isBase64Encoded;
            PresentationContentType = presentationContentType;
        }

        public static BodyDataPayload CreateFromRecord(HttpRequestRecord record, IDictionary<string, StringValues>? headers, ReadOnlySpan<byte> body, IBodyDataTransformer transformer)
        {
            if (body.IsEmpty)
            {
                return new BodyDataPayload(Convert.ToBase64String(Array.Empty<byte>()), true, "");
            }

            var payloadBody = body;
            var payloadBodyContentType = string.Empty;
            var transformedBodyContentType = string.Empty;
            if (headers != null && headers.TryGetValue("Content-Type", out var contentType))
            {
                if (transformer.TryTransform(record, body, contentType, out var result))
                {
                    payloadBodyContentType = result.TransformedContentType;
                    transformedBodyContentType = result.TransformedContentType;
                    payloadBody = result.Body;
                }
                else
                {
                    payloadBodyContentType = contentType;
                }
            }

            if (payloadBodyContentType.StartsWith("text/") ||
                payloadBodyContentType.StartsWith("application/json") ||
                payloadBodyContentType.StartsWith("application/x-www-form-urlencoded"))
            {
                return new BodyDataPayload(Encoding.UTF8.GetString(payloadBody), false, transformedBodyContentType);
            }
            else
            {
                return new BodyDataPayload(Convert.ToBase64String(payloadBody), true, transformedBodyContentType);
            }
        }
    }
}
