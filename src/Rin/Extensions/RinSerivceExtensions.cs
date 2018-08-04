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
            var storage = new InMemoryRecordStorage(options.RequestRecorder.RetentionMaxRequests);
            var eventBus = new MessageEventBus<RequestEventMessage>(new IMessageSubscriber<RequestEventMessage>[]
            {
                storage,
                new Rin.Hubs.RinCoreHub.MessageSubscriber(channel)
            });

            services.AddSingleton<IRecordStorage>(storage);
            services.AddSingleton<IMessageEventBus<RequestEventMessage>>(eventBus);
            services.AddSingleton<RinOptions>(options);
            services.AddSingleton<RinChannel>(channel);
        }
    }
}
