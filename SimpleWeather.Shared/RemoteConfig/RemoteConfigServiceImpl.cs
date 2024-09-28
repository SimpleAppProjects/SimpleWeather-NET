using SimpleWeather.Preferences;
using SimpleWeather.Resources.Strings;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Threading.Tasks;
#if __IOS__
using FirebaseRemoteConfig = Firebase.RemoteConfig.RemoteConfig;
#endif

namespace SimpleWeather.RemoteConfig
{
    public sealed partial class RemoteConfigServiceImpl : IRemoteConfigService
    {
        private const string DEFAULT_WEATHERPROVIDER_KEY = "default_weather_provider";

        // Shared Settings
        private readonly SettingsContainer RemoteConfigContainer = new SettingsContainer("firebase_remoteconfig");

        private string GetConfigString(string weatherAPI, bool useFallback = false)
        {
            if (useFallback)
            {
#if __IOS__
                return ConfigiOS.ResourceManager.GetString(weatherAPI);
#else
                return Config.ResourceManager.GetString(weatherAPI);
#endif
            }

#if __IOS__
            var remoteConfStr = FirebaseRemoteConfig.SharedInstance?.GetConfigValue(weatherAPI)?.StringValue;
            return remoteConfStr ??
#else
            return RemoteConfigContainer.GetValue<string>(weatherAPI) ??
#endif
#if __IOS__
                ConfigiOS.ResourceManager.GetString(weatherAPI);
#else
                Config.ResourceManager.GetString(weatherAPI);
#endif
        }

        private void SetConfigString(string key, string value)
        {
            RemoteConfigContainer.SetValue(key, value);
        }

        public string GetLocationProvider(string weatherAPI)
        {
#if __IOS__
            string configJson = FirebaseRemoteConfig.SharedInstance.GetConfigValue(weatherAPI)?.StringValue;
#else
            string configJson = GetConfigString(weatherAPI);
#endif

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

        public string GetDefaultWeatherProvider(LocationData.LocationData location)
        {
            if (LocationUtils.IsNWSSupported(location) && IsProviderEnabled(WeatherAPI.NWS))
            {
                return WeatherAPI.NWS;
            }
            else if (LocationUtils.IsFrance(location) && IsProviderEnabled(WeatherAPI.MeteoFrance))
            {
                return WeatherAPI.MeteoFrance;
            }
            else
            {
                return GetDefaultWeatherProvider();
            }
        }

        public string GetDefaultWeatherProvider(LocationData.LocationQuery location)
        {
            if (LocationUtils.IsNWSSupported(location) && IsProviderEnabled(WeatherAPI.NWS))
            {
                return WeatherAPI.NWS;
            }
            else if (LocationUtils.IsFrance(location) && IsProviderEnabled(WeatherAPI.MeteoFrance))
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
#if WINDOWS
                var remoteConfig = await Firebase.FirebaseHelper.GetFirebaseRemoteConfig();
                var config = await remoteConfig.GetRemoteConfig();

                if (config?.entries?.Count > 0)
                {
                    foreach (var prop in config.entries)
                    {
                        SetConfigString(prop.Key, prop.Value);
                    }
                }
#elif __IOS__
                await (FirebaseRemoteConfig.SharedInstance?.FetchAndActivateAsync() ?? Task.CompletedTask);
#else
                var db = await Firebase.FirebaseHelper.GetFirebaseDatabase();
#if __IOS__
                var config = await db.Child("ios_remote_config").OnceAsync<object>();
#else
                var config = await db.Child("uwp_remote_config").OnceAsync<object>();
#endif
                if (config?.Count > 0)
                {
                    foreach (var prop in config)
                    {
                        SetConfigString(prop.Key, prop.Object.ToString());
                    }
                }
#endif

                UpdateWeatherProvider();
            });
        }

        public bool GetBoolean(string key)
        {
#if __IOS__
            var remoteBoolVal = FirebaseRemoteConfig.SharedInstance?.GetConfigValue(key)?.BoolValue;
            return remoteBoolVal ?? false;
#else
            return RemoteConfigContainer.GetValue<bool>(key, defaultValue: Config.ResourceManager.GetString(key)?.TryParseBool() ?? false);
#endif
        }
    }
}
