using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Rin.Core.Record;
using Microsoft.Extensions.Primitives;

namespace Rin.Storage.Redis
{
    internal class TimeSpanJsonConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeSpan.FromTicks(reader.GetInt64());
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Ticks, options);
        }
    }

    internal class IPAddressJsonConverter : JsonConverter<System.Net.IPAddress>
    {
        public override IPAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (!string.IsNullOrEmpty(value))
            {
                return IPAddress.Parse(value);
            }
            return default;
        }

        public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    internal class QueryStringJsonConverter : JsonConverter<QueryString>
    {
        public override QueryString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return new QueryString(value ?? "");
        }

        public override void Write(Utf8JsonWriter writer, QueryString value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    internal class PathStringJsonConverter : JsonConverter<PathString>
    {
        public override PathString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return new PathString(value ?? "");
        }

        public override void Write(Utf8JsonWriter writer, PathString value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    internal class HostStringJsonConverter : JsonConverter<HostString>
    {
        public override HostString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return new HostString(value ?? "");
        }

        public override void Write(Utf8JsonWriter writer, HostString value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    internal class StringValuesJsonConverter : JsonConverter<StringValues>
    {
        public override StringValues Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new StringValues(JsonSerializer.Deserialize<string[]>(ref reader, options));
        }

        public override void Write(Utf8JsonWriter writer, StringValues value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.ToArray(), options);
        }
    }

    internal class TimelineEventJsonConverter : JsonConverter<ITimelineEvent>
    {
        public override ITimelineEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var readerTmp = reader;
            var timelineEvent = JsonSerializer.Deserialize<TimelineEvent_>(ref readerTmp, options);

            switch (timelineEvent.EventType)
            {
                case nameof(TimelineScope):
                    return JsonSerializer.Deserialize<ITimelineScope>(ref reader, options);
                case nameof(TimelineStamp):
                    return JsonSerializer.Deserialize<ITimelineStamp>(ref reader, options);
                default:
                    throw new NotSupportedException();
            }
        }

        public override void Write(Utf8JsonWriter writer, ITimelineEvent value, JsonSerializerOptions options)
        {
            if (value is ITimelineScope timelineScope)
            {
                JsonSerializer.Serialize(writer, timelineScope, options);
            }
            else if (value is ITimelineStamp timelineStamp)
            {
                JsonSerializer.Serialize(writer, timelineStamp, options);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }

    internal class TimelineScopeJsonConverter : JsonConverter<ITimelineScope>
    {
        public override ITimelineScope Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<TimelineScope_>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, ITimelineScope value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new TimelineScope_(value), options);
        }
    }

    internal class TimelineStampJsonConverter : JsonConverter<ITimelineStamp>
    {
        public override ITimelineStamp Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<TimelineStamp_>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, ITimelineStamp value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new TimelineStamp_(value), options);
        }
    }

    internal class TimelineEvent_
    {
        public string EventType { get; set; }
    }

    internal class TimelineScope_ : ITimelineScope
    {
        public string EventType { get; set; }

        public TimeSpan Duration { get; set; }

        public IReadOnlyCollection<ITimelineEvent> Children { get; set; }

        public string Name { get; set; }
        public string Category { get; set; }
        public string Data { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public void Complete()
        {
        }

        public void Dispose()
        {
        }

        public TimelineScope_()
        {}

        public TimelineScope_(ITimelineScope timelineScope)
        {
            EventType = timelineScope.EventType;
            Duration = timelineScope.Duration;
            Children = timelineScope.Children;
            Name = timelineScope.Name;
            Category = timelineScope.Category;
            Data = timelineScope.Data;
            Timestamp = timelineScope.Timestamp;
        }
    }

    internal class TimelineStamp_ : ITimelineStamp
    {
        public string EventType { get; set; }

        public string Name { get; set; }
        public string Category { get; set; }
        public string Data { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public TimelineStamp_()
        {}

        public TimelineStamp_(ITimelineStamp timelineScope)
        {
            EventType = timelineScope.EventType;
            Name = timelineScope.Name;
            Category = timelineScope.Category;
            Data = timelineScope.Data;
            Timestamp = timelineScope.Timestamp;
        }
    }
}
