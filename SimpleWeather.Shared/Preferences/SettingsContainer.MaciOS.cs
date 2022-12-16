#if __IOS__ || __MACCATALYST__ || __MACOS__
using Foundation;
using System;
using System.Globalization;
using System.Linq;

namespace SimpleWeather.Preferences
{
    public partial class SettingsContainer
    {
        private readonly object locker = new();

        private partial void Init() { }

        public partial T? GetValue<T>(string key, T defaultValue = default)
        {
            object value = null;

            lock (locker)
            {
                using var userDefaults = GetUserDefaults();

                if (userDefaults[key] == null)
                    return defaultValue;

                switch (defaultValue)
                {
                    case int i:
                        value = (int)(nint)userDefaults.IntForKey(key);
                        break;
                    case bool b:
                        value = userDefaults.BoolForKey(key);
                        break;
                    case long l:
                        var savedLong = userDefaults.StringForKey(key);
                        value = Convert.ToInt64(savedLong, CultureInfo.InvariantCulture);
                        break;
                    case double d:
                        value = userDefaults.DoubleForKey(key);
                        break;
                    case float f:
                        value = userDefaults.FloatForKey(key);
                        break;
                    case DateTime dt:
                        var savedDateTime = userDefaults.StringForKey(key);
                        var encodedDateTime = Convert.ToInt64(savedDateTime, CultureInfo.InvariantCulture);
                        value = DateTime.FromBinary(encodedDateTime);
                        break;
                    case string s:
                        // the case when the string is not null
                        value = userDefaults.StringForKey(key);
                        break;
                    default:
                        // the case when the string is null
                        if (typeof(T) == typeof(string))
                            value = userDefaults.StringForKey(key);
                        break;
                }
            }

            return (T)value;
        }

        public partial void SetValue<T>(string key, T? value)
        {
            CheckIsSupportedType<T>();

            lock (locker)
            {
                using var userDefaults = GetUserDefaults();

                if (value == null)
                {
                    if (userDefaults[key] != null)
                        userDefaults.RemoveObject(key);
                    return;
                }

                switch (value)
                {
                    case string s:
                        userDefaults.SetString(s, key);
                        break;
                    case int i:
                        userDefaults.SetInt(i, key);
                        break;
                    case bool b:
                        userDefaults.SetBool(b, key);
                        break;
                    case long l:
                        var valueString = Convert.ToString(value, CultureInfo.InvariantCulture);
                        userDefaults.SetString(valueString, key);
                        break;
                    case double d:
                        userDefaults.SetDouble(d, key);
                        break;
                    case float f:
                        userDefaults.SetFloat(f, key);
                        break;
                    case DateTime dt:
                        var encodedDateTime = Convert.ToString(dt.ToBinary(), CultureInfo.InvariantCulture);
                        userDefaults.SetString(encodedDateTime, key);
                        break;
                }
            }
        }

        public partial bool ContainsKey(string key)
        {
            lock (locker)
            {
                return GetUserDefaults()[key] != null;
            }
        }

        public partial void Remove(string key)
        {
            lock (locker)
            {
                using var userDefaults = GetUserDefaults();

                if (userDefaults[key] != null)
                    userDefaults.RemoveObject(key);
            }
        }

        public partial void Clear()
        {
            lock (locker)
            {
                using var userDefaults = GetUserDefaults();

                var items = userDefaults.ToDictionary();

                foreach (var item in items.Keys)
                {
                    if (item is NSString nsString)
                        userDefaults.RemoveObject(nsString);
                }
            }
        }

        private partial int GetPreferenceCount()
        {
            return GetUserDefaults().ToDictionary().Keys.Count(i => i is NSString);
        }

        private NSUserDefaults GetUserDefaults()
        {
            if (!string.IsNullOrWhiteSpace(SharedName))
                return new NSUserDefaults(SharedName, NSUserDefaultsType.SuiteName);
            else
                return NSUserDefaults.StandardUserDefaults;
        }
    }
}

#endif