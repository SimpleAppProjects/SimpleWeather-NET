using System;

namespace SimpleWeather.Weather_API.Metno
{
    public class AstroRootobject
    {
        public AstroMeta meta { get; set; }
        public Location location { get; set; }
    }

    public class AstroMeta
    {
        public string licenseurl { get; set; }
    }

    public class Location
    {
        public Time[] time { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string height { get; set; }
    }

    public class Time
    {
        public Low_Moon low_moon { get; set; }
        public High_Moon high_moon { get; set; }
        public Solarnoon solarnoon { get; set; }
        public Moonphase moonphase { get; set; }
        public Moonposition moonposition { get; set; }
        public Sunrise sunrise { get; set; }
        public string date { get; set; }
        public Moonshadow moonshadow { get; set; }
        public Moonrise moonrise { get; set; }
        public Solarmidnight solarmidnight { get; set; }
        public Sunset sunset { get; set; }
        public Moonset moonset { get; set; }
    }

    public class Low_Moon
    {
        public string elevation { get; set; }
        public DateTime time { get; set; }
        public string desc { get; set; }
    }

    public class High_Moon
    {
        public string elevation { get; set; }
        public DateTime time { get; set; }
        public string desc { get; set; }
    }

    public class Solarnoon
    {
        public string desc { get; set; }
        public DateTime time { get; set; }
        public string elevation { get; set; }
    }

    public class Moonphase
    {
        public string value { get; set; }
        public string desc { get; set; }
        public DateTime time { get; set; }
    }

    public class Moonposition
    {
        public string desc { get; set; }
        public string azimuth { get; set; }
        public string phase { get; set; }
        public DateTime time { get; set; }
        public string range { get; set; }
        public string elevation { get; set; }
    }

    public class Sunrise
    {
        public DateTime time { get; set; }
        public string desc { get; set; }
    }

    public class Moonshadow
    {
        public string elevation { get; set; }
        public DateTime time { get; set; }
        public string azimuth { get; set; }
        public string desc { get; set; }
    }

    public class Moonrise
    {
        public DateTime time { get; set; }
        public string desc { get; set; }
    }

    public class Solarmidnight
    {
        public DateTime time { get; set; }
        public string elevation { get; set; }
        public string desc { get; set; }
    }

    public class Sunset
    {
        public DateTime time { get; set; }
        public string desc { get; set; }
    }

    public class Moonset
    {
        public DateTime time { get; set; }
        public string desc { get; set; }
    }
}