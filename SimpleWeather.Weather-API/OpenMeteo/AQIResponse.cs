namespace SimpleWeather.Weather_API.OpenMeteo
{
    public class AQIResponse
    {
        public float? latitude { get; set; }
        public float? longitude { get; set; }
        public float? generationtime_ms { get; set; }
        public int? utc_offset_seconds { get; set; }
        public string timezone { get; set; }
        public string timezone_abbreviation { get; set; }
        public float? elevation { get; set; }
        public CurrentAQIUnits current_units { get; set; }
        public CurrentAQI current { get; set; }
    }

    public class CurrentAQIUnits
    {
        public string time { get; set; }
        public string interval { get; set; }
        public string us_aqi { get; set; }
        public string us_aqi_pm10 { get; set; }
        public string us_aqi_nitrogen_dioxide { get; set; }
        public string us_aqi_pm2_5 { get; set; }
        public string us_aqi_carbon_monoxide { get; set; }
        public string us_aqi_ozone { get; set; }
        public string us_aqi_sulphur_dioxide { get; set; }
        public string alder_pollen { get; set; }
        public string birch_pollen { get; set; }
        public string grass_pollen { get; set; }
        public string ragweed_pollen { get; set; }
    }

    public class CurrentAQI
    {
        public long? time { get; set; }
        public int? interval { get; set; }
        public int? us_aqi { get; set; }
        public int? us_aqi_pm10 { get; set; }
        public int? us_aqi_nitrogen_dioxide { get; set; }
        public int? us_aqi_pm2_5 { get; set; }
        public int? us_aqi_carbon_monoxide { get; set; }
        public int? us_aqi_ozone { get; set; }
        public int? us_aqi_sulphur_dioxide { get; set; }
        public float? alder_pollen { get; set; }
        public float? birch_pollen { get; set; }
        public float? grass_pollen { get; set; }
        public float? ragweed_pollen { get; set; }
    }
}
