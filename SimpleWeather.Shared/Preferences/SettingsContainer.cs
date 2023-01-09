using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleWeather.Preferences
{
    public sealed partial class SettingsContainer : ISettingsService
    {
        private readonly string SharedName;

        public SettingsContainer()
        {
            SharedName = string.Empty;
            Init();
        }

        public SettingsContainer(string name)
        {
            SharedName = name;
            Init();
        }

        private partial void Init();

        public partial T? GetValue<T>(string key, T defaultValue = default);
        public partial void SetValue<T>(string key, T? value);
        public partial bool ContainsKey(string key);
        public partial void Remove(string key);
        public partial void Clear();
#if WINDOWS || __ANDROID__
        public partial IDictionary<string, object> GetAllSettings();
#endif
        public int Count => GetPreferenceCount();
        private partial int GetPreferenceCount();

        internal static Type[] SupportedTypes = new Type[]
        {
            typeof(string),
            typeof(int),
            typeof(bool),
            typeof(long),
            typeof(double),
            typeof(float),
            typeof(DateTime),
        };

        internal static void CheckIsSupportedType<T>()
        {
            var type = typeof(T);
            if (!SupportedTypes.Contains(type))
            {
                throw new NotSupportedException($"Preferences using '{type}' type is not supported");
            }
        }
    }
}
