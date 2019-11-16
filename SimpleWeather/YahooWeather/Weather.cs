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
        public string woeid { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public string lat { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "long")]
        public string _long { get; set; }
        public string timezone_id { get; set; }
    }

    public class Current_Observation
    {
        public Wind wind { get; set; }
        public Atmosphere atmosphere { get; set; }
        public Astronomy astronomy { get; set; }
        public Condition condition { get; set; }
        public string pubDate { get; set; }
    }

    public class Wind
    {
        public string chill { get; set; }
        public string direction { get; set; }
        public string speed { get; set; }
    }

    public class Atmosphere
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "humidity")]
        private string _humidity { get; set; }
        public string pressure { get; set; }
        public string rising { get; set; }
        public string visibility { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string humidity { get { return _humidity + "%"; } set { _humidity = value; } }
    }

    public class Astronomy
    {
        public string sunrise { get; set; }
        public string sunset { get; set; }
    }

    public class Condition
    {
        public string code { get; set; }
        public string temperature { get; set; }
        public string text { get; set; }
    }

    public class Forecast
    {
        public string code { get; set; }
        public string date { get; set; }
        public string day { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string text { get; set; }
    }
}