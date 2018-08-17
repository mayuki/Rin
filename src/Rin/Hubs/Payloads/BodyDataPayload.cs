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

        public static BodyDataPayload CreateFromRecord(HttpRequestRecord record, IDictionary<string, StringValues> headers, byte[] body, IBodyDataTransformer transformer)
        {
            if (headers.TryGetValue("Content-Type", out var contentType))
            {
                var result = transformer.Transform(record, body, contentType);

                if (result.ContentType.StartsWith("text/") ||
                    result.ContentType.StartsWith("application/json") ||
                    result.ContentType.StartsWith("text/json") ||
                    result.ContentType.StartsWith("application/x-www-form-urlencoded"))
                {
                    return new BodyDataPayload(new UTF8Encoding(false).GetString(result.Body), false, result.TransformedContentType ?? "");
                }
                else
                {
                    return new BodyDataPayload(Convert.ToBase64String(result.Body), true, result.TransformedContentType ?? "");
                }
            }

            return new BodyDataPayload(Convert.ToBase64String(body), true, "");
        }
    }
}
