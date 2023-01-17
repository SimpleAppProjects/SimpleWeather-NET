#if !WINDOWS
using Microsoft.Maui.Storage;
using System.Collections.Generic;

namespace SimpleWeather.Preferences
{
    public partial class SettingsContainer
    {
        private readonly IPreferences preferences = Microsoft.Maui.Storage.Preferences.Default;

        private partial void Init() { }

        public partial T? GetValue<T>(string key, T defaultValue = default)
        {
            return preferences.Get(key, defaultValue, SharedName);
        }

        public partial void SetValue<T>(string key, T? value)
        {
            preferences.Set(key, value, SharedName);
        }

        public partial bool ContainsKey(string key)
        {
            return preferences.ContainsKey(key, SharedName);
        }

        public partial void Remove(string key)
        {
            preferences.Remove(key, SharedName);
        }

        public partial void Clear()
        {
            preferences.Clear(SharedName);
        }
    }
}
#endif