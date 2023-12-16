using System;

namespace SimpleWeather.Weather_API.WeatherBit
{
    public class HourlyRootobject
    {
        public string city_name { get; set; }
        public string state_code { get; set; }
        public string country_code { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public string timezone { get; set; }
        public HourlyDatum[] data { get; set; }
    }

    public class HourlyDatum
    {
        public long ts { get; set; }
        public DateTime timestamp_local { get; set; }
        public DateTime timestamp_utc { get; set; }
        public string datetime { get; set; }
        public float snow { get; set; }
        public float snow_depth { get; set; }
        public float precip { get; set; }
        public float temp { get; set; }
        public float dewpt { get; set; }
        public float app_temp { get; set; }
        public int rh { get; set; }
        public int clouds { get; set; }
        public ForecastWeather weather { get; set; }
        public float slp { get; set; }
        public float pres { get; set; }
        public float uv { get; set; }
        public float solar_rad { get; set; }
        public float ghi { get; set; }
        public float dhi { get; set; }
        public int dni { get; set; }
        public float vis { get; set; }
        public string pod { get; set; }
        public int pop { get; set; }
        public float wind_spd { get; set; }
        public float wind_gust_spd { get; set; }
        public int wind_dir { get; set; }
        public string wind_cdir { get; set; }
        public string wind_cdir_full { get; set; }
    }

    /*
    public class Weather
    {
        public string icon { get; set; }
        public int code { get; set; }
        public string description { get; set; }
    }
    */
}
