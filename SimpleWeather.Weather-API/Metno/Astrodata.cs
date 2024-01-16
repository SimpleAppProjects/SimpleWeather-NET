using System;

namespace SimpleWeather.Weather_API.Metno
{
    public class SunRootobject
    {
        public string copyright { get; set; }
        public string licenseURL { get; set; }
        public string type { get; set; }
        public Geometry geometry { get; set; }
        //public When when { get; set; }
        public SunProperties properties { get; set; }
    }

    public class MoonRootobject
    {
        public string copyright { get; set; }
        public string licenseURL { get; set; }
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public When when { get; set; }
        public MoonProperties properties { get; set; }
    }

    public class When
    {
        public DateTimeOffset[] interval { get; set; }
    }

    public class SunProperties
    {
        public string body { get; set; }
        public Sunrise sunrise { get; set; }
        public Sunset sunset { get; set; }
        public Solarnoon solarnoon { get; set; }
        public Solarmidnight solarmidnight { get; set; }
    }

    public class Sunrise
    {
        public string time { get; set; }
        public float azimuth { get; set; }
    }

    public class Sunset
    {
        public string time { get; set; }
        public float azimuth { get; set; }
    }

    public class Solarnoon
    {
        public string time { get; set; }
        public float disc_centre_elevation { get; set; }
        public bool visible { get; set; }
    }

    public class Solarmidnight
    {
        public string time { get; set; }
        public float disc_centre_elevation { get; set; }
        public bool visible { get; set; }
    }

    public class MoonProperties
    {
        public string body { get; set; }
        public Moonrise moonrise { get; set; }
        public Moonset moonset { get; set; }
        public High_Moon high_moon { get; set; }
        public Low_Moon low_moon { get; set; }
        public float moonphase { get; set; }
    }

    public class Moonrise
    {
        public string time { get; set; }
        public float azimuth { get; set; }
    }

    public class Moonset
    {
        public string time { get; set; }
        public float azimuth { get; set; }
    }

    public class High_Moon
    {
        public string time { get; set; }
        public float disc_centre_elevation { get; set; }
        public bool visible { get; set; }
    }

    public class Low_Moon
    {
        public string time { get; set; }
        public float disc_centre_elevation { get; set; }
        public bool visible { get; set; }
    }
}