using SimpleWeather.WeatherData;
using System.Collections.Generic;
using static SimpleWeather.Preferences.SettingsChangedEventArgs;

namespace SimpleWeather.Preferences
{
    public partial class SettingsManager
    {
        private const string VersionContainerName = "version";
        private const string DevSettingsContainerName = "devsettings";

        public event SettingsChangedEventHandler OnSettingsChanged;

        // Shared Settings
        private readonly ISettingsService LocalSettings = new SettingsContainer();
        private readonly ISettingsService WUSharedSettings = new SettingsContainer(WeatherAPI.WeatherUnderground);
        private readonly ISettingsService VersionSettings = new SettingsContainer(VersionContainerName);
        private readonly ISettingsService DevSettings = new SettingsContainer(DevSettingsContainerName);

        public T? GetValue<T>(string key, T defaultValue = default)
        {
            return LocalSettings.GetValue(key, defaultValue);
        }

        public void SetValue<T>(string key, T? value)
        {
            LocalSettings.SetValue(key, value);
            OnSettingsChanged?.Invoke(new SettingsChangedEventArgs { Key = key, NewValue = value });
        }

        public bool ContainsKey(string key)
        {
            return LocalSettings.ContainsKey(key);
        }

        public void Remove(string key)
        {
            LocalSettings.Remove(key);
        }

        public void Clear()
        {
            LocalSettings.Clear();
        }

#if WINDOWS_UWP || __ANDROID__
        public IDictionary<string, object> GetAllSettings()
        {
            return LocalSettings.GetAllSettings();
        }
#endif

        private void ClearDevSettings()
        {
            DevSettings.Clear();
        }

#if WINDOWS_UWP || __ANDROID__
        private IDictionary<string, object> GetAllDevSettings()
        {
            return DevSettings.GetAllSettings();
        }
#endif
    }
}