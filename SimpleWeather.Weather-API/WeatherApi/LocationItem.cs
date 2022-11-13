namespace SimpleWeather.Weather_API.WeatherApi
{
    public class Rootobject
    {
        public LocationItem[] LocationItems { get; set; }
    }

    public class LocationItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public string url { get; set; }
    }
}
