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
    public static class RinSerivceExtensions
    {
        public static void AddRin(this IServiceCollection services, Action<RinOptions> configure = null)
        {
            var options = new RinOptions();
            configure?.Invoke(options);

            services.AddHttpContextAccessor();

            var channel = new RinChannel();
            var eventBus = new MessageEventBus<RequestEventMessage>();
            var transformerSet = new BodyDataTransformerSet(
                new BodyDataTransformerPipeline(options.Inspector.RequestBodyDataTransformers),
                new BodyDataTransformerPipeline(options.Inspector.ResponseBodyDataTransformers)
            );

            // IMessageSubscriber<RequestEventMessage> services
            services.AddSingleton<IMessageSubscriber<RequestEventMessage>>(new Rin.Hubs.RinCoreHub.MessageSubscriber(channel));

            // Other services
            services.AddSingleton<BodyDataTransformerSet>(transformerSet);
            services.AddSingleton<IRecordStorage>(options.RequestRecorder.StorageFactory);
            services.AddSingleton<IMessageEventBus<RequestEventMessage>>(eventBus);
            services.AddSingleton<RinOptions>(options);
            services.AddSingleton<RinChannel>(channel);

            services.AddTransient<IResourceProvider, EmbeddedZipResourceProvider>();
        }
    }
}
