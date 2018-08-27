using Microsoft.Extensions.DependencyInjection;
using Rin.Features;
using Rin.Mvc;
using Rin.Mvc.DiagnosticListeners;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class RinMvcExtensions
    {
        public static void UseRinMvcSupport(this IApplicationBuilder app)
        {
            var diagnosticListener = app.ApplicationServices.GetService(typeof(DiagnosticListener)) as DiagnosticListener;
            diagnosticListener.SubscribeWithAdapter(new AspNetMvcCoreDiagnosticListener());

            app.Use(async (context, next) =>
            {
                var feature = context.Features.Get<IRinRequestRecordingFeature>();
                if (feature == null) goto NEXT;

                if (!context.Request.Headers.TryGetValue(Constants.RequestIdHeaderName, out var headerValue) || String.IsNullOrEmpty(headerValue)) goto NEXT;

                feature.Record.ParentId = headerValue.ToString();

                NEXT:

                await next();
            });
        }
    }
}
