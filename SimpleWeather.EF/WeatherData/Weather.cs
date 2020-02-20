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
    [JsonConverter(typeof(CustomJsonConverter<Weather>))]
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
                if (DateTimeOffset.TryParseExact(updatetimeblob, "dd.MM.yyyy HH:mm:ss zzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result))
                    return result;
                else
                    return new DateTimeOffset(long.Parse(updatetimeblob), TimeSpan.Zero).ToOffset(location.tz_offset);
            }
            set { updatetimeblob = value.ToString("dd.MM.yyyy HH:mm:ss zzzz"); }
        }

        [JsonIgnore]
        [NotMapped]
        public IList<Forecast> forecast { get; set; }

        [JsonIgnore]
        [NotMapped]
        public IList<HourlyForecast> hr_forecast { get; set; }

        [JsonIgnore]
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

        [JsonIgnore]
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

        [JsonIgnore]
        [Column("update_time", TypeName = "varchar")]
        // Keep DateTimeOffset column name to get data as string
        public string updatetimeblob { get; set; }

        public Weather()
        {
            // Needed for deserialization
        }

        public override void FromJson(Utf8JsonReader reader)
        {
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                    reader.Read(); // StartObject

                string property = reader.GetString();
                reader.Read(); // prop value

                switch (property)
                {
                    case nameof(location):
                        this.location = new Location();
                        this.location.FromJson(reader);
                        break;

                    case nameof(update_time):
                        bool parsed = DateTimeOffset.TryParseExact(reader.GetString(), "dd.MM.yyyy HH:mm:ss zzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result);
                        if (!parsed) // If we can't parse as DateTimeOffset try DateTime (data could be old)
                            result = DateTime.Parse(reader.GetString());
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
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        {
                            if (reader.TokenType == JsonTokenType.String)
                            {
                                var forecast = new Forecast();
                                forecast.FromJson(reader);
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
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        {
                            if (reader.TokenType == JsonTokenType.String)
                            {
                                var hrf = new HourlyForecast();
                                hrf.FromJson(reader);
                                hr_forecasts.Add(hrf);
                            }
                        }
                        this.hr_forecast = hr_forecasts;
                        break;

                    case nameof(txt_forecast):
                        // Set initial cap to 20
                        // Most provider forecasts are <= 10 (x2 for day & nt)
                        var txt_forecasts = new List<TextForecast>(20);
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        {
                            if (reader.TokenType == JsonTokenType.String)
                            {
                                var txtf = new TextForecast();
                                txtf.FromJson(reader);
                                txt_forecasts.Add(txtf);
                            }
                        }
                        this.txt_forecast = txt_forecasts;
                        break;

                    case nameof(condition):
                        this.condition = new Condition();
                        this.condition.FromJson(reader);
                        break;

                    case nameof(atmosphere):
                        this.atmosphere = new Atmosphere();
                        this.atmosphere.FromJson(reader);
                        break;

                    case nameof(astronomy):
                        this.astronomy = new Astronomy();
                        this.astronomy.FromJson(reader);
                        break;

                    case nameof(precipitation):
                        this.precipitation = new Precipitation();
                        this.precipitation.FromJson(reader);
                        break;

                    case nameof(ttl):
                        this.ttl = reader.GetString();
                        break;

                    case nameof(source):
                        this.source = reader.GetString();
                        break;

                    case nameof(query):
                        this.query = reader.GetString();
                        break;

                    case nameof(locale):
                        this.locale = reader.GetString();
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

                // "location" : ""
                writer.WritePropertyName(nameof(location));
                writer.WriteStringValue(location?.ToJson());

                // "update_time" : ""
                writer.WritePropertyName(nameof(update_time));
                writer.WriteStringValue(update_time.ToString("dd.MM.yyyy HH:mm:ss zzzz"));

                // "forecast" : ""
                if (forecast != null)
                {
                    writer.WritePropertyName(nameof(forecast));
                    writer.WriteStartArray();
                    foreach (Forecast cast in forecast)
                    {
                        writer.WriteStringValue(cast?.ToJson());
                    }
                    writer.WriteEndArray();
                }

                // "hr_forecast" : ""
                if (hr_forecast != null)
                {
                    writer.WritePropertyName(nameof(hr_forecast));
                    writer.WriteStartArray();
                    foreach (HourlyForecast hr_cast in hr_forecast)
                    {
                        writer.WriteStringValue(hr_cast?.ToJson());
                    }
                    writer.WriteEndArray();
                }

                // "txt_forecast" : ""
                if (txt_forecast != null)
                {
                    writer.WritePropertyName(nameof(txt_forecast));
                    writer.WriteStartArray();
                    foreach (TextForecast txt_cast in txt_forecast)
                    {
                        writer.WriteStringValue(txt_cast?.ToJson());
                    }
                    writer.WriteEndArray();
                }

                // "condition" : ""
                writer.WritePropertyName(nameof(condition));
                writer.WriteStringValue(condition?.ToJson());

                // "atmosphere" : ""
                writer.WritePropertyName(nameof(atmosphere));
                writer.WriteStringValue(atmosphere?.ToJson());

                // "astronomy" : ""
                writer.WritePropertyName(nameof(astronomy));
                writer.WriteStringValue(astronomy?.ToJson());

                // "precipitation" : ""
                if (precipitation != null)
                {
                    writer.WritePropertyName(nameof(precipitation));
                    writer.WriteStringValue(precipitation?.ToJson());
                }

                // "ttl" : ""
                writer.WritePropertyName(nameof(ttl));
                writer.WriteStringValue(ttl);

                // "source" : ""
                writer.WritePropertyName(nameof(source));
                writer.WriteStringValue(source);

                // "query" : ""
                writer.WritePropertyName(nameof(query));
                writer.WriteStringValue(query);

                // "locale" : ""
                writer.WritePropertyName(nameof(locale));
                writer.WriteStringValue(locale);

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
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

    [JsonConverter(typeof(CustomJsonConverter<Location>))]
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
                    case nameof(name):
                        this.name = reader.GetString();
                        break;

                    case nameof(latitude):
                        this.latitude = reader.GetString();
                        break;

                    case nameof(longitude):
                        this.longitude = reader.GetString();
                        break;

                    case nameof(tz_offset):
                        this.tz_offset = TimeSpan.Parse(reader.GetString());
                        break;

                    case nameof(tz_short):
                        this.tz_short = reader.GetString();
                        break;

                    case nameof(tz_long):
                        this.tz_long = reader.GetString();
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

                // "name" : ""
                writer.WritePropertyName(nameof(name));
                writer.WriteStringValue(name);

                // "latitude" : ""
                writer.WritePropertyName(nameof(latitude));
                writer.WriteStringValue(latitude);

                // "longitude" : ""
                writer.WritePropertyName(nameof(longitude));
                writer.WriteStringValue(longitude);

                // "tz_offset" : ""
                writer.WritePropertyName(nameof(tz_offset));
                writer.WriteStringValue(tz_offset.ToString("hh:mm:ss"));

                // "tz_short" : ""
                writer.WritePropertyName(nameof(tz_short));
                writer.WriteStringValue(tz_short);

                // "tz_long" : ""
                writer.WritePropertyName(nameof(tz_long));
                writer.WriteStringValue(tz_long);

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<Forecast>))]
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
                    case nameof(date):
                        this.date = DateTime.Parse(reader.GetString());
                        break;

                    case nameof(high_f):
                        this.high_f = reader.GetString();
                        break;

                    case nameof(high_c):
                        this.high_c = reader.GetString();
                        break;

                    case nameof(low_f):
                        this.low_f = reader.GetString();
                        break;

                    case nameof(low_c):
                        this.low_c = reader.GetString();
                        break;

                    case nameof(condition):
                        this.condition = reader.GetString();
                        break;

                    case nameof(icon):
                        this.icon = reader.GetString();
                        break;

                    case nameof(extras):
                        this.extras = new ForecastExtras();
                        this.extras.FromJson(reader);
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

                // "date" : ""
                writer.WritePropertyName(nameof(date));
                writer.WriteStringValue(date);

                // "high_f" : ""
                writer.WritePropertyName(nameof(high_f));
                writer.WriteStringValue(high_f);

                // "high_c" : ""
                writer.WritePropertyName(nameof(high_c));
                writer.WriteStringValue(high_c);

                // "low_f" : ""
                writer.WritePropertyName(nameof(low_f));
                writer.WriteStringValue(low_f);

                // "low_c" : ""
                writer.WritePropertyName(nameof(low_c));
                writer.WriteStringValue(low_c);

                // "condition" : ""
                writer.WritePropertyName(nameof(condition));
                writer.WriteStringValue(condition);

                // "icon" : ""
                writer.WritePropertyName(nameof(icon));
                writer.WriteStringValue(icon);

                // "extras" : ""
                if (extras != null)
                {
                    writer.WritePropertyName(nameof(extras));
                    writer.WriteStringValue(extras?.ToJson());
                }

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<HourlyForecast>))]
    public partial class HourlyForecast : CustomJsonObject
    {
        [JsonIgnore]
        public DateTimeOffset date
        {
            get
            {
                if (DateTimeOffset.TryParseExact(_date, "dd.MM.yyyy HH:mm:ss zzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result))
                    return result;
                else
                    return DateTimeOffset.Parse(_date);
            }
            set { _date = value.ToString("dd.MM.yyyy HH:mm:ss zzzz"); }
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

        [JsonPropertyName(nameof(date))]
        private string _date { get; set; }

        internal HourlyForecast()
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
                    case nameof(date):
                        this._date = reader.GetString();
                        break;

                    case nameof(high_f):
                        this.high_f = reader.GetString();
                        break;

                    case nameof(high_c):
                        this.high_c = reader.GetString();
                        break;

                    case nameof(condition):
                        this.condition = reader.GetString();
                        break;

                    case nameof(icon):
                        this.icon = reader.GetString();
                        break;

                    case nameof(pop):
                        this.pop = reader.GetString();
                        break;

                    case nameof(wind_degrees):
                        this.wind_degrees = reader.GetInt32();
                        break;

                    case nameof(wind_mph):
                        this.wind_mph = reader.GetSingle();
                        break;

                    case nameof(wind_kph):
                        this.wind_kph = reader.GetSingle();
                        break;

                    case nameof(extras):
                        this.extras = new ForecastExtras();
                        this.extras.FromJson(reader);
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

                // "date" : ""
                writer.WritePropertyName(nameof(date));
                writer.WriteStringValue(_date);

                // "high_f" : ""
                writer.WritePropertyName(nameof(high_f));
                writer.WriteStringValue(high_f);

                // "high_c" : ""
                writer.WritePropertyName(nameof(high_c));
                writer.WriteStringValue(high_c);

                // "condition" : ""
                writer.WritePropertyName(nameof(condition));
                writer.WriteStringValue(condition);

                // "icon" : ""
                writer.WritePropertyName(nameof(icon));
                writer.WriteStringValue(icon);

                // "pop" : ""
                writer.WritePropertyName(nameof(pop));
                writer.WriteStringValue(pop);

                // "wind_degrees" : ""
                writer.WritePropertyName(nameof(wind_degrees));
                writer.WriteNumberValue(wind_degrees);

                // "wind_mph" : ""
                writer.WritePropertyName(nameof(wind_mph));
                writer.WriteNumberValue(wind_mph);

                // "wind_kph" : ""
                writer.WritePropertyName(nameof(wind_kph));
                writer.WriteNumberValue(wind_kph);

                // "extras" : ""
                if (extras != null)
                {
                    writer.WritePropertyName(nameof(extras));
                    writer.WriteStringValue(extras?.ToJson());
                }

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<TextForecast>))]
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
                    case nameof(title):
                        this.title = reader.GetString();
                        break;

                    case nameof(fcttext):
                        this.fcttext = reader.GetString();
                        break;

                    case nameof(fcttext_metric):
                        this.fcttext_metric = reader.GetString();
                        break;

                    case nameof(icon):
                        this.icon = reader.GetString();
                        break;

                    case nameof(pop):
                        this.pop = reader.GetString();
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

                // "title" : ""
                writer.WritePropertyName(nameof(title));
                writer.WriteStringValue(title);

                // "fcttext" : ""
                writer.WritePropertyName(nameof(fcttext));
                writer.WriteStringValue(fcttext);

                // "fcttext_metric" : ""
                writer.WritePropertyName(nameof(fcttext_metric));
                writer.WriteStringValue(fcttext_metric);

                // "icon" : ""
                writer.WritePropertyName(nameof(icon));
                writer.WriteStringValue(icon);

                // "pop" : ""
                writer.WritePropertyName(nameof(pop));
                writer.WriteStringValue(pop);

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<ForecastExtras>))]
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
                    case nameof(feelslike_f):
                        this.feelslike_f = reader.GetSingle();
                        break;

                    case nameof(feelslike_c):
                        this.feelslike_c = reader.GetSingle();
                        break;

                    case nameof(humidity):
                        this.humidity = reader.GetString();
                        break;

                    case nameof(dewpoint_f):
                        this.dewpoint_f = reader.GetString();
                        break;

                    case nameof(dewpoint_c):
                        this.dewpoint_c = reader.GetString();
                        break;

                    case nameof(uv_index):
                        this.uv_index = reader.GetSingle();
                        break;

                    case nameof(pop):
                        this.pop = reader.GetString();
                        break;

                    case nameof(qpf_rain_in):
                        this.qpf_rain_in = reader.GetSingle();
                        break;

                    case nameof(qpf_rain_mm):
                        this.qpf_rain_mm = reader.GetSingle();
                        break;

                    case nameof(qpf_snow_in):
                        this.qpf_snow_in = reader.GetSingle();
                        break;

                    case nameof(qpf_snow_cm):
                        this.qpf_snow_cm = reader.GetSingle();
                        break;

                    case nameof(pressure_mb):
                        this.pressure_mb = reader.GetString();
                        break;

                    case nameof(pressure_in):
                        this.pressure_in = reader.GetString();
                        break;

                    case nameof(wind_degrees):
                        this.wind_degrees = reader.GetInt32();
                        break;

                    case nameof(wind_mph):
                        this.wind_mph = reader.GetSingle();
                        break;

                    case nameof(wind_kph):
                        this.wind_kph = reader.GetSingle();
                        break;

                    case nameof(visibility_mi):
                        this.visibility_mi = reader.GetString();
                        break;

                    case nameof(visibility_km):
                        this.visibility_km = reader.GetString();
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

                // "feelslike_f" : ""
                writer.WritePropertyName(nameof(feelslike_f));
                writer.WriteNumberValue(feelslike_f);

                // "feelslike_c" : ""
                writer.WritePropertyName(nameof(feelslike_c));
                writer.WriteNumberValue(feelslike_c);

                // "humidity" : ""
                writer.WritePropertyName(nameof(humidity));
                writer.WriteStringValue(humidity);

                // "dewpoint_f" : ""
                writer.WritePropertyName(nameof(dewpoint_f));
                writer.WriteStringValue(dewpoint_f);

                // "dewpoint_c" : ""
                writer.WritePropertyName(nameof(dewpoint_c));
                writer.WriteStringValue(dewpoint_c);

                // "uv_index" : ""
                writer.WritePropertyName(nameof(uv_index));
                writer.WriteNumberValue(uv_index);

                // "pop" : ""
                writer.WritePropertyName(nameof(pop));
                writer.WriteStringValue(pop);

                // "qpf_rain_in" : ""
                writer.WritePropertyName(nameof(qpf_rain_in));
                writer.WriteNumberValue(qpf_rain_in);

                // "qpf_rain_mm" : ""
                writer.WritePropertyName(nameof(qpf_rain_mm));
                writer.WriteNumberValue(qpf_rain_mm);

                // "qpf_snow_in" : ""
                writer.WritePropertyName(nameof(qpf_snow_in));
                writer.WriteNumberValue(qpf_snow_in);

                // "qpf_snow_cm" : ""
                writer.WritePropertyName(nameof(qpf_snow_cm));
                writer.WriteNumberValue(qpf_snow_cm);

                // "pressure_mb" : ""
                writer.WritePropertyName(nameof(pressure_mb));
                writer.WriteStringValue(pressure_mb);

                // "pressure_in" : ""
                writer.WritePropertyName(nameof(pressure_in));
                writer.WriteStringValue(pressure_in);

                // "wind_degrees" : ""
                writer.WritePropertyName(nameof(wind_degrees));
                writer.WriteNumberValue(wind_degrees);

                // "wind_mph" : ""
                writer.WritePropertyName(nameof(wind_mph));
                writer.WriteNumberValue(wind_mph);

                // "wind_kph" : ""
                writer.WritePropertyName(nameof(wind_kph));
                writer.WriteNumberValue(wind_kph);

                // "visibility_mi" : ""
                writer.WritePropertyName(nameof(visibility_mi));
                writer.WriteStringValue(visibility_mi);

                // "visibility_km" : ""
                writer.WritePropertyName(nameof(visibility_km));
                writer.WriteStringValue(visibility_km);

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<Condition>))]
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
                    case nameof(weather):
                        this.weather = reader.GetString();
                        break;

                    case nameof(temp_f):
                        this.temp_f = reader.GetSingle();
                        break;

                    case nameof(temp_c):
                        this.temp_c = reader.GetSingle();
                        break;

                    case nameof(wind_degrees):
                        this.wind_degrees = reader.GetInt32();
                        break;

                    case nameof(wind_mph):
                        this.wind_mph = reader.GetSingle();
                        break;

                    case nameof(wind_kph):
                        this.wind_kph = reader.GetSingle();
                        break;

                    case nameof(feelslike_f):
                        this.feelslike_f = reader.GetSingle();
                        break;

                    case nameof(feelslike_c):
                        this.feelslike_c = reader.GetSingle();
                        break;

                    case nameof(icon):
                        this.icon = reader.GetString();
                        break;

                    case nameof(beaufort):
                        this.beaufort = new Beaufort();
                        this.beaufort.FromJson(reader);
                        break;

                    case nameof(uv):
                        this.uv = new UV();
                        this.uv.FromJson(reader);
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

                // "weather" : ""
                writer.WritePropertyName(nameof(weather));
                writer.WriteStringValue(weather);

                // "temp_f" : ""
                writer.WritePropertyName(nameof(temp_f));
                writer.WriteNumberValue(temp_f);

                // "temp_c" : ""
                writer.WritePropertyName(nameof(temp_c));
                writer.WriteNumberValue(temp_c);

                // "wind_degrees" : ""
                writer.WritePropertyName(nameof(wind_degrees));
                writer.WriteNumberValue(wind_degrees);

                // "wind_mph" : ""
                writer.WritePropertyName(nameof(wind_mph));
                writer.WriteNumberValue(wind_mph);

                // "wind_kph" : ""
                writer.WritePropertyName(nameof(wind_kph));
                writer.WriteNumberValue(wind_kph);

                // "feelslike_f" : ""
                writer.WritePropertyName(nameof(feelslike_f));
                writer.WriteNumberValue(feelslike_f);

                // "feelslike_c" : ""
                writer.WritePropertyName(nameof(feelslike_c));
                writer.WriteNumberValue(feelslike_c);

                // "icon" : ""
                writer.WritePropertyName(nameof(icon));
                writer.WriteStringValue(icon);

                // "beaufort" : ""
                if (beaufort != null)
                {
                    writer.WritePropertyName(nameof(beaufort));
                    writer.WriteStringValue(beaufort?.ToJson());
                }

                // "uv" : ""
                if (uv != null)
                {
                    writer.WritePropertyName(nameof(uv));
                    writer.WriteStringValue(uv?.ToJson());
                }

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<Atmosphere>))]
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
                    case nameof(humidity):
                        this.humidity = reader.GetString();
                        break;

                    case nameof(pressure_mb):
                        this.pressure_mb = reader.GetString();
                        break;

                    case nameof(pressure_in):
                        this.pressure_in = reader.GetString();
                        break;

                    case nameof(pressure_trend):
                        this.pressure_trend = reader.GetString();
                        break;

                    case nameof(visibility_mi):
                        this.visibility_mi = reader.GetString();
                        break;

                    case nameof(visibility_km):
                        this.visibility_km = reader.GetString();
                        break;

                    case nameof(dewpoint_f):
                        this.dewpoint_f = reader.GetString();
                        break;

                    case nameof(dewpoint_c):
                        this.dewpoint_c = reader.GetString();
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

                // "humidity" : ""
                writer.WritePropertyName(nameof(humidity));
                writer.WriteStringValue(humidity);

                // "pressure_mb" : ""
                writer.WritePropertyName(nameof(pressure_mb));
                writer.WriteStringValue(pressure_mb);

                // "pressure_in" : ""
                writer.WritePropertyName(nameof(pressure_in));
                writer.WriteStringValue(pressure_in);

                // "pressure_trend" : ""
                writer.WritePropertyName(nameof(pressure_trend));
                writer.WriteStringValue(pressure_trend);

                // "visibility_mi" : ""
                writer.WritePropertyName(nameof(visibility_mi));
                writer.WriteStringValue(visibility_mi);

                // "visibility_km" : ""
                writer.WritePropertyName(nameof(visibility_km));
                writer.WriteStringValue(visibility_km);

                // "dewpoint_f" : ""
                writer.WritePropertyName(nameof(dewpoint_f));
                writer.WriteStringValue(dewpoint_f);

                // "dewpoint_c" : ""
                writer.WritePropertyName(nameof(dewpoint_c));
                writer.WriteStringValue(dewpoint_c);

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<Astronomy>))]
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
                    case nameof(sunrise):
                        this.sunrise = DateTime.Parse(reader.GetString());
                        break;

                    case nameof(sunset):
                        this.sunset = DateTime.Parse(reader.GetString());
                        break;

                    case nameof(moonrise):
                        this.moonrise = DateTime.Parse(reader.GetString());
                        break;

                    case nameof(moonset):
                        this.moonset = DateTime.Parse(reader.GetString());
                        break;

                    case nameof(moonphase):
                        this.moonphase = new MoonPhase();
                        this.moonphase.FromJson(reader);
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

                // "sunrise" : ""
                writer.WritePropertyName(nameof(sunrise));
                writer.WriteStringValue(sunrise);

                // "sunset" : ""
                writer.WritePropertyName(nameof(sunset));
                writer.WriteStringValue(sunset);

                // "moonrise" : ""
                writer.WritePropertyName(nameof(moonrise));
                writer.WriteStringValue(moonrise);

                // "moonset" : ""
                writer.WritePropertyName(nameof(moonset));
                writer.WriteStringValue(moonset);

                // "moonphase" : ""
                if (moonphase != null)
                {
                    writer.WritePropertyName(nameof(moonphase));
                    writer.WriteStringValue(moonphase?.ToJson());
                }

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<Precipitation>))]
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
                    case nameof(pop):
                        this.pop = reader.GetString();
                        break;

                    case nameof(qpf_rain_in):
                        this.qpf_rain_in = reader.GetSingle();
                        break;

                    case nameof(qpf_rain_mm):
                        this.qpf_rain_mm = reader.GetSingle();
                        break;

                    case nameof(qpf_snow_in):
                        this.qpf_snow_in = reader.GetSingle();
                        break;

                    case nameof(qpf_snow_cm):
                        this.qpf_snow_cm = reader.GetSingle();
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

                // "pop" : ""
                writer.WritePropertyName(nameof(pop));
                writer.WriteStringValue(pop);

                // "qpf_rain_in" : ""
                writer.WritePropertyName(nameof(qpf_rain_in));
                writer.WriteNumberValue(qpf_rain_in);

                // "qpf_rain_mm" : ""
                writer.WritePropertyName(nameof(qpf_rain_mm));
                writer.WriteNumberValue(qpf_rain_mm);

                // "qpf_snow_in" : ""
                writer.WritePropertyName(nameof(qpf_snow_in));
                writer.WriteNumberValue(qpf_snow_in);

                // "qpf_snow_cm" : ""
                writer.WritePropertyName(nameof(qpf_snow_cm));
                writer.WriteNumberValue(qpf_snow_cm);

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<Beaufort>))]
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
                    case nameof(scale):
                        this.scale = (BeaufortScale)reader.GetInt32();
                        break;

                    case nameof(desc):
                        this.desc = reader.GetString();
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

                // "scale" : ""
                writer.WritePropertyName(nameof(scale));
                writer.WriteNumberValue((int)scale);

                // "desc" : ""
                writer.WritePropertyName(nameof(desc));
                writer.WriteStringValue(desc);

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<MoonPhase>))]
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
                    case nameof(phase):
                        this.phase = (MoonPhaseType)reader.GetInt32();
                        break;

                    case nameof(desc):
                        this.desc = reader.GetString();
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

                // "phase" : ""
                writer.WritePropertyName(nameof(phase));
                writer.WriteNumberValue((int)phase);

                // "desc" : ""
                writer.WritePropertyName(nameof(desc));
                writer.WriteStringValue(desc);

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<UV>))]
    public partial class UV : CustomJsonObject
    {
        public float index { get; set; } = -1;
        public string desc { get; set; }

        internal UV()
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
                    case nameof(index):
                        this.index = reader.GetSingle();
                        break;

                    case nameof(desc):
                        this.desc = reader.GetString();
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

                // "scale" : ""
                writer.WritePropertyName(nameof(index));
                writer.WriteNumberValue(index);

                // "desc" : ""
                writer.WritePropertyName(nameof(desc));
                writer.WriteStringValue(desc);

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
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

        [JsonIgnore]
        [NotMapped]
        // Doesn't store this in db
        // For DateTimeOffset, offset isn't stored when saving to db
        // Store as string (blob) instead
        // If db previously stored DateTimeOffset (as ticks) retrieve and set offset
        public DateTimeOffset date
        {
            get
            {
                if (DateTimeOffset.TryParseExact(dateblob, "dd.MM.yyyy HH:mm:ss zzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result))
                    return result;
                else
                    return DateTimeOffset.Parse(dateblob);
            }
            set { dateblob = value.ToString("dd.MM.yyyy HH:mm:ss zzzz"); }
        }

        [Column(TypeName = "varchar")]
        public HourlyForecast hr_forecast { get; set; }

        [Key]
        [JsonPropertyName("date")]
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