using System.Collections.Generic;

namespace SimpleWeather.Preferences
{
    public sealed partial class SettingsContainer : ISettingsService
    {
        /*
         * Must implement the following:
         * 
         * public SettingsContainer();
         * public SettingsContainer(string name);
        */

        public partial T? GetValue<T>(string key, T defaultValue = default);
        public partial void SetValue<T>(string key, T? value);
        public partial bool ContainsKey(string key);
        public partial void Remove(string key);
        public partial void Clear();
#if WINDOWS_UWP || NETFX_CORE || NETSTANDARD || __ANDROID__
        public partial IDictionary<string, object> GetAllSettings();
#endif
        public int Count => GetPreferenceCount();
        private partial int GetPreferenceCount();
    }
}
