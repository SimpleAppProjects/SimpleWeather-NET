using SimpleWeather.LocationData;
using SimpleWeather.Weather_API.AccuWeather;
using SimpleWeather.Weather_API.Bing;
using SimpleWeather.Weather_API.WeatherApi;
using SimpleWeather.WeatherData;
using System;

namespace SimpleWeather.Weather_API.LocationData
{
    public sealed class WeatherLocationProviderFactoryImpl : IWeatherLocationProviderFactory
    {
        public IWeatherLocationProvider GetLocationProvider(string provider)
        {
            return provider switch
            {
                WeatherAPI.BingMaps => new BingMapsLocationProvider(),
                WeatherAPI.WeatherApi => new WeatherApiLocationProvider(),
                WeatherAPI.AccuWeather => new AccuWeatherLocationProvider(),
                _ => throw new ArgumentException($"Location provider not supported ({provider})")
            };
        }
    }
}
