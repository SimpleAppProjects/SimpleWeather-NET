using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SimpleWeather.Utils
{
    public static class Settings
    {
        public static bool WeatherLoaded { get { return isWeatherLoaded(); } set { setWeatherLoaded(value); } }
        public static string Unit { get { return getTempUnit(); } set { setTempUnit(value); } }
        public static string API { get { return getAPI(); } set { setAPI(value); } }
        public static string API_KEY { get { return getAPIKEY(); } set { setAPIKEY(value); } }

        private static StorageFolder appDataFolder = ApplicationData.Current.LocalFolder;
        private static StorageFile locationsFile;

        private static string Fahrenheit = "F";
        private static string Celsius = "C";

        private static string getTempUnit()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.ContainsKey("Units") || localSettings.Values["Units"] == null)
            {
                return Fahrenheit;
            }
            else if (localSettings.Values["Units"].Equals("C"))
                return Celsius;

            return Fahrenheit;
        }

        private static void setTempUnit(string value)
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            if (value == Celsius)
                localSettings.Values["Units"] = Celsius;
            else
                localSettings.Values["Units"] = Fahrenheit;
        }

        private static bool isWeatherLoaded()
        {
            if (locationsFile == null)
                locationsFile = appDataFolder.CreateFileAsync("locations.json", CreationCollisionOption.OpenIfExists).AsTask().GetAwaiter().GetResult();

            FileInfo fileinfo = new FileInfo(locationsFile.Path);

            if (fileinfo.Length == 0 || !fileinfo.Exists)
            {
                setWeatherLoaded(false);
                return false;
            }

            var localSettings = ApplicationData.Current.LocalSettings;

            if (localSettings.Values["weatherLoaded"] == null)
            {
                return false;
            }
            else if (localSettings.Values["weatherLoaded"].Equals("true"))
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
            var localSettings = ApplicationData.Current.LocalSettings;

            if (isLoaded)
            {
                localSettings.Values["weatherLoaded"] = "true";
            }
            else
            {
                localSettings.Values["weatherLoaded"] = "false";
            }
        }

        private static string getAPI()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.ContainsKey("API") || localSettings.Values["API"] == null)
            {
                setAPI("WUnderground");
                return "WUnderground";
            }
            else
                return (string)localSettings.Values["API"];
        }

        private static void setAPI(string value)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["API"] = value;
        }

        #region Yahoo Weather
        public static async Task<List<WeatherUtils.Coordinate>> getLocations()
        {
            if (locationsFile == null)
                locationsFile = await appDataFolder.CreateFileAsync("locations.json", CreationCollisionOption.OpenIfExists);

            FileInfo fileinfo = new FileInfo(locationsFile.Path);

            if (fileinfo.Length == 0 || !fileinfo.Exists)
                return null;

            List<WeatherUtils.Coordinate> locations = (List<WeatherUtils.Coordinate>) JSONParser.Deserializer(await FileUtils.ReadFile(locationsFile), typeof(List<WeatherUtils.Coordinate>));
            return locations;
        }

        public static async void saveLocations(List<WeatherUtils.Coordinate> locations)
        {
            if (locationsFile == null)
                locationsFile = await appDataFolder.CreateFileAsync("locations.json", CreationCollisionOption.OpenIfExists);

            JSONParser.Serializer(locations, locationsFile);
        }
        #endregion

        #region WeatherUnderground
        public static async Task<List<string>> getLocations_WU()
        {
            if (locationsFile == null)
                locationsFile = await appDataFolder.CreateFileAsync("locations.json", CreationCollisionOption.OpenIfExists);

            FileInfo fileinfo = new FileInfo(locationsFile.Path);

            if (fileinfo.Length == 0 || !fileinfo.Exists)
                return null;

            List<string> locations = (List<string>) JSONParser.Deserializer(await FileUtils.ReadFile(locationsFile), typeof(List<string>));
            return locations;
        }

        public static async void saveLocations(List<string> locations)
        {
            if (locationsFile == null)
                locationsFile = await appDataFolder.CreateFileAsync("locations.json", CreationCollisionOption.OpenIfExists);

            JSONParser.Serializer(locations, locationsFile);
        }

        private static string getAPIKEY()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.ContainsKey("API_KEY") || localSettings.Values["API_KEY"] == null)
            {
                String key = String.Empty;
                key = readAPIKEYfile().GetAwaiter().GetResult();

                if (!String.IsNullOrWhiteSpace(key))
                    setAPIKEY(key);

                return key;
            }
            else
                return (string)localSettings.Values["API_KEY"];
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
            var localSettings = ApplicationData.Current.LocalSettings;

            if (!String.IsNullOrWhiteSpace(API_KEY))
                localSettings.Values["API_KEY"] = API_KEY;
        }
        #endregion
    }
}
