using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Rin.Core.Event;
using Rin.Core.Record;
using Rin.Middlewares;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rin.Storage.Redis
{
    public class RedisRecordStorage : IRecordStorage
    {
        private const string RedisSubscriptionKey = "Rin.Storage.Redis.RedisRecordStorage-Subscription";
        private static readonly JsonSerializerSettings _jsonSerializerSettings;

        private readonly string _eventSourceKey = Guid.NewGuid().ToString();
        private readonly IMessageEventBus<RequestEventMessage> _eventBus;
        private readonly ConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redis;
        private ISubscriber _redisSubscriber;

        public static readonly Func<IServiceProvider, IRecordStorage> DefaultFactory = (services) =>
        {
            return new RedisRecordStorage(services.GetService<IMessageEventBus<RequestEventMessage>>());
        };

        static RedisRecordStorage()
        {
            _jsonSerializerSettings = new JsonSerializerSettings();
            _jsonSerializerSettings.Converters.Add(new StringValuesJsonConverter());
            _jsonSerializerSettings.Converters.Add(new IPAddressJsonConverter());
            _jsonSerializerSettings.Converters.Add(new QueryStringJsonConverter());
            _jsonSerializerSettings.Converters.Add(new PathStringJsonConverter());
            _jsonSerializerSettings.Converters.Add(new HostStringJsonConverter());
            _jsonSerializerSettings.Converters.Add(new TimelineEventJsonConverter());
        }

        public RedisRecordStorage(IMessageEventBus<RequestEventMessage> eventBus)
        {
            _eventBus = eventBus;
            _redisConnection = ConnectionMultiplexer.Connect("localhost");
            _redis = _redisConnection.GetDatabase();

            _redisSubscriber = _redisConnection.GetSubscriber();
            _redisSubscriber.Subscribe(RedisSubscriptionKey, (channel, value) =>
            {
                var message = Deserialize<RequestEventMessage>(value);

                // Ignore a messages from this storage.
                if (message.EventSource == _eventSourceKey) return;

                _eventBus.PostAsync(message);
            });
        }

        public async Task AddAsync(HttpRequestRecord entry)
        {
            await Task.WhenAll(
                _redis.ListLeftPushAsync("Rin.Storage.Records", entry.Id),
                _redis.StringSetAsync($"Rin.Storage.RecordEntry?{entry.Id}", Serialize(entry)),
                _redis.StringSetAsync($"Rin.Storage.RecordEntryInfo?{entry.Id}", Serialize(HttpRequestRecordInfo.CreateFromRecord(entry)))
            );
            await Task.WhenAll(
                _redis.ListTrimAsync("Rin.Storage.Records", 0, 99),
                _redis.KeyExpireAsync("Rin.Storage.Records", TimeSpan.FromMinutes(30)),
                _redis.KeyExpireAsync($"Rin.Storage.RecordEntry?{entry.Id}", TimeSpan.FromMinutes(30)),
                _redis.KeyExpireAsync($"Rin.Storage.RecordEntryInfo?{entry.Id}", TimeSpan.FromMinutes(30))
            );
        }

        public async Task<HttpRequestRecordInfo[]> GetAllAsync()
        {
            var ids = (await _redis.ListRangeAsync("Rin.Storage.Records")).ToStringArray();
            return (await Task.WhenAll(ids.Select(async x => Deserialize<HttpRequestRecordInfo>(await _redis.StringGetAsync($"Rin.Storage.RecordEntryInfo?{x}")))))
                .Where(x => x != null)
                .ToArray();
        }

        public async Task<RecordStorageTryGetResult> TryGetByIdAsync(string id)
        {
            var result = await _redis.StringGetAsync($"Rin.Storage.RecordEntry?{id}");
            if (result.HasValue)
            {
                return new RecordStorageTryGetResult(true, Deserialize<HttpRequestRecord>(result));
            }
            else
            {
                return new RecordStorageTryGetResult(false, null);
            }
        }

        public async Task UpdateAsync(HttpRequestRecord entry)
        {
            await Task.WhenAll(
                _redis.StringSetAsync($"Rin.Storage.RecordEntry?{entry.Id}", Serialize(entry)),
                _redis.StringSetAsync($"Rin.Storage.RecordEntryInfo?{entry.Id}", Serialize(HttpRequestRecordInfo.CreateFromRecord(entry)))
            );
        }

        async void IMessageSubscriber<RequestEventMessage>.Publish(RequestEventMessage message)
        {
            // Accept messages from Middleware. Drop if the message was published from other sources.
            if (message.EventSource != RequestRecorderMiddleware.EventSourceName) return;

            // Store a record into Redis and publish message to other servers.
            switch (message.Event)
            {
                case RequestEvent.BeginRequest:
                    await AddAsync(message.Value);
                    await _redis.PublishAsync(RedisSubscriptionKey, Serialize(new RequestEventMessage(_eventSourceKey, message.Value, RequestEvent.BeginRequest)));
                    break;
                case RequestEvent.CompleteRequest:
                    await UpdateAsync(message.Value);
                    await _redis.PublishAsync(RedisSubscriptionKey, Serialize(new RequestEventMessage(_eventSourceKey, message.Value, RequestEvent.CompleteRequest)));
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
