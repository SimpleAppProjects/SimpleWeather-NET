using SimpleWeather.LocationData;

namespace SimpleWeather.Weather_API.LocationData
{
    public interface IWeatherLocationProviderFactory
    {
        IWeatherLocationProvider GetLocationProvider(string provider);
    }
}
