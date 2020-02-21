using System.Runtime.Serialization;

namespace SimpleWeather.WeatherYahoo
{
    public class Rootobject
    {
        public Location location { get; set; }
        public Current_Observation current_observation { get; set; }
        public Forecast[] forecasts { get; set; }
    }

    public class Location
    {
        public int woeid { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public double lat { get; set; }
        [DataMember(Name = "long")]
        public double _long { get; set; }
        public string timezone_id { get; set; }
    }

    public class Current_Observation
    {
        public Wind wind { get; set; }
        public Atmosphere atmosphere { get; set; }
        public Astronomy astronomy { get; set; }
        public Condition condition { get; set; }
        public long pubDate { get; set; }
    }

    public class Wind
    {
        public int chill { get; set; }
        public int direction { get; set; }
        public float speed { get; set; }
    }

    public class Atmosphere
    {
        [DataMember(Name = "humidity")]
        private int _humidity { get; set; }
        public float pressure { get; set; }
        public int rising { get; set; }
        public float visibility { get; set; }

        [IgnoreDataMember]
        public string humidity { get { return _humidity + "%"; } set { _humidity = int.Parse(value); } }
    }

    public class Astronomy
    {
        public string sunrise { get; set; }
        public string sunset { get; set; }
    }

    public class Condition
    {
        public int code { get; set; }
        public int temperature { get; set; }
        public string text { get; set; }
    }

    public class Forecast
    {
        public int code { get; set; }
        public long date { get; set; }
        public string day { get; set; }
        public int high { get; set; }
        public int low { get; set; }
        public string text { get; set; }
    }
}