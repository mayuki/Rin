using Microsoft.AspNetCore.Http;
using Rin.Core;
using Rin.FrontendResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rin.Middlewares
{
    public class ResourcesMiddleware
    {
        private static readonly object _syncLock = new object();

        public ResourcesMiddleware(RequestDelegate next)
        {
        }

        public Task InvokeAsync(HttpContext context)
        {
            if (String.IsNullOrEmpty(context.Request.Path.Value))
            {
                context.Response.Redirect(context.Request.PathBase + "/", true);
                return Task.CompletedTask;
            }

            // TODO: ZipArchive(ZipArchiveEntry) provides a resource stream.
            // But the archive doesn't guarantee thread-safety. Only one stream can be open at once.
            lock (_syncLock)
            {
                Stream stream;
                String contentType;

                if (Resources.TryOpen(context.Request.Path.Value.TrimStart('/'), out stream, out contentType))
                {
                    using (stream)
                    {
                        WriteStreamToClientAsync(context, stream, contentType).Wait();
                        return Task.CompletedTask;
                    }
                }
                // for SPA (+ rewrite paths in HTML)
                else if (
                    context.Request.Headers.TryGetValue("Accept", out var acceptHeaders) &&
                    acceptHeaders.Any(x => x.Contains("text/html")) &&
                    Resources.TryOpen("index.html", out stream, out contentType)
                )
                {
                    using (stream)
                    {
                        WriteStreamToClientAsync(context,
                            RewriteHtmlIfNeeded(stream, contentType, context.Request.PathBase), contentType).Wait();
                        return Task.CompletedTask;
                    }
                }
            }


            context.Response.StatusCode = 404;
            return Task.CompletedTask;
        }

        private async Task WriteStreamToClientAsync(HttpContext context, Stream stream, string contentType)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = contentType;
            if (context.Request.Headers.TryGetValue("accept-encoding", out var headerValues) && headerValues.Any(x => x.Contains("gzip")))
            {
                context.Response.Headers["Content-Encoding"] = "gzip";
                using (var outputStream = new GZipStream(context.Response.Body, CompressionLevel.Fastest, true))
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
    }
}
