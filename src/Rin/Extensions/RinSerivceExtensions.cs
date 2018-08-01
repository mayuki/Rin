using Rin.Channel;
using Rin.Core;
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

            services.AddSingleton<RequestRecordStorage>(new RequestRecordStorage());
            services.AddSingleton<RinOptions>(options);
            services.AddSingleton<RinChannel>(new RinChannel());
        }
    }
}
