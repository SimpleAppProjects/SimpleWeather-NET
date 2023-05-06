#if !WINDOWS
using SimpleWeather.LocationData;
using SimpleWeather.Weather_API.Utils;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.Maui
{
    public partial class MauiLocationProvider : WeatherLocationProviderImpl, IRateLimitedRequest
    {
        public override bool KeyRequired => false;

        public override bool SupportsLocale => true;

        public override Task<bool> IsKeyValid(string key)
        {
            return Task.FromResult(false);
        }

        public override string GetAPIKey()
        {
            return null;
        }
    }
}
#endif