using SimpleWeather.Preferences;
using System;

namespace SimpleWeather.RemoteConfig
{
    public sealed partial class RemoteConfigServiceImpl
    {
        // Shared Settings
        private readonly SettingsContainer RemoteConfigContainer = new SettingsContainer("firebase_remoteconfig");

        private String GetConfigString(String weatherAPI, bool useFallback = false)
        {
            if (useFallback)
            {
                return SharedModule.Instance.ResLoader.GetString("/Config/" + weatherAPI);
            }

            return RemoteConfigContainer.GetValue<string>(weatherAPI) ?? SharedModule.Instance.ResLoader.GetString("/Config/" + weatherAPI);
        }

        public void SetConfigString(String key, String value)
        {
            RemoteConfigContainer.SetValue(key, value);
        }
    }
}
