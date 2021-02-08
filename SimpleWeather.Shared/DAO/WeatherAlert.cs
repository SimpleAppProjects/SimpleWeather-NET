using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using Utf8Json;
using SQLiteNetExtensions.Attributes;
using System.Runtime.Serialization;

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

    [JsonFormatter(typeof(CustomJsonConverter<WeatherAlert>))]
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

        public WeatherAlert()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref JsonReader extReader)
        {
            JsonReader reader;

            string jsonValue;

            var token = extReader.GetCurrentJsonToken();
            if (token == JsonToken.String)
                jsonValue = extReader.ReadString();
            else
                jsonValue = null;

            if (jsonValue == null)
                reader = extReader;
            else
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                reader = new JsonReader(Encoding.UTF8.GetBytes(jsonValue));
#pragma warning restore CA2000 // Dispose objects before losing scope
                reader.ReadNext(); // StartObject
            }

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.Read(); // prop value

                switch (property)
                {
                    case nameof(Type):
                        this.Type = (WeatherAlertType)reader.ReadInt32();
                        break;

                    case nameof(Title):
                        this.Title = reader.ReadString();
                        break;

                    case nameof(Message):
                        this.Message = reader.ReadString();
                        break;

                    case nameof(Attribution):
                        this.Attribution = reader.ReadString();
                        break;

                    case nameof(Date):
                        bool parsed = DateTimeOffset.TryParseExact(reader.ReadString(), DateTimeUtils.DATETIMEOFFSET_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result);
                        if (!parsed) // If we can't parse try without format
                            result = DateTimeOffset.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
                        this.Date = result;
                        break;

                    case nameof(ExpiresDate):
                        parsed = DateTimeOffset.TryParseExact(reader.ReadString(), DateTimeUtils.DATETIMEOFFSET_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
                        if (!parsed) // If we can't parse try without format
                            result = DateTimeOffset.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
                        this.ExpiresDate = result;
                        break;

                    case nameof(Notified):
                        this.Notified = reader.ReadBoolean();
                        break;

                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();

            // {
            writer.WriteBeginObject();

            // "Type" : ""
            writer.WritePropertyName(nameof(Type));
            writer.WriteInt32((int)Type);

            writer.WriteValueSeparator();

            // "Title" : ""
            writer.WritePropertyName(nameof(Title));
            writer.WriteString(Title);

            writer.WriteValueSeparator();

            // "Message" : ""
            writer.WritePropertyName(nameof(Message));
            writer.WriteString(Message);

            writer.WriteValueSeparator();

            // "Attribution" : ""
            writer.WritePropertyName(nameof(Attribution));
            writer.WriteString(Attribution);

            writer.WriteValueSeparator();

            // "Date" : ""
            writer.WritePropertyName(nameof(Date));
            writer.WriteString(Date.ToDateTimeOffsetFormat());

            writer.WriteValueSeparator();

            // "ExpiresDate" : ""
            writer.WritePropertyName(nameof(ExpiresDate));
            writer.WriteString(ExpiresDate.ToDateTimeOffsetFormat());

            writer.WriteValueSeparator();

            // "Notified" : ""
            writer.WritePropertyName(nameof(Notified));
            writer.WriteBoolean(Notified);

            // }
            writer.WriteEndObject();

            return writer.ToString();
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
        [IgnoreDataMember]
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