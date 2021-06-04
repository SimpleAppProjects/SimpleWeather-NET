using System.Runtime.Serialization;

namespace SimpleWeather.OpenWeather
{
    public class CurrentRootobject
    {
        public Coord coord { get; set; }
        public Weather[] weather { get; set; }
        [DataMember(Name = "base")]
        public string _base { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public Rain rain { get; set; }
        public Snow snow { get; set; }
        public long dt { get; set; }
        public CurrentSys sys { get; set; }
        public int timezone { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Coord
    {
        public float lon { get; set; }
        public float lat { get; set; }
    }

    public class Wind
    {
        public float speed { get; set; }
        public float deg { get; set; }
        public float? gust { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Rain
    {
        [DataMember(Name = "1h")]
        public float? _1h { get; set; }
        [DataMember(Name = "3h")]
        public float? _3h { get; set; }
    }

    public class Snow
    {
        [DataMember(Name = "1h")]
        public float? _1h { get; set; }
        [DataMember(Name = "3h")]
        public float? _3h { get; set; }
    }

    public class CurrentSys
    {
        public string country { get; set; }
        public long? sunrise { get; set; }
        public long? sunset { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class ForecastRootobject // Forecast / Daily
    {
        public List[] list { get; set; }
        public City city { get; set; }
    }

    public class City
    {
        public int id { get; set; }
        public string name { get; set; }
        public Coord coord { get; set; }
        public string country { get; set; }
        public int? sunrise { get; set; }
        public int? sunset { get; set; }
    }

    public class List
    {
        public long dt { get; set; }
        public Main main { get; set; }
        public Weather[] weather { get; set; }
        public Clouds clouds { get; set; }
        public Wind wind { get; set; }
        public int? visibility { get; set; }
        public float? pop { get; set; }
        public ForecastSys sys { get; set; }
        public string dt_txt { get; set; }
        public Rain rain { get; set; }
        public Snow snow { get; set; }
    }

    public class ForecastSys
    {
        public string pod { get; set; }
    }

    public class Main
    {
        public float temp { get; set; }
        public float? feels_like { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
        public float pressure { get; set; }
        public int humidity { get; set; }
        public float? sea_level { get; set; }
        public float? grnd_level { get; set; }
    }

    public class Rootobject
    {
        public int cod { get; set; }
        public string message { get; set; }
    }
}