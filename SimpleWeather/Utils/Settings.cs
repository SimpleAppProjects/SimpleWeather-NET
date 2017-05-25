using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static partial class Settings
    {
        public static bool WeatherLoaded { get { return isWeatherLoaded(); } set { setWeatherLoaded(value); } }
        public static string Unit { get { return getTempUnit(); } set { setTempUnit(value); } }
        public static string API { get { return getAPI(); } set { setAPI(value); } }
        public static string API_KEY { get { return getAPIKEY(); } set { setAPIKEY(value); } }

        // Units
        private static string Fahrenheit = "F";
        private static string Celsius = "C";

        // Settings Keys
        private static string KEY_API = "API";
        private static string KEY_APIKEY = "API_KEY";
        private static string KEY_USECELSIUS = "key_usecelsius";
        private static string KEY_UNITS = "Units";
        private static string KEY_WEATHERLOADED = "weatherLoaded";

        // Weather Data
        private static OrderedDictionary weatherData = new OrderedDictionary();
        private static bool loaded = false;

        static Settings()
        {
            init();
        }

        public static async Task loadIfNeeded()
        {
            if (!loaded)
            {
                await load();
                loaded = true;
            }
        }

        private static async Task load()
        {
            System.IO.FileInfo fileinfo = new System.IO.FileInfo(dataFile.Path);

            if (!fileinfo.Exists || (fileinfo.Exists && fileinfo.Length == 0))
                return;

            weatherData = (OrderedDictionary)JSONParser.Deserializer(await FileUtils.ReadFile(dataFile), typeof(OrderedDictionary));
        }

        public static async Task<List<string>> getLocations()
        {
            await loadIfNeeded();
            return weatherData.Keys.Cast<string>().ToList();
        }

        public static async Task<OrderedDictionary> getWeatherData()
        {
            await loadIfNeeded();
            return weatherData;
        }

        public static void saveWeatherData()
        {
            JSONParser.Serializer(weatherData, dataFile);
        }
    }
}
