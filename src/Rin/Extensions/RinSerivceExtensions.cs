using Rin.Channel;
using Rin.Core;
using Rin.Core.Event;
using Rin.Core.Record;
using Rin.Core.Resource;
using Rin.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RinServiceExtensions
    {
        public static void AddRin(this IServiceCollection services, Action<RinOptions> configure = null)
        {
            var options = new RinOptions();
            configure?.Invoke(options);

            services.AddHttpContextAccessor();

            var eventBus = new MessageEventBus<RequestEventMessage>();
            var eventBusStoreBody = new MessageEventBus<StoreBodyEventMessage>();
            var transformerSet = new BodyDataTransformerSet(
                new BodyDataTransformerPipeline(options.Inspector.RequestBodyDataTransformers),
                new BodyDataTransformerPipeline(options.Inspector.ResponseBodyDataTransformers)
            );

            // Other services
            services.AddSingleton<BodyDataTransformerSet>(transformerSet);
            services.AddSingleton<IRecordStorage>(options.RequestRecorder.StorageFactory);
            services.AddSingleton<IMessageEventBus<RequestEventMessage>>(eventBus);
            services.AddSingleton<IMessageEventBus<StoreBodyEventMessage>>(eventBusStoreBody);
            services.AddSingleton<RinOptions>(options);
            services.AddSingleton<RinChannel>();

            // IMessageSubscriber<RequestEventMessage> services
            services.AddSingleton<IMessageSubscriber<RequestEventMessage>>(x => new Rin.Hubs.RinCoreHub.MessageSubscriber(x.GetRequiredService<RinChannel>()));

            services.AddTransient<IResourceProvider, EmbeddedZipResourceProvider>();
        }
    }
}
