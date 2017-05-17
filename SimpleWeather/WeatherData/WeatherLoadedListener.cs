namespace SimpleWeather.WeatherData
{
    public interface WeatherLoadedListener
    {
        void onWeatherLoaded(int locationIdx, Weather weather);
    }
}
