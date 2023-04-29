using SimpleWeather.LocationData;
using SimpleWeather.Weather_API.AccuWeather;
using SimpleWeather.Weather_API.Bing;
using SimpleWeather.Weather_API.Maui;
using SimpleWeather.Weather_API.Utils;
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
#if __IOS__
                WeatherAPI.Apple => APIRequestUtils.IsRateLimited(provider) ? new BingMapsLocationProvider() : new MauiLocationProvider(),
#endif
                _ => throw new ArgumentException($"Location provider not supported ({provider})")
            };
        }
    }
}
