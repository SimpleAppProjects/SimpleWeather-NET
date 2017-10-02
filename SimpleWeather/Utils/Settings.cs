using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Utils
{
    public static partial class Settings
    {
        // Settings Members
        public static bool WeatherLoaded { get { return IsWeatherLoaded(); } set { SetWeatherLoaded(value); } }
        public static string Unit { get { return GetTempUnit(); } set { SetTempUnit(value); } }
        public static string API { get { return GetAPI(); } set { SetAPI(value); } }
        public static string API_KEY { get { return GetAPIKEY(); } set { SetAPIKEY(value); } }
        public static bool FollowGPS { get { return UseFollowGPS(); } set { SetFollowGPS(value); } }
        private static string LastGPSLocation { get { return GetLastGPSLocation(); } set { SetLastGPSLocation(value); } }
        public static int RefreshInterval { get { return GetRefreshInterval(); } set { SetRefreshInterval(value); } }
        public static LocationData HomeData { get { return GetHomeData(); } }
        public static DateTime UpdateTime { get { return GetUpdateTime(); } set { SetUpdateTime(value); } }

        // Data
        public static List<LocationData> LocationData { get { return Task.Run(() => GetLocationData()).Result; } }
        public static OrderedDictionary WeatherData { get { return Task.Run(() => GetWeatherData()).Result; } }

        // Units
        public const string Fahrenheit = "F";
        public const string Celsius = "C";
        private const string DEFAULT_UPDATE_INTERVAL = "30"; // 30 minutes

        // Settings Keys
        private const string KEY_API = "API";
        private const string KEY_APIKEY = "API_KEY";
        private const string KEY_USECELSIUS = "key_usecelsius";
        private const string KEY_UNITS = "Units";
        private const string KEY_WEATHERLOADED = "weatherLoaded";
        private const string KEY_FOLLOWGPS = "key_followgps";
        private const string KEY_LASTGPSLOCATION = "key_lastgpslocation";
        private const string KEY_REFRESHINTERVAL = "key_refreshinterval";
        private const string KEY_UPDATETIME = "key_updatetime";

        // APIs
        public const string API_WUnderground = "WUnderground";
        public const string API_Yahoo = "Yahoo";

        // Weather Data
        private static List<LocationData> locationData = new List<LocationData>();
        private static OrderedDictionary weatherData = new OrderedDictionary();
        private static LocationData lastGPSLocData = new LocationData();
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

            weatherData = await JSONParser.DeserializerAsync<OrderedDictionary>(dataFile);
            locationData = await JSONParser.DeserializerAsync<List<LocationData>>(locDataFile);

            if (!String.IsNullOrWhiteSpace(LastGPSLocation))
            {
                lastGPSLocData = await JSONParser.DeserializerAsync<LocationData>(LastGPSLocation);
            }

            // Setup location data if N/A
            if (locationData == null || locationData.Count == 0)
            {
                List<LocationData> data = new List<LocationData>();
                List<string> weatherDataKeys = weatherData.Keys.Cast<string>().ToList();

                foreach (string query in weatherDataKeys)
                {
                    LocationData loc = new LocationData(query)
                    {
                        longitude = double.Parse((weatherData[query] as Weather).location.longitude),
                        latitude = double.Parse((weatherData[query] as Weather).location.latitude)
                    };
                    data.Add(loc);
                }

                locationData = data;
                SaveLocationData();
            }
        }

        private static async Task<List<LocationData>> GetLocationData()
        {
            await LoadIfNeeded();
            return locationData;
        }

        private static async Task<OrderedDictionary> GetWeatherData()
        {
            await LoadIfNeeded();
            return weatherData;
        }

        public static async Task<LocationData> GetLastGPSLocData()
        {
            await LoadIfNeeded();

            if (lastGPSLocData != null && lastGPSLocData.locationType != LocationType.GPS)
                lastGPSLocData.locationType = LocationType.GPS;

            return lastGPSLocData;
        }

        public static void SaveWeatherData()
        {
            JSONParser.Serializer(weatherData, dataFile);
        }

        public static void SaveLocationData()
        {
            JSONParser.Serializer(locationData, locDataFile);
        }

        public static void SaveLastGPSLocData()
        {
            LastGPSLocation = JSONParser.Serializer(lastGPSLocData, typeof(LocationData));
        }

        public static void SaveLastGPSLocData(LocationData data)
        {
            lastGPSLocData = data;
            LastGPSLocation = JSONParser.Serializer(lastGPSLocData, typeof(LocationData));
        }

        private static LocationData GetHomeData()
        {
            LocationData homeData = null;

            if (FollowGPS)
                homeData = Task.Run(() => GetLastGPSLocData()).Result;
            else
                homeData = LocationData.First();

            return homeData;
        }
    }
}
