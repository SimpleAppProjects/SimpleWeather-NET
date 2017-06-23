using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static partial class Settings
    {
        // Settings Members
        public static bool WeatherLoaded { get { return IsWeatherLoaded(); } set { SetWeatherLoaded(value); } }
        public static string Unit { get { return GetTempUnit(); } set { SetTempUnit(value); } }
        public static string API { get { return GetAPI(); } set { SetAPI(value); } }
        public static string API_KEY { get { return GetAPIKEY(); } set { SetAPIKEY(value); } }

        // Units
        public const string Fahrenheit = "F";
        public const string Celsius = "C";

        // Settings Keys
        private const string KEY_API = "API";
        private const string KEY_APIKEY = "API_KEY";
        private const string KEY_USECELSIUS = "key_usecelsius";
        private const string KEY_UNITS = "Units";
        private const string KEY_WEATHERLOADED = "weatherLoaded";

        // APIs
        public const string API_WUnderground = "WUnderground";
        public const string API_Yahoo = "Yahoo";

        // Weather Data
        private static OrderedDictionary weatherData = new OrderedDictionary();
        private static bool loaded = false;

        static Settings()
        {
            Init();
        }

        public static async Task LoadIfNeeded()
        {
            if (!loaded)
            {
                await Load();
                loaded = true;
            }
        }

        private static async Task Load()
        {
            System.IO.FileInfo fileinfo = new System.IO.FileInfo(dataFile.Path);

            if (!fileinfo.Exists || (fileinfo.Exists && fileinfo.Length == 0))
                return;

            weatherData = await JSONParser.DeserializerAsync<OrderedDictionary>(await FileUtils.ReadFile(dataFile));
        }

        public static async Task<List<string>> GetLocations()
        {
            await LoadIfNeeded();
            return weatherData.Keys.Cast<string>().ToList();
        }

        public static async Task<OrderedDictionary> GetWeatherData()
        {
            await LoadIfNeeded();
            return weatherData;
        }

        public static void SaveWeatherData()
        {
            JSONParser.Serializer(weatherData, dataFile);
        }
    }
}
