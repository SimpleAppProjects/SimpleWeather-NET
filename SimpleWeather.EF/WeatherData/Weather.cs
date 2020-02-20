using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Utf8Json;

namespace SimpleWeather.WeatherData
{
    [JsonFormatter(typeof(CustomJsonConverter<Weather>))]
    [Table("weatherdata")]
    public partial class Weather : CustomJsonObject
    {
        [NotMapped]
        public const string NA = "N/A";

        [Column("locationblob", TypeName = "varchar")]
        public Location location { get; set; }

        [NotMapped]
        // Doesn't store this in db
        // For DateTimeOffset, offset isn't stored when saving to db
        // Store as string (blob) instead
        // If db previously stored DateTimeOffset (as ticks) retrieve and set offset
        public DateTimeOffset update_time
        {
            get
            {
                if (DateTimeOffset.TryParseExact(updatetimeblob, DateTimeUtils.DATETIMEOFFSET_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset result))
                    return result;
                else
                    return new DateTimeOffset(long.Parse(updatetimeblob), TimeSpan.Zero).ToOffset(location.tz_offset);
            }
            set { updatetimeblob = value.ToDateTimeOffsetFormat(); }
        }

        [IgnoreDataMember]
        [NotMapped]
        public IList<Forecast> forecast { get; set; }

        [IgnoreDataMember]
        [NotMapped]
        public IList<HourlyForecast> hr_forecast { get; set; }

        [IgnoreDataMember]
        [NotMapped]
        public IList<TextForecast> txt_forecast { get; set; }

        [Column("conditionblob", TypeName = "varchar")]
        public Condition condition { get; set; }

        [Column("atmosphereblob", TypeName = "varchar")]
        public Atmosphere atmosphere { get; set; }

        [Column("astronomyblob", TypeName = "varchar")]
        public Astronomy astronomy { get; set; }

        [Column("precipitationblob", TypeName = "varchar")]
        public Precipitation precipitation { get; set; }

        [IgnoreDataMember]
        [NotMapped]
        // Just for passing along to where its needed
        public IEnumerable<WeatherAlert> weather_alerts { get; set; }

        [Column(TypeName = "varchar")]
        public string ttl { get; set; }
        [Column(TypeName = "varchar")]
        public string source { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        public string query { get; set; }

        [Column(TypeName = "varchar")]
        public string locale { get; set; }

        [IgnoreDataMember]
        [Column("update_time", TypeName = "varchar")]
        // Keep DateTimeOffset column name to get data as string
        public string updatetimeblob { get; set; }

        public Weather()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref JsonReader reader)
        {
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.ReadNext(); // prop value

                switch (property)
                {
                    case nameof(location):
                        this.location = new Location();
                        this.location.FromJson(ref reader);
                        break;

                    case nameof(update_time):
                        bool parsed = DateTimeOffset.TryParseExact(reader.ReadString(), DateTimeUtils.DATETIMEOFFSET_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset result);
                        if (!parsed) // If we can't parse as DateTimeOffset try DateTime (data could be old)
                            result = DateTime.Parse(reader.ReadString());
                        else
                        {
                            // DateTimeOffset date stored in SQLite.NET doesn't store offset
                            // Try to convert to location's timezone if possible or if time is in UTC
                            if (this.location?.tz_offset != null && result.Offset.Ticks == 0)
                                result = result.ToOffset(this.location.tz_offset);
                        }
                        this.update_time = result;
                        break;

                    case nameof(forecast):
                        // Set initial cap to 10
                        // Most provider forecasts are <= 10
                        var forecasts = new List<Forecast>(10);
                        count = 0;
                        while (reader.ReadIsInArray(ref count))
                        {
                            if (reader.GetCurrentJsonToken() == JsonToken.String)
                            {
                                var forecast = new Forecast();
                                forecast.FromJson(ref reader);
                                forecasts.Add(forecast);
                            }
                        }
                        this.forecast = forecasts;
                        break;

                    case nameof(hr_forecast):
                        // Set initial cap to 90
                        // MetNo contains ~90 items, but HERE contains ~165
                        // If 90+ is needed, let the List impl allocate more
                        var hr_forecasts = new List<HourlyForecast>(90);
                        count = 0;
                        while (reader.ReadIsInArray(ref count))
                        {
                            if (reader.GetCurrentJsonToken() == JsonToken.String)
                            {
                                var hrf = new HourlyForecast();
                                hrf.FromJson(ref reader);
                                hr_forecasts.Add(hrf);
                            }
                        }
                        this.hr_forecast = hr_forecasts;
                        break;

                    case nameof(txt_forecast):
                        // Set initial cap to 20
                        // Most provider forecasts are <= 10 (x2 for day & nt)
                        var txt_forecasts = new List<TextForecast>(20);
                        count = 0;
                        while (reader.ReadIsInArray(ref count))
                        {
                            if (reader.GetCurrentJsonToken() == JsonToken.String)
                            {
                                var txtf = new TextForecast();
                                txtf.FromJson(ref reader);
                                txt_forecasts.Add(txtf);
                            }
                        }
                        this.txt_forecast = txt_forecasts;
                        break;

                    case nameof(condition):
                        this.condition = new Condition();
                        this.condition.FromJson(ref reader);
                        break;

                    case nameof(atmosphere):
                        this.atmosphere = new Atmosphere();
                        this.atmosphere.FromJson(ref reader);
                        break;

                    case nameof(astronomy):
                        this.astronomy = new Astronomy();
                        this.astronomy.FromJson(ref reader);
                        break;

                    case nameof(precipitation):
                        this.precipitation = new Precipitation();
                        this.precipitation.FromJson(ref reader);
                        break;

                    case nameof(ttl):
                        this.ttl = reader.ReadString();
                        break;

                    case nameof(source):
                        this.source = reader.ReadString();
                        break;

                    case nameof(query):
                        this.query = reader.ReadString();
                        break;

                    case nameof(locale):
                        this.locale = reader.ReadString();
                        break;

                    default:
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();

            // {
            writer.WriteBeginObject();

            // "location" : ""
            writer.WritePropertyName(nameof(location));
            writer.WriteString(location?.ToJson());

            writer.WriteValueSeparator();

            // "update_time" : ""
            writer.WritePropertyName(nameof(update_time));
            writer.WriteString(update_time.ToDateTimeOffsetFormat());

            writer.WriteValueSeparator();

            // "forecast" : ""
            if (forecast != null)
            {
                writer.WritePropertyName(nameof(forecast));
                writer.WriteBeginArray();
                var itemCount = 0;
                foreach (Forecast cast in forecast)
                {
                    if (itemCount > 0)
                        writer.WriteValueSeparator();
                    writer.WriteString(cast?.ToJson());
                    itemCount++;
                }
                writer.WriteEndArray();

                writer.WriteValueSeparator();
            }

            // "hr_forecast" : ""
            if (hr_forecast != null)
            {
                writer.WritePropertyName(nameof(hr_forecast));
                writer.WriteBeginArray();
                var itemCount = 0;
                foreach (HourlyForecast hr_cast in hr_forecast)
                {
                    if (itemCount > 0)
                        writer.WriteValueSeparator();
                    writer.WriteString(hr_cast?.ToJson());
                    itemCount++;
                }
                writer.WriteEndArray();

                writer.WriteValueSeparator();
            }

            // "txt_forecast" : ""
            if (txt_forecast != null)
            {
                writer.WritePropertyName(nameof(txt_forecast));
                writer.WriteBeginArray();
                var itemCount = 0;
                foreach (TextForecast txt_cast in txt_forecast)
                {
                    if (itemCount > 0)
                        writer.WriteValueSeparator();
                    writer.WriteString(txt_cast?.ToJson());
                    itemCount++;
                }
                writer.WriteEndArray();

                writer.WriteValueSeparator();
            }

            // "condition" : ""
            writer.WritePropertyName(nameof(condition));
            writer.WriteString(condition?.ToJson());

            writer.WriteValueSeparator();

            // "atmosphere" : ""
            writer.WritePropertyName(nameof(atmosphere));
            writer.WriteString(atmosphere?.ToJson());

            writer.WriteValueSeparator();

            // "astronomy" : ""
            writer.WritePropertyName(nameof(astronomy));
            writer.WriteString(astronomy?.ToJson());

            writer.WriteValueSeparator();

            // "precipitation" : ""
            if (precipitation != null)
            {
                writer.WritePropertyName(nameof(precipitation));
                writer.WriteString(precipitation?.ToJson());

                writer.WriteValueSeparator();
            }

            // "ttl" : ""
            writer.WritePropertyName(nameof(ttl));
            writer.WriteString(ttl);

            writer.WriteValueSeparator();

            // "source" : ""
            writer.WritePropertyName(nameof(source));
            writer.WriteString(source);

            writer.WriteValueSeparator();

            // "query" : ""
            writer.WritePropertyName(nameof(query));
            writer.WriteString(query);

            writer.WriteValueSeparator();

            // "locale" : ""
            writer.WritePropertyName(nameof(locale));
            writer.WriteString(locale);

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }

        public bool IsValid()
        {
            if (location == null || condition == null || atmosphere == null)
                return false;
            else
                return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Weather weather &&
                   EqualityComparer<Location>.Default.Equals(location, weather.location) &&
                   update_time.Equals(weather.update_time) &&
                   EqualityComparer<IList<Forecast>>.Default.Equals(forecast, weather.forecast) &&
                   EqualityComparer<IList<HourlyForecast>>.Default.Equals(hr_forecast, weather.hr_forecast) &&
                   EqualityComparer<IList<TextForecast>>.Default.Equals(txt_forecast, weather.txt_forecast) &&
                   EqualityComparer<Condition>.Default.Equals(condition, weather.condition) &&
                   EqualityComparer<Atmosphere>.Default.Equals(atmosphere, weather.atmosphere) &&
                   EqualityComparer<Astronomy>.Default.Equals(astronomy, weather.astronomy) &&
                   EqualityComparer<Precipitation>.Default.Equals(precipitation, weather.precipitation) &&
                   EqualityComparer<IEnumerable<WeatherAlert>>.Default.Equals(weather_alerts, weather.weather_alerts) &&
                   ttl == weather.ttl &&
                   source == weather.source &&
                   query == weather.query &&
                   locale == weather.locale &&
                   updatetimeblob == weather.updatetimeblob;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(location);
            hash.Add(update_time);
            hash.Add(forecast);
            hash.Add(hr_forecast);
            hash.Add(txt_forecast);
            hash.Add(condition);
            hash.Add(atmosphere);
            hash.Add(astronomy);
            hash.Add(precipitation);
            hash.Add(weather_alerts);
            hash.Add(ttl);
            hash.Add(source);
            hash.Add(query);
            hash.Add(locale);
            hash.Add(updatetimeblob);
            return hash.ToHashCode();
        }
    }

    [JsonFormatter(typeof(CustomJsonConverter<Location>))]
    public partial class Location : CustomJsonObject
    {
        public string name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public TimeSpan tz_offset { get; set; }
        public string tz_short { get; set; }
        public string tz_long { get; set; }

        internal Location()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref JsonReader extReader)
        {
            JsonReader reader;
            string jsonValue;

            if (extReader.GetCurrentJsonToken() == JsonToken.String)
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
            }

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.ReadNext(); // prop value

                switch (property)
                {
                    case nameof(name):
                        this.name = reader.ReadString();
                        break;

                    case nameof(latitude):
                        this.latitude = reader.ReadString();
                        break;

                    case nameof(longitude):
                        this.longitude = reader.ReadString();
                        break;

                    case nameof(tz_offset):
                        this.tz_offset = TimeSpan.Parse(reader.ReadString());
                        break;

                    case nameof(tz_short):
                        this.tz_short = reader.ReadString();
                        break;

                    case nameof(tz_long):
                        this.tz_long = reader.ReadString();
                        break;

                    default:
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();

            // {
            writer.WriteBeginObject();

            // "name" : ""
            writer.WritePropertyName(nameof(name));
            writer.WriteString(name);

            writer.WriteValueSeparator();

            // "latitude" : ""
            writer.WritePropertyName(nameof(latitude));
            writer.WriteString(latitude);

            writer.WriteValueSeparator();

            // "longitude" : ""
            writer.WritePropertyName(nameof(longitude));
            writer.WriteString(longitude);

            writer.WriteValueSeparator();

            // "tz_offset" : ""
            writer.WritePropertyName(nameof(tz_offset));
            writer.WriteString(tz_offset.ToZoneOffsetFormat());

            writer.WriteValueSeparator();

            // "tz_short" : ""
            writer.WritePropertyName(nameof(tz_short));
            writer.WriteString(tz_short);

            writer.WriteValueSeparator();

            // "tz_long" : ""
            writer.WritePropertyName(nameof(tz_long));
            writer.WriteString(tz_long);

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }
    }

    [JsonFormatter(typeof(CustomJsonConverter<Forecast>))]
    public partial class Forecast : CustomJsonObject
    {
        public DateTime date { get; set; }
        public string high_f { get; set; }
        public string high_c { get; set; }
        public string low_f { get; set; }
        public string low_c { get; set; }
        public string condition { get; set; }
        public string icon { get; set; }
        public ForecastExtras extras { get; set; }

        internal Forecast()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref JsonReader extReader)
        {
            JsonReader reader;
            string jsonValue;

            if (extReader.GetCurrentJsonToken() == JsonToken.String)
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
            }

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.ReadNext(); // prop value

                switch (property)
                {
                    case nameof(date):
                        this.date = DateTime.ParseExact(reader.ReadString(), DateTimeUtils.ISO8601_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;

                    case nameof(high_f):
                        this.high_f = reader.ReadString();
                        break;

                    case nameof(high_c):
                        this.high_c = reader.ReadString();
                        break;

                    case nameof(low_f):
                        this.low_f = reader.ReadString();
                        break;

                    case nameof(low_c):
                        this.low_c = reader.ReadString();
                        break;

                    case nameof(condition):
                        this.condition = reader.ReadString();
                        break;

                    case nameof(icon):
                        this.icon = reader.ReadString();
                        break;

                    case nameof(extras):
                        this.extras = new ForecastExtras();
                        this.extras.FromJson(ref reader);
                        break;

                    default:
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();

            // {
            writer.WriteBeginObject();

            // "date" : ""
            writer.WritePropertyName(nameof(date));
            writer.WriteString(date.ToISO8601Format());

            writer.WriteValueSeparator();

            // "high_f" : ""
            writer.WritePropertyName(nameof(high_f));
            writer.WriteString(high_f);

            writer.WriteValueSeparator();

            // "high_c" : ""
            writer.WritePropertyName(nameof(high_c));
            writer.WriteString(high_c);

            writer.WriteValueSeparator();

            // "low_f" : ""
            writer.WritePropertyName(nameof(low_f));
            writer.WriteString(low_f);

            writer.WriteValueSeparator();

            // "low_c" : ""
            writer.WritePropertyName(nameof(low_c));
            writer.WriteString(low_c);

            writer.WriteValueSeparator();

            // "condition" : ""
            writer.WritePropertyName(nameof(condition));
            writer.WriteString(condition);

            writer.WriteValueSeparator();

            // "icon" : ""
            writer.WritePropertyName(nameof(icon));
            writer.WriteString(icon);

            // "extras" : ""
            if (extras != null)
            {
                writer.WriteValueSeparator();

                writer.WritePropertyName(nameof(extras));
                writer.WriteString(extras?.ToJson());
            }

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }
    }

    [JsonFormatter(typeof(CustomJsonConverter<HourlyForecast>))]
    public partial class HourlyForecast : CustomJsonObject
    {
        [IgnoreDataMember]
        public DateTimeOffset date
        {
            get
            {
                if (DateTimeOffset.TryParseExact(_date, DateTimeUtils.DATETIMEOFFSET_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset result))
                    return result;
                else
                    return DateTimeOffset.Parse(_date);
            }
            set { _date = value.ToDateTimeOffsetFormat(); }
        }

        public string high_f { get; set; }
        public string high_c { get; set; }
        public string condition { get; set; }
        public string icon { get; set; }
        public string pop { get; set; }
        public int wind_degrees { get; set; }
        public float wind_mph { get; set; }
        public float wind_kph { get; set; }
        public ForecastExtras extras { get; set; }

        [DataMember(Name = nameof(date))]
        private string _date { get; set; }

        internal HourlyForecast()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref JsonReader extReader)
        {
            JsonReader reader;
            string jsonValue;

            if (extReader.GetCurrentJsonToken() == JsonToken.String)
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
            }

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.ReadNext(); // prop value

                switch (property)
                {
                    case nameof(date):
                        this._date = reader.ReadString();
                        break;

                    case nameof(high_f):
                        this.high_f = reader.ReadString();
                        break;

                    case nameof(high_c):
                        this.high_c = reader.ReadString();
                        break;

                    case nameof(condition):
                        this.condition = reader.ReadString();
                        break;

                    case nameof(icon):
                        this.icon = reader.ReadString();
                        break;

                    case nameof(pop):
                        this.pop = reader.ReadString();
                        break;

                    case nameof(wind_degrees):
                        this.wind_degrees = reader.ReadInt32();
                        break;

                    case nameof(wind_mph):
                        this.wind_mph = reader.ReadSingle();
                        break;

                    case nameof(wind_kph):
                        this.wind_kph = reader.ReadSingle();
                        break;

                    case nameof(extras):
                        this.extras = new ForecastExtras();
                        this.extras.FromJson(ref reader);
                        break;

                    default:
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();

            // {
            writer.WriteBeginObject();

            // "date" : ""
            writer.WritePropertyName(nameof(date));
            writer.WriteString(_date);

            writer.WriteValueSeparator();

            // "high_f" : ""
            writer.WritePropertyName(nameof(high_f));
            writer.WriteString(high_f);

            writer.WriteValueSeparator();

            // "high_c" : ""
            writer.WritePropertyName(nameof(high_c));
            writer.WriteString(high_c);

            writer.WriteValueSeparator();

            // "condition" : ""
            writer.WritePropertyName(nameof(condition));
            writer.WriteString(condition);

            writer.WriteValueSeparator();

            // "icon" : ""
            writer.WritePropertyName(nameof(icon));
            writer.WriteString(icon);

            writer.WriteValueSeparator();

            // "pop" : ""
            writer.WritePropertyName(nameof(pop));
            writer.WriteString(pop);

            writer.WriteValueSeparator();

            // "wind_degrees" : ""
            writer.WritePropertyName(nameof(wind_degrees));
            writer.WriteInt32(wind_degrees);

            writer.WriteValueSeparator();

            // "wind_mph" : ""
            writer.WritePropertyName(nameof(wind_mph));
            writer.WriteSingle(wind_mph);

            writer.WriteValueSeparator();

            // "wind_kph" : ""
            writer.WritePropertyName(nameof(wind_kph));
            writer.WriteSingle(wind_kph);

            // "extras" : ""
            if (extras != null)
            {
                writer.WriteValueSeparator();

                writer.WritePropertyName(nameof(extras));
                writer.WriteString(extras?.ToJson());
            }

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }
    }

    [JsonFormatter(typeof(CustomJsonConverter<TextForecast>))]
    public partial class TextForecast : CustomJsonObject
    {
        public string title { get; set; }
        public string fcttext { get; set; }
        public string fcttext_metric { get; set; }
        public string icon { get; set; }
        public string pop { get; set; }

        internal TextForecast()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref JsonReader extReader)
        {
            JsonReader reader;
            string jsonValue;

            if (extReader.GetCurrentJsonToken() == JsonToken.String)
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
            }

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.ReadNext(); // prop value

                switch (property)
                {
                    case nameof(title):
                        this.title = reader.ReadString();
                        break;

                    case nameof(fcttext):
                        this.fcttext = reader.ReadString();
                        break;

                    case nameof(fcttext_metric):
                        this.fcttext_metric = reader.ReadString();
                        break;

                    case nameof(icon):
                        this.icon = reader.ReadString();
                        break;

                    case nameof(pop):
                        this.pop = reader.ReadString();
                        break;

                    default:
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();

            // {
            writer.WriteBeginObject();

            // "title" : ""
            writer.WritePropertyName(nameof(title));
            writer.WriteString(title);

            writer.WriteValueSeparator();

            // "fcttext" : ""
            writer.WritePropertyName(nameof(fcttext));
            writer.WriteString(fcttext);

            writer.WriteValueSeparator();

            // "fcttext_metric" : ""
            writer.WritePropertyName(nameof(fcttext_metric));
            writer.WriteString(fcttext_metric);

            writer.WriteValueSeparator();

            // "icon" : ""
            writer.WritePropertyName(nameof(icon));
            writer.WriteString(icon);

            writer.WriteValueSeparator();

            // "pop" : ""
            writer.WritePropertyName(nameof(pop));
            writer.WriteString(pop);

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }
    }

    [JsonFormatter(typeof(CustomJsonConverter<ForecastExtras>))]
    public class ForecastExtras : CustomJsonObject
    {
        public float feelslike_f { get; set; }
        public float feelslike_c { get; set; }
        public string humidity { get; set; }
        public string dewpoint_f { get; set; }
        public string dewpoint_c { get; set; }
        public float uv_index { get; set; } = -1.0f;
        public string pop { get; set; }
        public float qpf_rain_in { get; set; } = -1.0f;
        public float qpf_rain_mm { get; set; } = -1.0f;
        public float qpf_snow_in { get; set; } = -1.0f;
        public float qpf_snow_cm { get; set; } = -1.0f;
        public string pressure_mb { get; set; }
        public string pressure_in { get; set; }
        public int wind_degrees { get; set; }
        public float wind_mph { get; set; } = -1.0f;
        public float wind_kph { get; set; } = -1.0f;
        public string visibility_mi { get; set; }
        public string visibility_km { get; set; }

        internal ForecastExtras()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref JsonReader extReader)
        {
            JsonReader reader;
            string jsonValue;

            if (extReader.GetCurrentJsonToken() == JsonToken.String)
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
            }

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.ReadNext(); // prop value

                switch (property)
                {
                    case nameof(feelslike_f):
                        this.feelslike_f = reader.ReadSingle();
                        break;

                    case nameof(feelslike_c):
                        this.feelslike_c = reader.ReadSingle();
                        break;

                    case nameof(humidity):
                        this.humidity = reader.ReadString();
                        break;

                    case nameof(dewpoint_f):
                        this.dewpoint_f = reader.ReadString();
                        break;

                    case nameof(dewpoint_c):
                        this.dewpoint_c = reader.ReadString();
                        break;

                    case nameof(uv_index):
                        this.uv_index = reader.ReadSingle();
                        break;

                    case nameof(pop):
                        this.pop = reader.ReadString();
                        break;

                    case nameof(qpf_rain_in):
                        this.qpf_rain_in = reader.ReadSingle();
                        break;

                    case nameof(qpf_rain_mm):
                        this.qpf_rain_mm = reader.ReadSingle();
                        break;

                    case nameof(qpf_snow_in):
                        this.qpf_snow_in = reader.ReadSingle();
                        break;

                    case nameof(qpf_snow_cm):
                        this.qpf_snow_cm = reader.ReadSingle();
                        break;

                    case nameof(pressure_mb):
                        this.pressure_mb = reader.ReadString();
                        break;

                    case nameof(pressure_in):
                        this.pressure_in = reader.ReadString();
                        break;

                    case nameof(wind_degrees):
                        this.wind_degrees = reader.ReadInt32();
                        break;

                    case nameof(wind_mph):
                        this.wind_mph = reader.ReadSingle();
                        break;

                    case nameof(wind_kph):
                        this.wind_kph = reader.ReadSingle();
                        break;

                    case nameof(visibility_mi):
                        this.visibility_mi = reader.ReadString();
                        break;

                    case nameof(visibility_km):
                        this.visibility_km = reader.ReadString();
                        break;

                    default:
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();
            // {
            writer.WriteBeginObject();

            // "feelslike_f" : ""
            writer.WritePropertyName(nameof(feelslike_f));
            writer.WriteSingle(feelslike_f);

            writer.WriteValueSeparator();

            // "feelslike_c" : ""
            writer.WritePropertyName(nameof(feelslike_c));
            writer.WriteSingle(feelslike_c);

            writer.WriteValueSeparator();

            // "humidity" : ""
            writer.WritePropertyName(nameof(humidity));
            writer.WriteString(humidity);

            writer.WriteValueSeparator();

            // "dewpoint_f" : ""
            writer.WritePropertyName(nameof(dewpoint_f));
            writer.WriteString(dewpoint_f);

            writer.WriteValueSeparator();

            // "dewpoint_c" : ""
            writer.WritePropertyName(nameof(dewpoint_c));
            writer.WriteString(dewpoint_c);

            writer.WriteValueSeparator();

            // "uv_index" : ""
            writer.WritePropertyName(nameof(uv_index));
            writer.WriteSingle(uv_index);

            writer.WriteValueSeparator();

            // "pop" : ""
            writer.WritePropertyName(nameof(pop));
            writer.WriteString(pop);

            writer.WriteValueSeparator();

            // "qpf_rain_in" : ""
            writer.WritePropertyName(nameof(qpf_rain_in));
            writer.WriteSingle(qpf_rain_in);

            writer.WriteValueSeparator();

            // "qpf_rain_mm" : ""
            writer.WritePropertyName(nameof(qpf_rain_mm));
            writer.WriteSingle(qpf_rain_mm);

            writer.WriteValueSeparator();

            // "qpf_snow_in" : ""
            writer.WritePropertyName(nameof(qpf_snow_in));
            writer.WriteSingle(qpf_snow_in);

            writer.WriteValueSeparator();

            // "qpf_snow_cm" : ""
            writer.WritePropertyName(nameof(qpf_snow_cm));
            writer.WriteSingle(qpf_snow_cm);

            writer.WriteValueSeparator();

            // "pressure_mb" : ""
            writer.WritePropertyName(nameof(pressure_mb));
            writer.WriteString(pressure_mb);

            writer.WriteValueSeparator();

            // "pressure_in" : ""
            writer.WritePropertyName(nameof(pressure_in));
            writer.WriteString(pressure_in);

            writer.WriteValueSeparator();

            // "wind_degrees" : ""
            writer.WritePropertyName(nameof(wind_degrees));
            writer.WriteInt32(wind_degrees);

            writer.WriteValueSeparator();

            // "wind_mph" : ""
            writer.WritePropertyName(nameof(wind_mph));
            writer.WriteSingle(wind_mph);

            writer.WriteValueSeparator();

            // "wind_kph" : ""
            writer.WritePropertyName(nameof(wind_kph));
            writer.WriteSingle(wind_kph);

            writer.WriteValueSeparator();

            // "visibility_mi" : ""
            writer.WritePropertyName(nameof(visibility_mi));
            writer.WriteString(visibility_mi);

            writer.WriteValueSeparator();

            // "visibility_km" : ""
            writer.WritePropertyName(nameof(visibility_km));
            writer.WriteString(visibility_km);

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }
    }

    [JsonFormatter(typeof(CustomJsonConverter<Condition>))]
    public partial class Condition : CustomJsonObject
    {
        public string weather { get; set; }
        public float temp_f { get; set; }
        public float temp_c { get; set; }
        public int wind_degrees { get; set; }
        public float wind_mph { get; set; }
        public float wind_kph { get; set; }
        public float feelslike_f { get; set; }
        public float feelslike_c { get; set; }
        public string icon { get; set; }
        public Beaufort beaufort { get; set; }
        public UV uv { get; set; }

        internal Condition()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref JsonReader extReader)
        {
            JsonReader reader;
            string jsonValue;

            if (extReader.GetCurrentJsonToken() == JsonToken.String)
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
            }

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.ReadNext(); // prop value

                switch (property)
                {
                    case nameof(weather):
                        this.weather = reader.ReadString();
                        break;

                    case nameof(temp_f):
                        this.temp_f = reader.ReadSingle();
                        break;

                    case nameof(temp_c):
                        this.temp_c = reader.ReadSingle();
                        break;

                    case nameof(wind_degrees):
                        this.wind_degrees = reader.ReadInt32();
                        break;

                    case nameof(wind_mph):
                        this.wind_mph = reader.ReadSingle();
                        break;

                    case nameof(wind_kph):
                        this.wind_kph = reader.ReadSingle();
                        break;

                    case nameof(feelslike_f):
                        this.feelslike_f = reader.ReadSingle();
                        break;

                    case nameof(feelslike_c):
                        this.feelslike_c = reader.ReadSingle();
                        break;

                    case nameof(icon):
                        this.icon = reader.ReadString();
                        break;

                    case nameof(beaufort):
                        this.beaufort = new Beaufort();
                        this.beaufort.FromJson(ref reader);
                        break;

                    case nameof(uv):
                        this.uv = new UV();
                        this.uv.FromJson(ref reader);
                        break;

                    default:
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();

            // {
            writer.WriteBeginObject();

            // "weather" : ""
            writer.WritePropertyName(nameof(weather));
            writer.WriteString(weather);

            writer.WriteValueSeparator();

            // "temp_f" : ""
            writer.WritePropertyName(nameof(temp_f));
            writer.WriteSingle(temp_f);

            writer.WriteValueSeparator();

            // "temp_c" : ""
            writer.WritePropertyName(nameof(temp_c));
            writer.WriteSingle(temp_c);

            writer.WriteValueSeparator();

            // "wind_degrees" : ""
            writer.WritePropertyName(nameof(wind_degrees));
            writer.WriteInt32(wind_degrees);

            writer.WriteValueSeparator();

            // "wind_mph" : ""
            writer.WritePropertyName(nameof(wind_mph));
            writer.WriteSingle(wind_mph);

            writer.WriteValueSeparator();

            // "wind_kph" : ""
            writer.WritePropertyName(nameof(wind_kph));
            writer.WriteSingle(wind_kph);

            writer.WriteValueSeparator();

            // "feelslike_f" : ""
            writer.WritePropertyName(nameof(feelslike_f));
            writer.WriteSingle(feelslike_f);

            writer.WriteValueSeparator();

            // "feelslike_c" : ""
            writer.WritePropertyName(nameof(feelslike_c));
            writer.WriteSingle(feelslike_c);

            writer.WriteValueSeparator();

            // "icon" : ""
            writer.WritePropertyName(nameof(icon));
            writer.WriteString(icon);

            // "beaufort" : ""
            if (beaufort != null)
            {
                writer.WriteValueSeparator();

                writer.WritePropertyName(nameof(beaufort));
                writer.WriteString(beaufort?.ToJson());
            }

            // "uv" : ""
            if (uv != null)
            {
                writer.WriteValueSeparator();

                writer.WritePropertyName(nameof(uv));
                writer.WriteString(uv?.ToJson());
            }

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }
    }

    [JsonFormatter(typeof(CustomJsonConverter<Atmosphere>))]
    public partial class Atmosphere : CustomJsonObject
    {
        public string humidity { get; set; }
        public string pressure_mb { get; set; }
        public string pressure_in { get; set; }
        public string pressure_trend { get; set; }
        public string visibility_mi { get; set; }
        public string visibility_km { get; set; }
        public string dewpoint_f { get; set; }
        public string dewpoint_c { get; set; }

        internal Atmosphere()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref JsonReader extReader)
        {
            JsonReader reader;
            string jsonValue;

            if (extReader.GetCurrentJsonToken() == JsonToken.String)
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
            }

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.ReadNext(); // prop value

                switch (property)
                {
                    case nameof(humidity):
                        this.humidity = reader.ReadString();
                        break;

                    case nameof(pressure_mb):
                        this.pressure_mb = reader.ReadString();
                        break;

                    case nameof(pressure_in):
                        this.pressure_in = reader.ReadString();
                        break;

                    case nameof(pressure_trend):
                        this.pressure_trend = reader.ReadString();
                        break;

                    case nameof(visibility_mi):
                        this.visibility_mi = reader.ReadString();
                        break;

                    case nameof(visibility_km):
                        this.visibility_km = reader.ReadString();
                        break;

                    case nameof(dewpoint_f):
                        this.dewpoint_f = reader.ReadString();
                        break;

                    case nameof(dewpoint_c):
                        this.dewpoint_c = reader.ReadString();
                        break;

                    default:
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();

            // {
            writer.WriteBeginObject();

            // "humidity" : ""
            writer.WritePropertyName(nameof(humidity));
            writer.WriteString(humidity);

            writer.WriteValueSeparator();

            // "pressure_mb" : ""
            writer.WritePropertyName(nameof(pressure_mb));
            writer.WriteString(pressure_mb);

            writer.WriteValueSeparator();

            // "pressure_in" : ""
            writer.WritePropertyName(nameof(pressure_in));
            writer.WriteString(pressure_in);

            writer.WriteValueSeparator();

            // "pressure_trend" : ""
            writer.WritePropertyName(nameof(pressure_trend));
            writer.WriteString(pressure_trend);

            writer.WriteValueSeparator();

            // "visibility_mi" : ""
            writer.WritePropertyName(nameof(visibility_mi));
            writer.WriteString(visibility_mi);

            writer.WriteValueSeparator();

            // "visibility_km" : ""
            writer.WritePropertyName(nameof(visibility_km));
            writer.WriteString(visibility_km);

            writer.WriteValueSeparator();

            // "dewpoint_f" : ""
            writer.WritePropertyName(nameof(dewpoint_f));
            writer.WriteString(dewpoint_f);

            writer.WriteValueSeparator();

            // "dewpoint_c" : ""
            writer.WritePropertyName(nameof(dewpoint_c));
            writer.WriteString(dewpoint_c);

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }
    }

    [JsonFormatter(typeof(CustomJsonConverter<Astronomy>))]
    public partial class Astronomy : CustomJsonObject
    {
        public DateTime sunrise { get; set; }
        public DateTime sunset { get; set; }
        public DateTime moonrise { get; set; }
        public DateTime moonset { get; set; }
        public MoonPhase moonphase { get; set; }

        internal Astronomy()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref JsonReader extReader)
        {
            JsonReader reader;
            string jsonValue;

            if (extReader.GetCurrentJsonToken() == JsonToken.String)
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
            }

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.ReadNext(); // prop value

                switch (property)
                {
                    case nameof(sunrise):
                        this.sunrise = DateTime.ParseExact(reader.ReadString(), DateTimeUtils.ISO8601_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;

                    case nameof(sunset):
                        this.sunset = DateTime.ParseExact(reader.ReadString(), DateTimeUtils.ISO8601_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;

                    case nameof(moonrise):
                        this.moonrise = DateTime.ParseExact(reader.ReadString(), DateTimeUtils.ISO8601_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;

                    case nameof(moonset):
                        this.moonset = DateTime.ParseExact(reader.ReadString(), DateTimeUtils.ISO8601_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;

                    case nameof(moonphase):
                        this.moonphase = new MoonPhase();
                        this.moonphase.FromJson(ref reader);
                        break;

                    default:
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();

            // {
            writer.WriteBeginObject();

            // "sunrise" : ""
            writer.WritePropertyName(nameof(sunrise));
            writer.WriteString(sunrise.ToISO8601Format());

            writer.WriteValueSeparator();

            // "sunset" : ""
            writer.WritePropertyName(nameof(sunset));
            writer.WriteString(sunset.ToISO8601Format());

            writer.WriteValueSeparator();

            // "moonrise" : ""
            writer.WritePropertyName(nameof(moonrise));
            writer.WriteString(moonrise.ToISO8601Format());

            writer.WriteValueSeparator();

            // "moonset" : ""
            writer.WritePropertyName(nameof(moonset));
            writer.WriteString(moonset.ToISO8601Format());

            // "moonphase" : ""
            if (moonphase != null)
            {
                writer.WriteValueSeparator();

                writer.WritePropertyName(nameof(moonphase));
                writer.WriteString(moonphase?.ToJson());
            }

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }
    }

    [JsonFormatter(typeof(CustomJsonConverter<Precipitation>))]
    public partial class Precipitation : CustomJsonObject
    {
        public string pop { get; set; }
        public float qpf_rain_in { get; set; }
        public float qpf_rain_mm { get; set; }
        public float qpf_snow_in { get; set; }
        public float qpf_snow_cm { get; set; }

        internal Precipitation()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref JsonReader extReader)
        {
            JsonReader reader;
            string jsonValue;

            if (extReader.GetCurrentJsonToken() == JsonToken.String)
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
            }

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.ReadNext(); // prop value

                switch (property)
                {
                    case nameof(pop):
                        this.pop = reader.ReadString();
                        break;

                    case nameof(qpf_rain_in):
                        this.qpf_rain_in = reader.ReadSingle();
                        break;

                    case nameof(qpf_rain_mm):
                        this.qpf_rain_mm = reader.ReadSingle();
                        break;

                    case nameof(qpf_snow_in):
                        this.qpf_snow_in = reader.ReadSingle();
                        break;

                    case nameof(qpf_snow_cm):
                        this.qpf_snow_cm = reader.ReadSingle();
                        break;

                    default:
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();

            // {
            writer.WriteBeginObject();

            // "pop" : ""
            writer.WritePropertyName(nameof(pop));
            writer.WriteString(pop);

            writer.WriteValueSeparator();

            // "qpf_rain_in" : ""
            writer.WritePropertyName(nameof(qpf_rain_in));
            writer.WriteSingle(qpf_rain_in);

            writer.WriteValueSeparator();

            // "qpf_rain_mm" : ""
            writer.WritePropertyName(nameof(qpf_rain_mm));
            writer.WriteSingle(qpf_rain_mm);

            writer.WriteValueSeparator();

            // "qpf_snow_in" : ""
            writer.WritePropertyName(nameof(qpf_snow_in));
            writer.WriteSingle(qpf_snow_in);

            writer.WriteValueSeparator();

            // "qpf_snow_cm" : ""
            writer.WritePropertyName(nameof(qpf_snow_cm));
            writer.WriteSingle(qpf_snow_cm);

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }
    }

    [JsonFormatter(typeof(CustomJsonConverter<Beaufort>))]
    public partial class Beaufort : CustomJsonObject
    {
        public enum BeaufortScale
        {
            B0 = 0,
            B1 = 1,
            B2 = 2,
            B3 = 3,
            B4 = 4,
            B5 = 5,
            B6 = 6,
            B7 = 7,
            B8 = 8,
            B9 = 9,
            B10 = 10,
            B11 = 11,
            B12 = 12
        }

        public BeaufortScale scale { get; set; }
        public string desc { get; set; }

        internal Beaufort()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref JsonReader extReader)
        {
            JsonReader reader;
            string jsonValue;

            if (extReader.GetCurrentJsonToken() == JsonToken.String)
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
            }

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.ReadNext(); // prop value

                switch (property)
                {
                    case nameof(scale):
                        this.scale = (BeaufortScale)reader.ReadInt32();
                        break;

                    case nameof(desc):
                        this.desc = reader.ReadString();
                        break;

                    default:
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();

            // {
            writer.WriteBeginObject();

            // "scale" : ""
            writer.WritePropertyName(nameof(scale));
            writer.WriteInt32((int)scale);

            writer.WriteValueSeparator();

            // "desc" : ""
            writer.WritePropertyName(nameof(desc));
            writer.WriteString(desc);

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }
    }

    [JsonFormatter(typeof(CustomJsonConverter<MoonPhase>))]
    public partial class MoonPhase : CustomJsonObject
    {
        public enum MoonPhaseType
        {
            NewMoon = 0,
            WaxingCrescent,
            FirstQtr,
            WaxingGibbous,
            FullMoon,
            WaningGibbous,
            LastQtr,
            WaningCrescent
        }

        public MoonPhaseType phase { get; set; }
        public string desc { get; set; }

        internal MoonPhase()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref JsonReader extReader)
        {
            JsonReader reader;
            string jsonValue;

            if (extReader.GetCurrentJsonToken() == JsonToken.String)
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
            }

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.ReadNext(); // prop value

                switch (property)
                {
                    case nameof(phase):
                        this.phase = (MoonPhaseType)reader.ReadInt32();
                        break;

                    case nameof(desc):
                        this.desc = reader.ReadString();
                        break;

                    default:
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();

            // {
            writer.WriteBeginObject();

            // "phase" : ""
            writer.WritePropertyName(nameof(phase));
            writer.WriteInt32((int)phase);

            writer.WriteValueSeparator();

            // "desc" : ""
            writer.WritePropertyName(nameof(desc));
            writer.WriteString(desc);

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }
    }

    [JsonFormatter(typeof(CustomJsonConverter<UV>))]
    public partial class UV : CustomJsonObject
    {
        public float index { get; set; } = -1;
        public string desc { get; set; }

        internal UV()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref JsonReader extReader)
        {
            JsonReader reader;
            string jsonValue;

            if (extReader.GetCurrentJsonToken() == JsonToken.String)
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
            }

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.ReadNext(); // prop value

                switch (property)
                {
                    case nameof(index):
                        this.index = reader.ReadSingle();
                        break;

                    case nameof(desc):
                        this.desc = reader.ReadString();
                        break;

                    default:
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();

            // {
            writer.WriteBeginObject();

            // "scale" : ""
            writer.WritePropertyName(nameof(index));
            writer.WriteSingle(index);

            writer.WriteValueSeparator();

            // "desc" : ""
            writer.WritePropertyName(nameof(desc));
            writer.WriteString(desc);

            // }
            writer.WriteEndObject();

            return writer.ToString();
        }
    }

    [Table("forecasts")]
    public class Forecasts
    {
        [Key]
        [Column(TypeName = "varchar")]
        public string query { get; set; }

        [Column(TypeName = "varchar")]
        public IList<Forecast> forecast { get; set; }
        [Column(TypeName = "varchar")]
        public IList<TextForecast> txt_forecast { get; set; }

        public Forecasts()
        {
        }

        public Forecasts(string query, IList<Forecast> forecast)
        {
            this.query = query;
            this.forecast = forecast;
        }
    }

    [Table("hr_forecasts")]
    public class HourlyForecasts
    {
        [Key]
        [Column(TypeName = "varchar")]
        public string query { get; set; }

        [IgnoreDataMember]
        [NotMapped]
        // Doesn't store this in db
        // For DateTimeOffset, offset isn't stored when saving to db
        // Store as string (blob) instead
        // If db previously stored DateTimeOffset (as ticks) retrieve and set offset
        public DateTimeOffset date
        {
            get
            {
                if (DateTimeOffset.TryParseExact(dateblob, DateTimeUtils.DATETIMEOFFSET_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset result))
                    return result;
                else
                    return DateTimeOffset.Parse(dateblob);
            }
            set { dateblob = value.ToDateTimeOffsetFormat(); }
        }

        [Column(TypeName = "varchar")]
        public HourlyForecast hr_forecast { get; set; }

        [Key]
        [DataMember(Name = "date")]
        [Column(TypeName = "varchar")]
        // Keep DateTimeOffset column name to get data as string
        public string dateblob { get; set; }

        public HourlyForecasts()
        {
        }

        public HourlyForecasts(string query, HourlyForecast forecast)
        {
            this.query = query;
            this.hr_forecast = forecast;
            this.date = forecast.date;
        }
    }
}