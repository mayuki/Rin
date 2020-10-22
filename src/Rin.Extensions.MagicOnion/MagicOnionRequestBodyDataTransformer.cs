using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MessagePack;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Rin.Core;
using Rin.Core.Record;

namespace Rin.Extensions.MagicOnion
{
    public class MagicOnionRequestBodyDataTransformer : IRequestBodyDataTransformer
    {
        private readonly MessagePackSerializerOptions _serializerOptions;
        private readonly MagicOnionServiceDefinition _serviceDefinition;

        public MagicOnionRequestBodyDataTransformer(MagicOnionServiceDefinition serviceDefinition, IOptionsMonitor<MagicOnionOptions> magicOnionOptions)
        {
            _serviceDefinition = serviceDefinition;
            _serializerOptions = magicOnionOptions.CurrentValue.SerializerOptions;
        }

        public bool CanTransform(HttpRequestRecord record, StringValues contentTypeHeaderValues)
        {
            // NOTE: Currently, this transformer can handle only Unary request.
            var methodHandler = _serviceDefinition.MethodHandlers.FirstOrDefault(x => string.Concat("/" + x.ServiceName, "/", x.MethodName) == record.Path);
            if (methodHandler == null || methodHandler.MethodType != MethodType.Unary)
            {
                return false;
            }

            return contentTypeHeaderValues.Any(x => x == "application/grpc");
        }

        public bool TryTransform(HttpRequestRecord record, ReadOnlySpan<byte> body, StringValues contentTypeHeaderValues, out BodyDataTransformResult result)
        {
            var methodHandler = _serviceDefinition.MethodHandlers.FirstOrDefault(x => string.Concat("/" + x.ServiceName, "/", x.MethodName) == record.Path);
            if (methodHandler == null)
            {
                result = default;
                return false;
            }

            var deserialized = MessagePackSerializer.Deserialize(methodHandler.RequestType, body.Slice(5).ToArray() /* Skip gRPC Compressed Flag + Message length */, _serializerOptions);
            result = new BodyDataTransformResult(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(deserialized, RinMagicOnionSupportJsonSerializerOptions.Default)), "application/grpc", "application/json");
            return true;
        }
    }

    public class MagicOnionResponseBodyDataTransformer : IResponseBodyDataTransformer
    {
        private readonly MessagePackSerializerOptions _serializerOptions;
        private readonly MagicOnionServiceDefinition _serviceDefinition;

        public MagicOnionResponseBodyDataTransformer(MagicOnionServiceDefinition serviceDefinition, IOptionsMonitor<MagicOnionOptions> magicOnionOptions)
        {
            _serviceDefinition = serviceDefinition;
            _serializerOptions = magicOnionOptions.CurrentValue.SerializerOptions;
        }

        public bool CanTransform(HttpRequestRecord record, StringValues contentTypeHeaderValues)
        {
            // NOTE: Currently, this transformer can handle only Unary request.
            var methodHandler = _serviceDefinition.MethodHandlers.FirstOrDefault(x => string.Concat("/" + x.ServiceName, "/", x.MethodName) == record.Path);
            if (methodHandler == null || methodHandler.MethodType != MethodType.Unary)
            {
                return false;
            }

            return contentTypeHeaderValues.Any(x => x == "application/grpc");
        }

        public bool TryTransform(HttpRequestRecord record, ReadOnlySpan<byte> body, StringValues contentTypeHeaderValues, out BodyDataTransformResult result)
        {
            var methodHandler = _serviceDefinition.MethodHandlers.FirstOrDefault(x => string.Concat("/" + x.ServiceName, "/", x.MethodName) == record.Path);
            if (methodHandler == null)
            {
                result = default;
                return false;
            }

            var deserialized = MessagePackSerializer.Deserialize(methodHandler.UnwrappedResponseType, body.Slice(5).ToArray() /* Skip gRPC Compressed Flag + Message length */, _serializerOptions);
            result = new BodyDataTransformResult(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(deserialized, RinMagicOnionSupportJsonSerializerOptions.Default)), "application/grpc", "application/json");
            return true;
        }
    }

    internal static class RinMagicOnionSupportJsonSerializerOptions
    {
        public static JsonSerializerOptions Default { get; } = new JsonSerializerOptions();

        static RinMagicOnionSupportJsonSerializerOptions()
        {
            Default.Converters.Add(new DynamicArgumentTupleJsonConverter());
        }
    }
}
