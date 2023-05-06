using System;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.WeatherKit
{
    public interface IWeatherKitJwtService
    {
        Task<string> GetBearerToken(bool forceRefresh = false);
    }

    public sealed class Auth
    {
        private static readonly Lazy<IWeatherKitJwtService> weatherKitJwtService = new(() =>
        {
            return new WeatherKitJwtServiceImpl();
        });

        public static IWeatherKitJwtService WeatherKitJwtService = weatherKitJwtService.Value;
    }
}
