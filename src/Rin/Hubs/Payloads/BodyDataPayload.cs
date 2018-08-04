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
    }
}
