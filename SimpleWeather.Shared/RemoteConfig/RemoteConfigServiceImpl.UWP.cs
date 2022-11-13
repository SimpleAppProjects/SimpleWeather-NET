#if WINDOWS_UWP
using System;
using Windows.Storage;

namespace SimpleWeather.RemoteConfig
{
    public sealed partial class RemoteConfigServiceImpl
    {
        // Shared Settings
        private readonly ApplicationDataContainer RemoteConfigContainer = ApplicationData.Current.LocalSettings
            .CreateContainer("firebase_remoteconfig", ApplicationDataCreateDisposition.Always);

        private String GetConfigString(String weatherAPI, bool useFallback = false)
        {
            if (useFallback)
            {
                return SharedModule.Instance.ResLoader.GetString("/Config/" + weatherAPI);
            }

            return RemoteConfigContainer.Values[weatherAPI]?.ToString() ?? SharedModule.Instance.ResLoader.GetString("/Config/" + weatherAPI);
        }

        public void SetConfigString(String key, String value)
        {
            RemoteConfigContainer.Values[key] = value;
        }
    }
}
#endif