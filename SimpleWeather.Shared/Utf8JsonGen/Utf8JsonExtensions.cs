using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SimpleWeather.Utf8JsonGen
{
    public static class Utf8JsonExtensions
    {
        public static void WriteSingle(this ref JsonWriter writer, Single? value)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteSingle(value.Value);
            }
        }

        public static void WriteInt32(this ref JsonWriter writer, int? value)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteInt32(value.Value);
            }
        }

        public static void WriteDouble(this ref JsonWriter writer, Double? value)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteDouble(value.Value);
            }
        }

        public static float? TryReadSingle(this ref JsonReader reader)
        {
            if (reader.GetCurrentJsonToken() == JsonToken.String)
            {
                if (float.TryParse(reader.ReadString(), NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
                    return result;
            }
            else if (reader.GetCurrentJsonToken() == JsonToken.Number)
            {
                return reader.ReadSingle();
            }
            else if (reader.GetCurrentJsonToken() == JsonToken.Null)
            {
                reader.ReadIsNull();
            }

            return null;
        }

        public static int? TryReadInt32(this ref JsonReader reader)
        {
            if (reader.GetCurrentJsonToken() == JsonToken.String)
            {
                if (int.TryParse(reader.ReadString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
                    return result;
            }
            else if (reader.GetCurrentJsonToken() == JsonToken.Number)
            {
                return reader.ReadInt32();
            }
            else if (reader.GetCurrentJsonToken() == JsonToken.Null)
            {
                reader.ReadIsNull();
            }

            return null;
        }

        public static double? TryReadDouble(this ref JsonReader reader)
        {
            if (reader.GetCurrentJsonToken() == JsonToken.String)
            {
                if (double.TryParse(reader.ReadString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                    return result;
            }
            else if (reader.GetCurrentJsonToken() == JsonToken.Number)
            {
                return reader.ReadDouble();
            }
            else if (reader.GetCurrentJsonToken() == JsonToken.Null)
            {
                reader.ReadIsNull();
            }

            return null;
        }

        public static DateTime TryReadDateTime(this ref JsonReader reader, string format)
        {
            var str = reader.ReadString();

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

        public static DateTimeOffset TryReadDateTimeOffset(this ref JsonReader reader, string format)
        {
            var str = reader.ReadString();

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
