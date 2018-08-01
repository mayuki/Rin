using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Rin.Channel;
using Rin.Core;
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
        private readonly RequestRecordStorage _requestEventStorage;
        private readonly RinChannel _rinChannel;
        private readonly RequestDelegate _next;

        public ChannelMiddleware(RequestDelegate next, RequestRecordStorage requestEventStorage, RinChannel rinChannel, IApplicationLifetime applicationLifetime)
        {
            _next = next;
            _requestEventStorage = requestEventStorage;
            _rinChannel = rinChannel;

            applicationLifetime.ApplicationStopping.Register(() => _rinChannel.Dispose());
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var conn = await context.WebSockets.AcceptWebSocketAsync();
                await _rinChannel.ManageAsync(conn, new RinCoreHub(_requestEventStorage, _rinChannel));
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
    }
}
