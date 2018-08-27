using Microsoft.Extensions.DependencyInjection;
using Rin.Features;
using Rin.Mvc;
using Rin.Mvc.DiagnosticListeners;
using Rin.Mvc.Filters;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class RinMvcExtensions
    {
        public static IMvcBuilder AddRinMvcSupport(this IMvcBuilder builder)
        {
            builder.AddMvcOptions(options =>
            {
                options.Filters.Add(new SetRequestIdActionFilter());
            });

            return builder;
        }

        public static void UseRinMvcSupport(this IApplicationBuilder app)
        {
            var diagnosticListener = app.ApplicationServices.GetService(typeof(DiagnosticListener)) as DiagnosticListener;
            diagnosticListener.SubscribeWithAdapter(new AspNetMvcCoreDiagnosticListener());

            app.Use(async (context, next) =>
            {
                var feature = context.Features.Get<IRinRequestRecordingFeature>();
                if (feature == null) goto NEXT;

                if (!context.Request.Cookies.TryGetValue(Constants.CookieName, out var cookieValue)) goto NEXT;

                feature.Record.ParentId = cookieValue;

                NEXT:

                await next();
            });
        }
    }
}
