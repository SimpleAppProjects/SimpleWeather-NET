using NodaTime;
using SimpleWeather.Json;
using SimpleWeather.Utils;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        [JsonIgnore]
        public const string NA = "N/A";

        [TextBlob(nameof(locationblob))]
        public Location location { get; set; }

        [Ignore]
        [JsonIgnore]
        [JsonConverter(typeof(DateTimeOffsetFormatter))]
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
                    return new DateTimeOffset(long.Parse(updatetimeblob, CultureInfo.InvariantCulture), TimeSpan.Zero).ToOffset(location.tz_offset);
            }
            set { updatetimeblob = value.ToDateTimeOffsetFormat(); }
        }

        [Ignore]
        public IList<Forecast> forecast { get; set; }

        [Ignore]
        public IList<HourlyForecast> hr_forecast { get; set; }

        [Ignore]
        public IList<TextForecast> txt_forecast { get; set; }

        [Ignore]
        public IList<MinutelyForecast> min_forecast { get; set; }
        [Ignore]
        public IList<AirQuality> aqi_forecast { get; set; }

        [TextBlob(nameof(conditionblob))]
        public Condition condition { get; set; }

        [TextBlob(nameof(atmosphereblob))]
        public Atmosphere atmosphere { get; set; }

        [TextBlob(nameof(astronomyblob))]
        public Astronomy astronomy { get; set; }

        [TextBlob(nameof(precipitationblob))]
        public Precipitation precipitation { get; set; }

        [Ignore]
        // Just for passing along to where its needed
        public ICollection<WeatherAlert> weather_alerts { get; set; }

        public int ttl { get; set; }
        public string source { get; set; }

        [PrimaryKey]
        public string query { get; set; }

        public string locale { get; set; }

        [JsonIgnore]
        public string locationblob { get; set; }

        [Column("update_time")]
        [JsonPropertyName("update_time")]
        // Keep DateTimeOffset column name to get data as string
        public string updatetimeblob { get; set; }

        [JsonIgnore]
        public string conditionblob { get; set; }

        [JsonIgnore]
        public string atmosphereblob { get; set; }

        [JsonIgnore]
        public string astronomyblob { get; set; }

        [JsonIgnore]
        public string precipitationblob { get; set; }

        public Weather()
        {
            // Needed for deserialization
        }

        public override void FromJson(ref Utf8JsonReader reader)
        {
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                    reader.Read(); // StartObject

                string property = reader.GetString(); // JsonTokenType.PropertyName

                reader.Read(); // Property Value

                switch (property)
                {
                    case nameof(location):
                        this.location = new Location();
                        this.location.FromJson(ref reader);
                        break;

                    case nameof(update_time):
                        bool parsed = DateTimeOffset.TryParseExact(reader.GetString(), DateTimeUtils.DATETIMEOFFSET_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset result);
                        if (!parsed) // If we can't parse as DateTimeOffset try DateTime (data could be old)
                            result = DateTime.Parse(reader.GetString(), CultureInfo.InvariantCulture);
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
                            if (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.StartObject)
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
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        {
                            if (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.StartObject)
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
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        {
                            if (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.StartObject)
                            {
                                var txtf = new TextForecast();
                                txtf.FromJson(ref reader);
                                txt_forecasts.Add(txtf);
                            }
                        }
                        this.txt_forecast = txt_forecasts;
                        break;

                    case nameof(min_forecast):
                        // Set initial cap to 60
                        // Minutely forecasts are usually only for an hour
                        var min_forecasts = new List<MinutelyForecast>(60);
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        {
                            if (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.StartObject)
                            {
                                var minF = new MinutelyForecast();
                                minF.FromJson(ref reader);
                                min_forecasts.Add(minF);
                            }
                        }
                        this.min_forecast = min_forecasts;
                        break;

                    case nameof(aqi_forecast):
                        // Set initial cap to 10
                        var aqi_forecasts = new List<AirQuality>(10);
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        {
                            if (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.StartObject)
                            {
                                var fcast = new AirQuality();
                                fcast.FromJson(ref reader);
                                aqi_forecasts.Add(fcast);
                            }
                        }
                        this.aqi_forecast = aqi_forecasts;
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

                    case nameof(weather_alerts):
                        // Set initial cap to 5
                        var alerts = new List<WeatherAlert>(5);
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        {
                            if (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.StartObject)
                            {
                                var alert = new WeatherAlert();
                                alert.FromJson(ref reader);
                                alerts.Add(alert);
                            }
                        }
                        this.weather_alerts = alerts;
                        break;

                    case nameof(ttl):
                        this.ttl = int.Parse(reader.GetString(), CultureInfo.InvariantCulture);
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
                        // ignore
                        break;
                }
            }
        }

        public override string ToJson()
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "location" : ""
            writer.WriteRawValueSafe(nameof(location), location?.ToJson());

            // "update_time" : ""
            writer.WriteString(nameof(update_time), update_time.ToDateTimeOffsetFormat());

            // "forecast" : ""
            if (forecast != null)
            {
                writer.WriteStartArray(nameof(forecast));
                foreach (Forecast cast in forecast)
                {
                    writer.WriteRawValue(cast?.ToJson());
                }
                writer.WriteEndArray();
            }

            // "hr_forecast" : ""
            if (hr_forecast != null)
            {
                writer.WriteStartArray(nameof(hr_forecast));
                foreach (HourlyForecast hr_cast in hr_forecast)
                {
                    writer.WriteRawValue(hr_cast?.ToJson());
                }
                writer.WriteEndArray();
            }

            // "txt_forecast" : ""
            if (txt_forecast != null)
            {
                writer.WriteStartArray(nameof(txt_forecast));
                foreach (TextForecast txt_cast in txt_forecast)
                {
                    writer.WriteRawValue(txt_cast?.ToJson());
                }
                writer.WriteEndArray();
            }

            // "min_forecast" : ""
            if (min_forecast != null)
            {
                writer.WriteStartArray(nameof(min_forecast));
                foreach (MinutelyForecast min_cast in min_forecast)
                {
                    writer.WriteRawValue(min_cast?.ToJson());
                }
                writer.WriteEndArray();
            }

            // "aqi_forecast" : ""
            if (aqi_forecast != null)
            {
                writer.WriteStartArray(nameof(aqi_forecast));
                foreach (AirQuality aqi_cast in aqi_forecast)
                {
                    writer.WriteRawValue(aqi_cast?.ToJson());
                }
                writer.WriteEndArray();
            }

            // "condition" : ""
            writer.WriteRawValueSafe(nameof(condition), condition?.ToJson());

            // "atmosphere" : ""
            writer.WriteRawValueSafe(nameof(atmosphere), atmosphere?.ToJson());

            // "astronomy" : ""
            writer.WriteRawValueSafe(nameof(astronomy), astronomy?.ToJson());

            // "precipitation" : ""
            if (precipitation != null)
            {
                writer.WriteRawValueSafe(nameof(precipitation), precipitation?.ToJson());
            }

            // "weather_alerts" : ""
            if (weather_alerts != null)
            {
                writer.WriteStartArray(nameof(weather_alerts));
                foreach (WeatherAlert alert in weather_alerts)
                {
                    writer.WriteRawValue(alert?.ToJson());
                }
                writer.WriteEndArray();
            }

            // "ttl" : ""
            writer.WriteString(nameof(ttl), ttl.ToInvariantString());

            // "source" : ""
            writer.WriteString(nameof(source), source);

            // "query" : ""
            writer.WriteString(nameof(query), query);

            // "locale" : ""
            writer.WriteString(nameof(locale), locale);

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
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
                   Object.Equals(location, weather.location) &&
                   //update_time.Equals(weather.update_time) &&
                   ((forecast == null && weather.forecast == null) || weather.forecast != null && forecast?.SequenceEqual(weather.forecast) == true) &&
                   ((hr_forecast == null && weather.hr_forecast == null) || weather.hr_forecast != null && hr_forecast?.SequenceEqual(weather.hr_forecast) == true) &&
                   ((txt_forecast == null && weather.txt_forecast == null) || weather.txt_forecast != null && txt_forecast?.SequenceEqual(weather.txt_forecast) == true) &&
                   ((min_forecast == null && weather.min_forecast == null) || weather.min_forecast != null && min_forecast?.SequenceEqual(weather.min_forecast) == true) &&
                   ((aqi_forecast == null && weather.aqi_forecast == null) || weather.aqi_forecast != null && aqi_forecast?.SequenceEqual(weather.aqi_forecast) == true) &&
                   Object.Equals(condition, weather.condition) &&
                   Object.Equals(atmosphere, weather.atmosphere) &&
                   Object.Equals(astronomy, weather.astronomy) &&
                   Object.Equals(precipitation, weather.precipitation) &&
                   ((weather_alerts == null && weather.weather_alerts == null) || weather.weather_alerts != null && weather_alerts?.SequenceEqual(weather.weather_alerts) == true) &&
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
            //hash.Add(update_time);
            hash.Add(forecast);
            hash.Add(hr_forecast);
            hash.Add(txt_forecast);
            hash.Add(min_forecast);
            hash.Add(aqi_forecast);
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
        public float? latitude { get; set; }
        public float? longitude { get; set; }
        public string tz_long { get; set; }

        [JsonIgnore]
        [Ignore]
        public TimeSpan tz_offset
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(tz_long))
                {
                    var nodaTz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(tz_long);
                    if (nodaTz != null)
                        return nodaTz.GetUtcOffset(SystemClock.Instance.GetCurrentInstant()).ToTimeSpan();
                }
                return TimeSpan.Zero;
            }
        }

        [JsonIgnore]
        [Ignore]
        public string tz_short
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(tz_long))
                {
                    var nodaTz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(tz_long);
                    if (nodaTz != null)
                    {
                        return new ZonedDateTime(SystemClock.Instance.GetCurrentInstant(), nodaTz)
                            .ToString("%x", LocaleUtils.GetLocale());
                    }
                }
                return "UTC";
            }
        }

        public Location()
        {
            // Needed for deserialization
            tz_long = "UTC";
        }

        public override bool Equals(object obj)
        {
            return obj is Location location &&
                   name == location.name &&
                   latitude == location.latitude &&
                   longitude == location.longitude &&
                   tz_long == location.tz_long;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, latitude, longitude, tz_long);
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
                    case nameof(name):
                        this.name = reader.GetString();
                        break;

                    case nameof(latitude):
                        this.latitude = reader.TryGetSingle();
                        break;

                    case nameof(longitude):
                        this.longitude = reader.TryGetSingle();
                        break;

                    case nameof(tz_long):
                        this.tz_long = reader.GetString();
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "name" : ""
            writer.WriteString(nameof(name), name);

            // "latitude" : ""
            writer.WriteSingle(nameof(latitude), latitude);

            // "longitude" : ""
            writer.WriteSingle(nameof(longitude), longitude);

            // "tz_long" : ""
            writer.WriteString(nameof(tz_long), tz_long);

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [JsonDerivedType(typeof(Forecast))]
    [JsonDerivedType(typeof(HourlyForecast))]
    public abstract class BaseForecast : CustomJsonObject
    {
        public float? high_f { get; set; }
        public float? high_c { get; set; }
        public string condition { get; set; }
        public string icon { get; set; }
        public ForecastExtras extras { get; set; }
    }

    [JsonConverter(typeof(CustomJsonConverter<Forecast>))]
    public partial class Forecast : BaseForecast
    {
        [JsonConverter(typeof(ISO8601DateTimeFormatter))]
        public DateTime date { get; set; }

        public float? low_f { get; set; }
        public float? low_c { get; set; }

        public Forecast()
        {
            // Needed for deserialization
        }

        public override bool Equals(object obj)
        {
            return obj is Forecast forecast &&
                   date == forecast.date &&
                   high_f == forecast.high_f &&
                   high_c == forecast.high_c &&
                   low_f == forecast.low_f &&
                   low_c == forecast.low_c &&
                   condition == forecast.condition &&
                   icon == forecast.icon &&
                   Object.Equals(extras, forecast.extras);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(date, high_f, high_c, low_f, low_c, condition, icon, extras);
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
                    case nameof(date):
                        this.date = DateTime.Parse(reader.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;

                    case nameof(high_f):
                        this.high_f = reader.TryGetSingle();
                        break;

                    case nameof(high_c):
                        this.high_c = reader.TryGetSingle();
                        break;

                    case nameof(low_f):
                        this.low_f = reader.TryGetSingle();
                        break;

                    case nameof(low_c):
                        this.low_c = reader.TryGetSingle();
                        break;

                    case nameof(condition):
                        this.condition = reader.GetString();
                        break;

                    case nameof(icon):
                        this.icon = reader.GetString();
                        break;

                    case nameof(extras):
                        this.extras = new ForecastExtras();
                        this.extras.FromJson(ref reader);
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "date" : ""
            writer.WriteString(nameof(date), date.ToISO8601Format());

            // "high_f" : ""
            writer.WriteSingle(nameof(high_f), high_f);

            // "high_c" : ""
            writer.WriteSingle(nameof(high_c), high_c);

            // "low_f" : ""
            writer.WriteSingle(nameof(low_f), low_f);

            // "low_c" : ""
            writer.WriteSingle(nameof(low_c), low_c);

            // "condition" : ""
            writer.WriteString(nameof(condition), condition);

            // "icon" : ""
            writer.WriteString(nameof(icon), icon);

            // "extras" : ""
            if (extras != null)
            {
                writer.WriteRawValueSafe(nameof(extras), extras?.ToJson());
            }

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<HourlyForecast>))]
    public partial class HourlyForecast : BaseForecast
    {
        [JsonPropertyName("date")]
        [JsonConverter(typeof(DateTimeOffsetFormatter))]
        public DateTimeOffset date { get; set; }

        public int? wind_degrees { get; set; }
        public float? wind_mph { get; set; }
        public float? wind_kph { get; set; }

        public HourlyForecast()
        {
            // Needed for deserialization
        }

        public override bool Equals(object obj)
        {
            return obj is HourlyForecast forecast &&
                   date == forecast.date &&
                   high_f == forecast.high_f &&
                   high_c == forecast.high_c &&
                   condition == forecast.condition &&
                   icon == forecast.icon &&
                   wind_degrees == forecast.wind_degrees &&
                   wind_mph == forecast.wind_mph &&
                   wind_kph == forecast.wind_kph &&
                   Object.Equals(extras, forecast.extras);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(date);
            hash.Add(high_f);
            hash.Add(high_c);
            hash.Add(condition);
            hash.Add(icon);
            hash.Add(wind_degrees);
            hash.Add(wind_mph);
            hash.Add(wind_kph);
            hash.Add(extras);
            return hash.ToHashCode();
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
                    case nameof(date):
                        this.date = reader.TryGetDateTimeOffset(DateTimeUtils.DATETIMEOFFSET_FORMAT);
                        break;

                    case nameof(high_f):
                        this.high_f = reader.TryGetSingle();
                        break;

                    case nameof(high_c):
                        this.high_c = reader.TryGetSingle();
                        break;

                    case nameof(condition):
                        this.condition = reader.GetString();
                        break;

                    case nameof(icon):
                        this.icon = reader.GetString();
                        break;

                    case nameof(wind_degrees):
                        this.wind_degrees = reader.TryGetInt32();
                        break;

                    case nameof(wind_mph):
                        this.wind_mph = reader.TryGetSingle();
                        break;

                    case nameof(wind_kph):
                        this.wind_kph = reader.TryGetSingle();
                        break;

                    case nameof(extras):
                        this.extras = new ForecastExtras();
                        this.extras.FromJson(ref reader);
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "date" : ""
            writer.WriteString(nameof(date), date.ToDateTimeOffsetFormat());

            // "high_f" : ""
            writer.WriteSingle(nameof(high_f), high_f);

            // "high_c" : ""
            writer.WriteSingle(nameof(high_c), high_c);

            // "condition" : ""
            writer.WriteString(nameof(condition), condition);

            // "icon" : ""
            writer.WriteString(nameof(icon), icon);

            // "wind_degrees" : ""
            writer.WriteInt32(nameof(wind_degrees), wind_degrees);

            // "wind_mph" : ""
            writer.WriteSingle(nameof(wind_mph), wind_mph);

            // "wind_kph" : ""
            writer.WriteSingle(nameof(wind_kph), wind_kph);

            // "extras" : ""
            if (extras != null)
            {
                writer.WriteRawValueSafe(nameof(extras), extras?.ToJson());
            }

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<MinutelyForecast>))]
    public partial class MinutelyForecast : CustomJsonObject
    {
        [JsonConverter(typeof(ISO8601DateTimeOffsetFormatter))]
        public DateTimeOffset date { get; set; }

        public float? rain_mm { get; set; }

        public MinutelyForecast()
        {
            // Needed for deserialization
        }

        public override bool Equals(object obj)
        {
            return obj is MinutelyForecast forecast &&
                   date == forecast.date &&
                   rain_mm == forecast.rain_mm;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(date);
            hash.Add(rain_mm);
            return hash.ToHashCode();
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
                    case nameof(date):
                        this.date = reader.TryGetDateTimeOffset(DateTimeUtils.ISO8601_DATETIMEOFFSET_FORMAT);
                        break;

                    case nameof(rain_mm):
                        this.rain_mm = reader.TryGetSingle();
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "date" : ""
            writer.WriteString(nameof(date), date.ToISO8601Format());

            // "rain_mm" : ""
            writer.WriteSingle(nameof(rain_mm), rain_mm);

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<TextForecast>))]
    public partial class TextForecast : CustomJsonObject
    {
        [JsonConverter(typeof(ISO8601DateTimeOffsetFormatter))]
        public DateTimeOffset date { get; set; }

        public string fcttext { get; set; }
        public string fcttext_metric { get; set; }

        public TextForecast()
        {
            // Needed for deserialization
        }

        public override bool Equals(object obj)
        {
            return obj is TextForecast forecast &&
                   date == forecast.date &&
                   fcttext == forecast.fcttext &&
                   fcttext_metric == forecast.fcttext_metric;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(date);
            hash.Add(fcttext);
            hash.Add(fcttext_metric);
            return hash.ToHashCode();
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
                    case nameof(date):
                        this.date = reader.TryGetDateTimeOffset(DateTimeUtils.ISO8601_DATETIMEOFFSET_FORMAT);
                        break;

                    case nameof(fcttext):
                        this.fcttext = reader.GetString();
                        break;

                    case nameof(fcttext_metric):
                        this.fcttext_metric = reader.GetString();
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "date" : ""
            writer.WriteString(nameof(date), date.ToISO8601Format());

            // "fcttext" : ""
            writer.WriteString(nameof(fcttext), fcttext);

            // "fcttext_metric" : ""
            writer.WriteString(nameof(fcttext_metric), fcttext_metric);

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<ForecastExtras>))]
    public class ForecastExtras : CustomJsonObject
    {
        public float? feelslike_f { get; set; }
        public float? feelslike_c { get; set; }
        public int? humidity { get; set; }
        public float? dewpoint_f { get; set; }
        public float? dewpoint_c { get; set; }
        public float? uv_index { get; set; }
        public int? pop { get; set; }
        public int? cloudiness { get; set; }
        public float? qpf_rain_in { get; set; }
        public float? qpf_rain_mm { get; set; }
        public float? qpf_snow_in { get; set; }
        public float? qpf_snow_cm { get; set; }
        public float? pressure_mb { get; set; }
        public float? pressure_in { get; set; }
        public int? wind_degrees { get; set; }
        public float? wind_mph { get; set; }
        public float? wind_kph { get; set; }
        public float? visibility_mi { get; set; }
        public float? visibility_km { get; set; }
        public float? windgust_mph { get; set; }
        public float? windgust_kph { get; set; }

        public ForecastExtras()
        {
            // Needed for deserialization
        }

        public override bool Equals(object obj)
        {
            return obj is ForecastExtras extras &&
                   feelslike_f == extras.feelslike_f &&
                   feelslike_c == extras.feelslike_c &&
                   humidity == extras.humidity &&
                   dewpoint_f == extras.dewpoint_f &&
                   dewpoint_c == extras.dewpoint_c &&
                   uv_index == extras.uv_index &&
                   pop == extras.pop &&
                   cloudiness == extras.cloudiness &&
                   qpf_rain_in == extras.qpf_rain_in &&
                   qpf_rain_mm == extras.qpf_rain_mm &&
                   qpf_snow_in == extras.qpf_snow_in &&
                   qpf_snow_cm == extras.qpf_snow_cm &&
                   pressure_mb == extras.pressure_mb &&
                   pressure_in == extras.pressure_in &&
                   wind_degrees == extras.wind_degrees &&
                   wind_mph == extras.wind_mph &&
                   wind_kph == extras.wind_kph &&
                   visibility_mi == extras.visibility_mi &&
                   visibility_km == extras.visibility_km &&
                   windgust_mph == extras.windgust_mph &&
                   windgust_kph == extras.windgust_kph;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(feelslike_f);
            hash.Add(feelslike_c);
            hash.Add(humidity);
            hash.Add(dewpoint_f);
            hash.Add(dewpoint_c);
            hash.Add(uv_index);
            hash.Add(pop);
            hash.Add(cloudiness);
            hash.Add(qpf_rain_in);
            hash.Add(qpf_rain_mm);
            hash.Add(qpf_snow_in);
            hash.Add(qpf_snow_cm);
            hash.Add(pressure_mb);
            hash.Add(pressure_in);
            hash.Add(wind_degrees);
            hash.Add(wind_mph);
            hash.Add(wind_kph);
            hash.Add(visibility_mi);
            hash.Add(visibility_km);
            hash.Add(windgust_mph);
            hash.Add(windgust_kph);
            return hash.ToHashCode();
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
                    case nameof(feelslike_f):
                        this.feelslike_f = reader.TryGetSingle();
                        break;

                    case nameof(feelslike_c):
                        this.feelslike_c = reader.TryGetSingle();
                        break;

                    case nameof(humidity):
                        this.humidity = reader.TryGetInt32();
                        break;

                    case nameof(dewpoint_f):
                        this.dewpoint_f = reader.TryGetSingle();
                        break;

                    case nameof(dewpoint_c):
                        this.dewpoint_c = reader.TryGetSingle();
                        break;

                    case nameof(uv_index):
                        this.uv_index = reader.TryGetSingle();
                        break;

                    case nameof(pop):
                        this.pop = reader.TryGetInt32();
                        break;

                    case nameof(cloudiness):
                        this.cloudiness = reader.TryGetInt32();
                        break;

                    case nameof(qpf_rain_in):
                        this.qpf_rain_in = reader.TryGetSingle();
                        break;

                    case nameof(qpf_rain_mm):
                        this.qpf_rain_mm = reader.TryGetSingle();
                        break;

                    case nameof(qpf_snow_in):
                        this.qpf_snow_in = reader.TryGetSingle();
                        break;

                    case nameof(qpf_snow_cm):
                        this.qpf_snow_cm = reader.TryGetSingle();
                        break;

                    case nameof(pressure_mb):
                        this.pressure_mb = reader.TryGetSingle();
                        break;

                    case nameof(pressure_in):
                        this.pressure_in = reader.TryGetSingle();
                        break;

                    case nameof(wind_degrees):
                        this.wind_degrees = reader.TryGetInt32();
                        break;

                    case nameof(wind_mph):
                        this.wind_mph = reader.TryGetSingle();
                        break;

                    case nameof(wind_kph):
                        this.wind_kph = reader.TryGetSingle();
                        break;

                    case nameof(visibility_mi):
                        this.visibility_mi = reader.TryGetSingle();
                        break;

                    case nameof(visibility_km):
                        this.visibility_km = reader.TryGetSingle();
                        break;

                    case nameof(windgust_mph):
                        this.windgust_mph = reader.TryGetSingle();
                        break;

                    case nameof(windgust_kph):
                        this.windgust_kph = reader.TryGetSingle();
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "feelslike_f" : ""
            writer.WriteSingle(nameof(feelslike_f), feelslike_f);

            // "feelslike_c" : ""
            writer.WriteSingle(nameof(feelslike_c), feelslike_c);

            // "humidity" : ""
            writer.WriteInt32(nameof(humidity), humidity);

            // "dewpoint_f" : ""
            writer.WriteSingle(nameof(dewpoint_f), dewpoint_f);

            // "dewpoint_c" : ""
            writer.WriteSingle(nameof(dewpoint_c), dewpoint_c);

            // "uv_index" : ""
            writer.WriteSingle(nameof(uv_index), uv_index);

            // "pop" : ""
            writer.WriteInt32(nameof(pop), pop);

            // "cloudiness" : ""
            writer.WriteInt32(nameof(cloudiness), cloudiness);

            // "qpf_rain_in" : ""
            writer.WriteSingle(nameof(qpf_rain_in), qpf_rain_in);

            // "qpf_rain_mm" : ""
            writer.WriteSingle(nameof(qpf_rain_mm), qpf_rain_mm);

            // "qpf_snow_in" : ""
            writer.WriteSingle(nameof(qpf_snow_in), qpf_snow_in);

            // "qpf_snow_cm" : ""
            writer.WriteSingle(nameof(qpf_snow_cm), qpf_snow_cm);

            // "pressure_mb" : ""
            writer.WriteSingle(nameof(pressure_mb), pressure_mb);

            // "pressure_in" : ""
            writer.WriteSingle(nameof(pressure_in), pressure_in);

            // "wind_degrees" : ""
            writer.WriteInt32(nameof(wind_degrees), wind_degrees);

            // "wind_mph" : ""
            writer.WriteSingle(nameof(wind_mph), wind_mph);

            // "wind_kph" : ""
            writer.WriteSingle(nameof(wind_kph), wind_kph);

            // "visibility_mi" : ""
            writer.WriteSingle(nameof(visibility_mi), visibility_mi);

            // "visibility_km" : ""
            writer.WriteSingle(nameof(visibility_km), visibility_km);

            // "windgust_mph" : ""
            writer.WriteSingle(nameof(windgust_mph), windgust_mph);

            // "windgust_kph" : ""
            writer.WriteSingle(nameof(windgust_kph), windgust_kph);

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<Condition>))]
    public partial class Condition : CustomJsonObject
    {
        public string weather { get; set; }
        public float? temp_f { get; set; }
        public float? temp_c { get; set; }
        public int? wind_degrees { get; set; }
        public float? wind_mph { get; set; }
        public float? wind_kph { get; set; }
        public float? windgust_mph { get; set; }
        public float? windgust_kph { get; set; }
        public float? feelslike_f { get; set; }
        public float? feelslike_c { get; set; }
        public string icon { get; set; }
        public Beaufort beaufort { get; set; }
        public UV uv { get; set; }
        public float? high_f { get; set; }
        public float? high_c { get; set; }
        public float? low_f { get; set; }
        public float? low_c { get; set; }
        public AirQuality airQuality { get; set; }
        public Pollen pollen { get; set; }
        [JsonConverter(typeof(ISO8601DateTimeOffsetFormatter))]
        public DateTimeOffset observation_time { get; set; }
        public string summary { get; set; }

        public Condition()
        {
            // Needed for deserialization
        }

        public override bool Equals(object obj)
        {
            return obj is Condition condition &&
                   weather == condition.weather &&
                   temp_f == condition.temp_f &&
                   temp_c == condition.temp_c &&
                   wind_degrees == condition.wind_degrees &&
                   wind_mph == condition.wind_mph &&
                   wind_kph == condition.wind_kph &&
                   feelslike_f == condition.feelslike_f &&
                   feelslike_c == condition.feelslike_c &&
                   icon == condition.icon &&
                   Object.Equals(beaufort, condition.beaufort) &&
                   Object.Equals(uv, condition.uv) &&
                   high_f == condition.high_f &&
                   high_c == condition.high_c &&
                   low_f == condition.low_f &&
                   low_c == condition.low_c &&
                   Object.Equals(airQuality, condition.airQuality) &&
                   Object.Equals(pollen, condition.pollen) &&
                   observation_time == condition.observation_time &&
                   summary == condition.summary;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(weather);
            hash.Add(temp_f);
            hash.Add(temp_c);
            hash.Add(wind_degrees);
            hash.Add(wind_mph);
            hash.Add(wind_kph);
            hash.Add(feelslike_f);
            hash.Add(feelslike_c);
            hash.Add(icon);
            hash.Add(beaufort);
            hash.Add(uv);
            hash.Add(high_f);
            hash.Add(high_c);
            hash.Add(low_f);
            hash.Add(low_c);
            hash.Add(airQuality);
            hash.Add(pollen);
            hash.Add(observation_time);
            hash.Add(summary);
            return hash.ToHashCode();
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
                    case nameof(weather):
                        this.weather = reader.GetString();
                        break;

                    case nameof(temp_f):
                        this.temp_f = reader.TryGetSingle();
                        break;

                    case nameof(temp_c):
                        this.temp_c = reader.TryGetSingle();
                        break;

                    case nameof(wind_degrees):
                        this.wind_degrees = reader.TryGetInt32();
                        break;

                    case nameof(wind_mph):
                        this.wind_mph = reader.TryGetSingle();
                        break;

                    case nameof(wind_kph):
                        this.wind_kph = reader.TryGetSingle();
                        break;

                    case nameof(windgust_mph):
                        this.windgust_mph = reader.TryGetSingle();
                        break;

                    case nameof(windgust_kph):
                        this.windgust_kph = reader.TryGetSingle();
                        break;

                    case nameof(feelslike_f):
                        this.feelslike_f = reader.TryGetSingle();
                        break;

                    case nameof(feelslike_c):
                        this.feelslike_c = reader.TryGetSingle();
                        break;

                    case nameof(icon):
                        this.icon = reader.GetString();
                        break;

                    case nameof(beaufort):
                        this.beaufort = new Beaufort();
                        this.beaufort.FromJson(ref reader);
                        break;

                    case nameof(uv):
                        this.uv = new UV();
                        this.uv.FromJson(ref reader);
                        break;

                    case nameof(high_f):
                        this.high_f = reader.TryGetSingle();
                        break;

                    case nameof(high_c):
                        this.high_c = reader.TryGetSingle();
                        break;

                    case nameof(low_f):
                        this.low_f = reader.TryGetSingle();
                        break;

                    case nameof(low_c):
                        this.low_c = reader.TryGetSingle();
                        break;

                    case nameof(airQuality):
                        this.airQuality = new AirQuality();
                        this.airQuality.FromJson(ref reader);
                        break;

                    case nameof(pollen):
                        this.pollen = new Pollen();
                        this.pollen.FromJson(ref reader);
                        break;

                    case nameof(observation_time):
                        this.observation_time = reader.TryGetDateTimeOffset(DateTimeUtils.ISO8601_DATETIMEOFFSET_FORMAT);
                        break;

                    case nameof(summary):
                        this.summary = reader.GetString();
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "weather" : ""
            writer.WriteString(nameof(weather), weather);

            // "temp_f" : ""
            writer.WriteSingle(nameof(temp_f), temp_f);

            // "temp_c" : ""
            writer.WriteSingle(nameof(temp_c), temp_c);

            // "wind_degrees" : ""
            writer.WriteInt32(nameof(wind_degrees), wind_degrees);

            // "wind_mph" : ""
            writer.WriteSingle(nameof(wind_mph), wind_mph);

            // "wind_kph" : ""
            writer.WriteSingle(nameof(wind_kph), wind_kph);

            // "windgust_mph" : ""
            writer.WriteSingle(nameof(windgust_mph), windgust_mph);

            // "windgust_kph" : ""
            writer.WriteSingle(nameof(windgust_kph), windgust_kph);

            // "feelslike_f" : ""
            writer.WriteSingle(nameof(feelslike_f), feelslike_f);

            // "feelslike_c" : ""
            writer.WriteSingle(nameof(feelslike_c), feelslike_c);

            // "icon" : ""
            writer.WriteString(nameof(icon), icon);

            // "beaufort" : ""
            if (beaufort != null)
            {
                writer.WriteRawValueSafe(nameof(beaufort), beaufort?.ToJson());
            }

            // "uv" : ""
            if (uv != null)
            {
                writer.WriteRawValueSafe(nameof(uv), uv?.ToJson());
            }

            // "high_f" : ""
            writer.WriteSingle(nameof(high_f), high_f);

            // "high_c" : ""
            writer.WriteSingle(nameof(high_c), high_c);

            // "low_f" : ""
            writer.WriteSingle(nameof(low_f), low_f);

            // "low_c" : ""
            writer.WriteSingle(nameof(low_c), low_c);

            // "airQuality" : ""
            if (airQuality != null)
            {
                writer.WriteRawValueSafe(nameof(airQuality), airQuality?.ToJson());
            }

            // "pollen" : ""
            if (pollen != null)
            {
                writer.WriteRawValueSafe(nameof(pollen), pollen?.ToJson());
            }

            // "observation_time" : ""
            writer.WriteString(nameof(observation_time), observation_time.ToISO8601Format());

            // "summary" : ""
            writer.WriteString(nameof(summary), summary);

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<Atmosphere>))]
    public partial class Atmosphere : CustomJsonObject
    {
        public int? humidity { get; set; }
        public float? pressure_mb { get; set; }
        public float? pressure_in { get; set; }
        public string pressure_trend { get; set; }
        public float? visibility_mi { get; set; }
        public float? visibility_km { get; set; }
        public float? dewpoint_f { get; set; }
        public float? dewpoint_c { get; set; }

        public Atmosphere()
        {
            // Needed for deserialization
        }

        public override bool Equals(object obj)
        {
            return obj is Atmosphere atmosphere &&
                   humidity == atmosphere.humidity &&
                   pressure_mb == atmosphere.pressure_mb &&
                   pressure_in == atmosphere.pressure_in &&
                   pressure_trend == atmosphere.pressure_trend &&
                   visibility_mi == atmosphere.visibility_mi &&
                   visibility_km == atmosphere.visibility_km &&
                   dewpoint_f == atmosphere.dewpoint_f &&
                   dewpoint_c == atmosphere.dewpoint_c;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(humidity, pressure_mb, pressure_in, pressure_trend, visibility_mi, visibility_km, dewpoint_f, dewpoint_c);
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
                    case nameof(humidity):
                        this.humidity = reader.TryGetInt32();
                        break;

                    case nameof(pressure_mb):
                        this.pressure_mb = reader.TryGetSingle();
                        break;

                    case nameof(pressure_in):
                        this.pressure_in = reader.TryGetSingle();
                        break;

                    case nameof(pressure_trend):
                        this.pressure_trend = reader.GetString();
                        break;

                    case nameof(visibility_mi):
                        this.visibility_mi = reader.TryGetSingle();
                        break;

                    case nameof(visibility_km):
                        this.visibility_km = reader.TryGetSingle();
                        break;

                    case nameof(dewpoint_f):
                        this.dewpoint_f = reader.TryGetSingle();
                        break;

                    case nameof(dewpoint_c):
                        this.dewpoint_c = reader.TryGetSingle();
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "humidity" : ""
            writer.WriteInt32(nameof(humidity), humidity);

            // "pressure_mb" : ""
            writer.WriteSingle(nameof(pressure_mb), pressure_mb);

            // "pressure_in" : ""
            writer.WriteSingle(nameof(pressure_in), pressure_in);

            // "pressure_trend" : ""
            writer.WriteString(nameof(pressure_trend), pressure_trend);

            // "visibility_mi" : ""
            writer.WriteSingle(nameof(visibility_mi), visibility_mi);

            // "visibility_km" : ""
            writer.WriteSingle(nameof(visibility_km), visibility_km);

            // "dewpoint_f" : ""
            writer.WriteSingle(nameof(dewpoint_f), dewpoint_f);

            // "dewpoint_c" : ""
            writer.WriteSingle(nameof(dewpoint_c), dewpoint_c);

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<Astronomy>))]
    public partial class Astronomy : CustomJsonObject
    {
        [JsonConverter(typeof(ISO8601DateTimeFormatter))]
        public DateTime sunrise { get; set; }

        [JsonConverter(typeof(ISO8601DateTimeFormatter))]
        public DateTime sunset { get; set; }

        [JsonConverter(typeof(ISO8601DateTimeFormatter))]
        public DateTime moonrise { get; set; }

        [JsonConverter(typeof(ISO8601DateTimeFormatter))]
        public DateTime moonset { get; set; }

        public MoonPhase moonphase { get; set; }

        public Astronomy()
        {
            // Needed for deserialization
        }

        public override bool Equals(object obj)
        {
            // Only compare times by the second
            return obj is Astronomy astronomy &&
                   sunrise.Trim(TimeSpan.TicksPerSecond) == astronomy.sunrise.Trim(TimeSpan.TicksPerSecond) &&
                   sunset.Trim(TimeSpan.TicksPerSecond) == astronomy.sunset.Trim(TimeSpan.TicksPerSecond) &&
                   moonrise.Trim(TimeSpan.TicksPerSecond) == astronomy.moonrise.Trim(TimeSpan.TicksPerSecond) &&
                   moonset.Trim(TimeSpan.TicksPerSecond) == astronomy.moonset.Trim(TimeSpan.TicksPerSecond) &&
                   Object.Equals(moonphase, astronomy.moonphase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(sunrise, sunset, moonrise, moonset, moonphase);
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
                    case nameof(sunrise):
                        this.sunrise = reader.TryGetDateTime(DateTimeUtils.ISO8601_DATETIME_FORMAT);
                        break;

                    case nameof(sunset):
                        this.sunset = reader.TryGetDateTime(DateTimeUtils.ISO8601_DATETIME_FORMAT);
                        break;

                    case nameof(moonrise):
                        this.moonrise = reader.TryGetDateTime(DateTimeUtils.ISO8601_DATETIME_FORMAT);
                        break;

                    case nameof(moonset):
                        this.moonset = reader.TryGetDateTime(DateTimeUtils.ISO8601_DATETIME_FORMAT);
                        break;

                    case nameof(moonphase):
                        this.moonphase = new MoonPhase();
                        this.moonphase.FromJson(ref reader);
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "sunrise" : ""
            if (sunrise != null)
            {
                writer.WriteString(nameof(sunrise), sunrise.ToISO8601Format());
            }

            // "sunset" : ""
            if (sunset != null)
            {
                writer.WriteString(nameof(sunset), sunset.ToISO8601Format());
            }

            // "moonrise" : ""
            if (moonrise != null)
            {
                writer.WriteString(nameof(moonrise), moonrise.ToISO8601Format());
            }

            // "moonset" : ""
            if (moonset != null)
            {
                writer.WriteString(nameof(moonset), moonset.ToISO8601Format());
            }

            // "moonphase" : ""
            if (moonphase != null)
            {
                writer.WriteRawValueSafe(nameof(moonphase), moonphase.ToJson());
            }

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<Precipitation>))]
    public partial class Precipitation : CustomJsonObject
    {
        public int? pop { get; set; }
        public int? cloudiness { get; set; }
        public float? qpf_rain_in { get; set; }
        public float? qpf_rain_mm { get; set; }
        public float? qpf_snow_in { get; set; }
        public float? qpf_snow_cm { get; set; }

        public Precipitation()
        {
            // Needed for deserialization
        }

        public override bool Equals(object obj)
        {
            return obj is Precipitation precipitation &&
                   pop == precipitation.pop &&
                   cloudiness == precipitation.cloudiness &&
                   qpf_rain_in == precipitation.qpf_rain_in &&
                   qpf_rain_mm == precipitation.qpf_rain_mm &&
                   qpf_snow_in == precipitation.qpf_snow_in &&
                   qpf_snow_cm == precipitation.qpf_snow_cm;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(pop, cloudiness, qpf_rain_in, qpf_rain_mm, qpf_snow_in, qpf_snow_cm);
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
                    case nameof(pop):
                        this.pop = reader.TryGetInt32();
                        break;

                    case nameof(cloudiness):
                        this.cloudiness = reader.TryGetInt32();
                        break;

                    case nameof(qpf_rain_in):
                        this.qpf_rain_in = reader.TryGetSingle();
                        break;

                    case nameof(qpf_rain_mm):
                        this.qpf_rain_mm = reader.TryGetSingle();
                        break;

                    case nameof(qpf_snow_in):
                        this.qpf_snow_in = reader.TryGetSingle();
                        break;

                    case nameof(qpf_snow_cm):
                        this.qpf_snow_cm = reader.TryGetSingle();
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "pop" : ""
            writer.WriteInt32(nameof(pop), pop);

            // "cloudiness" : ""
            writer.WriteInt32(nameof(cloudiness), cloudiness);

            // "qpf_rain_in" : ""
            writer.WriteSingle(nameof(qpf_rain_in), qpf_rain_in);

            // "qpf_rain_mm" : ""
            writer.WriteSingle(nameof(qpf_rain_mm), qpf_rain_mm);

            // "qpf_snow_in" : ""
            writer.WriteSingle(nameof(qpf_snow_in), qpf_snow_in);

            // "qpf_snow_cm" : ""
            writer.WriteSingle(nameof(qpf_snow_cm), qpf_snow_cm);

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
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

        public Beaufort()
        {
            // Needed for deserialization
        }

        public Beaufort(BeaufortScale scale)
        {
            this.scale = scale;
        }

        public override bool Equals(object obj)
        {
            return obj is Beaufort beaufort &&
                   scale == beaufort.scale;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(scale);
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
                    case nameof(scale):
                        this.scale = (BeaufortScale)reader.GetInt32();
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "scale" : ""
            writer.WriteInt32(nameof(scale), (int)scale);

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
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

        public MoonPhase()
        {
            // Needed for deserialization
        }

        public MoonPhase(MoonPhaseType moonPhaseType)
        {
            this.phase = moonPhaseType;
        }

        public override bool Equals(object obj)
        {
            return obj is MoonPhase phase &&
                   this.phase == phase.phase;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(phase);
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
                    case nameof(phase):
                        this.phase = (MoonPhaseType)reader.GetInt32();
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "phase" : ""
            writer.WriteInt32(nameof(phase), (int)phase);

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<UV>))]
    public partial class UV : CustomJsonObject
    {
        public float? index { get; set; }

        public UV()
        {
            // Needed for deserialization
        }

        public UV(float index)
        {
            this.index = index;
        }

        public override bool Equals(object obj)
        {
            return obj is UV uV &&
                   index == uV.index;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(index);
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
                    case nameof(index):
                        this.index = reader.TryGetSingle();
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "scale" : ""
            writer.WriteSingle(nameof(index), index);

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<AirQuality>))]
    public partial class AirQuality : CustomJsonObject
    {
        public int? index { get; set; }
        public string attribution { get; set; }
        public int? no2 { get; set; }
        public int? o3 { get; set; }
        public int? so2 { get; set; }
        public int? pm25 { get; set; }
        public int? pm10 { get; set; }
        public int? co { get; set; }
        [JsonConverter(typeof(SimpleNullableDateTimeFormatter))]
        public DateTime? date { get; set; }

        public AirQuality()
        {
            // Needed for deserialization
        }

        public override bool Equals(object obj)
        {
            return obj is AirQuality quality &&
                   index == quality.index &&
                   attribution == quality.attribution &&
                   no2 == quality.no2 &&
                   o3 == quality.o3 &&
                   so2 == quality.so2 &&
                   pm25 == quality.pm25 &&
                   pm10 == quality.pm10 &&
                   co == quality.co &&
                   date == quality.date;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(index);
            hash.Add(attribution);
            hash.Add(no2);
            hash.Add(o3);
            hash.Add(so2);
            hash.Add(pm25);
            hash.Add(pm10);
            hash.Add(co);
            hash.Add(date);
            return hash.ToHashCode();
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
                    case nameof(index):
                        this.index = reader.TryGetInt32();
                        break;

                    case nameof(attribution):
                        this.attribution = reader.GetString();
                        break;

                    case nameof(no2):
                        this.no2 = reader.TryGetInt32();
                        break;

                    case nameof(o3):
                        this.o3 = reader.TryGetInt32();
                        break;

                    case nameof(so2):
                        this.so2 = reader.TryGetInt32();
                        break;

                    case nameof(pm25):
                        this.pm25 = reader.TryGetInt32();
                        break;

                    case nameof(pm10):
                        this.pm10 = reader.TryGetInt32();
                        break;

                    case nameof(co):
                        this.co = reader.TryGetInt32();
                        break;

                    case nameof(date):
                        this.date = reader.TryGetDateTime("yyyy-MM-dd");
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "index" : ""
            writer.WriteSingle(nameof(index), index);

            // "attribution" : ""
            writer.WriteString(nameof(attribution), attribution);

            if (no2.HasValue)
            {
                // "no2" : ""
                writer.WriteSingle(nameof(no2), no2.Value);
            }

            if (o3.HasValue)
            {
                // "o3" : ""
                writer.WriteSingle(nameof(o3), o3.Value);
            }

            if (so2.HasValue)
            {
                // "so2" : ""
                writer.WriteSingle(nameof(so2), so2.Value);
            }

            if (pm25.HasValue)
            {
                // "no2" : ""
                writer.WriteSingle(nameof(pm25), pm25.Value);
            }

            if (pm10.HasValue)
            {
                // "pm10" : ""
                writer.WriteSingle(nameof(pm10), pm10.Value);
            }

            if (co.HasValue)
            {
                // "co" : ""
                writer.WriteSingle(nameof(co), co.Value);
            }

            if (date.HasValue)
            {
                // "date" : ""
                writer.WriteString(nameof(date), date.Value.ToInvariantString("yyyy-MM-dd"));
            }

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [JsonConverter(typeof(CustomJsonConverter<Pollen>))]
    public partial class Pollen : CustomJsonObject
    {
        public enum PollenCount
        {
            Unknown,
            Low,
            Moderate,
            High,
            VeryHigh
        }

        public PollenCount? treePollenCount;
        public PollenCount? grassPollenCount;
        public PollenCount? ragweedPollenCount;
        public string attribution;

        public Pollen()
        {
            // Needed for deserialization
        }

        public override bool Equals(object obj)
        {
            return obj is Pollen pollen &&
                   treePollenCount == pollen.treePollenCount &&
                   grassPollenCount == pollen.grassPollenCount &&
                   ragweedPollenCount == pollen.ragweedPollenCount &&
                   attribution == pollen.attribution;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(treePollenCount, grassPollenCount, ragweedPollenCount, attribution);
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
                    case nameof(treePollenCount):
                        this.treePollenCount = reader.TryGetInt32()?.Let(x => (PollenCount)x);
                        break;

                    case nameof(grassPollenCount):
                        this.grassPollenCount = reader.TryGetInt32()?.Let(x => (PollenCount)x);
                        break;

                    case nameof(ragweedPollenCount):
                        this.ragweedPollenCount = reader.TryGetInt32()?.Let(x => (PollenCount)x);
                        break;

                    case nameof(attribution):
                        this.attribution = reader.GetString();
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
            using var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "treePollenCount" : ""
            writer.WriteInt32(nameof(treePollenCount), (int)(treePollenCount ?? PollenCount.Unknown));

            // "grassPollenCount" : ""
            writer.WriteInt32(nameof(grassPollenCount), (int)(grassPollenCount ?? PollenCount.Unknown));

            // "ragweedPollenCount" : ""
            writer.WriteInt32(nameof(ragweedPollenCount), (int)(ragweedPollenCount ?? PollenCount.Unknown));

            // "attribution" : ""
            writer.WriteString(nameof(attribution), attribution);

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [Table(TABLE_NAME)]
    public class Forecasts
    {
        public const string TABLE_NAME = "forecasts";

        [PrimaryKey]
        public string query { get; set; }

        [TextBlob(nameof(forecastblob))]
        public IList<Forecast> forecast { get; set; }

        [TextBlob(nameof(txtforecastblob))]
        public IList<TextForecast> txt_forecast { get; set; }

        [TextBlob(nameof(minforecastblob))]
        public IList<MinutelyForecast> min_forecast { get; set; }

        [TextBlob(nameof(aqiforecastblob))]
        public IList<AirQuality> aqi_forecast { get; set; }

        [JsonIgnore]
        public string forecastblob { get; set; }

        [JsonIgnore]
        public string txtforecastblob { get; set; }

        [JsonIgnore]
        public string minforecastblob { get; set; }

        [JsonIgnore]
        public string aqiforecastblob { get; set; }

        public Forecasts()
        {
        }

        public Forecasts(Weather weatherData)
        {
            this.query = weatherData?.query;
            this.forecast = weatherData?.forecast;
            this.txt_forecast = weatherData?.txt_forecast;
            this.min_forecast = weatherData?.min_forecast;
            this.aqi_forecast = weatherData?.aqi_forecast;
        }
    }

    [Table(TABLE_NAME)]
    public class HourlyForecasts
    {
        public const string TABLE_NAME = "hr_forecasts";

        [PrimaryKey]
        public string id { get; set; }

        [Indexed(Name = "queryIdx", Order = 1)]
        public string query { get; set; }

        [JsonIgnore]
        [Ignore]
        // Doesn't store this in db
        // For DateTimeOffset, offset isn't stored when saving to db
        // Store as string (blob) instead
        // If db previously stored DateTimeOffset (as ticks) retrieve and set offset
        public DateTimeOffset date
        {
            get
            {
                if (DateTimeOffset.TryParseExact(dateblob, "yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset result))
                    return result;
                else
                    return DateTimeOffset.Parse(dateblob, CultureInfo.InvariantCulture);
            }
            set { dateblob = value.ToString("yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture); }
        }

        [TextBlob(nameof(hrforecastblob))]
        [NotNull]
        public HourlyForecast hr_forecast { get; set; }

        [Indexed(Name = "dateIdx", Order = 2)]
        [JsonPropertyName("date")]
        [NotNull]
        // Keep DateTimeOffset column name to get data as string
        public string dateblob { get; set; }

        [JsonIgnore]
        public string hrforecastblob { get; set; }

        public HourlyForecasts()
        {
        }

        public HourlyForecasts(string query, HourlyForecast forecast)
        {
            this.query = query;
            this.hr_forecast = forecast;
            this.date = forecast.date;
            this.id = query + '|' + dateblob;
        }
    }
}