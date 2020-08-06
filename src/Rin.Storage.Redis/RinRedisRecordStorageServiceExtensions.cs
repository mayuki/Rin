using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rin.Core.Record;
using Rin.Storage.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RinRedisRecordStorageServiceExtensions
    {
        /// <summary>
        /// Add the Redis-backed <see cref="IRecordStorage"/> service and options.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        public static void AddRinRedisStorage(this IServiceCollection services, Action<RedisRecordStorageOptions>? configure = null)
        {
            services.AddOptions<RedisRecordStorageOptions>();
            services.Configure<RedisRecordStorageOptions>(configure);

            services.Replace(new ServiceDescriptor(typeof(IRecordStorage), typeof(RedisRecordStorage), ServiceLifetime.Singleton));
        }
    }
}
