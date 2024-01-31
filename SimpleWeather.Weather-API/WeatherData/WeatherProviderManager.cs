using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.WeatherData
{
    public sealed class WeatherProviderManager : IWeatherProvider
    {
        private IWeatherProvider _weatherProvider = null;
        private readonly SettingsManager SettingsManager;

        internal WeatherProviderManager(SettingsManager settingsManager)
        {
            this.SettingsManager = settingsManager;
            UpdateAPI();
        }

        public void UpdateAPI()
        {
            string API = SettingsManager.API;
            _weatherProvider = GetWeatherProvider(API);
        }

        public IWeatherProvider GetWeatherProvider(string API)
        {
            return WeatherModule.Instance.WeatherProviderFactory.GetWeatherProvider(API);
        }

        public bool IsKeyRequired(string API)
        {
            var provider = GetWeatherProvider(API);
            return provider.KeyRequired;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public Task<bool> IsKeyValid(string key, string API)
        {
            var provider = GetWeatherProvider(API);
            return provider.IsKeyValid(key);
        }

        public AuthType GetAuthType(string API)
        {
            var provider = GetWeatherProvider(API);
            return provider.AuthType;
        }

        // Provider dependent methods
        public string WeatherAPI => _weatherProvider.WeatherAPI;

        public IWeatherLocationProvider LocationProvider => _weatherProvider.LocationProvider;

        public bool KeyRequired => _weatherProvider.KeyRequired;

        public bool SupportsWeatherLocale => _weatherProvider.SupportsWeatherLocale;

        public bool SupportsAlerts => _weatherProvider.SupportsAlerts;

        public bool NeedsExternalAlertData => _weatherProvider.NeedsExternalAlertData;

        public int HourlyForecastInterval => _weatherProvider.HourlyForecastInterval;

        public AuthType AuthType => _weatherProvider.AuthType;

        public bool IsRegionSupported(SimpleWeather.LocationData.LocationData location)
        {
            return _weatherProvider.IsRegionSupported(location);
        }

        public bool IsRegionSupported(SimpleWeather.LocationData.LocationQuery location)
        {
            return _weatherProvider.IsRegionSupported(location);
        }

        public Task UpdateLocationData(SimpleWeather.LocationData.LocationData location)
        {
            return _weatherProvider.UpdateLocationData(location);
        }

        public string UpdateLocationQuery(SimpleWeather.WeatherData.Weather weather)
        {
            return _weatherProvider.UpdateLocationQuery(weather);
        }

        public string UpdateLocationQuery(SimpleWeather.LocationData.LocationData location)
        {
            return _weatherProvider.UpdateLocationQuery(location);
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public Task<ObservableCollection<LocationQuery>> GetLocations(string ac_query)
        {
            return _weatherProvider.GetLocations(ac_query);
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public Task<LocationQuery> GetLocation(SimpleWeather.LocationData.Location location)
        {
            return GetLocation(new WeatherUtils.Coordinate(location));
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coord)
        {
            return _weatherProvider.GetLocation(coord);
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public Task<SimpleWeather.WeatherData.Weather> GetWeather(SimpleWeather.LocationData.LocationData location)
        {
            return _weatherProvider.GetWeather(location);
        }

        public Task<ICollection<WeatherAlert>> GetAlerts(SimpleWeather.LocationData.LocationData location)
        {
            return _weatherProvider.GetAlerts(location);
        }

        public string LocaleToLangCode(string iso, string name)
        {
            return _weatherProvider.LocaleToLangCode(iso, name);
        }

        public string GetWeatherIcon(string icon)
        {
            return _weatherProvider.GetWeatherIcon(icon);
        }

        public string GetWeatherIcon(bool isNight, string icon)
        {
            return _weatherProvider.GetWeatherIcon(isNight, icon);
        }

        public string GetWeatherCondition(string icon)
        {
            return _weatherProvider.GetWeatherCondition(icon);
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public Task<bool> IsKeyValid(string key)
        {
            return _weatherProvider.IsKeyValid(key);
        }

        public string GetAPIKey()
        {
            return _weatherProvider.GetAPIKey();
        }

        public bool IsNight(SimpleWeather.WeatherData.Weather weather)
        {
            return _weatherProvider.IsNight(weather);
        }
    }
}
