#if NETSTANDARD2_0
using System;
using System.Collections.Generic;

namespace SimpleWeather.Preferences
{
    public partial class SettingsContainer
    {
        private FilePropertySet _propertySet;

        private partial void Init()
        {
            _propertySet = new FilePropertySet(SharedName);
        }

        public partial T? GetValue<T>(string key, T defaultValue = default)
        {
            if (_propertySet.ContainsKey(key))
            {
                return (T)_propertySet[key] ?? defaultValue;
            }
            else
            {
                return defaultValue;
            }
        }

        public partial void SetValue<T>(string key, T? value)
        {
            _propertySet[key] = value;
        }

        public partial bool ContainsKey(string key)
        {
            return _propertySet.ContainsKey(key);
        }

        public partial void Remove(string key)
        {
            _propertySet.Remove(key);
        }

        public partial void Clear()
        {
            _propertySet.Clear();
        }

        private partial int GetPreferenceCount()
        {
            return _propertySet.Count;
        }
    }
}
#endif