#if WINDOWS_UWP
using System.Collections.Generic;
using Windows.Storage;

namespace SimpleWeather.Preferences
{
    public partial class SettingsContainer
    {
        private ApplicationDataContainer container;

        private partial void Init()
        {
            if (string.IsNullOrWhiteSpace(SharedName))
            {
                container = ApplicationData.Current.LocalSettings;
            }
            else
            {
                container = ApplicationData.Current.LocalSettings.CreateContainer(SharedName, ApplicationDataCreateDisposition.Always);
            }
        }

        public partial T? GetValue<T>(string key, T defaultValue = default)
        {
            if (container.Values.TryGetValue(key, out object? value))
            {
                return (T)value!;
            }

            return defaultValue;
        }

        public partial void SetValue<T>(string key, T? value)
        {
            container.Values[key] = value;
        }

        public partial bool ContainsKey(string key)
        {
            return container.Values.ContainsKey(key);
        }

        public partial void Remove(string key)
        {
            container.Values.Remove(key);
        }

        public partial void Clear()
        {
            container.Values.Clear();
        }

        public partial IDictionary<string, object> GetAllSettings()
        {
            return container.Values;
        }

        private partial int GetPreferenceCount()
        {
            return container.Values.Keys.Count;
        }
    }
}

#endif