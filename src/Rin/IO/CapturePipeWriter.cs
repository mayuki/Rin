using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Rin.IO
{
    public class CapturePipeWriter : PipeWriter
    {
        private readonly PipeWriter _pipeWriter;
        private readonly MemoryStream _capturedDataStream;

        public CapturePipeWriter(PipeWriter pipeWriter, MemoryStream capturedDataStream)
        {
            _pipeWriter = pipeWriter;
            _capturedDataStream = capturedDataStream;
        }

        public override void Advance(int bytes)
        {
            _capturedDataStream.Write(_pipeWriter.GetSpan(bytes).Slice(0, bytes));

            _pipeWriter.Advance(bytes);
        }

        public override Memory<byte> GetMemory(int sizeHint = 0)
        {
            return _pipeWriter.GetMemory(sizeHint);
        }

        public override Span<byte> GetSpan(int sizeHint = 0)
        {
            return _pipeWriter.GetSpan(sizeHint);
        }

        public override void CancelPendingFlush()
        {
            _pipeWriter.CancelPendingFlush();
        }

        public override void Complete(Exception exception = null)
        {
            _pipeWriter.Complete(exception);
        }

        public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _pipeWriter.FlushAsync(cancellationToken);
        }
    }
}