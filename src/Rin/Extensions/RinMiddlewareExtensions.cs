using Rin.Core;
using Rin.Middlewares;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class RinMiddlewareExtensions
    {
        public static void UseRin(this IApplicationBuilder app)
        {
            app.UseRinInspector();
            app.UseRinRecorder();
        }

        private static void UseRinInspector(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService(typeof(RinOptions)) as RinOptions;

            app.Map(options.Inspector.MountPath, branch =>
            {
                branch.UseWebSockets();
                branch.UseRinInspectorApi();
                branch.UseRinDownloadEndpoints();

                // ResourcesMiddleware must be last at pipeline.
                branch.UseRinEmbeddedResources();
            });
        }

        private static void UseRinDownloadEndpoints(this IApplicationBuilder app)
        {
            app.Map("/download/request", x => x.UseMiddleware<DownloadRequestBodyMiddleware>());
            app.Map("/download/response", x => x.UseMiddleware<DownloadResponseBodyMiddleware>());
        }

        private static void UseRinEmbeddedResources(this IApplicationBuilder app)
        {
            app.UseMiddleware<ResourcesMiddleware>();
        }

        private static void UseRinInspectorApi(this IApplicationBuilder app)
        {
            app.Map("/chan", x => x.UseMiddleware<ChannelMiddleware>());
        }

        private static void UseRinRecorder(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestRecorderMiddleware>();
        }

        public static void UseRinDiagnosticsHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<DiagnosticsMiddleware>();
        }
    }
}
