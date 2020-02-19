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
    [JsonConverter(typeof(CustomJsonConverter))]
    [Table("weatherdata")]
    public partial class Weather
    {
        [JsonIgnore]
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

        [JsonConstructor]
        public Weather()
        {
            // Needed for deserialization
        }

        public static Weather FromJson(JsonReader reader)
        {
            Weather obj = null;

            try
            {
                obj = new Weather();

                while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                {
                    if (reader.TokenType == JsonToken.StartObject)
                        reader.Read(); // StartObject

                    string property = reader.Value?.ToString();
                    reader.Read(); // prop value

                    switch (property)
                    {
                        case nameof(location):
                            obj.location = Location.FromJson(reader);
                            break;

                        case nameof(update_time):
                            bool parsed = DateTimeOffset.TryParseExact(reader.Value?.ToString(), "dd.MM.yyyy HH:mm:ss zzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result);
                            if (!parsed) // If we can't parse as DateTimeOffset try DateTime (data could be old)
                                result = DateTime.Parse(reader.Value?.ToString());
                            else
                            {
                                // DateTimeOffset date stored in SQLite.NET doesn't store offset
                                // Try to convert to location's timezone if possible or if time is in UTC
                                if (obj.location?.tz_offset != null && result.Offset.Ticks == 0)
                                    result = result.ToOffset(obj.location.tz_offset);
                            }
                            obj.update_time = result;
                            break;

                        case nameof(forecast):
                            // Set initial cap to 10
                            // Most provider forecasts are <= 10
                            var forecasts = new List<Forecast>(10);
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                if (reader.TokenType == JsonToken.String)
                                    forecasts.Add(Forecast.FromJson(reader));
                            }
                            obj.forecast = forecasts;
                            break;

                        case nameof(hr_forecast):
                            // Set initial cap to 90
                            // MetNo contains ~90 items, but HERE contains ~165
                            // If 90+ is needed, let the List impl allocate more
                            var hr_forecasts = new List<HourlyForecast>(90);
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                if (reader.TokenType == JsonToken.String)
                                    hr_forecasts.Add(HourlyForecast.FromJson(reader));
                            }
                            obj.hr_forecast = hr_forecasts;
                            break;

                        case nameof(txt_forecast):
                            // Set initial cap to 20
                            // Most provider forecasts are <= 10 (x2 for day & nt)
                            var txt_forecasts = new List<TextForecast>(20);
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                if (reader.TokenType == JsonToken.String)
                                    txt_forecasts.Add(TextForecast.FromJson(reader));
                            }
                            obj.txt_forecast = txt_forecasts;
                            break;

                        case nameof(condition):
                            obj.condition = Condition.FromJson(reader);
                            break;

                        case nameof(atmosphere):
                            obj.atmosphere = Atmosphere.FromJson(reader);
                            break;

                        case nameof(astronomy):
                            obj.astronomy = Astronomy.FromJson(reader);
                            break;

                        case nameof(precipitation):
                            obj.precipitation = Precipitation.FromJson(reader);
                            break;

                        case nameof(ttl):
                            obj.ttl = reader.Value?.ToString();
                            break;

                        case nameof(source):
                            obj.source = reader.Value?.ToString();
                            break;

                        case nameof(query):
                            obj.query = reader.Value?.ToString();
                            break;

                        case nameof(locale):
                            obj.locale = reader.Value?.ToString();
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

            return obj;
        }

        public string ToJson()
        {
            using (var sw = new System.IO.StringWriter())
            using (var writer = new JsonTextWriter(sw))
            {
                // {
                writer.WriteStartObject();

                // "location" : ""
                writer.WritePropertyName(nameof(location));
                writer.WriteValue(location?.ToJson());

                // "update_time" : ""
                writer.WritePropertyName(nameof(update_time));
                writer.WriteValue(update_time.ToString("dd.MM.yyyy HH:mm:ss zzzz"));

                // "forecast" : ""
                if (forecast != null)
                {
                    writer.WritePropertyName(nameof(forecast));
                    writer.WriteStartArray();
                    foreach (Forecast cast in forecast)
                    {
                        writer.WriteValue(cast?.ToJson());
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
                        writer.WriteValue(hr_cast?.ToJson());
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
                        writer.WriteValue(txt_cast?.ToJson());
                    }
                    writer.WriteEndArray();
                }

                // "condition" : ""
                writer.WritePropertyName(nameof(condition));
                writer.WriteValue(condition?.ToJson());

                // "atmosphere" : ""
                writer.WritePropertyName(nameof(atmosphere));
                writer.WriteValue(atmosphere?.ToJson());

                // "astronomy" : ""
                writer.WritePropertyName(nameof(astronomy));
                writer.WriteValue(astronomy?.ToJson());

                // "precipitation" : ""
                if (precipitation != null)
                {
                    writer.WritePropertyName(nameof(precipitation));
                    writer.WriteValue(precipitation?.ToJson());
                }

                // "ttl" : ""
                writer.WritePropertyName(nameof(ttl));
                writer.WriteValue(ttl);

                // "source" : ""
                writer.WritePropertyName(nameof(source));
                writer.WriteValue(source);

                // "query" : ""
                writer.WritePropertyName(nameof(query));
                writer.WriteValue(query);

                // "locale" : ""
                writer.WritePropertyName(nameof(locale));
                writer.WriteValue(locale);

                // }
                writer.WriteEndObject();

                return sw.ToString();
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

    [JsonConverter(typeof(CustomJsonConverter))]
    public partial class Location
    {
        public string name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public TimeSpan tz_offset { get; set; }
        public string tz_short { get; set; }
        public string tz_long { get; set; }

        [JsonConstructor]
        private Location()
        {
            // Needed for deserialization
        }

        public static Location FromJson(JsonReader extReader)
        {
            Location obj = null;
            bool disposeReader = false;
            JsonReader reader = null;

            try
            {
                obj = new Location();
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
                        case nameof(name):
                            obj.name = reader.Value?.ToString();
                            break;

                        case nameof(latitude):
                            obj.latitude = reader.Value?.ToString();
                            break;

                        case nameof(longitude):
                            obj.longitude = reader.Value?.ToString();
                            break;

                        case nameof(tz_offset):
                            obj.tz_offset = TimeSpan.Parse(reader.Value?.ToString());
                            break;

                        case nameof(tz_short):
                            obj.tz_short = reader.Value?.ToString();
                            break;

                        case nameof(tz_long):
                            obj.tz_long = reader.Value?.ToString();
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

                // "name" : ""
                writer.WritePropertyName(nameof(name));
                writer.WriteValue(name);

                // "latitude" : ""
                writer.WritePropertyName(nameof(latitude));
                writer.WriteValue(latitude);

                // "longitude" : ""
                writer.WritePropertyName(nameof(longitude));
                writer.WriteValue(longitude);

                // "tz_offset" : ""
                writer.WritePropertyName(nameof(tz_offset));
                writer.WriteValue(tz_offset);

                // "tz_short" : ""
                writer.WritePropertyName(nameof(tz_short));
                writer.WriteValue(tz_short);

                // "tz_long" : ""
                writer.WritePropertyName(nameof(tz_long));
                writer.WriteValue(tz_long);

                // }
                writer.WriteEndObject();

                return sw.ToString();
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public partial class Forecast
    {
        public DateTime date { get; set; }
        public string high_f { get; set; }
        public string high_c { get; set; }
        public string low_f { get; set; }
        public string low_c { get; set; }
        public string condition { get; set; }
        public string icon { get; set; }
        public ForecastExtras extras { get; set; }

        [JsonConstructor]
        private Forecast()
        {
            // Needed for deserialization
        }

        public static Forecast FromJson(JsonReader extReader)
        {
            Forecast obj = null;
            bool disposeReader = false;
            JsonReader reader = null;

            try
            {
                obj = new Forecast();
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
                        case nameof(date):
                            obj.date = DateTime.Parse(reader.Value?.ToString());
                            break;

                        case nameof(high_f):
                            obj.high_f = reader.Value?.ToString();
                            break;

                        case nameof(high_c):
                            obj.high_c = reader.Value?.ToString();
                            break;

                        case nameof(low_f):
                            obj.low_f = reader.Value?.ToString();
                            break;

                        case nameof(low_c):
                            obj.low_c = reader.Value?.ToString();
                            break;

                        case nameof(condition):
                            obj.condition = reader.Value?.ToString();
                            break;

                        case nameof(icon):
                            obj.icon = reader.Value?.ToString();
                            break;

                        case nameof(extras):
                            obj.extras = ForecastExtras.FromJson(reader);
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

                // "date" : ""
                writer.WritePropertyName(nameof(date));
                writer.WriteValue(date);

                // "high_f" : ""
                writer.WritePropertyName(nameof(high_f));
                writer.WriteValue(high_f);

                // "high_c" : ""
                writer.WritePropertyName(nameof(high_c));
                writer.WriteValue(high_c);

                // "low_f" : ""
                writer.WritePropertyName(nameof(low_f));
                writer.WriteValue(low_f);

                // "low_c" : ""
                writer.WritePropertyName(nameof(low_c));
                writer.WriteValue(low_c);

                // "condition" : ""
                writer.WritePropertyName(nameof(condition));
                writer.WriteValue(condition);

                // "icon" : ""
                writer.WritePropertyName(nameof(icon));
                writer.WriteValue(icon);

                // "extras" : ""
                if (extras != null)
                {
                    writer.WritePropertyName(nameof(extras));
                    writer.WriteValue(extras?.ToJson());
                }

                // }
                writer.WriteEndObject();

                return sw.ToString();
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public partial class HourlyForecast
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

        [JsonProperty(PropertyName = nameof(date))]
        private string _date { get; set; }

        [JsonConstructor]
        private HourlyForecast()
        {
            // Needed for deserialization
        }

        public static HourlyForecast FromJson(JsonReader extReader)
        {
            HourlyForecast obj = null;
            bool disposeReader = false;
            JsonReader reader = null;

            try
            {
                obj = new HourlyForecast();
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
                        case nameof(date):
                            obj._date = reader.Value?.ToString();
                            break;

                        case nameof(high_f):
                            obj.high_f = reader.Value?.ToString();
                            break;

                        case nameof(high_c):
                            obj.high_c = reader.Value?.ToString();
                            break;

                        case nameof(condition):
                            obj.condition = reader.Value?.ToString();
                            break;

                        case nameof(icon):
                            obj.icon = reader.Value?.ToString();
                            break;

                        case nameof(pop):
                            obj.pop = reader.Value?.ToString();
                            break;

                        case nameof(wind_degrees):
                            obj.wind_degrees = int.Parse(reader.Value?.ToString());
                            break;

                        case nameof(wind_mph):
                            obj.wind_mph = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(wind_kph):
                            obj.wind_kph = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(extras):
                            obj.extras = ForecastExtras.FromJson(reader);
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

                // "date" : ""
                writer.WritePropertyName(nameof(date));
                writer.WriteValue(_date);

                // "high_f" : ""
                writer.WritePropertyName(nameof(high_f));
                writer.WriteValue(high_f);

                // "high_c" : ""
                writer.WritePropertyName(nameof(high_c));
                writer.WriteValue(high_c);

                // "condition" : ""
                writer.WritePropertyName(nameof(condition));
                writer.WriteValue(condition);

                // "icon" : ""
                writer.WritePropertyName(nameof(icon));
                writer.WriteValue(icon);

                // "pop" : ""
                writer.WritePropertyName(nameof(pop));
                writer.WriteValue(pop);

                // "wind_degrees" : ""
                writer.WritePropertyName(nameof(wind_degrees));
                writer.WriteValue(wind_degrees);

                // "wind_mph" : ""
                writer.WritePropertyName(nameof(wind_mph));
                writer.WriteValue(wind_mph);

                // "wind_kph" : ""
                writer.WritePropertyName(nameof(wind_kph));
                writer.WriteValue(wind_kph);

                // "extras" : ""
                if (extras != null)
                {
                    writer.WritePropertyName(nameof(extras));
                    writer.WriteValue(extras?.ToJson());
                }

                // }
                writer.WriteEndObject();

                return sw.ToString();
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public partial class TextForecast
    {
        public string title { get; set; }
        public string fcttext { get; set; }
        public string fcttext_metric { get; set; }
        public string icon { get; set; }
        public string pop { get; set; }

        [JsonConstructor]
        private TextForecast()
        {
            // Needed for deserialization
        }

        public static TextForecast FromJson(JsonReader extReader)
        {
            TextForecast obj = null;
            bool disposeReader = false;
            JsonReader reader = null;

            try
            {
                obj = new TextForecast();
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
                        case nameof(title):
                            obj.title = reader.Value?.ToString();
                            break;

                        case nameof(fcttext):
                            obj.fcttext = reader.Value?.ToString();
                            break;

                        case nameof(fcttext_metric):
                            obj.fcttext_metric = reader.Value?.ToString();
                            break;

                        case nameof(icon):
                            obj.icon = reader.Value?.ToString();
                            break;

                        case nameof(pop):
                            obj.pop = reader.Value?.ToString();
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

                // "title" : ""
                writer.WritePropertyName(nameof(title));
                writer.WriteValue(title);

                // "fcttext" : ""
                writer.WritePropertyName(nameof(fcttext));
                writer.WriteValue(fcttext);

                // "fcttext_metric" : ""
                writer.WritePropertyName(nameof(fcttext_metric));
                writer.WriteValue(fcttext_metric);

                // "icon" : ""
                writer.WritePropertyName(nameof(icon));
                writer.WriteValue(icon);

                // "pop" : ""
                writer.WritePropertyName(nameof(pop));
                writer.WriteValue(pop);

                // }
                writer.WriteEndObject();

                return sw.ToString();
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public class ForecastExtras
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

        [JsonConstructor]
        internal ForecastExtras()
        {
            // Needed for deserialization
        }

        public static ForecastExtras FromJson(JsonReader extReader)
        {
            ForecastExtras obj = null;
            bool disposeReader = false;
            JsonReader reader = null;

            try
            {
                obj = new ForecastExtras();
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
                        case nameof(feelslike_f):
                            obj.feelslike_f = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(feelslike_c):
                            obj.feelslike_c = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(humidity):
                            obj.humidity = reader.Value?.ToString();
                            break;

                        case nameof(dewpoint_f):
                            obj.dewpoint_f = reader.Value?.ToString();
                            break;

                        case nameof(dewpoint_c):
                            obj.dewpoint_c = reader.Value?.ToString();
                            break;

                        case nameof(uv_index):
                            obj.uv_index = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(pop):
                            obj.pop = reader.Value?.ToString();
                            break;

                        case nameof(qpf_rain_in):
                            obj.qpf_rain_in = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(qpf_rain_mm):
                            obj.qpf_rain_mm = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(qpf_snow_in):
                            obj.qpf_snow_in = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(qpf_snow_cm):
                            obj.qpf_snow_cm = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(pressure_mb):
                            obj.pressure_mb = reader.Value?.ToString();
                            break;

                        case nameof(pressure_in):
                            obj.pressure_in = reader.Value?.ToString();
                            break;

                        case nameof(wind_degrees):
                            obj.wind_degrees = int.Parse(reader.Value?.ToString());
                            break;

                        case nameof(wind_mph):
                            obj.wind_mph = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(wind_kph):
                            obj.wind_kph = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(visibility_mi):
                            obj.visibility_mi = reader.Value?.ToString();
                            break;

                        case nameof(visibility_km):
                            obj.visibility_km = reader.Value?.ToString();
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

                // "feelslike_f" : ""
                writer.WritePropertyName(nameof(feelslike_f));
                writer.WriteValue(feelslike_f);

                // "feelslike_c" : ""
                writer.WritePropertyName(nameof(feelslike_c));
                writer.WriteValue(feelslike_c);

                // "humidity" : ""
                writer.WritePropertyName(nameof(humidity));
                writer.WriteValue(humidity);

                // "dewpoint_f" : ""
                writer.WritePropertyName(nameof(dewpoint_f));
                writer.WriteValue(dewpoint_f);

                // "dewpoint_c" : ""
                writer.WritePropertyName(nameof(dewpoint_c));
                writer.WriteValue(dewpoint_c);

                // "uv_index" : ""
                writer.WritePropertyName(nameof(uv_index));
                writer.WriteValue(uv_index);

                // "pop" : ""
                writer.WritePropertyName(nameof(pop));
                writer.WriteValue(pop);

                // "qpf_rain_in" : ""
                writer.WritePropertyName(nameof(qpf_rain_in));
                writer.WriteValue(qpf_rain_in);

                // "qpf_rain_mm" : ""
                writer.WritePropertyName(nameof(qpf_rain_mm));
                writer.WriteValue(qpf_rain_mm);

                // "qpf_snow_in" : ""
                writer.WritePropertyName(nameof(qpf_snow_in));
                writer.WriteValue(qpf_snow_in);

                // "qpf_snow_cm" : ""
                writer.WritePropertyName(nameof(qpf_snow_cm));
                writer.WriteValue(qpf_snow_cm);

                // "pressure_mb" : ""
                writer.WritePropertyName(nameof(pressure_mb));
                writer.WriteValue(pressure_mb);

                // "pressure_in" : ""
                writer.WritePropertyName(nameof(pressure_in));
                writer.WriteValue(pressure_in);

                // "wind_degrees" : ""
                writer.WritePropertyName(nameof(wind_degrees));
                writer.WriteValue(wind_degrees);

                // "wind_mph" : ""
                writer.WritePropertyName(nameof(wind_mph));
                writer.WriteValue(wind_mph);

                // "wind_kph" : ""
                writer.WritePropertyName(nameof(wind_kph));
                writer.WriteValue(wind_kph);

                // "visibility_mi" : ""
                writer.WritePropertyName(nameof(visibility_mi));
                writer.WriteValue(visibility_mi);

                // "visibility_km" : ""
                writer.WritePropertyName(nameof(visibility_km));
                writer.WriteValue(visibility_km);

                // }
                writer.WriteEndObject();

                return sw.ToString();
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public partial class Condition
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

        [JsonConstructor]
        private Condition()
        {
            // Needed for deserialization
        }

        public static Condition FromJson(JsonReader extReader)
        {
            Condition obj = null;
            bool disposeReader = false;
            JsonReader reader = null;

            try
            {
                obj = new Condition();
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
                        case nameof(weather):
                            obj.weather = reader.Value?.ToString();
                            break;

                        case nameof(temp_f):
                            obj.temp_f = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(temp_c):
                            obj.temp_c = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(wind_degrees):
                            obj.wind_degrees = int.Parse(reader.Value?.ToString());
                            break;

                        case nameof(wind_mph):
                            obj.wind_mph = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(wind_kph):
                            obj.wind_kph = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(feelslike_f):
                            obj.feelslike_f = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(feelslike_c):
                            obj.feelslike_c = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(icon):
                            obj.icon = reader.Value?.ToString();
                            break;

                        case nameof(beaufort):
                            obj.beaufort = Beaufort.FromJson(reader);
                            break;

                        case nameof(uv):
                            obj.uv = UV.FromJson(reader);
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

                // "weather" : ""
                writer.WritePropertyName(nameof(weather));
                writer.WriteValue(weather);

                // "temp_f" : ""
                writer.WritePropertyName(nameof(temp_f));
                writer.WriteValue(temp_f);

                // "temp_c" : ""
                writer.WritePropertyName(nameof(temp_c));
                writer.WriteValue(temp_c);

                // "wind_degrees" : ""
                writer.WritePropertyName(nameof(wind_degrees));
                writer.WriteValue(wind_degrees);

                // "wind_mph" : ""
                writer.WritePropertyName(nameof(wind_mph));
                writer.WriteValue(wind_mph);

                // "wind_kph" : ""
                writer.WritePropertyName(nameof(wind_kph));
                writer.WriteValue(wind_kph);

                // "feelslike_f" : ""
                writer.WritePropertyName(nameof(feelslike_f));
                writer.WriteValue(feelslike_f);

                // "feelslike_c" : ""
                writer.WritePropertyName(nameof(feelslike_c));
                writer.WriteValue(feelslike_c);

                // "icon" : ""
                writer.WritePropertyName(nameof(icon));
                writer.WriteValue(icon);

                // "beaufort" : ""
                if (beaufort != null)
                {
                    writer.WritePropertyName(nameof(beaufort));
                    writer.WriteValue(beaufort?.ToJson());
                }

                // "uv" : ""
                if (uv != null)
                {
                    writer.WritePropertyName(nameof(uv));
                    writer.WriteValue(uv?.ToJson());
                }

                // }
                writer.WriteEndObject();

                return sw.ToString();
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public partial class Atmosphere
    {
        public string humidity { get; set; }
        public string pressure_mb { get; set; }
        public string pressure_in { get; set; }
        public string pressure_trend { get; set; }
        public string visibility_mi { get; set; }
        public string visibility_km { get; set; }
        public string dewpoint_f { get; set; }
        public string dewpoint_c { get; set; }

        [JsonConstructor]
        private Atmosphere()
        {
            // Needed for deserialization
        }

        public static Atmosphere FromJson(JsonReader extReader)
        {
            Atmosphere obj = null;
            bool disposeReader = false;
            JsonReader reader = null;

            try
            {
                obj = new Atmosphere();
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
                        case nameof(humidity):
                            obj.humidity = reader.Value?.ToString();
                            break;

                        case nameof(pressure_mb):
                            obj.pressure_mb = reader.Value?.ToString();
                            break;

                        case nameof(pressure_in):
                            obj.pressure_in = reader.Value?.ToString();
                            break;

                        case nameof(pressure_trend):
                            obj.pressure_trend = reader.Value?.ToString();
                            break;

                        case nameof(visibility_mi):
                            obj.visibility_mi = reader.Value?.ToString();
                            break;

                        case nameof(visibility_km):
                            obj.visibility_km = reader.Value?.ToString();
                            break;

                        case nameof(dewpoint_f):
                            obj.dewpoint_f = reader.Value?.ToString();
                            break;

                        case nameof(dewpoint_c):
                            obj.dewpoint_c = reader.Value?.ToString();
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

                // "humidity" : ""
                writer.WritePropertyName(nameof(humidity));
                writer.WriteValue(humidity);

                // "pressure_mb" : ""
                writer.WritePropertyName(nameof(pressure_mb));
                writer.WriteValue(pressure_mb);

                // "pressure_in" : ""
                writer.WritePropertyName(nameof(pressure_in));
                writer.WriteValue(pressure_in);

                // "pressure_trend" : ""
                writer.WritePropertyName(nameof(pressure_trend));
                writer.WriteValue(pressure_trend);

                // "visibility_mi" : ""
                writer.WritePropertyName(nameof(visibility_mi));
                writer.WriteValue(visibility_mi);

                // "visibility_km" : ""
                writer.WritePropertyName(nameof(visibility_km));
                writer.WriteValue(visibility_km);

                // "dewpoint_f" : ""
                writer.WritePropertyName(nameof(dewpoint_f));
                writer.WriteValue(dewpoint_f);

                // "dewpoint_c" : ""
                writer.WritePropertyName(nameof(dewpoint_c));
                writer.WriteValue(dewpoint_c);

                // }
                writer.WriteEndObject();

                return sw.ToString();
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public partial class Astronomy
    {
        public DateTime sunrise { get; set; }
        public DateTime sunset { get; set; }
        public DateTime moonrise { get; set; }
        public DateTime moonset { get; set; }
        public MoonPhase moonphase { get; set; }

        [JsonConstructor]
        private Astronomy()
        {
            // Needed for deserialization
        }

        public static Astronomy FromJson(JsonReader extReader)
        {
            Astronomy obj = null;
            bool disposeReader = false;
            JsonReader reader = null;

            try
            {
                obj = new Astronomy();
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
                        case nameof(sunrise):
                            obj.sunrise = DateTime.Parse(reader.Value?.ToString());
                            break;

                        case nameof(sunset):
                            obj.sunset = DateTime.Parse(reader.Value?.ToString());
                            break;

                        case nameof(moonrise):
                            obj.moonrise = DateTime.Parse(reader.Value?.ToString());
                            break;

                        case nameof(moonset):
                            obj.moonset = DateTime.Parse(reader.Value?.ToString());
                            break;

                        case nameof(moonphase):
                            obj.moonphase = MoonPhase.FromJson(reader);
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

                // "sunrise" : ""
                writer.WritePropertyName(nameof(sunrise));
                writer.WriteValue(sunrise);

                // "sunset" : ""
                writer.WritePropertyName(nameof(sunset));
                writer.WriteValue(sunset);

                // "moonrise" : ""
                writer.WritePropertyName(nameof(moonrise));
                writer.WriteValue(moonrise);

                // "moonset" : ""
                writer.WritePropertyName(nameof(moonset));
                writer.WriteValue(moonset);

                // "moonphase" : ""
                if (moonphase != null)
                {
                    writer.WritePropertyName(nameof(moonphase));
                    writer.WriteValue(moonphase?.ToJson());
                }

                // }
                writer.WriteEndObject();

                return sw.ToString();
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public partial class Precipitation
    {
        public string pop { get; set; }
        public float qpf_rain_in { get; set; }
        public float qpf_rain_mm { get; set; }
        public float qpf_snow_in { get; set; }
        public float qpf_snow_cm { get; set; }

        [JsonConstructor]
        private Precipitation()
        {
            // Needed for deserialization
        }

        public static Precipitation FromJson(JsonReader extReader)
        {
            Precipitation obj = null;
            bool disposeReader = false;
            JsonReader reader = null;

            try
            {
                obj = new Precipitation();
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
                        case nameof(pop):
                            obj.pop = reader.Value?.ToString();
                            break;

                        case nameof(qpf_rain_in):
                            obj.qpf_rain_in = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(qpf_rain_mm):
                            obj.qpf_rain_mm = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(qpf_snow_in):
                            obj.qpf_snow_in = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(qpf_snow_cm):
                            obj.qpf_snow_cm = float.Parse(reader.Value?.ToString());
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

                // "pop" : ""
                writer.WritePropertyName(nameof(pop));
                writer.WriteValue(pop);

                // "qpf_rain_in" : ""
                writer.WritePropertyName(nameof(qpf_rain_in));
                writer.WriteValue(qpf_rain_in);

                // "qpf_rain_mm" : ""
                writer.WritePropertyName(nameof(qpf_rain_mm));
                writer.WriteValue(qpf_rain_mm);

                // "qpf_snow_in" : ""
                writer.WritePropertyName(nameof(qpf_snow_in));
                writer.WriteValue(qpf_snow_in);

                // "qpf_snow_cm" : ""
                writer.WritePropertyName(nameof(qpf_snow_cm));
                writer.WriteValue(qpf_snow_cm);

                // }
                writer.WriteEndObject();

                return sw.ToString();
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public partial class Beaufort
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

        [JsonConstructor]
        private Beaufort()
        {
            // Needed for deserialization
        }

        public static Beaufort FromJson(JsonReader extReader)
        {
            Beaufort obj = null;
            bool disposeReader = false;
            JsonReader reader = null;

            try
            {
                obj = new Beaufort();
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
                        case nameof(scale):
                            obj.scale = (BeaufortScale)int.Parse(reader.Value?.ToString());
                            break;

                        case nameof(desc):
                            obj.desc = reader.Value?.ToString();
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

                // "scale" : ""
                writer.WritePropertyName(nameof(scale));
                writer.WriteValue((int)scale);

                // "desc" : ""
                writer.WritePropertyName(nameof(desc));
                writer.WriteValue(desc);

                // }
                writer.WriteEndObject();

                return sw.ToString();
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public partial class MoonPhase
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

        [JsonConstructor]
        private MoonPhase()
        {
            // Needed for deserialization
        }

        public static MoonPhase FromJson(JsonReader extReader)
        {
            MoonPhase obj = null;
            bool disposeReader = false;
            JsonReader reader = null;

            try
            {
                obj = new MoonPhase();
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
                        case nameof(phase):
                            obj.phase = (MoonPhaseType)int.Parse(reader.Value?.ToString());
                            break;

                        case nameof(desc):
                            obj.desc = reader.Value?.ToString();
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

                // "phase" : ""
                writer.WritePropertyName(nameof(phase));
                writer.WriteValue((int)phase);

                // "desc" : ""
                writer.WritePropertyName(nameof(desc));
                writer.WriteValue(desc);

                // }
                writer.WriteEndObject();

                return sw.ToString();
            }
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public partial class UV
    {
        public float index { get; set; } = -1;
        public string desc { get; set; }

        [JsonConstructor]
        private UV()
        {
            // Needed for deserialization
        }

        public static UV FromJson(JsonReader extReader)
        {
            UV obj = null;
            bool disposeReader = false;
            JsonReader reader = null;

            try
            {
                obj = new UV();
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
                        case nameof(index):
                            obj.index = float.Parse(reader.Value?.ToString());
                            break;

                        case nameof(desc):
                            obj.desc = reader.Value?.ToString();
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

                // "scale" : ""
                writer.WritePropertyName(nameof(index));
                writer.WriteValue(index);

                // "desc" : ""
                writer.WritePropertyName(nameof(desc));
                writer.WriteValue(desc);

                // }
                writer.WriteEndObject();

                return sw.ToString();
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
        [JsonProperty(PropertyName = "date")]
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