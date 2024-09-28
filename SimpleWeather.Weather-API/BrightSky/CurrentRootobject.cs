using System;

namespace SimpleWeather.Weather_API.BrightSky
{
    public class CurrentRootobject
    {
        public CurrentWeather weather { get; set; }
        public Source[] sources { get; set; }
    }

    public class CurrentWeather
    {
        public int source_id { get; set; }
        public DateTimeOffset timestamp { get; set; }
        public float? cloud_cover { get; set; }
        public string condition { get; set; }
        public float? dew_point { get; set; }
        public float? solar_10 { get; set; }
        public float? solar_30 { get; set; }
        public float? solar_60 { get; set; }
        public float? precipitation_10 { get; set; }
        public float? precipitation_30 { get; set; }
        public float? precipitation_60 { get; set; }
        public float? pressure_msl { get; set; }
        public int? relative_humidity { get; set; }
        public int? visibility { get; set; }
        public int? wind_direction_10 { get; set; }
        public int? wind_direction_30 { get; set; }
        public int? wind_direction_60 { get; set; }
        public float? wind_speed_10 { get; set; }
        public float? wind_speed_30 { get; set; }
        public float? wind_speed_60 { get; set; }
        public int? wind_gust_direction_10 { get; set; }
        public int? wind_gust_direction_30 { get; set; }
        public int? wind_gust_direction_60 { get; set; }
        public float? wind_gust_speed_10 { get; set; }
        public float? wind_gust_speed_30 { get; set; }
        public float? wind_gust_speed_60 { get; set; }
        public float? sunshine_30 { get; set; }
        public float? sunshine_60 { get; set; }
        public float? temperature { get; set; }
        //public Fallback_Source_Ids fallback_source_ids { get; set; }
        public string icon { get; set; }
    }

    /*
    public class Fallback_Source_Ids
    {
    }
    */

    public partial class Source
    {
        public int id { get; set; }
        public string dwd_station_id { get; set; }
        public string observation_type { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public float height { get; set; }
        public string station_name { get; set; }
        public string wmo_station_id { get; set; }
        public DateTimeOffset? first_record { get; set; }
        public DateTimeOffset? last_record { get; set; }
        public float distance { get; set; }
    }
}
