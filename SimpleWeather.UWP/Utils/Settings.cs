using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.Utils
{
    public static partial class Settings
    {
        // Shared Settings
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        // App data files
        private static StorageFolder appDataFolder = ApplicationData.Current.LocalFolder;
        private static StorageFile dataFile;

        private static async void Init()
        {
            if (dataFile == null)
                dataFile = await appDataFolder.CreateFileAsync("data.json", CreationCollisionOption.OpenIfExists);
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
            Task.Factory.StartNew(async () => await LoadIfNeeded()).Unwrap().Wait();

            FileInfo fileinfo = new FileInfo(dataFile.Path);

            if (fileinfo.Length == 0 || !fileinfo.Exists)
            {
                SetWeatherLoaded(false);
                return false;
            }

            if (weatherData.Count > 0)
            {
                SetWeatherLoaded(true);
                return true;
            }
            else if (localSettings.Values[KEY_WEATHERLOADED] == null)
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
                SetAPI(API_WUnderground);
                return API_WUnderground;
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
                key = ReadAPIKEYfile().GetAwaiter().GetResult();

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
            if (!String.IsNullOrWhiteSpace(API_KEY))
                localSettings.Values[KEY_APIKEY] = API_KEY;
        }
        #endregion
    }
}
