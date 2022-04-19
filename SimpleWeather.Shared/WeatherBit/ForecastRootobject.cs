using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherBit
{
    public class ForecastRootobject
    {
        public ForecastDatum[] data { get; set; }
        public string city_name { get; set; }
        public float lon { get; set; }
        public string timezone { get; set; }
        public float lat { get; set; }
        public string country_code { get; set; }
        public string state_code { get; set; }
    }

    public class ForecastDatum
    {
        public int moonrise_ts { get; set; }
        public string wind_cdir { get; set; }
        public int rh { get; set; }
        public float pres { get; set; }
        public float high_temp { get; set; }
        public int sunset_ts { get; set; }
        public float ozone { get; set; }
        public float moon_phase { get; set; }
        public float wind_gust_spd { get; set; }
        public float snow_depth { get; set; }
        public int clouds { get; set; }
        public int ts { get; set; }
        public int sunrise_ts { get; set; }
        public float app_min_temp { get; set; }
        public float wind_spd { get; set; }
        public int pop { get; set; }
        public string wind_cdir_full { get; set; }
        public float slp { get; set; }
        public float moon_phase_lunation { get; set; }
        public string valid_date { get; set; }
        public float app_max_temp { get; set; }
        public float vis { get; set; }
        public float dewpt { get; set; }
        public float snow { get; set; }
        public float uv { get; set; }
        public ForecastWeather weather { get; set; }
        public int wind_dir { get; set; }
        //public object max_dhi { get; set; }
        public int clouds_hi { get; set; }
        public float precip { get; set; }
        public float low_temp { get; set; }
        public float max_temp { get; set; }
        public int moonset_ts { get; set; }
        public string datetime { get; set; }
        public float temp { get; set; }
        public float min_temp { get; set; }
        public int clouds_mid { get; set; }
        public int clouds_low { get; set; }
    }

    public class ForecastWeather
    {
        public string icon { get; set; }
        public int code { get; set; }
        public string description { get; set; }
    }

}
