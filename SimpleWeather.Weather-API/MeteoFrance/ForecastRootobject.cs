using System;

namespace SimpleWeather.Weather_API.MeteoFrance
{
    public class ForecastRootobject
    {
        public DateTimeOffset? update_time { get; set; }
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public ForecastProperties properties { get; set; }
    }

    public class ForecastProperties
    {
        public int? altitude { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public string french_department { get; set; }
        public int? rain_product_available { get; set; }
        public string timezone { get; set; }
        public string insee { get; set; }
        public int? bulletin_cote { get; set; }
        public Daily_Forecast[] daily_forecast { get; set; }
        public Forecast[] forecast { get; set; }
        public Probability_Forecast[] probability_forecast { get; set; }
    }

    public class Daily_Forecast
    {
        public DateTimeOffset? time { get; set; }
        public float? T_min { get; set; }
        public float? T_max { get; set; }
        public float? T_sea { get; set; }
        public int? relative_humidity_min { get; set; }
        public int? relative_humidity_max { get; set; }
        public float? total_precipitation_24h { get; set; }
        public float? uv_index { get; set; }
        public string daily_weather_icon { get; set; }
        public string daily_weather_description { get; set; }
        public DateTimeOffset? sunrise_time { get; set; }
        public DateTimeOffset? sunset_time { get; set; }
    }

    public class Forecast
    {
        public DateTimeOffset? time { get; set; }
        public float? T { get; set; }
        public float? T_windchill { get; set; }
        public int? relative_humidity { get; set; }
        public float? P_sea { get; set; }
        public float? wind_speed { get; set; }
        public float? wind_speed_gust { get; set; }
        public int? wind_direction { get; set; }
        public string wind_icon { get; set; }
        public float? rain_1h { get; set; }
        public float? rain_3h { get; set; }
        public float? rain_6h { get; set; }
        public float? rain_12h { get; set; }
        public float? rain_24h { get; set; }
        public float? snow_1h { get; set; }
        public float? snow_3h { get; set; }
        public float? snow_6h { get; set; }
        public float? snow_12h { get; set; }
        public float? snow_24h { get; set; }
        public int? iso0 { get; set; }
        public string rain_snow_limit { get; set; }
        public int? total_cloud_cover { get; set; }
        public string weather_icon { get; set; }
        public string weather_description { get; set; }
        public int? weather_confidence_index { get; set; }
    }

    public class Probability_Forecast
    {
        public DateTimeOffset? time { get; set; }
        public float? rain_hazard_3h { get; set; }
        public float? rain_hazard_6h { get; set; }
        public float? snow_hazard_3h { get; set; }
        public float? snow_hazard_6h { get; set; }
        public int? freezing_hazard { get; set; }
        public int? storm_hazard { get; set; }
    }
}