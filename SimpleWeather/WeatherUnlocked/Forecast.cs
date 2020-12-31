using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.WeatherUnlocked
{
    public class ForecastRootobject
    {
        public Day[] Days { get; set; }
    }

    public class Day
    {
        public string date { get; set; }
        public string sunrise_time { get; set; }
        public string sunset_time { get; set; }
        public string moonrise_time { get; set; }
        public string moonset_time { get; set; }
        public float temp_max_c { get; set; }
        public float temp_max_f { get; set; }
        public float temp_min_c { get; set; }
        public float temp_min_f { get; set; }
        public float precip_total_mm { get; set; }
        public float precip_total_in { get; set; }
        public float rain_total_mm { get; set; }
        public float rain_total_in { get; set; }
        public float snow_total_mm { get; set; }
        public float snow_total_in { get; set; }
        public float prob_precip_pct { get; set; }
        public float humid_max_pct { get; set; }
        public float humid_min_pct { get; set; }
        public float windspd_max_mph { get; set; }
        public float windspd_max_kmh { get; set; }
        public float windspd_max_kts { get; set; }
        public float windspd_max_ms { get; set; }
        public float windgst_max_mph { get; set; }
        public float windgst_max_kmh { get; set; }
        public float windgst_max_kts { get; set; }
        public float windgst_max_ms { get; set; }
        public float slp_max_in { get; set; }
        public float slp_max_mb { get; set; }
        public float slp_min_in { get; set; }
        public float slp_min_mb { get; set; }
        public Timeframe[] Timeframes { get; set; }
    }

    public class Timeframe
    {
        public string date { get; set; }
        public int time { get; set; }
        public string utcdate { get; set; }
        public int utctime { get; set; }
        public string wx_desc { get; set; }
        public int wx_code { get; set; }
        public string wx_icon { get; set; }
        public float temp_c { get; set; }
        public float temp_f { get; set; }
        public float feelslike_c { get; set; }
        public float feelslike_f { get; set; }
        public float winddir_deg { get; set; }
        public string winddir_compass { get; set; }
        public float windspd_mph { get; set; }
        public float windspd_kmh { get; set; }
        public float windspd_kts { get; set; }
        public float windspd_ms { get; set; }
        public float windgst_mph { get; set; }
        public float windgst_kmh { get; set; }
        public float windgst_kts { get; set; }
        public float windgst_ms { get; set; }
        public float cloud_low_pct { get; set; }
        public float cloud_mid_pct { get; set; }
        public float cloud_high_pct { get; set; }
        public float cloudtotal_pct { get; set; }
        public float precip_mm { get; set; }
        public float precip_in { get; set; }
        public float rain_mm { get; set; }
        public float rain_in { get; set; }
        public float snow_mm { get; set; }
        public float snow_in { get; set; }
        public float snow_accum_cm { get; set; }
        public float snow_accum_in { get; set; }
        public string prob_precip_pct { get; set; }
        public float humid_pct { get; set; }
        public float dewpoint_c { get; set; }
        public float dewpoint_f { get; set; }
        public float vis_km { get; set; }
        public float vis_mi { get; set; }
        public float slp_mb { get; set; }
        public float slp_in { get; set; }
    }

}
