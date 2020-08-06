using System;

namespace Rin.Storage.Redis
{
    public class RedisRecordStorageOptions
    {
        /// <summary>
        /// Expiration of a record in Redis.
        /// </summary>
        public TimeSpan Expiry { get; set; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// A prefix of the Redis keys for Rin records. If Rin is running on multiple applications, you can change the prefix to separate records.
        /// </summary>
        public string KeyPrefix { get; set; } = "Rin.Storage.";

        /// <summary>
        /// Redis database for Rin records.
        /// </summary>
        public int Database { get; set; } = -1;

        /// <summary>
        /// Connection configuration of Redis. The value is passed to <see cref="StackExchange.Redis.ConnectionMultiplexer"/>.
        /// </summary>
        public string ConnectionConfiguration { get; set; } = "localhost";
    }
}
