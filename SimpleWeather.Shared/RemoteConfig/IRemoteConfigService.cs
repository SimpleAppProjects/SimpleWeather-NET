using System.Threading.Tasks;

namespace SimpleWeather.RemoteConfig
{
    public interface IRemoteConfigService
    {
        string GetLocationProvider(string weatherAPI);
        bool IsProviderEnabled(string weatherAPI);
        bool UpdateWeatherProvider();

        string GetDefaultWeatherProvider();

        string GetDefaultWeatherProvider(LocationData.LocationData location);

        string GetDefaultWeatherProvider(LocationData.LocationQuery location);

        Task CheckConfig();
    }
}
