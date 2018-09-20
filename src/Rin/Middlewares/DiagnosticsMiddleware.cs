using Microsoft.AspNetCore.Http;
using Rin.Core.Record;
using Rin.Features;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rin.Middlewares
{
    public class DiagnosticsMiddleware
    {
        private readonly RequestDelegate _next;

        public DiagnosticsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                var feature = context.Features.Get<IRinRequestRecordingFeature>();
                if (feature != null)
                {
                    feature.Record.Exception = new ExceptionData(ex);
                }
                throw;
            }
        }
    }
}
