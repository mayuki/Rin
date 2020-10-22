using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rin.Extensions.MagicOnion
{

    internal class DynamicArgumentTupleJsonConverter<T1, T2> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6, T7> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
                JsonSerializer.Serialize(writer, value.Item7, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6, T7, T8> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
                JsonSerializer.Serialize(writer, value.Item7, options);
                JsonSerializer.Serialize(writer, value.Item8, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6, T7, T8, T9> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
                JsonSerializer.Serialize(writer, value.Item7, options);
                JsonSerializer.Serialize(writer, value.Item8, options);
                JsonSerializer.Serialize(writer, value.Item9, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
                JsonSerializer.Serialize(writer, value.Item7, options);
                JsonSerializer.Serialize(writer, value.Item8, options);
                JsonSerializer.Serialize(writer, value.Item9, options);
                JsonSerializer.Serialize(writer, value.Item10, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
                JsonSerializer.Serialize(writer, value.Item7, options);
                JsonSerializer.Serialize(writer, value.Item8, options);
                JsonSerializer.Serialize(writer, value.Item9, options);
                JsonSerializer.Serialize(writer, value.Item10, options);
                JsonSerializer.Serialize(writer, value.Item11, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
                JsonSerializer.Serialize(writer, value.Item7, options);
                JsonSerializer.Serialize(writer, value.Item8, options);
                JsonSerializer.Serialize(writer, value.Item9, options);
                JsonSerializer.Serialize(writer, value.Item10, options);
                JsonSerializer.Serialize(writer, value.Item11, options);
                JsonSerializer.Serialize(writer, value.Item12, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
                JsonSerializer.Serialize(writer, value.Item7, options);
                JsonSerializer.Serialize(writer, value.Item8, options);
                JsonSerializer.Serialize(writer, value.Item9, options);
                JsonSerializer.Serialize(writer, value.Item10, options);
                JsonSerializer.Serialize(writer, value.Item11, options);
                JsonSerializer.Serialize(writer, value.Item12, options);
                JsonSerializer.Serialize(writer, value.Item13, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
                JsonSerializer.Serialize(writer, value.Item7, options);
                JsonSerializer.Serialize(writer, value.Item8, options);
                JsonSerializer.Serialize(writer, value.Item9, options);
                JsonSerializer.Serialize(writer, value.Item10, options);
                JsonSerializer.Serialize(writer, value.Item11, options);
                JsonSerializer.Serialize(writer, value.Item12, options);
                JsonSerializer.Serialize(writer, value.Item13, options);
                JsonSerializer.Serialize(writer, value.Item14, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
                JsonSerializer.Serialize(writer, value.Item7, options);
                JsonSerializer.Serialize(writer, value.Item8, options);
                JsonSerializer.Serialize(writer, value.Item9, options);
                JsonSerializer.Serialize(writer, value.Item10, options);
                JsonSerializer.Serialize(writer, value.Item11, options);
                JsonSerializer.Serialize(writer, value.Item12, options);
                JsonSerializer.Serialize(writer, value.Item13, options);
                JsonSerializer.Serialize(writer, value.Item14, options);
                JsonSerializer.Serialize(writer, value.Item15, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
                JsonSerializer.Serialize(writer, value.Item7, options);
                JsonSerializer.Serialize(writer, value.Item8, options);
                JsonSerializer.Serialize(writer, value.Item9, options);
                JsonSerializer.Serialize(writer, value.Item10, options);
                JsonSerializer.Serialize(writer, value.Item11, options);
                JsonSerializer.Serialize(writer, value.Item12, options);
                JsonSerializer.Serialize(writer, value.Item13, options);
                JsonSerializer.Serialize(writer, value.Item14, options);
                JsonSerializer.Serialize(writer, value.Item15, options);
                JsonSerializer.Serialize(writer, value.Item16, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
                JsonSerializer.Serialize(writer, value.Item7, options);
                JsonSerializer.Serialize(writer, value.Item8, options);
                JsonSerializer.Serialize(writer, value.Item9, options);
                JsonSerializer.Serialize(writer, value.Item10, options);
                JsonSerializer.Serialize(writer, value.Item11, options);
                JsonSerializer.Serialize(writer, value.Item12, options);
                JsonSerializer.Serialize(writer, value.Item13, options);
                JsonSerializer.Serialize(writer, value.Item14, options);
                JsonSerializer.Serialize(writer, value.Item15, options);
                JsonSerializer.Serialize(writer, value.Item16, options);
                JsonSerializer.Serialize(writer, value.Item17, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
                JsonSerializer.Serialize(writer, value.Item7, options);
                JsonSerializer.Serialize(writer, value.Item8, options);
                JsonSerializer.Serialize(writer, value.Item9, options);
                JsonSerializer.Serialize(writer, value.Item10, options);
                JsonSerializer.Serialize(writer, value.Item11, options);
                JsonSerializer.Serialize(writer, value.Item12, options);
                JsonSerializer.Serialize(writer, value.Item13, options);
                JsonSerializer.Serialize(writer, value.Item14, options);
                JsonSerializer.Serialize(writer, value.Item15, options);
                JsonSerializer.Serialize(writer, value.Item16, options);
                JsonSerializer.Serialize(writer, value.Item17, options);
                JsonSerializer.Serialize(writer, value.Item18, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
                JsonSerializer.Serialize(writer, value.Item7, options);
                JsonSerializer.Serialize(writer, value.Item8, options);
                JsonSerializer.Serialize(writer, value.Item9, options);
                JsonSerializer.Serialize(writer, value.Item10, options);
                JsonSerializer.Serialize(writer, value.Item11, options);
                JsonSerializer.Serialize(writer, value.Item12, options);
                JsonSerializer.Serialize(writer, value.Item13, options);
                JsonSerializer.Serialize(writer, value.Item14, options);
                JsonSerializer.Serialize(writer, value.Item15, options);
                JsonSerializer.Serialize(writer, value.Item16, options);
                JsonSerializer.Serialize(writer, value.Item17, options);
                JsonSerializer.Serialize(writer, value.Item18, options);
                JsonSerializer.Serialize(writer, value.Item19, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> : JsonConverter<global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>>
    {
        public override global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, global::MagicOnion.DynamicArgumentTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            {
                JsonSerializer.Serialize(writer, value.Item1, options);
                JsonSerializer.Serialize(writer, value.Item2, options);
                JsonSerializer.Serialize(writer, value.Item3, options);
                JsonSerializer.Serialize(writer, value.Item4, options);
                JsonSerializer.Serialize(writer, value.Item5, options);
                JsonSerializer.Serialize(writer, value.Item6, options);
                JsonSerializer.Serialize(writer, value.Item7, options);
                JsonSerializer.Serialize(writer, value.Item8, options);
                JsonSerializer.Serialize(writer, value.Item9, options);
                JsonSerializer.Serialize(writer, value.Item10, options);
                JsonSerializer.Serialize(writer, value.Item11, options);
                JsonSerializer.Serialize(writer, value.Item12, options);
                JsonSerializer.Serialize(writer, value.Item13, options);
                JsonSerializer.Serialize(writer, value.Item14, options);
                JsonSerializer.Serialize(writer, value.Item15, options);
                JsonSerializer.Serialize(writer, value.Item16, options);
                JsonSerializer.Serialize(writer, value.Item17, options);
                JsonSerializer.Serialize(writer, value.Item18, options);
                JsonSerializer.Serialize(writer, value.Item19, options);
                JsonSerializer.Serialize(writer, value.Item20, options);
            }
            writer.WriteEndArray();
        }
    }

    internal class DynamicArgumentTupleJsonConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.FullName.StartsWith("MagicOnion.DynamicArgumentTuple");
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeToConvert.GenericTypeArguments.Length switch
            {
                2 => typeof(DynamicArgumentTupleJsonConverter<,>),
                3 => typeof(DynamicArgumentTupleJsonConverter<,,>),
                4 => typeof(DynamicArgumentTupleJsonConverter<,,,>),
                5 => typeof(DynamicArgumentTupleJsonConverter<,,,,>),
                6 => typeof(DynamicArgumentTupleJsonConverter<,,,,,>),
                7 => typeof(DynamicArgumentTupleJsonConverter<,,,,,,>),
                8 => typeof(DynamicArgumentTupleJsonConverter<,,,,,,,>),
                9 => typeof(DynamicArgumentTupleJsonConverter<,,,,,,,,>),
                10 => typeof(DynamicArgumentTupleJsonConverter<,,,,,,,,,>),
                11 => typeof(DynamicArgumentTupleJsonConverter<,,,,,,,,,,>),
                12 => typeof(DynamicArgumentTupleJsonConverter<,,,,,,,,,,,>),
                13 => typeof(DynamicArgumentTupleJsonConverter<,,,,,,,,,,,,>),
                14 => typeof(DynamicArgumentTupleJsonConverter<,,,,,,,,,,,,,>),
                15 => typeof(DynamicArgumentTupleJsonConverter<,,,,,,,,,,,,,,>),
                16 => typeof(DynamicArgumentTupleJsonConverter<,,,,,,,,,,,,,,,>),
                17 => typeof(DynamicArgumentTupleJsonConverter<,,,,,,,,,,,,,,,,>),
                18 => typeof(DynamicArgumentTupleJsonConverter<,,,,,,,,,,,,,,,,,>),
                19 => typeof(DynamicArgumentTupleJsonConverter<,,,,,,,,,,,,,,,,,,>),
                20 => typeof(DynamicArgumentTupleJsonConverter<,,,,,,,,,,,,,,,,,,,>),
            };

            return (JsonConverter)Activator.CreateInstance(converterType.MakeGenericType(typeToConvert.GenericTypeArguments));
        }
    }
}