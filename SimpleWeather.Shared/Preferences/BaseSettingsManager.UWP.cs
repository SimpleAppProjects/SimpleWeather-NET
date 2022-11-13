using SimpleWeather.WeatherData;
using System.Collections.Generic;
using Windows.Storage;

namespace SimpleWeather.Preferences
{
    public abstract partial class BaseSettingsManager : ISettingsService
    {
        private const string VersionContainerName = "version";
        private const string DevSettingsContainerName = "devsettings";

        // Shared Settings
        protected readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        protected virtual partial void Init()
        {
            LocalSettings.CreateContainer(WeatherAPI.WeatherUnderground, ApplicationDataCreateDisposition.Always);
            LocalSettings.CreateContainer(VersionContainerName, ApplicationDataCreateDisposition.Always);
        }

        public T? GetValue<T>(string key, T defaultValue = default)
        {
            if (LocalSettings.Values.TryGetValue(key, out object? value))
            {
                return (T)value!;
            }

            return defaultValue;
        }

        public void SetValue<T>(string key, T? value)
        {
            LocalSettings.Values[key] = value;

            OnSettingsChanged?.Invoke(new SettingsChangedEventArgs { Key = key, NewValue = value });
        }

        public bool ContainsKey(string key)
        {
            return LocalSettings.Values.ContainsKey(key);
        }

        public void Remove(string key)
        {
            LocalSettings.Values.Remove(key);
        }

        protected T? GetWUSharedValue<T>(string key, T defaultValue = default)
        {
            if (LocalSettings.Containers[WeatherAPI.WeatherUnderground].Values.TryGetValue(key, out object? value))
            {
                return (T)value!;
            }

            return defaultValue;
        }

        protected void SetWUSharedValue<T>(string key, T? value)
        {
            LocalSettings.Containers[WeatherAPI.WeatherUnderground].Values[key] = value;

            OnSettingsChanged?.Invoke(new SettingsChangedEventArgs { Key = key, NewValue = value });
        }

        protected bool WUSharedContainsKey(string key)
        {
            return LocalSettings.Containers[WeatherAPI.WeatherUnderground].Values.ContainsKey(key);
        }

        protected void WUSharedRemove(string key)
        {
            LocalSettings.Containers[WeatherAPI.WeatherUnderground].Values.Remove(key);
        }

        protected T? GetVersionValue<T>(string key, T defaultValue = default)
        {
            if (LocalSettings.Containers[VersionContainerName].Values.TryGetValue(key, out object? value))
            {
                return (T)value!;
            }

            return defaultValue;
        }

        protected void SetVersionValue<T>(string key, T? value)
        {
            LocalSettings.Containers[VersionContainerName].Values[key] = value;

            OnSettingsChanged?.Invoke(new SettingsChangedEventArgs { Key = key, NewValue = value });
        }

        protected bool VersionContainsKey(string key)
        {
            return LocalSettings.Containers[VersionContainerName].Values.ContainsKey(key);
        }

        protected void VersionRemove(string key)
        {
            LocalSettings.Containers[VersionContainerName].Values.Remove(key);
        }

        protected T? GetDevSettingsValue<T>(string key, T defaultValue = default)
        {
            if (LocalSettings.Containers[DevSettingsContainerName].Values.TryGetValue(key, out object? value))
            {
                return (T)value!;
            }

            return defaultValue;
        }

        protected void SetDevSettingsValue<T>(string key, T? value)
        {
            LocalSettings.Containers[DevSettingsContainerName].Values[key] = value;
        }

        protected void ClearDevSettings()
        {
            LocalSettings.Containers[DevSettingsContainerName].Values.Clear();
        }

        protected IDictionary<string, object> GetAllDevSettings()
        {
            return LocalSettings.Containers[DevSettingsContainerName].Values;
        }
    }
}