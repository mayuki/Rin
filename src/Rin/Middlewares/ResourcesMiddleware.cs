using Microsoft.AspNetCore.Http;
using Rin.Core.Resource;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rin.Middlewares
{
    public class ResourcesMiddleware
    {
        private IEnumerable<IResourceProvider> _resourceProviders;

        public ResourcesMiddleware(RequestDelegate next, IEnumerable<IResourceProvider> resourceProviders)
        {
            _resourceProviders = resourceProviders;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            foreach (var provider in _resourceProviders)
            {
                if (await provider.TryProcessAsync(context))
                {
                    return;
                }
            }

            context.Response.StatusCode = 404;
        }
    }
}
