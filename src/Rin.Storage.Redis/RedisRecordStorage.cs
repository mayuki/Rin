using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Rin.Core;
using Rin.Core.Event;
using Rin.Core.Record;
using Rin.Middlewares;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rin.Storage.Redis
{
    public class RedisRecordStorageOptions
    {
        public TimeSpan Expiry { get; set; } = TimeSpan.FromMinutes(30);
        public string KeyPrefix { get; set; } = "Rin.Storage.";
        public int RetentionMaxRequests { get; set; } = 100;
        public int Database { get; set; } = -1;
        public string ConnectionConfiguration { get; set; } = "localhost";
    }

    public class RedisRecordStorage : IRecordStorage
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings;
        private static readonly string _serializeVersion;
        private const string RedisSubscriptionKey = "RedisRecordStorage-Subscription";

        private readonly RedisRecordStorageOptions _options;
        private readonly string _eventSourceKey = Guid.NewGuid().ToString();
        private readonly IMessageEventBus<RequestEventMessage> _eventBus;
        private readonly ConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redis;
        private ISubscriber _redisSubscriber;

        public static Func<IServiceProvider, IRecordStorage> DefaultFactoryWithOptions(Action<RedisRecordStorageOptions> configure)
        {
            return (services) =>
            {
                var retentionMaxRequests = services.GetService<RinOptions>().RequestRecorder.RetentionMaxRequests;
                var options = new RedisRecordStorageOptions() { RetentionMaxRequests = retentionMaxRequests };
                configure?.Invoke(options);

                return new RedisRecordStorage(options, services.GetService<IMessageEventBus<RequestEventMessage>>());
            };
        }

        static RedisRecordStorage()
        {
            _jsonSerializerSettings = new JsonSerializerSettings();
            _jsonSerializerSettings.Converters.Add(new StringValuesJsonConverter());
            _jsonSerializerSettings.Converters.Add(new IPAddressJsonConverter());
            _jsonSerializerSettings.Converters.Add(new QueryStringJsonConverter());
            _jsonSerializerSettings.Converters.Add(new PathStringJsonConverter());
            _jsonSerializerSettings.Converters.Add(new HostStringJsonConverter());
            _jsonSerializerSettings.Converters.Add(new TimelineEventJsonConverter());

            _serializeVersion = typeof(Rin.Core.Record.HttpRequestRecord).Assembly.GetName().Version.ToString();
        }

        public RedisRecordStorage(RedisRecordStorageOptions options, IMessageEventBus<RequestEventMessage> eventBus)
        {
            _options = options;
            _eventBus = eventBus;
            _redisConnection = ConnectionMultiplexer.Connect(_options.ConnectionConfiguration);
            _redis = _redisConnection.GetDatabase(_options.Database);

            _redisSubscriber = _redisConnection.GetSubscriber();
            _redisSubscriber.Subscribe(GetRedisKey(RedisSubscriptionKey), (channel, value) =>
            {
                var message = Deserialize<RequestEventMessage>(value);

                // Ignore a messages from this storage.
                if (message.EventSource == _eventSourceKey) return;

                _eventBus.PostAsync(message);
            });
        }

        private string GetRedisKey(string key)
        {
            return $"{_options.KeyPrefix}[{_serializeVersion}].{key}";
        }

        public async Task AddAsync(HttpRequestRecord entry)
        {
            await Task.WhenAll(
                _redis.ListLeftPushAsync(GetRedisKey($"Records"), entry.Id),
                _redis.StringSetAsync(GetRedisKey($"RecordEntry?{entry.Id}"), Serialize(entry)),
                _redis.StringSetAsync(GetRedisKey($"RecordEntryInfo?{entry.Id}"), Serialize(HttpRequestRecordInfo.CreateFromRecord(entry)))
            );
            await Task.WhenAll(
                _redis.ListTrimAsync(GetRedisKey($"Records"), 0, _options.RetentionMaxRequests),
                _redis.KeyExpireAsync(GetRedisKey($"Records"), _options.Expiry),
                _redis.KeyExpireAsync(GetRedisKey($"RecordEntry?{entry.Id}"), _options.Expiry),
                _redis.KeyExpireAsync(GetRedisKey($"RecordEntryInfo?{entry.Id}"), _options.Expiry)
            );
        }

        public async Task<HttpRequestRecordInfo[]> GetAllAsync()
        {
            var ids = (await _redis.ListRangeAsync(GetRedisKey($"Records"))).ToStringArray();
            return (await Task.WhenAll(ids.Select(async x => Deserialize<HttpRequestRecordInfo>(await _redis.StringGetAsync(GetRedisKey($"RecordEntryInfo?{x}"))))))
                .Where(x => x != null)
                .ToArray();
        }

        public async Task<RecordStorageTryGetResult<HttpRequestRecord>> TryGetDetailByIdAsync(string id)
        {
            var result = await _redis.StringGetAsync(GetRedisKey($"RecordEntry?{id}"));
            if (result.HasValue)
            {
                return RecordStorageTryGetResult.Create(true, Deserialize<HttpRequestRecord>(result));
            }
            else
            {
                return RecordStorageTryGetResult.Create(false, default(HttpRequestRecord));
            }
        }

        public async Task UpdateAsync(HttpRequestRecord entry)
        {
            await Task.WhenAll(
                _redis.StringSetAsync(GetRedisKey($"RecordEntry?{entry.Id}"), Serialize(entry)),
                _redis.StringSetAsync(GetRedisKey($"RecordEntryInfo?{entry.Id}"), Serialize(HttpRequestRecordInfo.CreateFromRecord(entry)))
            );
        }

        public async Task<RecordStorageTryGetResult<byte[]>> TryGetResponseBodyByIdAsync(string id)
        {
            var result = await _redis.StringGetAsync(GetRedisKey($"RecordEntry.ResponseBody?{id}"));
            if (result.HasValue)
            {
                return RecordStorageTryGetResult.Create(true, (byte[])result);
            }
            else
            {
                return RecordStorageTryGetResult.Create(false, default(byte[]));
            }
        }

        public async Task<RecordStorageTryGetResult<byte[]>> TryGetRequestBodyByIdAsync(string id)
        {
            var result = await _redis.StringGetAsync(GetRedisKey($"RecordEntry.RequestBody?{id}"));
            if (result.HasValue)
            {
                return RecordStorageTryGetResult.Create(true, (byte[])result);
            }
            else
            {
                return RecordStorageTryGetResult.Create(false, default(byte[]));
            }
        }

        async Task IMessageSubscriber<StoreBodyEventMessage>.Publish(StoreBodyEventMessage message)
        {
            switch (message.Event)
            {
                case StoreBodyEvent.Request:
                    await _redis.StringSetAsync(GetRedisKey($"RecordEntry.RequestBody?{message.Id}"), message.Body, expiry: _options.Expiry);
                    break;
                case StoreBodyEvent.Response:
                    await _redis.StringSetAsync(GetRedisKey($"RecordEntry.ResponseBody?{message.Id}"), message.Body, expiry: _options.Expiry);
                    break;
            }
        }

        async Task IMessageSubscriber<RequestEventMessage>.Publish(RequestEventMessage message)
        {
            // Accept messages from Middleware. Drop if the message was published from other sources.
            if (message.EventSource != RequestRecorderMiddleware.EventSourceName) return;

            // Store a record into Redis and publish message to other servers.
            switch (message.Event)
            {
                case RequestEvent.BeginRequest:
                    await AddAsync(message.Value);
                    await _redis.PublishAsync(GetRedisKey(RedisSubscriptionKey), Serialize(new RequestEventMessage(_eventSourceKey, message.Value, RequestEvent.BeginRequest)));
                    break;
                case RequestEvent.CompleteRequest:
                    await UpdateAsync(message.Value);
                    await _redis.PublishAsync(GetRedisKey(RedisSubscriptionKey), Serialize(new RequestEventMessage(_eventSourceKey, message.Value, RequestEvent.CompleteRequest)));
                    break;
            }
        }

        public void Dispose()
        {
            _redisSubscriber?.UnsubscribeAll();
            _redisSubscriber = null;
        }

        private string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, _jsonSerializerSettings);
        }

        private T Deserialize<T>(string value)
        {
            if (value == null) return default(T);

            var result = JsonConvert.DeserializeObject<T>(value, _jsonSerializerSettings);
            return result;
        }
    }
}
