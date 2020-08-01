using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Rin.IO;

namespace Rin.Features
{
    public class CaptureHttpResponseBodyFeature : IHttpResponseBodyFeature
    {
        private readonly IHttpResponseBodyFeature _responseBodyFeature;
        private readonly CapturePipeWriter _capturePipeWriter;
        private readonly DataCaptureStream _captureStream;
        private readonly MemoryStream _capturedDataStream;

        public DataCaptureStream CaptureStream => _captureStream;

        public CaptureHttpResponseBodyFeature(IHttpResponseBodyFeature responseBodyFeature)
        {
            _responseBodyFeature = responseBodyFeature;
            _capturedDataStream = new MemoryStream();
            _captureStream = new DataCaptureStream(responseBodyFeature.Stream, _capturedDataStream);
            _capturePipeWriter = new CapturePipeWriter(responseBodyFeature.Writer, _capturedDataStream);
        }

        public Task CompleteAsync()
        {
            return _responseBodyFeature.CompleteAsync();
        }

        public void DisableBuffering()
        {
            _responseBodyFeature.DisableBuffering();
        }

        public Task SendFileAsync(string path, long offset, long? count, CancellationToken cancellationToken = default)
        {
            _captureStream.CaptureStream.Write(File.ReadAllBytes(path));
            return _responseBodyFeature.SendFileAsync(path, offset, count, cancellationToken);
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return _responseBodyFeature.StartAsync(cancellationToken);
        }

        public Stream Stream => _captureStream;

        public PipeWriter Writer => _capturePipeWriter;
    }
}