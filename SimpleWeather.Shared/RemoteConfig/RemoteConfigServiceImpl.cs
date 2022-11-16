using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Threading.Tasks;

namespace SimpleWeather.RemoteConfig
{
    public sealed partial class RemoteConfigServiceImpl : IRemoteConfigService
    {
        private const String DEFAULT_WEATHERPROVIDER_KEY = "default_weather_provider";

        public string GetLocationProvider(string weatherAPI)
        {
            string configJson = GetConfigString(weatherAPI);

            var config = JSONParser.Deserializer<WeatherProviderConfig>(configJson);

            return config?.locSource;
        }

        public bool IsProviderEnabled(string weatherAPI)
        {
            var config = GetConfig(weatherAPI);

            if (config != null)
            {
                return config.enabled;
            }

            return true;
        }

        public bool UpdateWeatherProvider()
        {
            var SettingsManager = DI.Utils.SettingsManager;
            string API = SettingsManager.API;

            var config = GetConfig(API);

            if (config != null)
            {
                bool isEnabled = config.enabled;

                if (!isEnabled)
                {
                    if (!String.IsNullOrWhiteSpace(config.newWeatherSource))
                    {
                        SettingsManager.API = config.newWeatherSource;
                    }
                    else
                    {
                        SettingsManager.API = GetDefaultWeatherProvider();
                    }
                    return true;
                }
            }

            return false;
        }

        public string GetDefaultWeatherProvider()
        {
            return GetConfigString(DEFAULT_WEATHERPROVIDER_KEY) ?? WeatherAPI.WeatherApi;
        }

        public string GetDefaultWeatherProvider(string countryCode)
        {
            if (LocationUtils.IsUS(countryCode))
            {
                return WeatherAPI.NWS;
            }
            else if (LocationUtils.IsFrance(countryCode))
            {
                return WeatherAPI.MeteoFrance;
            }
            else
            {
                return GetDefaultWeatherProvider();
            }
        }

        private WeatherProviderConfig GetConfig(string API)
        {
            WeatherProviderConfig config;

            try
            {
                config = JSONParser.Deserializer<WeatherProviderConfig>(GetConfigString(API));
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e, "Error loading remote config");

                try
                {
                    config = JSONParser.Deserializer<WeatherProviderConfig>(GetConfigString(API, true));
                }
                catch
                {
                    config = null;
                }
            }

            return config;
        }

        public Task CheckConfig()
        {
            return Task.Run(async () =>
            {
#if WINDOWS_UWP
                var db = await Firebase.FirebaseDatabaseHelper.GetFirebaseDatabase();
                var uwpConfig = await db.Child("uwp_remote_config").OnceAsync<object>();
                if (uwpConfig?.Count > 0)
                {
                    foreach (var prop in uwpConfig)
                    {
                        SetConfigString(prop.Key, prop.Object.ToString());
                    }
                }
#endif

                UpdateWeatherProvider();
            });
        }
    }
}
