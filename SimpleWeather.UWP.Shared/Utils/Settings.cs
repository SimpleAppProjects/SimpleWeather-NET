﻿using SimpleWeather.UWP.Helpers;
using SimpleWeather.WeatherData;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace SimpleWeather.Utils
{
    public static partial class Settings
    {
        public static UserThemeMode UserTheme { get { return GetUserTheme(); } set { SetUserTheme(value); } }

        // Shared Settings
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        // App data files
        private static StorageFolder appDataFolder = ApplicationData.Current.LocalFolder;

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void Init()
        {
            if (locationDB == null)
            {
                locationDB = new SQLiteAsyncConnection(
                    Path.Combine(appDataFolder.Path, "locations.db"), SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);
                var conn = locationDB.GetConnection();
                var _lock = conn.Lock();
                conn.BusyTimeout = TimeSpan.FromSeconds(5);
                conn.EnableWriteAheadLogging();
                _lock.Dispose();
            }

            if (weatherDB == null)
            {
                weatherDB = new SQLiteAsyncConnection(
                    Path.Combine(appDataFolder.Path, "weatherdata.db"), SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);
                var conn = weatherDB.GetConnection();
                var _lock = conn.Lock();
                conn.BusyTimeout = TimeSpan.FromSeconds(5);
                conn.EnableWriteAheadLogging();
                _lock.Dispose();
            }

            if (tzDBConnStr == null)
                tzDBConnStr = Path.Combine(appDataFolder.Path, "tzdb.db");

            localSettings.CreateContainer(WeatherAPI.WeatherUnderground, ApplicationDataCreateDisposition.Always);
            localSettings.CreateContainer("version", ApplicationDataCreateDisposition.Always);
        }

        private static string GetTempUnit()
        {
            if (!localSettings.Values.ContainsKey(KEY_UNITS) || localSettings.Values[KEY_UNITS] == null)
            {
                return Fahrenheit;
            }
            else if (localSettings.Values[KEY_UNITS].Equals(Celsius))
                return Celsius;

            return Fahrenheit;
        }

        private static void SetTempUnit(string value)
        {
            if (value == Celsius)
                localSettings.Values[KEY_UNITS] = Celsius;
            else
                localSettings.Values[KEY_UNITS] = Fahrenheit;
        }

        private static bool IsWeatherLoaded()
        {
            if (!DBUtils.LocationDataExists(locationDB))
            {
                if (!DBUtils.WeatherDataExists(weatherDB))
                {
                    SetWeatherLoaded(false);
                    return false;
                }
            }

            if (localSettings.Values[KEY_WEATHERLOADED] == null)
            {
                return false;
            }
            else if (localSettings.Values[KEY_WEATHERLOADED].Equals(true))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void SetWeatherLoaded(bool isLoaded)
        {
            localSettings.Values[KEY_WEATHERLOADED] = isLoaded;
        }

        private static string GetAPI()
        {
            if (!localSettings.Values.ContainsKey(KEY_API) || localSettings.Values[KEY_API] == null)
            {
                SetAPI(WeatherAPI.Here);
                return WeatherAPI.Here;
            }
            else
                return (string)localSettings.Values[KEY_API];
        }

        private static void SetAPI(string value)
        {
            localSettings.Values[KEY_API] = value;
        }

        private static string GetAPIKEY()
        {
            if (!localSettings.Values.ContainsKey(KEY_APIKEY) || localSettings.Values[KEY_APIKEY] == null)
            {
                return String.Empty;
            }
            else
                return (string)localSettings.Values[KEY_APIKEY];
        }

        private static void SetAPIKEY(string API_KEY)
        {
            localSettings.Values[KEY_APIKEY] = API_KEY;
        }

        private static bool UseFollowGPS()
        {
            if (!localSettings.Values.ContainsKey(KEY_FOLLOWGPS) || localSettings.Values[KEY_FOLLOWGPS] == null)
            {
                SetFollowGPS(false);
                return false;
            }
            else
                return (bool)localSettings.Values[KEY_FOLLOWGPS];
        }

        private static void SetFollowGPS(bool value)
        {
            localSettings.Values[KEY_FOLLOWGPS] = value;
        }

        private static string GetLastGPSLocation()
        {
            if (!localSettings.Values.ContainsKey(KEY_LASTGPSLOCATION) || localSettings.Values[KEY_LASTGPSLOCATION] == null)
                return null;
            else
                return (string)localSettings.Values[KEY_LASTGPSLOCATION];
        }

        private static void SetLastGPSLocation(string value)
        {
            localSettings.Values[KEY_LASTGPSLOCATION] = value;
        }

        private static int GetRefreshInterval()
        {
            if (!localSettings.Values.ContainsKey(KEY_REFRESHINTERVAL) || localSettings.Values[KEY_REFRESHINTERVAL] == null)
            {
                var interval = int.Parse(DEFAULT_UPDATE_INTERVAL, CultureInfo.InvariantCulture);
                SetRefreshInterval(interval);
                return interval;
            }
            else
                return (int)localSettings.Values[KEY_REFRESHINTERVAL];
        }

        private static void SetRefreshInterval(int value)
        {
            localSettings.Values[KEY_REFRESHINTERVAL] = value;
        }

        private static DateTime GetUpdateTime()
        {
            if (localSettings.Values.ContainsKey(KEY_UPDATETIME) && localSettings.Values[KEY_UPDATETIME] != null)
            {
                if (DateTime.TryParse(localSettings.Values[KEY_UPDATETIME] as string, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
            }

            return DateTime.MinValue;
        }

        public static void SetUpdateTime(DateTime value)
        {
            localSettings.Values[KEY_UPDATETIME] = value.ToString(CultureInfo.InvariantCulture);
        }

        private static int GetDBVersion()
        {
            if (!localSettings.Values.ContainsKey(KEY_DBVERSION) || localSettings.Values[KEY_DBVERSION] == null)
            {
                SetDBVersion(0);
                return 0;
            }
            else
                return (int)localSettings.Values[KEY_DBVERSION];
        }

        private static void SetDBVersion(int value)
        {
            localSettings.Values[KEY_DBVERSION] = value;
        }

        private static bool UseAlerts()
        {
            if (!localSettings.Values.ContainsKey(KEY_USEALERTS) || localSettings.Values[KEY_USEALERTS] == null)
            {
                SetAlerts(false);
                return false;
            }
            else
                return (bool)localSettings.Values[KEY_USEALERTS];
        }

        private static void SetAlerts(bool value)
        {
            localSettings.Values[KEY_USEALERTS] = value;
        }

        private static bool IsKeyVerified()
        {
            if (localSettings.Containers.ContainsKey(WeatherAPI.WeatherUnderground))
            {
                if (localSettings.Containers[WeatherAPI.WeatherUnderground].Values.TryGetValue(KEY_APIKEY_VERIFIED, out object value))
                    return (bool)value;
            }

            return false;
        }

        private static void SetKeyVerified(bool value)
        {
            localSettings.Containers[WeatherAPI.WeatherUnderground].Values[KEY_APIKEY_VERIFIED] = value;

            if (!value)
                localSettings.Containers[WeatherAPI.WeatherUnderground].Values.Remove(KEY_APIKEY_VERIFIED);
        }

        private static bool IsPersonalKey()
        {
            if (localSettings.Containers.ContainsKey(WeatherAPI.WeatherUnderground))
            {
                if (localSettings.Containers[WeatherAPI.WeatherUnderground].Values.TryGetValue(KEY_USEPERSONALKEY, out object value))
                    return (bool)value;
            }

            return false;
        }

        private static void SetPersonalKey(bool value)
        {
            localSettings.Containers[WeatherAPI.WeatherUnderground].Values[KEY_USEPERSONALKEY] = value;
        }

        private static int GetVersionCode()
        {
            if (localSettings.Containers.ContainsKey("version"))
            {
                if (localSettings.Containers["version"].Values.TryGetValue(KEY_CURRENTVERSION, out object value))
                    return (int)value;
            }

            return 0;
        }

        private static void SetVersionCode(int value)
        {
            localSettings.Containers["version"].Values[KEY_CURRENTVERSION] = value;
        }

        private static bool IsOnBoardingComplete()
        {
            LoadIfNeeded();

            if (!localSettings.Values.ContainsKey(KEY_ONBOARDINGCOMPLETE) || localSettings.Values[KEY_ONBOARDINGCOMPLETE] == null)
            {
                SetOnBoardingComplete(false);
                return false;
            }
            else
                return (bool)localSettings.Values[KEY_ONBOARDINGCOMPLETE];
        }

        private static void SetOnBoardingComplete(bool value)
        {
            localSettings.Values[KEY_ONBOARDINGCOMPLETE] = value;
        }

        private static UserThemeMode GetUserTheme()
        {
            if (!localSettings.Values.ContainsKey(KEY_USERTHEME) || localSettings.Values[KEY_USERTHEME] == null)
            {
                SetUserTheme(UserThemeMode.System);
                return UserThemeMode.System;
            }
            else
                return (UserThemeMode)localSettings.Values[KEY_USERTHEME];
        }

        private static void SetUserTheme(UserThemeMode value)
        {
            localSettings.Values[KEY_USERTHEME] = (int)value;
        }
    }
}