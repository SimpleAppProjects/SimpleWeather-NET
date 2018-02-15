using Newtonsoft.Json;
using SimpleWeather.Utils;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public class WeatherAlert
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

        public WeatherAlert(WeatherUnderground.Alert alert)
        {
            // Alert Type
            switch(alert.type)
            {
                case "HUR": // Hurricane Local Statement
                    Type = WeatherAlertType.HurricaneLocalStatement;
                    Severity = WeatherAlertSeverity.Moderate;
                    break;
                case "HWW": // Hurricane Wind Warning
                    Type = WeatherAlertType.HurricaneWindWarning;
                    Severity = WeatherAlertSeverity.Extreme;
                    break;
                case "TOR": // Tornado Warning
                    Type = WeatherAlertType.TornadoWarning;
                    Severity = WeatherAlertSeverity.Extreme;
                    break;
                case "TOW": // Tornado Watch
                    Type = WeatherAlertType.TornadoWatch;
                    Severity = WeatherAlertSeverity.Moderate;
                    break;
                case "WRN": // Severe Thunderstorm Warning
                    Type = WeatherAlertType.SevereThunderstormWarning;
                    Severity = WeatherAlertSeverity.Extreme;
                    break;
                case "SEW": // Severe Thunderstorm Watch
                    Type = WeatherAlertType.SevereThunderstormWatch;
                    Severity = WeatherAlertSeverity.Moderate;
                    break;
                case "WIN": // Winter Weather Advisory
                    Type = WeatherAlertType.WinterWeather;
                    Severity = WeatherAlertSeverity.Severe;
                    break;
                case "FLO": // Flood Warning
                    Type = WeatherAlertType.FloodWarning;
                    Severity = WeatherAlertSeverity.Extreme;
                    break;
                case "WAT": // Flood Watch
                    Type = WeatherAlertType.FloodWatch;
                    Severity = WeatherAlertSeverity.Severe;
                    break;
                case "WND": // High Wind Advisory
                    Type = WeatherAlertType.HighWind;
                    Severity = WeatherAlertSeverity.Moderate;
                    break;
                case "HEA": // Heat Advisory
                    Type = WeatherAlertType.Heat;
                    Severity = WeatherAlertSeverity.Moderate;
                    break;
                case "FOG": // Dense Fog Advisory
                    Type = WeatherAlertType.DenseFog;
                    Severity = WeatherAlertSeverity.Severe;
                    break;
                case "FIR": // Fire Weather Advisory
                    Type = WeatherAlertType.Fire;
                    Severity = WeatherAlertSeverity.Moderate;
                    break;
                case "VOL": // Volcanic Activity Statement
                    Type = WeatherAlertType.Volcano;
                    Severity = WeatherAlertSeverity.Severe;
                    break;
                case "SVR": // Severe Weather Statement
                    Type = WeatherAlertType.SevereWeather;
                    Severity = WeatherAlertSeverity.Severe;
                    break;
                case "SPE": // Special Weather Statement
                    Type = WeatherAlertType.SpecialWeatherAlert;
                    Severity = WeatherAlertSeverity.Severe;
                    break;
                default:
                    Type = WeatherAlertType.SpecialWeatherAlert;
                    Severity = WeatherAlertSeverity.Severe;
                    break;
            }

            if (String.IsNullOrWhiteSpace(alert.wtype_meteoalarm_name))
            {
                // NWS Alerts
                Title = alert.description;
                Message = alert.message;

                Date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(alert.date_epoch));
                ExpiresDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(alert.expires_epoch));

                Attribution = "Information provided by the U.S. National Weather Service";
            }
            else
            {
                // Meteoalarm.eu Alerts
                Title = alert.wtype_meteoalarm_name;
                Message = alert.description;
                Attribution = alert.attribution;

                if (long.TryParse(alert.date_epoch, out long date_epoch))
                    Date = DateTimeOffset.FromUnixTimeSeconds(date_epoch);
                else
                    Date = DateTimeOffset.Parse(alert.date);

                if (long.TryParse(alert.expires_epoch, out long expires_epoch))
                    ExpiresDate = DateTimeOffset.FromUnixTimeSeconds(expires_epoch);
                else
                    ExpiresDate = DateTimeOffset.Parse(alert.expires);
            }
        }

        public WeatherAlert(NWS.Graph alert)
        {
            // Alert Type
            switch (alert._event)
            {
                case "Hurricane Local Statement":
                case "Hurricane Force Wind Watch":
                case "Hurricane Watch":
                case "Hurricane Force Wind Warning":
                case "Hurricane Warning":
                    Type = WeatherAlertType.HurricaneWindWarning;
                    break;
                case "Tornado Warning":
                    Type = WeatherAlertType.TornadoWarning;
                    break;
                case "Tornado Watch":
                    Type = WeatherAlertType.TornadoWatch;
                    break;
                case "Severe Thunderstorm Warning":
                    Type = WeatherAlertType.SevereThunderstormWarning;
                    break;
                case "Severe Thunderstorm Watch":
                    Type = WeatherAlertType.SevereThunderstormWatch;
                    break;
                case "Excessive Heat Warning":
                case "Excessive Heat Watch":
                case "Heat Advisory":
                    Type = WeatherAlertType.Heat;
                    break;
                case "Dense Fog Advisory":
                    Type = WeatherAlertType.DenseFog;
                    break;
                case "Dense Smoke Advisory":
                    Type = WeatherAlertType.DenseSmoke;
                    break;
                case "Extreme Fire Danger":
                case "Fire Warning":
                case "Fire Weather Watch":
                    Type = WeatherAlertType.Fire;
                    break;
                case "Volcano Warning":
                    Type = WeatherAlertType.Volcano;
                    break;
                case "Extreme Wind Warning":
                case "High Wind Warning":
                case "High Wind Watch":
                case "Lake Wind Advisory":
                case "Wind Advisory":
                    Type = WeatherAlertType.HighWind;
                    break;
                case "Lake Effect Snow Advisory":
                case "Lake Effect Snow Warning":
                case "Lake Effect Snow Watch":
                case "Snow Squall Warning":
                case "Ice Storm Warning":
                case "Winter Storm Warning":
                case "Winter Storm Watch":
                case "Winter Weather Advisory":
                    Type = WeatherAlertType.WinterWeather;
                    break;
                case "Earthquake Warning":
                    Type = WeatherAlertType.EarthquakeWarning;
                    break;
                case "Gale Warning":
                case "Gale Watch":
                    Type = WeatherAlertType.GaleWarning;
                    break;
                default:
                    if (alert._event.Contains("Flood Warning"))
                        Type = WeatherAlertType.FloodWarning;
                    else if (alert._event.Contains("Flood"))
                        Type = WeatherAlertType.FloodWatch;
                    else if (alert._event.Contains("Snow") || alert._event.Contains("Blizzard") ||
                        alert._event.Contains("Winter") || alert._event.Contains("Ice") ||
                        alert._event.Contains("Avalanche") || alert._event.Contains("Cold") ||
                        alert._event.Contains("Freez") || alert._event.Contains("Frost") ||
                        alert._event.Contains("Chill"))
                    {
                        Type = WeatherAlertType.WinterWeather;
                    }
                    else if (alert._event.Contains("Dust"))
                        Type = WeatherAlertType.DustAdvisory;
                    else if (alert._event.Contains("Small Craft"))
                        Type = WeatherAlertType.SmallCraft;
                    else if (alert._event.Contains("Storm"))
                        Type = WeatherAlertType.StormWarning;
                    else if (alert._event.Contains("Tsunami"))
                        Type = WeatherAlertType.TsunamiWarning;
                    else
                        Type = WeatherAlertType.SpecialWeatherAlert;
                    break;
            }

            switch (alert.severity)
            {
                case "Minor":
                    Severity = WeatherAlertSeverity.Minor;
                    break;
                case "Moderate":
                    Severity = WeatherAlertSeverity.Moderate;
                    break;
                case "Severe":
                    Severity = WeatherAlertSeverity.Severe;
                    break;
                case "Extreme":
                    Severity = WeatherAlertSeverity.Extreme;
                    break;
                case "Unknown":
                default:
                    Severity = WeatherAlertSeverity.Unknown;
                    break;
            }

            Title = alert._event;
            Message = string.Format("{0}\n{1}", alert.description, alert.instruction);

            Date = alert.sent;
            ExpiresDate = alert.expires;

            Attribution = "Information provided by the U.S. National Weather Service";
        }

        public static WeatherAlert FromJson(JsonReader extReader)
        {
            WeatherAlert obj = null;

            try
            {
                obj = new WeatherAlert();
                JsonReader reader;

                if (extReader.Value == null)
                    reader = extReader;
                else
                {
                    reader = new JsonTextReader(new System.IO.StringReader(extReader.Value.ToString()));
                    reader.Read(); // StartObject
                }

                while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                {
                    string property = reader.Value?.ToString();
                    reader.Read(); // prop value

                    switch (property)
                    {
                        case "Type":
                            obj.Type = (WeatherAlertType)int.Parse(reader.Value.ToString());
                            break;
                        case "Title":
                            obj.Title = reader.Value.ToString();
                            break;
                        case "Message":
                            obj.Message = reader.Value.ToString();
                            break;
                        case "Attribution":
                            obj.Attribution = reader.Value.ToString();
                            break;
                        case "Date":
                            obj.Date = DateTimeOffset.Parse(reader.Value.ToString());
                            break;
                        case "ExpiresDate":
                            obj.ExpiresDate = DateTimeOffset.Parse(reader.Value.ToString());
                            break;
                        case "Notified":
                            obj.Notified = (bool)reader.Value;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                obj = null;
            }

            return obj;
        }

        public string ToJson()
        {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);

            // {
            writer.WriteStartObject();

            // "Type" : ""
            writer.WritePropertyName("Type");
            writer.WriteValue((int)Type);

            // "Title" : ""
            writer.WritePropertyName("Title");
            writer.WriteValue(Title);

            // "Message" : ""
            writer.WritePropertyName("Message");
            writer.WriteValue(Message);

            // "Attribution" : ""
            writer.WritePropertyName("Attribution");
            writer.WriteValue(Attribution);

            // "Date" : ""
            writer.WritePropertyName("Date");
            writer.WriteValue(Date);

            // "ExpiresDate" : ""
            writer.WritePropertyName("ExpiresDate");
            writer.WriteValue(ExpiresDate);

            // "Notified" : ""
            writer.WritePropertyName("Notified");
            writer.WriteValue(Notified);

            // }
            writer.WriteEndObject();

            return sw.ToString();
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
        [PrimaryKey]
        public string query { get; set; }
        [TextBlob("alertsblob")]
        public List<WeatherAlert> alerts { get; set; }
        [JsonIgnore]
        [Column("weather_alerts")]
        public string alertsblob { get; set; }

        public WeatherAlerts()
        {
        }

        public WeatherAlerts(string query, List<WeatherAlert> alerts)
        {
            this.query = query;
            this.alerts = alerts;
        }
    }
}
