using Microsoft.AspNetCore.Http;
using Rin.FrontendResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rin.Core.Resource
{
    public class EmbeddedZipResourceProvider : IResourceProvider
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1,1);

        public async Task<bool> TryProcessAsync(HttpContext context)
        {
            if (String.IsNullOrEmpty(context.Request.Path.Value))
            {
                context.Response.Redirect(context.Request.PathBase + "/", true);
                return true;
            }

            // TODO: ZipArchive(ZipArchiveEntry) provides a resource stream.
            // But the archive doesn't guarantee thread-safety. Only one stream can be open at once.
            await _semaphore.WaitAsync();
            try
            {
                if (Resources.TryOpen(context.Request.Path.Value.TrimStart('/'), out var resourceStream, out var contentType))
                {
                    using (resourceStream)
                    {
                        await WriteStreamToClientAsync(context, resourceStream, contentType);
                        return true;
                    }
                }
                // for SPA (+ rewrite paths in HTML)
                else if (
                    context.Request.Headers.TryGetValue("Accept", out var acceptHeaders) &&
                    acceptHeaders.Any(x => x.Contains("text/html")) &&
                    Resources.TryOpen("index.html", out resourceStream, out contentType)
                )
                {
                    using (resourceStream)
                    {
                        await WriteStreamToClientAsync(context, RewriteHtmlIfNeeded(resourceStream, contentType, context.Request.PathBase), contentType);
                        return true;
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }

            return false;
        }

        private async Task WriteStreamToClientAsync(HttpContext context, Stream stream, string contentType)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = contentType;

            if (context.Request.Headers.TryGetValue("accept-encoding", out var headerValues) && headerValues.Any(x => x.Contains("gzip")))
            {
                context.Response.Headers["Content-Encoding"] = "gzip";
                using (var outputStream = new GZipStream(new ForceAsyncStreamWrapper(context.Response.Body), CompressionLevel.Fastest, true))
                {
                    await stream.CopyToAsync(outputStream);
                }
            }
            else
            {
                await stream.CopyToAsync(context.Response.Body);
            }
        }

        private Stream RewriteHtmlIfNeeded(Stream stream, string contentType, string pathBase)
        {
            if (!contentType.StartsWith("text/html")) return stream;

            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var content = reader.ReadToEnd();

                // Rewrite 'src' or 'href' attributes which are starts with './' to PathBase.
                content = Regex.Replace(content, "(href|src)=\"./", "$1=\"" + pathBase + "/")
                    // Rewrite config for channel endpoint path
                    .Replace("data-rin-config-path-base=\"\"", "data-rin-config-path-base=\"" + pathBase + "\"");
                return new MemoryStream(new UTF8Encoding(false).GetBytes(content));
            }
        }

        // WORKAROUND: `GZipStream` uses Synchronous IO method in `Dispose` method. It causes the exception to be thrown on ASP.NET Core 3.0 or later.
        internal class ForceAsyncStreamWrapper : Stream
        {
            public Stream BaseStream { get; }

            public ForceAsyncStreamWrapper(Stream stream)
            {
                BaseStream = stream;
            }

            public override void Flush()
            {
                BaseStream.FlushAsync().GetAwaiter().GetResult();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return BaseStream.ReadAsync(buffer, offset, count).GetAwaiter().GetResult();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return BaseStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                BaseStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                BaseStream.WriteAsync(buffer, offset, count).GetAwaiter().GetResult();
            }

            public override bool CanRead => BaseStream.CanRead;

            public override bool CanSeek => BaseStream.CanSeek;

            public override bool CanWrite => BaseStream.CanWrite;

            public override long Length => BaseStream.Length;

            public override long Position
            {
                get => BaseStream.Position;
                set => BaseStream.Position = value;
            }
        }
    }
}
