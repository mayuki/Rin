using Microsoft.AspNetCore.Http;
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
using Microsoft.Extensions.Hosting;

namespace Rin.Middlewares
{
    internal class ChannelMiddleware
    {
        private readonly IRecordStorage _storage;
        private readonly RinChannel _rinChannel;
        private readonly BodyDataTransformerSet _bodyDataTransformerSet;

        private readonly RequestDelegate _next;

        public ChannelMiddleware(RequestDelegate next, IRecordStorage storage, RinChannel rinChannel, BodyDataTransformerSet bodyDataTransformerSet, IHostApplicationLifetime applicationLifetime)
        {
            _next = next;

            _storage = storage;
            _rinChannel = rinChannel;
            _bodyDataTransformerSet = bodyDataTransformerSet;

            applicationLifetime.ApplicationStopping.Register(() => _rinChannel.Dispose());
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var conn = await context.WebSockets.AcceptWebSocketAsync();

                var hubName = default(string);
                if (context.Request.Query.TryGetValue("hub", out var hubName_))
                {
                    hubName = hubName_.ToString();
                }

                IHub hub;
                switch (hubName)
                {
                    case nameof(RinCoreHub):
                    default:
                        hub = new RinCoreHub(_storage, _rinChannel, _bodyDataTransformerSet);
                        break;
                }

                await _rinChannel.ManageAsync(conn, hub);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
    }
}
