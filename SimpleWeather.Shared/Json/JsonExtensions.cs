using System;
using System.Globalization;
using System.Text.Json;

namespace SimpleWeather.Json
{
    public static class JsonExtensions
    {
        public static void WriteSingle(this Utf8JsonWriter writer, string propertyName, Single? value)
        {
            if (value == null)
            {
                writer.WriteNull(propertyName);
            }
            else
            {
                writer.WriteNumber(propertyName, value.Value);
            }
        }

        public static void WriteInt32(this Utf8JsonWriter writer, string propertyName, int? value)
        {
            if (value == null)
            {
                writer.WriteNull(propertyName);
            }
            else
            {
                writer.WriteNumber(propertyName, value.Value);
            }
        }

        public static void WriteDouble(this Utf8JsonWriter writer, string propertyName, Double? value)
        {
            if (value == null)
            {
                writer.WriteNull(propertyName);
            }
            else
            {
                writer.WriteNumber(propertyName, value.Value);
            }
        }

        public static void WriteRawValueSafe(this Utf8JsonWriter writer, string propertyName, string value)
        {
            writer.WritePropertyName(propertyName);

            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteRawValue(value);
            }
        }

        public static float? TryGetSingle(this ref Utf8JsonReader reader)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (float.TryParse(reader.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
                    return result;
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetSingle();
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                // ignore
            }

            return null;
        }

        public static int? TryGetInt32(this ref Utf8JsonReader reader)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (int.TryParse(reader.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
                    return result;
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                // ignore
            }

            return null;
        }

        public static double? TryGetDouble(this ref Utf8JsonReader reader)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (double.TryParse(reader.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                    return result;
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetDouble();
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                // ignore
            }

            return null;
        }

        public static DateTime TryGetDateTime(this ref Utf8JsonReader reader, string format)
        {
            var str = reader.GetString();

            var parsed = DateTime.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime result);
            if (parsed)
            {
                return result;
            }
            else // Date isn't in our format; try parsing without format
            {
                return DateTime.Parse(str, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
        }

        public static DateTimeOffset TryGetDateTimeOffset(this ref Utf8JsonReader reader, string format)
        {
            var str = reader.GetString();

            var parsed = DateTimeOffset.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset result);
            if (parsed)
            {
                return result;
            }
            else // Date isn't in our format; try parsing without format
            {
                return DateTimeOffset.Parse(str, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
        }
    }
}
