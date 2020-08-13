using Microsoft.Extensions.DependencyInjection;
using Rin.Core;
using Rin.Core.Event;
using Rin.Core.Record;
using Rin.Middlewares;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Rin.Storage.Redis
{
    /// <summary>
    /// This storage persists a request records and provides pub/sub. It is backed by Redis.
    /// </summary>
    public class RedisRecordStorage : IRecordStorage
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions;
        private static readonly string _serializeVersion;
        private const string RedisSubscriptionKey = "RedisRecordStorage-Subscription";

        private readonly RinOptions _rinOptions;
        private readonly RedisRecordStorageOptions _options;
        private readonly string _eventSourceKey = Guid.NewGuid().ToString();
        private readonly IMessageEventBus<RequestEventMessage> _eventBus;
        private readonly ConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redis;
        private ISubscriber _redisSubscriber;

        static RedisRecordStorage()
        {
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new TimeSpanJsonConverter());
            _jsonSerializerOptions.Converters.Add(new StringValuesJsonConverter());
            _jsonSerializerOptions.Converters.Add(new IPAddressJsonConverter());
            _jsonSerializerOptions.Converters.Add(new QueryStringJsonConverter());
            _jsonSerializerOptions.Converters.Add(new PathStringJsonConverter());
            _jsonSerializerOptions.Converters.Add(new HostStringJsonConverter());
            _jsonSerializerOptions.Converters.Add(new TimelineScopeJsonConverter());
            _jsonSerializerOptions.Converters.Add(new TimelineStampJsonConverter());
            _jsonSerializerOptions.Converters.Add(new TimelineEventJsonConverter());

            _serializeVersion = typeof(Rin.Core.Record.HttpRequestRecord).Assembly.GetName().Version!.ToString();
        }

        public RedisRecordStorage(IOptions<RedisRecordStorageOptions> options, IOptions<RinOptions> rinOptions, IMessageEventBus<RequestEventMessage> eventBus)
        {
            _options = options.Value;
            _rinOptions = rinOptions.Value;

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
                _redis.StringSetAsync(GetRedisKey($"RecordEntry?{entry.Id}"), Serialize(entry), _options.Expiry),
                _redis.StringSetAsync(GetRedisKey($"RecordEntryInfo?{entry.Id}"), Serialize(HttpRequestRecordInfo.CreateFromRecord(entry)), _options.Expiry)
            );
            await Task.WhenAll(
                _redis.ListTrimAsync(GetRedisKey($"Records"), 0, _rinOptions.RequestRecorder.RetentionMaxRequests),
                _redis.KeyExpireAsync(GetRedisKey($"Records"), _options.Expiry)
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
                _redis.StringSetAsync(GetRedisKey($"RecordEntry?{entry.Id}"), Serialize(entry), _options.Expiry),
                _redis.StringSetAsync(GetRedisKey($"RecordEntryInfo?{entry.Id}"), Serialize(HttpRequestRecordInfo.CreateFromRecord(entry)), _options.Expiry)
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
                    await _redis.StringSetAsync(GetRedisKey($"RecordEntry.RequestBody?{message.Id}"), message.Body, _options.Expiry);
                    break;
                case StoreBodyEvent.Response:
                    await _redis.StringSetAsync(GetRedisKey($"RecordEntry.ResponseBody?{message.Id}"), message.Body, _options.Expiry);
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
            return JsonSerializer.Serialize(value, _jsonSerializerOptions);
        }

        private T Deserialize<T>(string value)
        {
            if (value == null) return default(T);

            var result = JsonSerializer.Deserialize<T>(value, _jsonSerializerOptions);
            return result;
        }
    }
}
