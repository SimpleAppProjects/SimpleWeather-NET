namespace SimpleWeather.WeatherData
{
    public interface IWeatherErrorListener
    {
        void OnWeatherError(Utils.WeatherException wEx);
    }
}
