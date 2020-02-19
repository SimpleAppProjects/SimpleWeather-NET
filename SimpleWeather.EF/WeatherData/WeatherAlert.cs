using Newtonsoft.Json;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

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

    [JsonConverter(typeof(CustomJsonConverter))]
    public partial class WeatherAlert
    {
        public WeatherAlertType Type { get; set; } = WeatherAlertType.SpecialWeatherAlert;
        public WeatherAlertSeverity Severity { get; set; } = WeatherAlertSeverity.Unknown;
        public string Title { get; set; }
        public string Message { get; set; }
        public string Attribution { get; set; }
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset ExpiresDate { get; set; }
        public bool Notified { get; set; } = false;

        [JsonConstructor]
        private WeatherAlert()
        {
            // Needed for deserialization
        }

        public static WeatherAlert FromJson(JsonReader extReader)
        {
            WeatherAlert obj = null;
            bool disposeReader = false;
            JsonReader reader = null;

            try
            {
                obj = new WeatherAlert();
                string jsonValue;

                if (extReader.TokenType == JsonToken.String || extReader.Read() && extReader.TokenType == JsonToken.String)
                    jsonValue = extReader.Value?.ToString();
                else
                    jsonValue = null;

                if (jsonValue == null)
                    reader = extReader;
                else
                {
                    disposeReader = true;
#pragma warning disable CA2000 // Dispose objects before losing scope
                    reader = new JsonTextReader(new System.IO.StringReader(jsonValue)) { CloseInput = true };
#pragma warning restore CA2000 // Dispose objects before losing scope
                    reader.Read(); // StartObject
                }

                while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                {
                    if (reader.TokenType == JsonToken.StartObject)
                        reader.Read(); // StartObject

                    string property = reader.Value?.ToString();
                    reader.Read(); // prop value

                    switch (property)
                    {
                        case nameof(Type):
                            obj.Type = (WeatherAlertType)int.Parse(reader.Value?.ToString());
                            break;

                        case nameof(Title):
                            obj.Title = reader.Value?.ToString();
                            break;

                        case nameof(Message):
                            obj.Message = reader.Value?.ToString();
                            break;

                        case nameof(Attribution):
                            obj.Attribution = reader.Value?.ToString();
                            break;

                        case nameof(Date):
                            bool parsed = DateTimeOffset.TryParseExact(reader.Value?.ToString(), "dd.MM.yyyy HH:mm:ss zzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result);
                            if (!parsed) // If we can't parse try without format
                                result = DateTimeOffset.Parse(reader.Value?.ToString());
                            obj.Date = result;
                            break;

                        case nameof(ExpiresDate):
                            parsed = DateTimeOffset.TryParseExact(reader.Value?.ToString(), "dd.MM.yyyy HH:mm:ss zzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
                            if (!parsed) // If we can't parse try without format
                                result = DateTimeOffset.Parse(reader.Value?.ToString());
                            obj.ExpiresDate = result;
                            break;

                        case nameof(Notified):
                            obj.Notified = (bool)reader.Value;
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                obj = null;
            }
            finally
            {
                if (disposeReader)
                    reader?.Close();
            }

            return obj;
        }

        public string ToJson()
        {
            using (var sw = new System.IO.StringWriter())
            using (var writer = new JsonTextWriter(sw))
            {
                // {
                writer.WriteStartObject();

                // "Type" : ""
                writer.WritePropertyName(nameof(Type));
                writer.WriteValue((int)Type);

                // "Title" : ""
                writer.WritePropertyName(nameof(Title));
                writer.WriteValue(Title);

                // "Message" : ""
                writer.WritePropertyName(nameof(Message));
                writer.WriteValue(Message);

                // "Attribution" : ""
                writer.WritePropertyName(nameof(Attribution));
                writer.WriteValue(Attribution);

                // "Date" : ""
                writer.WritePropertyName(nameof(Date));
                writer.WriteValue(Date.ToString("dd.MM.yyyy HH:mm:ss zzzz"));

                // "ExpiresDate" : ""
                writer.WritePropertyName(nameof(ExpiresDate));
                writer.WriteValue(ExpiresDate.ToString("dd.MM.yyyy HH:mm:ss zzzz"));

                // "Notified" : ""
                writer.WritePropertyName(nameof(Notified));
                writer.WriteValue(Notified);

                // }
                writer.WriteEndObject();

                return sw.ToString();
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