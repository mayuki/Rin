using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rin.IO
{
    public class DataCaptureStream : Stream
    {
        private MemoryStream _capturedData = new MemoryStream();
        private Stream _innerStream;

        public DataCaptureStream(Stream stream)
        {
            _innerStream = stream;
            _capturedData = new MemoryStream();
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
    }
}
