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

        private static async void init()
        {
            if (dataFile == null)
                dataFile = await appDataFolder.CreateFileAsync("data.json", CreationCollisionOption.OpenIfExists);
        }

        private static string getTempUnit()
        {
            if (!localSettings.Values.ContainsKey(KEY_UNITS) || localSettings.Values[KEY_UNITS] == null)
            {
                return Fahrenheit;
            }
            else if (localSettings.Values[KEY_UNITS].Equals(Celsius))
                return Celsius;

            return Fahrenheit;
        }

        private static void setTempUnit(string value)
        {
            if (value == Celsius)
                localSettings.Values[KEY_UNITS] = Celsius;
            else
                localSettings.Values[KEY_UNITS] = Fahrenheit;
        }

        private static bool isWeatherLoaded()
        {
            Task.Factory.StartNew(async () => await loadIfNeeded()).Unwrap().Wait();

            FileInfo fileinfo = new FileInfo(dataFile.Path);

            if (fileinfo.Length == 0 || !fileinfo.Exists)
            {
                setWeatherLoaded(false);
                return false;
            }

            if (weatherData.Count > 0)
            {
                setWeatherLoaded(true);
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

        private static void setWeatherLoaded(bool isLoaded)
        {
            localSettings.Values[KEY_WEATHERLOADED] = isLoaded;
        }

        private static string getAPI()
        {
            if (!localSettings.Values.ContainsKey(KEY_API) || localSettings.Values[KEY_API] == null)
            {
                setAPI("WUnderground");
                return "WUnderground";
            }
            else
                return (string)localSettings.Values[KEY_API];
        }

        private static void setAPI(string value)
        {
            localSettings.Values[KEY_API] = value;
        }

        #region WeatherUnderground
        private static string getAPIKEY()
        {
            if (!localSettings.Values.ContainsKey(KEY_APIKEY) || localSettings.Values[KEY_APIKEY] == null)
            {
                String key = String.Empty;
                key = readAPIKEYfile().GetAwaiter().GetResult();

                if (!String.IsNullOrWhiteSpace(key))
                    setAPIKEY(key);

                return key;
            }
            else
                return (string)localSettings.Values[KEY_APIKEY];
        }

        private static async Task<string> readAPIKEYfile()
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

        private static void setAPIKEY(string API_KEY)
        {
            if (!String.IsNullOrWhiteSpace(API_KEY))
                localSettings.Values[KEY_APIKEY] = API_KEY;
        }
        #endregion
    }
}
