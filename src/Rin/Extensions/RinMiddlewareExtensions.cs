using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Rin.Core;
using Rin.Core.Event;
using Rin.Core.Record;
using Rin.Middlewares;
using Rin.Middlewares.Api;
using System;
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
            var options = app.ApplicationServices.GetService(typeof(RinOptions)) as RinOptions;
            if (options == null)
            {
                throw new InvalidOperationException("Rin Services are not registered. Please call 'services.AddRin()' in a Startup class");
            }

            var env = app.ApplicationServices.GetService(typeof(IHostingEnvironment)) as IHostingEnvironment;
            if (env.IsProduction() && !options.RequestRecorder.AllowRunningOnProduction)
            {
                throw new InvalidOperationException("Rin requires non-Production environment to run. If you want to run in Production environment, configure AllowRunningOnProduction option.");
            }

            app.UseRinMessageBus();

            app.UseRinInspector();
            app.UseRinRecorder();
        }

        private static void UseRinMessageBus(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetService<IMessageEventBus<RequestEventMessage>>();
            var eventBusStoreBody = app.ApplicationServices.GetService<IMessageEventBus<StoreBodyEventMessage>>();

            var subscribers = app.ApplicationServices.GetServices<IMessageSubscriber<RequestEventMessage>>();
            var subscribersStoreBody = app.ApplicationServices.GetServices<IMessageSubscriber<StoreBodyEventMessage>>();
            var recoder = app.ApplicationServices.GetService<IRecordStorage>();

            eventBus.Subscribe(subscribers.Concat(new[] { recoder }));
            eventBusStoreBody.Subscribe(subscribersStoreBody.Concat(new[] { recoder }));
        }

        private static void UseRinInspector(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService(typeof(RinOptions)) as RinOptions;

            app.Map(options.Inspector.MountPath, branch =>
            {
                branch.UseWebSockets();
                branch.UseRinApi();
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

        private static void UseRinApi(this IApplicationBuilder app)
        {
            app.Map("/api/GetDetailById", x => x.UseMiddleware<GetDetailByIdMiddleware>());
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
