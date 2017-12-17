using Newtonsoft.Json;
using SimpleWeather.Utils;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;
using System;
using System.Globalization;

namespace SimpleWeather.WeatherData
{
    [JsonConverter(typeof(CustomJsonConverter))]
    [Table("weatherdata")]
    public class Weather
    {
        [TextBlob("locationblob")]
        public Location location { get; set; }
        public DateTime update_time { get; set; }
        [TextBlob("forecastblob")]
        public Forecast[] forecast { get; set; }
        [TextBlob("hrforecastblob")]
        public HourlyForecast[] hr_forecast { get; set; }
        [TextBlob("txtforecastblob")]
        public TextForecast[] txt_forecast { get; set; }
        [TextBlob("conditionblob")]
        public Condition condition { get; set; }
        [TextBlob("atmosphereblob")]
        public Atmosphere atmosphere { get; set; }
        [TextBlob("astronomyblob")]
        public Astronomy astronomy { get; set; }
        [TextBlob("precipitationblob")]
        public Precipitation precipitation { get; set; }
        public string ttl { get; set; }
        public string source { get; set; }
        [PrimaryKey]
        public string query { get; set; }
        public string locale { get; set; }

        [JsonIgnore]
        public string locationblob { get; set; }
        [JsonIgnore]
        public string forecastblob { get; set; }
        [JsonIgnore]
        public string hrforecastblob { get; set; }
        [JsonIgnore]
        public string txtforecastblob { get; set; }
        [JsonIgnore]
        public string conditionblob { get; set; }
        [JsonIgnore]
        public string atmosphereblob { get; set; }
        [JsonIgnore]
        public string astronomyblob { get; set; }
        [JsonIgnore]
        public string precipitationblob { get; set; }

        [JsonConstructor]
        public Weather()
        {
            // Needed for deserialization
        }

        public Weather(WeatherYahoo.Rootobject root)
        {
            location = new Location(root.query);
            update_time = DateTime.ParseExact(root.query.created,
                            "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            forecast = new Forecast[root.query.results.channel.item.forecast.Length];
            for (int i = 0; i < forecast.Length; i++)
            {
                forecast[i] = new Forecast(root.query.results.channel.item.forecast[i]);
            }
            condition = new Condition(root.query.results.channel);
            atmosphere = new Atmosphere(root.query.results.channel.atmosphere);
            astronomy = new Astronomy(root.query.results.channel.astronomy);
            ttl = root.query.results.channel.ttl;

            source = Settings.API_Yahoo;
        }

        public Weather(WeatherUnderground.Rootobject root)
        {
            location = new Location(root.current_observation);
            update_time = DateTime.Parse(root.current_observation.local_time_rfc822);
            forecast = new Forecast[root.forecast.simpleforecast.forecastday.Length];
            for (int i = 0; i < forecast.Length; i++)
            {
                forecast[i] = new Forecast(root.forecast.simpleforecast.forecastday[i]);
            }
            hr_forecast = new HourlyForecast[root.hourly_forecast.Length];
            for (int i = 0; i < hr_forecast.Length; i++)
            {
                hr_forecast[i] = new HourlyForecast(root.hourly_forecast[i]);
            }
            txt_forecast = new TextForecast[root.forecast.txt_forecast.forecastday.Length];
            for (int i = 0; i < txt_forecast.Length; i++)
            {
                txt_forecast[i] = new TextForecast(root.forecast.txt_forecast.forecastday[i]);
            }
            condition = new Condition(root.current_observation);
            atmosphere = new Atmosphere(root.current_observation);
            astronomy = new Astronomy(root.sun_phase);
            precipitation = new Precipitation(root.forecast.simpleforecast.forecastday[0]);
            ttl = "60";

            source = Settings.API_WUnderground;
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
                        case "location":
                            obj.location = Location.FromJson(reader);
                            break;
                        case "update_time":
                            obj.update_time = DateTime.Parse(reader.Value.ToString());
                            break;
                        case "forecast":
                            System.Collections.Generic.List<Forecast> forecasts = new System.Collections.Generic.List<Forecast>();
                            while(reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                if (reader.TokenType == JsonToken.String)
                                    forecasts.Add(Forecast.FromJson(reader));
                            }
                            obj.forecast = forecasts.ToArray();
                            break;
                        case "hr_forecast":
                            System.Collections.Generic.List<HourlyForecast> hr_forecasts = new System.Collections.Generic.List<HourlyForecast>();
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                if (reader.TokenType == JsonToken.String)
                                    hr_forecasts.Add(HourlyForecast.FromJson(reader));
                            }
                            obj.hr_forecast = hr_forecasts.ToArray();
                            break;
                        case "txt_forecast":
                            System.Collections.Generic.List<TextForecast> txt_forecasts = new System.Collections.Generic.List<TextForecast>();
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                if (reader.TokenType == JsonToken.String)
                                    txt_forecasts.Add(TextForecast.FromJson(reader));
                            }
                            obj.txt_forecast = txt_forecasts.ToArray();
                            break;
                        case "condition":
                            obj.condition = Condition.FromJson(reader);
                            break;
                        case "atmosphere":
                            obj.atmosphere = Atmosphere.FromJson(reader);
                            break;
                        case "astronomy":
                            obj.astronomy = Astronomy.FromJson(reader);
                            break;
                        case "precipitation":
                            obj.precipitation = Precipitation.FromJson(reader);
                            break;
                        case "ttl":
                            obj.ttl = reader.Value.ToString();
                            break;
                        case "source":
                            obj.source = reader.Value.ToString();
                            break;
                        case "query":
                            obj.query = reader.Value.ToString();
                            break;
                        case "locale":
                            obj.locale = reader.Value.ToString();
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

            // "location" : ""
            writer.WritePropertyName("location");
            writer.WriteValue(location.ToJson());

            // "update_time" : ""
            writer.WritePropertyName("update_time");
            writer.WriteValue(update_time);

            // "forecast" : ""
            writer.WritePropertyName("forecast");
            writer.WriteStartArray();
            foreach (Forecast cast in forecast)
            {
                writer.WriteValue(cast.ToJson());
            }
            writer.WriteEndArray();

            // "hr_forecast" : ""
            writer.WritePropertyName("hr_forecast");
            writer.WriteStartArray();
            foreach (HourlyForecast hr_cast in hr_forecast)
            {
                writer.WriteValue(hr_cast.ToJson());
            }
            writer.WriteEndArray();

            // "txt_forecast" : ""
            writer.WritePropertyName("txt_forecast");
            writer.WriteStartArray();
            foreach (TextForecast txt_cast in txt_forecast)
            {
                writer.WriteValue(txt_cast.ToJson());
            }
            writer.WriteEndArray();

            // "condition" : ""
            writer.WritePropertyName("condition");
            writer.WriteValue(condition.ToJson());

            // "atmosphere" : ""
            writer.WritePropertyName("atmosphere");
            writer.WriteValue(atmosphere.ToJson());

            // "astronomy" : ""
            writer.WritePropertyName("astronomy");
            writer.WriteValue(astronomy.ToJson());

            // "precipitation" : ""
            writer.WritePropertyName("precipitation");
            writer.WriteValue(precipitation.ToJson());

            // "ttl" : ""
            writer.WritePropertyName("ttl");
            writer.WriteValue(ttl);

            // "source" : ""
            writer.WritePropertyName("source");
            writer.WriteValue(source);

            // "query" : ""
            writer.WritePropertyName("query");
            writer.WriteValue(query);

            // "locale" : ""
            writer.WritePropertyName("locale");
            writer.WriteValue(locale);

            // }
            writer.WriteEndObject();

            return sw.ToString();
        }

        public bool IsValid()
        {
            if (location == null || (forecast == null || forecast.Length == 0)
                || condition == null || atmosphere == null || astronomy == null)
                return false;
            else
                return true;
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public class Location
    {
        public string name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public TimeSpan tz_offset { get; set; }

        [JsonConstructor]
        private Location()
        {
            // Needed for deserialization
        }

        public Location(WeatherUnderground.Current_Observation condition)
        {
            name = condition.display_location.full;
            latitude = condition.display_location.latitude;
            longitude = condition.display_location.longitude;
            if (condition.local_tz_offset.StartsWith("-"))
                tz_offset = -TimeSpan.ParseExact(condition.local_tz_offset, "\\-hhmm", null);
            else
                tz_offset = TimeSpan.ParseExact(condition.local_tz_offset, "\\+hhmm", null);
        }

        public Location(WeatherYahoo.Query query)
        {
            name = query.results.channel.location.city + "," + query.results.channel.location.region;
            latitude = query.results.channel.item.lat;
            longitude = query.results.channel.item._long;
            saveTimeZone(query);
        }

        private void saveTimeZone(WeatherYahoo.Query query)
        {
            /* Get TimeZone info by using UTC and local build time */
            // Now
            DateTime utc = DateTime.ParseExact(query.created,
                            "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture);

            // There
            int AMPMidx = query.results.channel.lastBuildDate.LastIndexOf(" AM ");
            if (AMPMidx < 0)
                AMPMidx = query.results.channel.lastBuildDate.LastIndexOf(" PM ");

            DateTime there = DateTime.ParseExact(query.results.channel.lastBuildDate.Substring(0, AMPMidx + 4),
                "ddd, dd MMM yyyy hh:mm tt ", null);
            TimeSpan offset = there - utc;

            tz_offset = TimeSpan.Parse(string.Format("{0}:{1}", offset.Hours, offset.Minutes));
        }

        public static Location FromJson(JsonReader extReader)
        {
            Location obj = null;

            try
            {
                obj = new Location();
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
                        case "name":
                            obj.name = reader.Value.ToString();
                            break;
                        case "latitude":
                            obj.latitude = reader.Value.ToString();
                            break;
                        case "longitude":
                            obj.longitude = reader.Value.ToString();
                            break;
                        case "tz_offset":
                            obj.tz_offset = TimeSpan.Parse(reader.Value.ToString());
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

            // "name" : ""
            writer.WritePropertyName("name");
            writer.WriteValue(name);

            // "latitude" : ""
            writer.WritePropertyName("latitude");
            writer.WriteValue(latitude);

            // "longitude" : ""
            writer.WritePropertyName("longitude");
            writer.WriteValue(longitude);

            // "tz_offset" : ""
            writer.WritePropertyName("tz_offset");
            writer.WriteValue(tz_offset);

            // }
            writer.WriteEndObject();

            return sw.ToString();
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public class Forecast
    {
        public DateTime date { get; set; }
        public string high_f { get; set; }
        public string high_c { get; set; }
        public string low_f { get; set; }
        public string low_c { get; set; }
        public string condition { get; set; }
        public string icon { get; set; }

        [JsonConstructor]
        private Forecast()
        {
            // Needed for deserialization
        }

        public Forecast(WeatherYahoo.Forecast forecast)
        {
            date = DateTime.Parse(forecast.date, 
                CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            high_f = forecast.high;
            high_c = ConversionMethods.FtoC(high_f);
            low_f = forecast.low;
            low_c = ConversionMethods.FtoC(low_f);
            condition = forecast.text;
            icon = forecast.code;
        }

        public Forecast(WeatherUnderground.Forecastday1 forecast)
        {
            date = ConversionMethods.ToEpochDateTime(forecast.date.epoch).ToLocalTime();
            high_f = forecast.high.fahrenheit;
            high_c = forecast.high.celsius;
            low_f = forecast.low.fahrenheit;
            low_c = forecast.low.celsius;
            condition = forecast.conditions;
            icon = forecast.icon_url.Replace("http://icons.wxug.com/i/c/k/", "").Replace(".gif", "");
        }

        public static Forecast FromJson(JsonReader extReader)
        {
            Forecast obj = null;

            try
            {
                obj = new Forecast();
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
                        case "date":
                            obj.date = DateTime.Parse(reader.Value.ToString());
                            break;
                        case "high_f":
                            obj.high_f = reader.Value.ToString();
                            break;
                        case "high_c":
                            obj.high_c = reader.Value.ToString();
                            break;
                        case "low_f":
                            obj.low_f = reader.Value.ToString();
                            break;
                        case "low_c":
                            obj.low_c = reader.Value.ToString();
                            break;
                        case "condition":
                            obj.condition = reader.Value.ToString();
                            break;
                        case "icon":
                            obj.icon = reader.Value.ToString();
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

            // "date" : ""
            writer.WritePropertyName("date");
            writer.WriteValue(date);

            // "high_f" : ""
            writer.WritePropertyName("high_f");
            writer.WriteValue(high_f);

            // "high_c" : ""
            writer.WritePropertyName("high_c");
            writer.WriteValue(high_c);

            // "low_f" : ""
            writer.WritePropertyName("low_f");
            writer.WriteValue(low_f);

            // "low_c" : ""
            writer.WritePropertyName("low_c");
            writer.WriteValue(low_c);

            // "condition" : ""
            writer.WritePropertyName("condition");
            writer.WriteValue(condition);

            // "icon" : ""
            writer.WritePropertyName("icon");
            writer.WriteValue(icon);

            // }
            writer.WriteEndObject();

            return sw.ToString();
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public class HourlyForecast
    {
        public DateTime date { get; set; }
        public string high_f { get; set; }
        public string high_c { get; set; }
        public string condition { get; set; }
        public string icon { get; set; }
        public string pop { get; set; }
        public int wind_degrees { get; set; }
        public float wind_mph { get; set; }
        public float wind_kph { get; set; }

        [JsonConstructor]
        private HourlyForecast()
        {
            // Needed for deserialization
        }

        public HourlyForecast(WeatherUnderground.Hourly_Forecast hr_forecast)
        {
            date = ConversionMethods.ToEpochDateTime(hr_forecast.FCTTIME.epoch).ToLocalTime();
            high_f = hr_forecast.temp.english;
            high_c = hr_forecast.temp.metric;
            condition = hr_forecast.condition;
            icon = hr_forecast.icon_url.Replace("http://icons.wxug.com/i/c/k/", "").Replace(".gif", "");
            pop = hr_forecast.pop;
            wind_degrees = int.Parse(hr_forecast.wdir.degrees);
            wind_mph = float.Parse(hr_forecast.wspd.english);
            wind_kph = float.Parse(hr_forecast.wspd.metric);
        }

        public static HourlyForecast FromJson(JsonReader extReader)
        {
            HourlyForecast obj = null;

            try
            {
                obj = new HourlyForecast();
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
                        case "date":
                            obj.date = DateTime.Parse(reader.Value.ToString());
                            break;
                        case "high_f":
                            obj.high_f = reader.Value.ToString();
                            break;
                        case "high_c":
                            obj.high_c = reader.Value.ToString();
                            break;
                        case "condition":
                            obj.condition = reader.Value.ToString();
                            break;
                        case "icon":
                            obj.icon = reader.Value.ToString();
                            break;
                        case "pop":
                            obj.pop = reader.Value.ToString();
                            break;
                        case "wind_degrees":
                            obj.wind_degrees = int.Parse(reader.Value.ToString());
                            break;
                        case "wind_mph":
                            obj.wind_mph = float.Parse(reader.Value.ToString());
                            break;
                        case "wind_kph":
                            obj.wind_kph = float.Parse(reader.Value.ToString());
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

            // "date" : ""
            writer.WritePropertyName("date");
            writer.WriteValue(date);

            // "high_f" : ""
            writer.WritePropertyName("high_f");
            writer.WriteValue(high_f);

            // "high_c" : ""
            writer.WritePropertyName("high_c");
            writer.WriteValue(high_c);

            // "condition" : ""
            writer.WritePropertyName("condition");
            writer.WriteValue(condition);

            // "icon" : ""
            writer.WritePropertyName("icon");
            writer.WriteValue(icon);

            // "pop" : ""
            writer.WritePropertyName("pop");
            writer.WriteValue(pop);

            // "wind_degrees" : ""
            writer.WritePropertyName("wind_degrees");
            writer.WriteValue(wind_degrees);

            // "wind_mph" : ""
            writer.WritePropertyName("wind_mph");
            writer.WriteValue(wind_mph);

            // "wind_kph" : ""
            writer.WritePropertyName("wind_kph");
            writer.WriteValue(wind_kph);

            // }
            writer.WriteEndObject();

            return sw.ToString();
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public class TextForecast
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

        public TextForecast(WeatherUnderground.Forecastday txt_forecast)
        {
            title = txt_forecast.title;
            fcttext = txt_forecast.fcttext;
            fcttext_metric = txt_forecast.fcttext_metric;
            icon = txt_forecast.icon_url.Replace("http://icons.wxug.com/i/c/k/", "").Replace(".gif", "");
            pop = txt_forecast.pop;
        }

        public static TextForecast FromJson(JsonReader extReader)
        {
            TextForecast obj = null;

            try
            {
                obj = new TextForecast();
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
                        case "title":
                            obj.title = reader.Value.ToString();
                            break;
                        case "fcttext":
                            obj.fcttext = reader.Value.ToString();
                            break;
                        case "fcttext_metric":
                            obj.fcttext_metric = reader.Value.ToString();
                            break;
                        case "icon":
                            obj.icon = reader.Value.ToString();
                            break;
                        case "pop":
                            obj.pop = reader.Value.ToString();
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

            // "title" : ""
            writer.WritePropertyName("title");
            writer.WriteValue(title);

            // "fcttext" : ""
            writer.WritePropertyName("fcttext");
            writer.WriteValue(fcttext);

            // "fcttext_metric" : ""
            writer.WritePropertyName("fcttext_metric");
            writer.WriteValue(fcttext_metric);

            // "icon" : ""
            writer.WritePropertyName("icon");
            writer.WriteValue(icon);

            // "pop" : ""
            writer.WritePropertyName("pop");
            writer.WriteValue(pop);

            // }
            writer.WriteEndObject();

            return sw.ToString();
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public class Condition
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

        [JsonConstructor]
        private Condition()
        {
            // Needed for deserialization
        }

        public Condition(WeatherUnderground.Current_Observation condition)
        {
            weather = condition.weather;
            temp_f = condition.temp_f;
            temp_c = condition.temp_c;
            wind_degrees = condition.wind_degrees;
            wind_mph = condition.wind_mph;
            wind_kph = condition.wind_kph;
            feelslike_f = condition.feelslike_f;
            feelslike_c = condition.feelslike_c;
            icon = condition.icon_url.Replace("http://icons.wxug.com/i/c/k/", "").Replace(".gif", "");
        }

        public Condition(WeatherYahoo.Channel channel)
        {
            weather = channel.item.condition.text;
            temp_f = float.Parse(channel.item.condition.temp);
            temp_c = float.Parse(ConversionMethods.FtoC(channel.item.condition.temp));
            wind_degrees = int.Parse(channel.wind.direction);
            wind_kph = float.Parse(channel.wind.speed);
            wind_mph = float.Parse(ConversionMethods.KphToMph(channel.wind.speed));
            feelslike_f = float.Parse(channel.wind.chill);
            feelslike_c = float.Parse(ConversionMethods.FtoC(channel.wind.chill));
            icon = channel.item.condition.code;
        }

        public static Condition FromJson(JsonReader extReader)
        {
            Condition obj = null;

            try
            {
                obj = new Condition();
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
                        case "weather":
                            obj.weather = reader.Value.ToString();
                            break;
                        case "temp_f":
                            obj.temp_f = float.Parse(reader.Value.ToString());
                            break;
                        case "temp_c":
                            obj.temp_c = float.Parse(reader.Value.ToString());
                            break;
                        case "wind_degrees":
                            obj.wind_degrees = int.Parse(reader.Value.ToString());
                            break;
                        case "wind_mph":
                            obj.wind_mph = float.Parse(reader.Value.ToString());
                            break;
                        case "wind_kph":
                            obj.wind_kph = float.Parse(reader.Value.ToString());
                            break;
                        case "feelslike_f":
                            obj.feelslike_f = float.Parse(reader.Value.ToString());
                            break;
                        case "feelslike_c":
                            obj.feelslike_c = float.Parse(reader.Value.ToString());
                            break;
                        case "icon":
                            obj.icon = reader.Value.ToString();
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

            // "weather" : ""
            writer.WritePropertyName("weather");
            writer.WriteValue(weather);

            // "temp_f" : ""
            writer.WritePropertyName("temp_f");
            writer.WriteValue(temp_f);

            // "temp_c" : ""
            writer.WritePropertyName("temp_c");
            writer.WriteValue(temp_c);

            // "wind_degrees" : ""
            writer.WritePropertyName("wind_degrees");
            writer.WriteValue(wind_degrees);

            // "wind_mph" : ""
            writer.WritePropertyName("wind_mph");
            writer.WriteValue(wind_mph);

            // "wind_kph" : ""
            writer.WritePropertyName("wind_kph");
            writer.WriteValue(wind_kph);

            // "feelslike_f" : ""
            writer.WritePropertyName("feelslike_f");
            writer.WriteValue(feelslike_f);

            // "feelslike_c" : ""
            writer.WritePropertyName("feelslike_c");
            writer.WriteValue(feelslike_c);

            // "icon" : ""
            writer.WritePropertyName("icon");
            writer.WriteValue(icon);

            // }
            writer.WriteEndObject();

            return sw.ToString();
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public class Atmosphere
    {
        public string humidity { get; set; }
        public string pressure_mb { get; set; }
        public string pressure_in { get; set; }
        public string pressure_trend { get; set; }
        public string visibility_mi { get; set; }
        public string visibility_km { get; set; }

        [JsonConstructor]
        private Atmosphere()
        {
            // Needed for deserialization
        }

        public Atmosphere(WeatherUnderground.Current_Observation condition)
        {
            humidity = condition.relative_humidity;
            pressure_mb = condition.pressure_mb;
            pressure_in = condition.pressure_in;
            pressure_trend = condition.pressure_trend;
            visibility_mi = condition.visibility_mi;
            visibility_km = condition.visibility_km;
        }

        public Atmosphere(WeatherYahoo.Atmosphere atmosphere)
        {
            humidity = atmosphere.humidity;
            pressure_mb = atmosphere.pressure;
            pressure_in = ConversionMethods.MBToInHg(pressure_mb);
            pressure_trend = atmosphere.rising;
            visibility_km = atmosphere.visibility;
            visibility_mi = ConversionMethods.KmToMi(visibility_km);
        }

        public static Atmosphere FromJson(JsonReader extReader)
        {
            Atmosphere obj = null;

            try
            {
                obj = new Atmosphere();
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
                        case "humidity":
                            obj.humidity = reader.Value.ToString();
                            break;
                        case "pressure_mb":
                            obj.pressure_mb = reader.Value.ToString();
                            break;
                        case "pressure_in":
                            obj.pressure_in = reader.Value.ToString();
                            break;
                        case "pressure_trend":
                            obj.pressure_trend = reader.Value.ToString();
                            break;
                        case "visibility_mi":
                            obj.visibility_mi = reader.Value.ToString();
                            break;
                        case "visibility_km":
                            obj.visibility_km = reader.Value.ToString();
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

            // "humidity" : ""
            writer.WritePropertyName("humidity");
            writer.WriteValue(humidity);

            // "pressure_mb" : ""
            writer.WritePropertyName("pressure_mb");
            writer.WriteValue(pressure_mb);

            // "pressure_in" : ""
            writer.WritePropertyName("pressure_in");
            writer.WriteValue(pressure_in);

            // "pressure_trend" : ""
            writer.WritePropertyName("pressure_trend");
            writer.WriteValue(pressure_trend);

            // "visibility_mi" : ""
            writer.WritePropertyName("visibility_mi");
            writer.WriteValue(visibility_mi);

            // "visibility_km" : ""
            writer.WritePropertyName("visibility_km");
            writer.WriteValue(visibility_km);

            // }
            writer.WriteEndObject();

            return sw.ToString();
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public class Astronomy
    {
        public DateTime sunrise { get; set; }
        public DateTime sunset { get; set; }

        [JsonConstructor]
        private Astronomy()
        {
            // Needed for deserialization
        }

        public Astronomy(WeatherUnderground.Sun_Phase sun_phase)
        {
            sunset = DateTime.Parse(string.Format("{0}:{1}",
                sun_phase.sunset.hour, sun_phase.sunset.minute));
            sunrise = DateTime.Parse(string.Format("{0}:{1}",
                sun_phase.sunrise.hour, sun_phase.sunrise.minute));
        }

        public Astronomy(WeatherYahoo.Astronomy astronomy)
        {
            sunrise = DateTime.Parse(astronomy.sunrise);
            sunset = DateTime.Parse(astronomy.sunset);
        }

        public static Astronomy FromJson(JsonReader extReader)
        {
            Astronomy obj = null;

            try
            {
                obj = new Astronomy();
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
                        case "sunrise":
                            obj.sunrise = DateTime.Parse(reader.Value.ToString());
                            break;
                        case "sunset":
                            obj.sunset = DateTime.Parse(reader.Value.ToString());
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

            // "sunrise" : ""
            writer.WritePropertyName("sunrise");
            writer.WriteValue(sunrise);

            // "sunset" : ""
            writer.WritePropertyName("sunset");
            writer.WriteValue(sunset);

            // }
            writer.WriteEndObject();

            return sw.ToString();
        }
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    public class Precipitation
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

        public Precipitation(WeatherUnderground.Forecastday1 forecast)
        {
            pop = forecast.pop.ToString();
            qpf_rain_in = forecast.qpf_allday._in.GetValueOrDefault(0.00f);
            qpf_rain_mm = forecast.qpf_allday.mm.GetValueOrDefault();
            qpf_snow_in = forecast.snow_allday._in.GetValueOrDefault(0.00f);
            qpf_snow_cm = forecast.snow_allday.cm.GetValueOrDefault();
        }

        public static Precipitation FromJson(JsonReader extReader)
        {
            Precipitation obj = null;

            try
            {
                obj = new Precipitation();
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
                        case "pop":
                            obj.pop = reader.Value.ToString();
                            break;
                        case "qpf_rain_in":
                            obj.qpf_rain_in = float.Parse(reader.Value.ToString());
                            break;
                        case "qpf_rain_mm":
                            obj.qpf_rain_mm = float.Parse(reader.Value.ToString());
                            break;
                        case "qpf_snow_in":
                            obj.qpf_snow_in = float.Parse(reader.Value.ToString());
                            break;
                        case "qpf_snow_cm":
                            obj.qpf_snow_cm = float.Parse(reader.Value.ToString());
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

            // "pop" : ""
            writer.WritePropertyName("pop");
            writer.WriteValue(pop);

            // "qpf_rain_in" : ""
            writer.WritePropertyName("qpf_rain_in");
            writer.WriteValue(qpf_rain_in);

            // "qpf_rain_mm" : ""
            writer.WritePropertyName("qpf_rain_mm");
            writer.WriteValue(qpf_rain_mm);

            // "qpf_snow_in" : ""
            writer.WritePropertyName("qpf_snow_in");
            writer.WriteValue(qpf_snow_in);

            // "qpf_snow_cm" : ""
            writer.WritePropertyName("qpf_snow_cm");
            writer.WriteValue(qpf_snow_cm);

            // }
            writer.WriteEndObject();

            return sw.ToString();
        }
    }
}