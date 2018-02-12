using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace SimpleWeather.Utils
{
    public static partial class Settings
    {
        // Shared Settings
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        // App data files
        private static StorageFolder appDataFolder = ApplicationData.Current.LocalFolder;

        private static void Init()
        {
            if (locationDB == null)
                locationDB = new SQLiteAsyncConnection(
                    Path.Combine(appDataFolder.Path, "locations.db"));

            if (weatherDB == null)
                weatherDB = new SQLiteAsyncConnection(
                    Path.Combine(appDataFolder.Path, "weatherdata.db"));
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
            if (!Task.Run(() => DBUtils.LocationDataExists(locationDB)).Result)
            {
                if (!Task.Run(() => DBUtils.WeatherDataExists(weatherDB)).Result)
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
                SetAPI(WeatherData.WeatherAPI.WeatherUnderground);
                return WeatherData.WeatherAPI.WeatherUnderground;
            }
            else
                return (string)localSettings.Values[KEY_API];
        }

        private static void SetAPI(string value)
        {
            localSettings.Values[KEY_API] = value;
        }

        #region WeatherUnderground
        private static string GetAPIKEY()
        {
            if (!localSettings.Values.ContainsKey(KEY_APIKEY) || localSettings.Values[KEY_APIKEY] == null)
            {
                String key = String.Empty;
                key = Task.Run(() => ReadAPIKEYfile()).Result;

                if (!String.IsNullOrWhiteSpace(key))
                    SetAPIKEY(key);

                return key;
            }
            else
                return (string)localSettings.Values[KEY_APIKEY];
        }

        private static async Task<string> ReadAPIKEYfile()
        {
            // Read key from file
            String key = String.Empty;
            try
            {
                StorageFile keyFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///API_KEY.txt"));
                FileInfo fileinfo = new FileInfo(keyFile.Path);

                if (fileinfo.Length != 0 || fileinfo.Exists)
                {
                    StreamReader reader = new StreamReader(await keyFile.OpenStreamForReadAsync());
                    key = reader.ReadLine();
                    reader.Dispose();
                }
            }
            catch (FileNotFoundException) { }

            return key;
        }

        private static void SetAPIKEY(string API_KEY)
        {
            localSettings.Values[KEY_APIKEY] = API_KEY;
        }
        #endregion

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
                SetRefreshInterval(int.Parse(DEFAULT_UPDATE_INTERVAL));
                return int.Parse(DEFAULT_UPDATE_INTERVAL);
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
            if (!localSettings.Values.ContainsKey(KEY_UPDATETIME) || localSettings.Values[KEY_UPDATETIME] == null)
                return DateTime.MinValue;
            else
                return DateTime.Parse((string)localSettings.Values[KEY_UPDATETIME]);
        }

        public static void SetUpdateTime(DateTime value)
        {
            localSettings.Values[KEY_UPDATETIME] = value.ToString();
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
    }
}
