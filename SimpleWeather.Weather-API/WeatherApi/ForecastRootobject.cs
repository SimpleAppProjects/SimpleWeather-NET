using System;
using System.Runtime.Serialization;

namespace SimpleWeather.Weather_API.WeatherApi
{
    public class ForecastRootobject
    {
        public Location location { get; set; }
        public Current current { get; set; }
        public Forecast forecast { get; set; }
        public Alerts alerts { get; set; }
    }

    public class Location
    {
        public string name { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public string tz_id { get; set; }
        public long localtime_epoch { get; set; }
        public string localtime { get; set; }
    }

    public class Current
    {
        public long last_updated_epoch { get; set; }
        public string last_updated { get; set; }
        public float temp_c { get; set; }
        public float temp_f { get; set; }
        public int is_day { get; set; }
        public Condition condition { get; set; }
        public float wind_mph { get; set; }
        public float wind_kph { get; set; }
        public int wind_degree { get; set; }
        public string wind_dir { get; set; }
        public float pressure_mb { get; set; }
        public float pressure_in { get; set; }
        public float precip_mm { get; set; }
        public float precip_in { get; set; }
        public int humidity { get; set; }
        public int cloud { get; set; }
        public float feelslike_c { get; set; }
        public float feelslike_f { get; set; }
        public float vis_km { get; set; }
        public float vis_miles { get; set; }
        public float uv { get; set; }
        public float gust_mph { get; set; }
        public float gust_kph { get; set; }
        public Air_Quality air_quality { get; set; }
    }

    public class Condition
    {
        public string text { get; set; }
        public string icon { get; set; }
        public int? code { get; set; }
    }

    public class Air_Quality
    {
        public float? co { get; set; }
        public float? no2 { get; set; }
        public float? o3 { get; set; }
        public float? so2 { get; set; }
        public float? pm2_5 { get; set; }
        public float? pm10 { get; set; }
        public int? usepaindex { get; set; }
        public int? gbdefraindex { get; set; }
    }

    public class Forecast
    {
        public Forecastday[] forecastday { get; set; }
    }

    public class Forecastday
    {
        public string date { get; set; }
        public long date_epoch { get; set; }
        public Day day { get; set; }
        public Astro astro { get; set; }
        public Hour[] hour { get; set; }
    }

    public class Day
    {
        public float maxtemp_c { get; set; }
        public float maxtemp_f { get; set; }
        public float mintemp_c { get; set; }
        public float mintemp_f { get; set; }
        public float avgtemp_c { get; set; }
        public float avgtemp_f { get; set; }
        public float maxwind_mph { get; set; }
        public float maxwind_kph { get; set; }
        public float totalprecip_mm { get; set; }
        public float totalprecip_in { get; set; }
        public float avgvis_km { get; set; }
        public float avgvis_miles { get; set; }
        public float avghumidity { get; set; }
        public int? daily_will_it_rain { get; set; }
        public int? daily_chance_of_rain { get; set; }
        public int? daily_will_it_snow { get; set; }
        public int? daily_chance_of_snow { get; set; }
        public Condition condition { get; set; }
        public float uv { get; set; }
    }

    public class Astro
    {
        public string sunrise { get; set; }
        public string sunset { get; set; }
        public string moonrise { get; set; }
        public string moonset { get; set; }
        public string moon_phase { get; set; }
        public string moon_illumination { get; set; }
    }

    public class Hour
    {
        public long time_epoch { get; set; }
        public string time { get; set; }
        public float temp_c { get; set; }
        public float temp_f { get; set; }
        public int is_day { get; set; }
        public Condition condition { get; set; }
        public float wind_mph { get; set; }
        public float wind_kph { get; set; }
        public int wind_degree { get; set; }
        public string wind_dir { get; set; }
        public float pressure_mb { get; set; }
        public float pressure_in { get; set; }
        public float precip_mm { get; set; }
        public float precip_in { get; set; }
        public int humidity { get; set; }
        public int cloud { get; set; }
        public float feelslike_c { get; set; }
        public float feelslike_f { get; set; }
        public float windchill_c { get; set; }
        public float windchill_f { get; set; }
        public float heatindex_c { get; set; }
        public float heatindex_f { get; set; }
        public float dewpoint_c { get; set; }
        public float dewpoint_f { get; set; }
        public int will_it_rain { get; set; }
        public int? chance_of_rain { get; set; }
        public int will_it_snow { get; set; }
        public int? chance_of_snow { get; set; }
        public float vis_km { get; set; }
        public float vis_miles { get; set; }
        public float gust_mph { get; set; }
        public float gust_kph { get; set; }
        public float uv { get; set; }
    }

    public class Alerts
    {
        public Alert[] alert { get; set; }
    }

    public class Alert
    {
        public string headline { get; set; }
        public string msgtype { get; set; }
        public string severity { get; set; }
        public string urgency { get; set; }
        public string areas { get; set; }
        public string category { get; set; }
        public string certainty { get; set; }
        [DataMember(Name = "event")]
        public string _event { get; set; }
        public string note { get; set; }
        public DateTimeOffset effective { get; set; }
        public DateTimeOffset expires { get; set; }
        public string desc { get; set; }
        public string instruction { get; set; }
    }
}
