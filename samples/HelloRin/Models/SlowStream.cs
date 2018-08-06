using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HelloRin.Models
{
    public class SlowStream : Stream
    {
        public Stream BaseStream { get; }

        public override bool CanRead => BaseStream.CanRead;

        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => BaseStream.CanWrite;

        public override long Length => BaseStream.Length;

        public override long Position { get => BaseStream.Position; set => BaseStream.Position = value; }

        public SlowStream(Stream baseStream)
        {
            BaseStream = baseStream;
        }

        public override void Flush()
        {
            Thread.Sleep(1000);
            BaseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Thread.Sleep(1000);
            return BaseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            Thread.Sleep(1000);
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Thread.Sleep(1000);
            BaseStream.Write(buffer, offset, count);
        }

        public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            await Task.Delay(1000);
            await base.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await Task.Delay(1000);
            return await base.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Delay(1000);
            return await base.ReadAsync(buffer, cancellationToken);
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(1000);
            await base.FlushAsync(cancellationToken);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await Task.Delay(1000);
            await base.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Delay(1000);
            await base.WriteAsync(buffer, cancellationToken);
        }
    }
}
