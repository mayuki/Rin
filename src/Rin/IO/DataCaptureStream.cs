using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rin.IO
{
    public class DataCaptureStream : Stream
    {
        private readonly MemoryStream _capturedData;
        private readonly Stream _innerStream;

        public DataCaptureStream(Stream stream, MemoryStream capturedDataStream = null)
        {
            _innerStream = stream;
            _capturedData = capturedDataStream ?? new MemoryStream();
        }

        public byte[] GetCapturedData() => _capturedData.ToArray();

        public override bool CanRead => _innerStream.CanRead;

        public override bool CanSeek => _innerStream.CanSeek;

        public override bool CanWrite => _innerStream.CanWrite;

        public override long Length => _innerStream.Length;

        public override long Position { get => _innerStream.Position; set => _innerStream.Position = value; }

        public override void Flush()
        {
            _capturedData.Flush();
            _innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            _capturedData.Read(buffer, offset, count);
            return _innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            _capturedData.Seek(offset, origin);
            return _innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _capturedData.SetLength(value);
            _innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _capturedData.Write(buffer, offset, count);
            _innerStream.Write(buffer, offset, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Task.WhenAll(
                _capturedData.WriteAsync(buffer, offset, count, cancellationToken),
                _innerStream.WriteAsync(buffer, offset, count, cancellationToken)
            );
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAll(
                _capturedData.FlushAsync(cancellationToken),
                _innerStream.FlushAsync(cancellationToken)
            );
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var results = await Task.WhenAll(
                _capturedData.ReadAsync(buffer, offset, count, cancellationToken),
                _innerStream.ReadAsync(buffer, offset, count, cancellationToken)
            );
            return results[1];
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
