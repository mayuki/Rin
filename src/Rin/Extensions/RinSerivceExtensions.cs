using Rin.Channel;
using Rin.Core;
using Rin.Core.Event;
using Rin.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RinSerivceExtensions
    {
        public static void AddRin(this IServiceCollection services, Action<RinOptions> configure = null)
        {
            var options = new RinOptions();
            configure?.Invoke(options);

            services.AddHttpContextAccessor();

            var channel = new RinChannel();
            var storage = new InMemoryMessageStorage<HttpRequestRecord>(options.RequestRecorder.RetentionMaxRequests);
            var eventBus = new MessageEventBus<HttpRequestRecord>(new[] { storage });

            services.AddSingleton<IMessageStorage<HttpRequestRecord>>(storage);
            services.AddSingleton<IMessageEventBus<HttpRequestRecord>>(eventBus);
            services.AddSingleton<RinOptions>(options);
            services.AddSingleton<RinChannel>(channel);
        }
    }
}
