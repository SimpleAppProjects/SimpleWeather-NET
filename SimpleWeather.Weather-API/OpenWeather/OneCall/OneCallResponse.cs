using System.Runtime.Serialization;

namespace SimpleWeather.Weather_API.OpenWeather.OneCall
{
    public class Rootobject
    {
        public float lat { get; set; }
        public float lon { get; set; }
        public string timezone { get; set; }
        public int timezone_offset { get; set; }
        public Current current { get; set; }
        public Hourly[] hourly { get; set; }
        public Daily[] daily { get; set; }
        public Alert[] alerts { get; set; }
        public Minutely[] minutely { get; set; }
    }

    public class Current
    {
        public long dt { get; set; }
        public long sunrise { get; set; }
        public long sunset { get; set; }
        public float temp { get; set; }
        public float feels_like { get; set; }
        public float pressure { get; set; }
        public int humidity { get; set; }
        public float dew_point { get; set; }
        public int clouds { get; set; }
        public float uvi { get; set; }
        public int visibility { get; set; }
        public float wind_speed { get; set; }
        public float? wind_gust { get; set; }
        public int wind_deg { get; set; }
        public Rain rain { get; set; }
        public Snow snow { get; set; }
        public Weather[] weather { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Hourly
    {
        public long dt { get; set; }
        public float temp { get; set; }
        public float feels_like { get; set; }
        public float pressure { get; set; }
        public int humidity { get; set; }
        public float dew_point { get; set; }
        public int clouds { get; set; }
        public float? pop { get; set; }
        public float? uvi { get; set; }
        public int? visibility { get; set; }
        public float wind_speed { get; set; }
        public float? wind_gust { get; set; }
        public int wind_deg { get; set; }
        public Rain rain { get; set; }
        public Snow snow { get; set; }
        public Weather[] weather { get; set; }
    }

    public class Rain
    {
        [DataMember(Name = "1h")]
        public float _1h { get; set; }
    }

    public class Snow
    {
        [DataMember(Name = "1h")]
        public float _1h { get; set; }
    }

    public class Daily
    {
        public long dt { get; set; }
        public long sunrise { get; set; }
        public long sunset { get; set; }
        public long? moonrise { get; set; }
        public long? moonset { get; set; }
        public float? moon_phase { get; set; }
        public Temp temp { get; set; }
        public Feels_Like feels_like { get; set; }
        public float pressure { get; set; }
        public int humidity { get; set; }
        public float dew_point { get; set; }
        public float wind_speed { get; set; }
        public float? wind_gust { get; set; }
        public int wind_deg { get; set; }
        public int clouds { get; set; }
        public float? pop { get; set; }
        public float uvi { get; set; }
        public int? visibility { get; set; }
        public float? rain { get; set; }
        public float? snow { get; set; }
        public Weather[] weather { get; set; }
        public string summary { get; set; }
    }

    public class Temp
    {
        public float morn { get; set; }
        public float day { get; set; }
        public float eve { get; set; }
        public float night { get; set; }
        public float min { get; set; }
        public float max { get; set; }
    }

    public class Feels_Like
    {
        public float morn { get; set; }
        public float day { get; set; }
        public float eve { get; set; }
        public float night { get; set; }
    }

    public class Alert
    {
        public string sender_name { get; set; }
        [DataMember(Name = "event")]
        public string _event { get; set; }
        public long start { get; set; }
        public long end { get; set; }
        public string description { get; set; }
    }

    public class Minutely
    {
        public long dt { get; set; }
        public float precipitation { get; set; }
    }
}