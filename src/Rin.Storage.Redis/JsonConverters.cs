using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;
using Rin.Core.Record;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Primitives;

namespace Rin.Storage.Redis
{
    internal class IPAddressJsonConverter : JsonConverter<System.Net.IPAddress>
    {
        public override IPAddress ReadJson(JsonReader reader, Type objectType, IPAddress existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = reader.Value as string;
            if (!String.IsNullOrEmpty(value))
            {
                return IPAddress.Parse(value);
            }
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, IPAddress value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    internal class QueryStringJsonConverter : JsonConverter<QueryString>
    {
        public override QueryString ReadJson(JsonReader reader, Type objectType, QueryString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = reader.Value as string;
            return new QueryString(value ?? "");
        }

        public override void WriteJson(JsonWriter writer, QueryString value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    internal class PathStringJsonConverter : JsonConverter<Microsoft.AspNetCore.Http.PathString>
    {
        public override PathString ReadJson(JsonReader reader, Type objectType, PathString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = reader.Value as string;
            return new PathString(value ?? "");
        }

        public override void WriteJson(JsonWriter writer, PathString value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    internal class HostStringJsonConverter : JsonConverter<Microsoft.AspNetCore.Http.HostString>
    {
        public override HostString ReadJson(JsonReader reader, Type objectType, HostString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = reader.Value as string;
            return new HostString(value ?? "");
        }

        public override void WriteJson(JsonWriter writer, HostString value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    internal class StringValuesJsonConverter : JsonConverter<StringValues>
    {
        public override StringValues ReadJson(JsonReader reader, Type objectType, StringValues existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = JArray.ReadFrom(reader).ToObject<string[]>();
            return new StringValues(value ?? Array.Empty<string>());
        }

        public override void WriteJson(JsonWriter writer, StringValues value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToArray());
        }
    }

    internal class TimelineEventJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ITimelineEvent) || objectType == typeof(ITimelineScope) || objectType == typeof(ITimelineStamp));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var item = JToken.ReadFrom(reader);
            if (item.Type == JTokenType.Null) return null;

            switch ((string)item["EventType"])
            {
                case nameof(TimelineScope):
                    return item.ToObject<TimelineScope_>(serializer);
                case nameof(TimelineStamp):
                    return item.ToObject<TimelineStamp_>(serializer);
                default:
                    throw new NotSupportedException();
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

    internal class TimelineScope_ : ITimelineScope
    {
        public TimeSpan Duration { get; set; }

        public IReadOnlyCollection<ITimelineEvent> Children { get; set; }

        public string EventType { get; set; }

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
    }

    internal class TimelineStamp_ : ITimelineStamp
    {
        public string EventType { get; set; }

        public string Name { get; set; }
        public string Category { get; set; }
        public string Data { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
