namespace SimpleWeather.RemoteConfig
{
    public sealed class WeatherProviderConfig
    {
        public bool enabled { get; set; }
        public string locSource { get; set; }
        public string newWeatherSource { get; set; }
    }
}
