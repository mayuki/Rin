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
using Rin.Features;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RinSerivceExtensions
    {
        public static void AddRin(this IServiceCollection services, Action<RinOptions> configure = null)
        {
            var options = new RinOptions();
            configure?.Invoke(options);

            services.AddHttpContextAccessor();

            // Other services
            services.AddSingleton<IRinRequestRecordingFeatureAccessor>(new RinRequestRecordingFeatureAccessor());
            services.AddSingleton<BodyDataTransformerSet>(serviceProvider =>
            {
                var transformers = serviceProvider.GetServices<IBodyDataTransformer>().ToArray();
                return new BodyDataTransformerSet(new BodyDataTransformerPipeline(transformers), new BodyDataTransformerPipeline(transformers));
            });
            services.AddSingleton<IRecordStorage>(options.RequestRecorder.StorageFactory);
            services.AddSingleton<IMessageEventBus<RequestEventMessage>>(new MessageEventBus<RequestEventMessage>());
            services.AddSingleton<IMessageEventBus<StoreBodyEventMessage>>(new MessageEventBus<StoreBodyEventMessage>());
            services.AddSingleton<RinOptions>(options);
            services.AddSingleton<RinChannel>();

            // IMessageSubscriber<RequestEventMessage> services
            services.AddSingleton<IMessageSubscriber<RequestEventMessage>>(x => new Rin.Hubs.RinCoreHub.MessageSubscriber(x.GetRequiredService<RinChannel>()));

            services.AddTransient<IResourceProvider, EmbeddedZipResourceProvider>();
        }
    }
}
