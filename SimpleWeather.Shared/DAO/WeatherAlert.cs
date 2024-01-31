using SimpleWeather.Json;
using SimpleWeather.Utils;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleWeather.WeatherData
{
    public enum WeatherAlertSeverity
    {
        Unknown = -1,
        Minor = 0,
        Moderate,
        Severe,
        Extreme,
    }

    public enum WeatherAlertType
    {
        SpecialWeatherAlert = 0,
        HurricaneLocalStatement,
        HurricaneWindWarning,
        TornadoWarning,
        TornadoWatch,
        SevereThunderstormWarning,
        SevereThunderstormWatch,
        WinterWeather,
        FloodWarning,
        FloodWatch,
        HighWind,
        SevereWeather,
        Heat,
        DenseFog,
        Fire,
        Volcano,
        DenseSmoke,
        DustAdvisory,
        EarthquakeWarning,
        GaleWarning,
        SmallCraft,
        StormWarning,
        TsunamiWatch,
        TsunamiWarning,
    }

    [JsonConverter(typeof(CustomJsonConverter<WeatherAlert>))]
    public partial class WeatherAlert : CustomJsonObject
    {
        public WeatherAlertType Type { get; set; } = WeatherAlertType.SpecialWeatherAlert;
        public WeatherAlertSeverity Severity { get; set; } = WeatherAlertSeverity.Unknown;
        public string Title { get; set; }
        public string Message { get; set; }
        public string Attribution { get; set; }
        [JsonConverter(typeof(DateTimeOffsetFormatter))]
        public DateTimeOffset Date { get; set; }
        [JsonConverter(typeof(DateTimeOffsetFormatter))]
        public DateTimeOffset ExpiresDate { get; set; }
        public bool Notified { get; set; } = false;

        public WeatherAlert()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref Utf8JsonReader reader)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var jsonValue = reader.GetString();

                if (jsonValue == null) return;

                var extReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonValue));
                FromJson(ref extReader);
                return;
            }

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                    reader.Read(); // StartObject

                string property = reader.GetString(); // JsonTokenType.PropertyName

                reader.Read(); // Property Value

                switch (property)
                {
                    case nameof(Type):
                        this.Type = (WeatherAlertType)reader.GetInt32();
                        break;

                    case nameof(Severity):
                        this.Severity = (WeatherAlertSeverity)reader.GetInt32();
                        break;

                    case nameof(Title):
                        this.Title = reader.GetString();
                        break;

                    case nameof(Message):
                        this.Message = reader.GetString();
                        break;

                    case nameof(Attribution):
                        this.Attribution = reader.GetString();
                        break;

                    case nameof(Date):
                        bool parsed = DateTimeOffset.TryParseExact(reader.GetString(), DateTimeUtils.DATETIMEOFFSET_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result);
                        if (!parsed) // If we can't parse try without format
                            result = DateTimeOffset.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        this.Date = result;
                        break;

                    case nameof(ExpiresDate):
                        parsed = DateTimeOffset.TryParseExact(reader.GetString(), DateTimeUtils.DATETIMEOFFSET_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
                        if (!parsed) // If we can't parse try without format
                            result = DateTimeOffset.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        this.ExpiresDate = result;
                        break;

                    case nameof(Notified):
                        this.Notified = reader.GetBoolean();
                        break;

                    default:
                        // ignore
                        break;
                }
            }
        }

        public override string ToJson()
        {
            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "Type" : ""
            writer.WriteInt32(nameof(Type), (int)Type);

            // "Severity" : ""
            writer.WriteInt32(nameof(Severity), (int)Severity);

            // "Title" : ""
            writer.WriteString(nameof(Title), Title);

            // "Message" : ""
            writer.WriteString(nameof(Message), Message);

            // "Attribution" : ""
            writer.WriteString(nameof(Attribution), Attribution);

            // "Date" : ""
            writer.WriteString(nameof(Date), Date.ToDateTimeOffsetFormat());

            // "ExpiresDate" : ""
            writer.WriteString(nameof(ExpiresDate), ExpiresDate.ToDateTimeOffsetFormat());

            // "Notified" : ""
            writer.WriteBoolean(nameof(Notified), Notified);

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public override bool Equals(object obj)
        {
            var alert = obj as WeatherAlert;
            return alert != null &&
                   Type == alert.Type &&
                   Severity == alert.Severity &&
                   Title == alert.Title &&
                   //Message == alert.Message &&
                   Attribution == alert.Attribution &&
                   //Date.Equals(alert.Date) &&
                   ExpiresDate.Equals(alert.ExpiresDate);
        }

        public override int GetHashCode()
        {
            var hashCode = 68217818;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + Severity.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            //hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Attribution);
            //hashCode = hashCode * -1521134295 + EqualityComparer<DateTimeOffset>.Default.GetHashCode(Date);
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTimeOffset>.Default.GetHashCode(ExpiresDate);
            return hashCode;
        }
    }

    [Table(TABLE_NAME)]
    public class WeatherAlerts
    {
        public const String TABLE_NAME = "weatheralerts";

        [PrimaryKey]
        public string query { get; set; }
        [TextBlob("alertsblob")]
        public ICollection<WeatherAlert> alerts { get; set; }
        [Column("weather_alerts")]
        [JsonIgnore]
        public string alertsblob { get; set; }

        public WeatherAlerts()
        {
        }

        public WeatherAlerts(string query, ICollection<WeatherAlert> alerts)
        {
            this.query = query;
            this.alerts = alerts;
        }
    }
}