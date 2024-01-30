using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimpleWeather.Json
{
    public class DateTimeOffsetFormatter : JsonConverter<DateTimeOffset>
    {
        private const string FORMAT = DateTimeUtils.DATETIMEOFFSET_FORMAT;

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTimeOffset.ParseExact(reader.GetString(), FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToDateTimeOffsetFormat());
        }
    }

    public class ISO8601DateTimeOffsetFormatter : JsonConverter<DateTimeOffset>
    {
        private const string FORMAT = DateTimeUtils.ISO8601_DATETIMEOFFSET_FORMAT;

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTimeOffset.ParseExact(reader.GetString(), FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToISO8601Format());
        }
    }

    public class ISO8601DateTimeFormatter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToISO8601Format());
        }
    }

    public class SimpleNullableDateTimeFormatter : JsonConverter<DateTime?>
    {
        private const string FORMAT = "yyyy-MM-dd";

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();

            if (DateTime.TryParseExact(str, FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime result))
            {
                return result;
            }
            else
            {
                return DateTime.Parse(str, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString(FORMAT, CultureInfo.InvariantCulture));
        }
    }
}
