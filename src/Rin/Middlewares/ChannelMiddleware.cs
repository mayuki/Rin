using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Rin.Channel;
using Rin.Core;
using Rin.Core.Event;
using Rin.Core.Record;
using Rin.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rin.Middlewares
{
    internal class ChannelMiddleware
    {
        private readonly IRecordStorage _storage;
        private readonly RinChannel _rinChannel;
        private readonly IBodyDataTransformer _bodyDataTransformer;

        private readonly RequestDelegate _next;

        public ChannelMiddleware(RequestDelegate next, IRecordStorage storage, RinChannel rinChannel, IBodyDataTransformer bodyDataTransformer, IApplicationLifetime applicationLifetime)
        {
            _next = next;

            _storage = storage;
            _rinChannel = rinChannel;
            _bodyDataTransformer = bodyDataTransformer;

            applicationLifetime.ApplicationStopping.Register(() => _rinChannel.Dispose());
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var conn = await context.WebSockets.AcceptWebSocketAsync();

                var hub = new RinCoreHub(_storage, _rinChannel, _bodyDataTransformer);
                await _rinChannel.ManageAsync(conn, hub);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
    }
}
