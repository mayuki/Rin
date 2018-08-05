using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Rin.Channel;
using Rin.Core;
using Rin.Core.Event;
using Rin.Core.Record;
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
        private readonly IMessageEventBus<RequestEventMessage> _eventBus;

        public RequestRecorderMiddleware(RequestDelegate next, IMessageEventBus<RequestEventMessage> eventBus, RinChannel rinChannel)
        {
            _next = next;
            _eventBus = eventBus;
        }

        public async Task InvokeAsync(HttpContext context, RinOptions options)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.Path.StartsWithSegments(options.Inspector.MountPath) || (options.RequestRecorder.Excludes?.Invoke(request) ?? false))
            {
                await _next(context);
                return;
            }

            var record = new HttpRequestRecord()
            {
                Id = Guid.NewGuid().ToString(),
                IsHttps = request.IsHttps,
                Host = request.Host,
                QueryString = request.QueryString,
                Path = request.Path,
                Method = request.Method,
                RequestReceivedAt = DateTime.Now,
                RequestHeaders = request.Headers.ToDictionary(k => k.Key, v => v.Value),
                RemoteIpAddress = request.HttpContext.Connection.RemoteIpAddress,
                Traces = new System.Collections.Concurrent.ConcurrentQueue<TraceLogRecord>()
            };

            await _eventBus.PostAsync(new RequestEventMessage(record, RequestEvent.BeginRequest));

            var feature = new RinRequestRecordingFeature(record);
            context.Features.Set<IRinRequestRecordingFeature>(feature);

            if (options.RequestRecorder.EnableBodyCapturing)
            {
                context.EnableResponseDataCapturing();
                request.EnableBuffering();
            }
            response.OnStarting(OnStarting, record);
            response.OnCompleted(OnCompleted, record);

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

                record.ResponseStatusCode = response.StatusCode;
                record.ResponseHeaders = response.Headers.ToDictionary(k => k.Key, v => v.Value);

                if (options.RequestRecorder.EnableBodyCapturing)
                {
                    var memoryStreamRequestBody = new MemoryStream();
                    request.Body.Position = 0; // rewind the stream to head
                    await request.Body.CopyToAsync(memoryStreamRequestBody);
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

            return _eventBus.PostAsync(new RequestEventMessage(record, RequestEvent.CompleteRequest)).AsTask();
        }
    }
}
