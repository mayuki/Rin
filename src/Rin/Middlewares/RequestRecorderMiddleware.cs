using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Rin.Channel;
using Rin.Core;
using Rin.Core.Event;
using Rin.Extensions;
using Rin.Features;
using Rin.Hubs;
using Rin.Hubs.HubClients;
using Rin.Hubs.Payloads;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rin.Middlewares
{
    public class RequestRecorderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMessageEventBus<HttpRequestRecord> _eventBus;
        private readonly IRinCoreHubClient _hubClient;

        public RequestRecorderMiddleware(RequestDelegate next, IMessageEventBus<HttpRequestRecord> eventBus, RinChannel rinChannel)
        {
            _next = next;
            _eventBus = eventBus;
            _hubClient = rinChannel.GetClient<RinCoreHub, IRinCoreHubClient>();
        }

        public async Task InvokeAsync(HttpContext context, RinOptions options)
        {
            if (context.Request.Path.StartsWithSegments(options.Inspector.MountPath) || (options.RequestRecorder.Excludes?.Invoke(context.Request.Path) ?? false))
            {
                await _next(context);
                return;
            }

            var record = new HttpRequestRecord()
            {
                Id = Guid.NewGuid().ToString(),
                IsHttps = context.Request.IsHttps,
                Host = context.Request.Host,
                QueryString = context.Request.QueryString,
                Path = context.Request.Path,
                Method = context.Request.Method,
                RequestReceivedAt = DateTime.Now,
                RequestHeaders = context.Request.Headers.ToDictionary(k => k.Key, v => v.Value),
                RemoteIpAddress = context.Request.HttpContext.Connection.RemoteIpAddress,
                Traces = new System.Collections.Concurrent.ConcurrentQueue<TraceLogRecord>()
            };

            await _eventBus.PostAsync(record);

            _hubClient.RequestBegin(new RequestEventPayload(record)).Forget();

            var feature = new RinRequestRecordingFeature(record);
            context.Features.Set<IRinRequestRecordingFeature>(feature);

            if (options.RequestRecorder.EnableBodyCapturing)
            {
                context.EnableResponseDataCapturing();
                context.Request.EnableBuffering();
            }
            context.Response.OnStarting(OnStarting, record);
            context.Response.OnCompleted(OnCompleted, record);

            record.ProcessingStartedAt = DateTime.Now;
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                record.Exception = ex;
                throw;
            }
            finally
            {
                record.ProcessingCompletedAt = DateTime.Now;

                record.ResponseStatusCode = context.Response.StatusCode;
                record.ResponseHeaders = context.Response.Headers.ToDictionary(k => k.Key, v => v.Value);

                if (options.RequestRecorder.EnableBodyCapturing)
                {
                    var memoryStreamRequestBody = new MemoryStream();
                    await context.Request.Body.CopyToAsync(memoryStreamRequestBody);
                    record.RequestBody = memoryStreamRequestBody.ToArray();
                    record.ResponseBody = feature.ResponseDataStream.GetCapturedData();
                }

                var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionFeature != null)
                {
                    record.Exception = exceptionFeature.Error;
                }
            }
        }

        private Task OnStarting(object state)
        {
            var record = ((HttpRequestRecord)state);
            record.TransferringStartedAt = DateTime.Now;
            return Task.CompletedTask;
        }

        private Task OnCompleted(object state)
        {
            var record = ((HttpRequestRecord)state);

            record.TransferringCompletedAt = DateTime.Now;

            _hubClient.RequestEnd(new RequestEventPayload(record)).Forget();

            return Task.CompletedTask;
        }
    }
}
