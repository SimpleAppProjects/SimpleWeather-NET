namespace SimpleWeather.WeatherData
{
    public interface IWeatherLoadedListener
    {
        void OnWeatherLoaded(int locationIdx, Weather weather);
    }
}
