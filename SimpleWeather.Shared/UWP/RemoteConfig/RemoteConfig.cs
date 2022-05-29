#if WINDOWS_UWP
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace SimpleWeather.RemoteConfig
{
    public sealed partial class RemoteConfig
    {
        // Shared Settings
        private static ApplicationDataContainer RemoteConfigContainer = ApplicationData.Current.LocalSettings
            .CreateContainer("firebase_remoteconfig", ApplicationDataCreateDisposition.Always);

        private static String GetConfigString(String weatherAPI, bool useFallback = false)
        {
            if (useFallback)
            {
                return SharedModule.Instance.ResLoader.GetString("/Config/" + weatherAPI);
            }

            return RemoteConfigContainer.Values[weatherAPI]?.ToString() ?? SharedModule.Instance.ResLoader.GetString("/Config/" + weatherAPI);
        }

        public static void SetConfigString(String key, String value)
        {
            RemoteConfigContainer.Values[key] = value;
        }
    }
}
#endif