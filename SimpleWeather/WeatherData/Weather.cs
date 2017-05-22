using SimpleWeather.Utils;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace SimpleWeather.WeatherData
{
    public class Weather
    {
        public Location location { get; set; }
        public DateTime update_time { get; set; }
        public Forecast[] forecast { get; set; }
        public Condition condition { get; set; }
        public Atmosphere atmosphere { get; set; }
        public Astronomy astronomy { get; set; }
        public string ttl { get; set; }

        [Newtonsoft.Json.JsonConstructor]
        private Weather()
        {
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
            condition = new Condition(root.current_observation);
            atmosphere = new Atmosphere(root.current_observation);
            astronomy = new Astronomy(root.sun_phase);
            ttl = "60";
        }
    }

    public class Location
    {
        public string name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public TimeSpan tz_offset { get; set; }

        [Newtonsoft.Json.JsonConstructor]
        private Location()
        {
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
    }

    public class Forecast
    {
        public DateTime date { get; set; }
        public string high_f { get; set; }
        public string high_c { get; set; }
        public string low_f { get; set; }
        public string low_c { get; set; }
        public string condition { get; set; }
        public string icon { get; set; }

        [Newtonsoft.Json.JsonConstructor]
        private Forecast()
        {
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
            date = ConversionMethods.ToEpochDateTime(forecast.date.epoch);
            high_f = forecast.high.fahrenheit;
            high_c = forecast.high.celsius;
            low_f = forecast.low.fahrenheit;
            low_c = forecast.low.celsius;
            condition = forecast.conditions;
            icon = forecast.icon_url.Replace("http://icons.wxug.com/i/c/k/", "").Replace(".gif", "");
        }
    }

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

        [Newtonsoft.Json.JsonConstructor]
        private Condition()
        {
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
            wind_mph = float.Parse(ConversionMethods.kphTomph(channel.wind.speed));
            feelslike_f = float.Parse(channel.wind.chill);
            feelslike_c = float.Parse(ConversionMethods.FtoC(channel.wind.chill));
            icon = channel.item.condition.code;
        }
    }

    public class Atmosphere
    {
        public string humidity { get; set; }
        public string pressure_mb { get; set; }
        public string pressure_in { get; set; }
        public string pressure_trend { get; set; }
        public string visibility_mi { get; set; }
        public string visibility_km { get; set; }

        [Newtonsoft.Json.JsonConstructor]
        private Atmosphere()
        {
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
            visibility_mi = ConversionMethods.kmToMi(visibility_km);
        }
    }

    public class Astronomy
    {
        public DateTime sunrise { get; set; }
        public DateTime sunset { get; set; }

        [Newtonsoft.Json.JsonConstructor]
        private Astronomy()
        {
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
    }
}