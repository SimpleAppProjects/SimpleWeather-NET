using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
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
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset ExpiresDate { get; set; }
        public bool Notified { get; set; } = false;

        private WeatherAlert()
        {
            // Needed for deserialization
        }

        public override void FromJson(Utf8JsonReader extReader)
        {
            Utf8JsonReader reader;

            string jsonValue;

            if (extReader.TokenType == JsonTokenType.String || extReader.Read() && extReader.TokenType == JsonTokenType.String)
                jsonValue = extReader.GetString();
            else
                jsonValue = null;

            if (jsonValue == null)
                reader = extReader;
            else
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonValue));
#pragma warning restore CA2000 // Dispose objects before losing scope
                reader.Read(); // StartObject
            }

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                    reader.Read(); // StartObject

                string property = reader.GetString();
                reader.Read(); // prop value

                switch (property)
                {
                    case nameof(Type):
                        this.Type = (WeatherAlertType)reader.GetInt32();
                        break;

                    case nameof(Title):
                        this.Title = reader.GetString();
                        break;

                    case nameof(Message):
                        this.Message = reader.GetString();
                        break;

                    case nameof(Attribution):
                        this.Attribution = extReader.GetString();
                        break;

                    case nameof(Date):
                        bool parsed = DateTimeOffset.TryParseExact(reader.GetString(), "dd.MM.yyyy HH:mm:ss zzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result);
                        if (!parsed) // If we can't parse try without format
                            result = DateTimeOffset.Parse(reader.GetString());
                        this.Date = result;
                        break;

                    case nameof(ExpiresDate):
                        parsed = DateTimeOffset.TryParseExact(reader.GetString(), "dd.MM.yyyy HH:mm:ss zzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
                        if (!parsed) // If we can't parse try without format
                            result = DateTimeOffset.Parse(reader.GetString());
                        this.ExpiresDate = result;
                        break;

                    case nameof(Notified):
                        this.Notified = reader.GetBoolean();
                        break;

                    default:
                        break;
                }
            }
        }

        public override string ToJson()
        {
            using (var stream = new System.IO.MemoryStream())
            using (var writer = new Utf8JsonWriter(stream))
            {
                // {
                writer.WriteStartObject();

                // "Type" : ""
                writer.WritePropertyName(nameof(Type));
                writer.WriteNumberValue((int)Type);

                // "Title" : ""
                writer.WritePropertyName(nameof(Title));
                writer.WriteStringValue(Title);

                // "Message" : ""
                writer.WritePropertyName(nameof(Message));
                writer.WriteStringValue(Message);

                // "Attribution" : ""
                writer.WritePropertyName(nameof(Attribution));
                writer.WriteStringValue(Attribution);

                // "Date" : ""
                writer.WritePropertyName(nameof(Date));
                writer.WriteStringValue(Date.ToString("dd.MM.yyyy HH:mm:ss zzzz"));

                // "ExpiresDate" : ""
                writer.WritePropertyName(nameof(ExpiresDate));
                writer.WriteStringValue(ExpiresDate.ToString("dd.MM.yyyy HH:mm:ss zzzz"));

                // "Notified" : ""
                writer.WritePropertyName(nameof(Notified));
                writer.WriteBooleanValue(Notified);

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public override bool Equals(object obj)
        {
            var alert = obj as WeatherAlert;
            return alert != null &&
                   Type == alert.Type &&
                   Severity == alert.Severity &&
                   Title == alert.Title &&
                   Message == alert.Message &&
                   Attribution == alert.Attribution &&
                   Date.Equals(alert.Date) &&
                   ExpiresDate.Equals(alert.ExpiresDate);
        }

        public override int GetHashCode()
        {
            var hashCode = 68217818;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + Severity.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Attribution);
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTimeOffset>.Default.GetHashCode(Date);
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTimeOffset>.Default.GetHashCode(ExpiresDate);
            return hashCode;
        }
    }

    [Table("weatheralerts")]
    public class WeatherAlerts
    {
        [Key]
        [Column(TypeName = "varchar")]
        public string query { get; set; }
        [Column("weather_alerts", TypeName = "varchar")]
        public IEnumerable<WeatherAlert> alerts { get; set; }

        public WeatherAlerts()
        {
        }

        public WeatherAlerts(string query, IEnumerable<WeatherAlert> alerts)
        {
            this.query = query;
            this.alerts = alerts;
        }
    }
}