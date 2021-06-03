using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.RemoteConfig
{
    public sealed partial class RemoteConfig
    {
        private const String DEFAULT_WEATHERPROVIDER_KEY = "default_weather_provider";

        public static LocationProviderImpl GetLocationProvider(string weatherAPI)
        {
            string configJson = GetConfigString(weatherAPI);

            var config = JSONParser.Deserializer<WeatherProviderConfig>(configJson);

            if (config != null)
            {
                switch (config.locSource)
                {
#if false
                    case WeatherAPI.Here:
                        return new HERE.HERELocationProvider();
#endif
                    case WeatherAPI.BingMaps:
                        return new Bing.BingMapsLocationProvider();
                    case WeatherAPI.WeatherApi:
                        return new WeatherApi.WeatherApiLocationProvider();
                }
            }

            return null;
        }

        public static bool IsProviderEnabled(string weatherAPI)
        {
            string configJson = GetConfigString(weatherAPI);

            var config = JSONParser.Deserializer<WeatherProviderConfig>(configJson);

            if (config != null)
            {
                return config.enabled;
            }

            return true;
        }

        public static bool UpdateWeatherProvider()
        {
            string API = Settings.API;

            string configJson = GetConfigString(API);
            var config = JSONParser.Deserializer<WeatherProviderConfig>(configJson);

            if (config != null)
            {
                bool isEnabled = config.enabled;

                if (!isEnabled)
                {
                    if (!String.IsNullOrWhiteSpace(config.newWeatherSource))
                    {
                        Settings.API = config.newWeatherSource;
                        WeatherManager.GetInstance().UpdateAPI();
                    }
                    else
                    {
                        Settings.API = GetDefaultWeatherProvider();
                        WeatherManager.GetInstance().UpdateAPI();
                    }
                    return true;
                }
            }

            return false;
        }

        public static string GetDefaultWeatherProvider()
        {
            return GetConfigString(DEFAULT_WEATHERPROVIDER_KEY) ?? WeatherAPI.WeatherUnlocked;
        }

        public static string GetDefaultWeatherProvider(string countryCode)
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
    }
}
