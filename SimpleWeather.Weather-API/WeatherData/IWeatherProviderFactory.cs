using SimpleWeather.WeatherData;

namespace SimpleWeather.Weather_API.WeatherData
{
    public interface IWeatherProviderFactory
    {
        IWeatherProvider GetWeatherProvider(string provider);
    }
}
