using System;

namespace SimpleWeather.Weather_API.MeteoFrance
{
    public class CurrentsRootobject
    {
        public DateTimeOffset? update_time { get; set; }
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public CurrentProperties properties { get; set; }
    }

    public class CurrentProperties
    {
        public string timezone { get; set; }
        public Gridded gridded { get; set; }
    }

    public class Gridded
    {
        public DateTimeOffset? time { get; set; }
        public float? T { get; set; }
        public float? wind_speed { get; set; }
        public float? wind_speed_gust { get; set; }
        public int? wind_direction { get; set; }
        public string wind_icon { get; set; }
        public string weather_icon { get; set; }
        public string weather_description { get; set; }
    }
}
