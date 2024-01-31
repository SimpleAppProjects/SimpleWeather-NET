using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleWeather.Json
{
    public class NullableJsonStringEnumConverter<T> : JsonConverter<T>
    {
        private readonly Type _underlyingType;

        public NullableJsonStringEnumConverter()
        {
            _underlyingType = Nullable.GetUnderlyingType(typeof(T));
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(T).IsAssignableFrom(typeToConvert);
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value)) return default;

            // for performance, parse with ignoreCase:false first.
            if (!Enum.TryParse(_underlyingType, value, ignoreCase: false, out object result) &&
                !Enum.TryParse(_underlyingType, value, ignoreCase: true, out result))
            {
                throw new JsonException($"Unable to convert \"{value}\" to Enum \"{_underlyingType}\".");
            }
            return (T)result;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString());
        }
    }

    public class CamelCaseJsonStringEnumConverter<T> : JsonStringEnumConverter<T> where T : struct, Enum
    {
        public CamelCaseJsonStringEnumConverter() : base(JsonNamingPolicy.CamelCase) { }
    }

    // Source: https://github.com/dotnet/runtime/issues/74385#issuecomment-1705083109
    public class JsonStringEnumMemberConverter<T> : JsonStringEnumConverter<T> where T : struct, Enum
    {
        public JsonStringEnumMemberConverter() : base(namingPolicy: ResolveNamingPolicy()) { }

        private static JsonNamingPolicy? ResolveNamingPolicy()
        {
            var map = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => (f.Name, AttributeName: f.GetCustomAttribute<EnumMemberAttribute>()?.Value))
                .Where(pair => pair.AttributeName != null)
                .ToDictionary();

            return map.Count > 0 ? new EnumMemberNamingPolicy(map!) : null;
        }

        private sealed class EnumMemberNamingPolicy(Dictionary<string, string> map) : JsonNamingPolicy
        {
            public override string ConvertName(string name)
                => map.TryGetValue(name, out string? newName) ? newName : name;
        }
    }
}
