using System;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleWeather.Json
{
    // https://github.com/dotnet/runtime/issues/92091#issuecomment-1720998105
    public class ForgivingStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.False => "false",
                JsonTokenType.True => "true",
                JsonTokenType.Number => Encoding.UTF8.GetString(reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan),
                _ => reader.GetString(),
            };
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
