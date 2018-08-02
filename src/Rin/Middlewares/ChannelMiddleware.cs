using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Rin.Channel;
using Rin.Core;
using Rin.Core.Event;
using Rin.Core.Storage;
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
        private readonly IMessageStorage<HttpRequestRecord> _storage;
        private readonly IMessageEventBus<HttpRequestRecord> _eventBus;
        private readonly RinChannel _rinChannel;
        private readonly RequestDelegate _next;

        public ChannelMiddleware(RequestDelegate next, IMessageEventBus<HttpRequestRecord> eventBus, IMessageStorage<HttpRequestRecord> storage, RinChannel rinChannel, IApplicationLifetime applicationLifetime)
        {
            _next = next;

            _storage = storage;
            _eventBus = eventBus;
            _rinChannel = rinChannel;

            applicationLifetime.ApplicationStopping.Register(() => _rinChannel.Dispose());
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var conn = await context.WebSockets.AcceptWebSocketAsync();

                var hub = new RinCoreHub(_storage, _rinChannel);
                await _rinChannel.ManageAsync(conn, hub);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
    }
}
