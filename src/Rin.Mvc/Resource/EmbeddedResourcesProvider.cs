using Microsoft.AspNetCore.Http;
using Rin.Core.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rin.Mvc.Resource
{
    public class EmbeddedResourcesProvider : IResourceProvider
    {
        private static Dictionary<string, (string Content, string ContentType)> _resourceNameByPath = new Dictionary<string, (string Content, string ContentType)>();

        static EmbeddedResourcesProvider()
        {
            var asm = typeof(EmbeddedResourcesProvider).Assembly;
            _resourceNameByPath = asm.GetManifestResourceNames()
                .ToDictionary(
                    k => Constants.MvcStaticResourcesRoot + k.Replace(asm.GetName().Name + ".EmbeddedResources.", ""),
                    v =>
                    {
                        using (var reader = new StreamReader(asm.GetManifestResourceStream(v)))
                        {
                            return (reader.ReadToEnd(), Resources.GetContentType(v));
                        }
                    });
        }

        public async Task<bool> TryProcessAsync(HttpContext context)
        {
            if (_resourceNameByPath.TryGetValue(context.Request.Path, out var content))
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = content.ContentType;

                await context.Response.WriteAsync(content.Content);
                return true;
            }

            return false;
        }
    }
}
