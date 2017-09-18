namespace SimpleWeather.WeatherData
{
    public interface IWeatherLoadedListener
    {
        void OnWeatherLoaded(LocationData location, Weather weather);
    }
}
