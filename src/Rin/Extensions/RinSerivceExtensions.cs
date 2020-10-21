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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rin.Extensions;
using Rin.Features;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RinServiceExtensions
    {
        public static IRinBuilder AddRin(this IServiceCollection services, Action<RinOptions>? configure = null)
        {
            var options = new RinOptions();
            configure?.Invoke(options);

            services.AddHttpContextAccessor();

            // Other services
            services.AddSingleton<IRinRequestRecordingFeatureAccessor>(new RinRequestRecordingFeatureAccessor());
            services.AddSingleton<BodyDataTransformerSet>(serviceProvider =>
            {
                var requestTransformers = serviceProvider.GetServices<IRequestBodyDataTransformer>();
                var responseTransformers = serviceProvider.GetServices<IResponseBodyDataTransformer>();
                var transformers = serviceProvider.GetServices<IBodyDataTransformer>();

                return new BodyDataTransformerSet(new BodyDataTransformerPipeline(requestTransformers.Concat(transformers)), new BodyDataTransformerPipeline(responseTransformers.Concat(transformers)));
            });
            services.TryAddSingleton<IRecordStorage, InMemoryRecordStorage>();
            services.AddSingleton<IMessageEventBus<RequestEventMessage>, MessageEventBus<RequestEventMessage>>();
            services.AddSingleton<IMessageEventBus<StoreBodyEventMessage>, MessageEventBus<StoreBodyEventMessage>>();
            services.AddSingleton<RinOptions>(options);
            services.AddSingleton<RinChannel>();

            // IMessageSubscriber<RequestEventMessage> services
            services.AddSingleton<IMessageSubscriber<RequestEventMessage>>(x => new Rin.Hubs.RinCoreHub.MessageSubscriber(x.GetRequiredService<RinChannel>()));

            services.AddTransient<IResourceProvider, EmbeddedZipResourceProvider>();

            return new RinBuilder(services);
        }
    }
}
