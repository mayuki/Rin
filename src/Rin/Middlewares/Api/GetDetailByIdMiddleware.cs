using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Rin.Core.Record;
using Rin.Hubs.Payloads;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rin.Middlewares.Api
{
    public class GetDetailByIdMiddleware
    {
        private IRecordStorage _storage;

        public GetDetailByIdMiddleware(RequestDelegate next, IRecordStorage storage)
        {
            _storage = storage;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Query.TryGetValue("id", out var id))
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Missing required parameter: id");
                return;
            }

            var result = await _storage.TryGetDetailByIdAsync(id);
            if (!result.Succeed || result.Value == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new RequestRecordDetailPayload(result.Value)));
        }
    }
}
