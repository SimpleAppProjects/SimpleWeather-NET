using System;

namespace SimpleWeather.Weather_API.BrightSky
{
    public class ForecastRootobject
    {
        public ForecastWeather[] weather { get; set; }
        public Source[] sources { get; set; }
    }

    public class ForecastWeather
    {
        public DateTimeOffset timestamp { get; set; }
        public int source_id { get; set; }
        public float? precipitation { get; set; }
        public float? pressure_msl { get; set; }
        public float? sunshine { get; set; }
        public float? temperature { get; set; }
        public float? wind_direction { get; set; }
        public float? wind_speed { get; set; }
        public float? cloud_cover { get; set; }
        public float? dew_point { get; set; }
        public float? relative_humidity { get; set; }
        public float? visibility { get; set; }
        public float? wind_gust_direction { get; set; }
        public float? wind_gust_speed { get; set; }
        public string condition { get; set; }
        public int? precipitation_probability { get; set; }
        public int? precipitation_probability_6h { get; set; }
        public float? solar { get; set; }
        public string icon { get; set; }
    }
}
