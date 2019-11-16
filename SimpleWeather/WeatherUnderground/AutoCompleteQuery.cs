namespace SimpleWeather.WeatherUnderground
{
    public class AC_Rootobject
    {
        public AC_RESULT[] RESULTS { get; set; }
    }

    public class AC_RESULT
    {
        public string name { get; set; }
        public string type { get; set; }
        public string c { get; set; }
        public string zmw { get; set; }
        public string tz { get; set; }
        public string tzs { get; set; }
        public string l { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
    }
}