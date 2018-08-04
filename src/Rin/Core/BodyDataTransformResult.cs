namespace Rin.Core
{
    public struct BodyDataTransformResult
    {
        public string TransformedContentType { get; }
        public string ContentType { get; }
        public byte[] Body { get; }

        public BodyDataTransformResult(byte[] body, string contentType, string transformedContentType)
        {
            Body = body;
            ContentType = contentType;
            TransformedContentType = transformedContentType;
        }
    }
}
