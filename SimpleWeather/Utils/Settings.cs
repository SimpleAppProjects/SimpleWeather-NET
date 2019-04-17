using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SimpleWeather.WeatherData;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;
using SimpleWeather.Location;

namespace SimpleWeather.Utils
{
    public static partial class Settings
    {
        // Settings Members
        public static bool WeatherLoaded { get { return IsWeatherLoaded(); } set { SetWeatherLoaded(value); } }
        public static bool OnBoardComplete { get { return IsOnBoardingComplete(); } set { SetOnBoardingComplete(value); } }
        public static string Unit { get { return GetTempUnit(); } set { SetTempUnit(value); } }
        public static string API { get { return GetAPI(); } set { SetAPI(value); } }
        public static string API_KEY { get { return GetAPIKEY(); } set { SetAPIKEY(value); } }
        public static bool KeyVerified { get { return IsKeyVerified(); } set { SetKeyVerified(value); } }
        public static bool IsFahrenheit { get { return Unit == Fahrenheit; } }
        public static bool FollowGPS { get { return UseFollowGPS(); } set { SetFollowGPS(value); } }
        private static string LastGPSLocation { get { return GetLastGPSLocation(); } set { SetLastGPSLocation(value); } }
        public static LocationData HomeData { get { return GetHomeData(); } }
        public static DateTime UpdateTime { get { return GetUpdateTime(); } set { SetUpdateTime(value); } }
        public static int RefreshInterval { get { return GetRefreshInterval(); } set { SetRefreshInterval(value); } }
        public static bool ShowAlerts { get { return UseAlerts(); } set { SetAlerts(value); } }
        public static bool UsePersonalKey { get { return IsPersonalKey(); } set { SetPersonalKey(value); } }
        public static int VersionCode { get { return GetVersionCode(); } set { SetVersionCode(value); } }

        // Database
        internal static int DBVersion { get { return GetDBVersion(); } set { SetDBVersion(value); } }

        // Data
        internal const int CurrentDBVersion = 4;
        private static SQLiteAsyncConnection locationDB;
        private static SQLiteAsyncConnection weatherDB;
        private const int CACHE_LIMIT = 10;
        public const int MAX_LOCATIONS = 10;

        // Units
        public const string Fahrenheit = "F";
        public const string Celsius = "C";
        private const string DEFAULT_UPDATE_INTERVAL = "60"; // 60 minutes (1hr)
        public const int DefaultInterval = 60;
        public const int READ_TIMEOUT = 10000; // 10s

        // Settings Keys
        private const string KEY_API = "API";
        private const string KEY_APIKEY = "API_KEY";
        private const string KEY_APIKEY_VERIFIED = "API_KEY_VERIFIED";
        public const string KEY_USECELSIUS = "key_usecelsius";
        public const string KEY_UNITS = "Units";
        private const string KEY_WEATHERLOADED = "weatherLoaded";
        private const string KEY_FOLLOWGPS = "key_followgps";
        private const string KEY_LASTGPSLOCATION = "key_lastgpslocation";
        public const string KEY_REFRESHINTERVAL = "key_refreshinterval";
        private const string KEY_UPDATETIME = "key_updatetime";
        private const string KEY_DBVERSION = "key_dbversion";
        public const string KEY_USEALERTS = "key_usealerts";
        private const string KEY_USEPERSONALKEY = "key_usepersonalkey";
        private const string KEY_CURRENTVERSION = "key_currentversion";
        private const string KEY_ONBOARDINGCOMPLETE = "key_onboardcomplete";

        // Weather Data
        private static LocationData lastGPSLocData = new LocationData();
        private static bool loaded = false;

        static Settings()
        {
            Init();
        }

        private static async Task LoadIfNeeded()
        {
            if (!loaded)
            {
                await Load();
                loaded = true;
            }
        }

        private static async Task Load()
        {
            // Create DB tables
            await locationDB.CreateTableAsync<LocationData>();
            await locationDB.CreateTableAsync<Favorites>();
            await weatherDB.CreateTableAsync<Weather>();
            await weatherDB.CreateTableAsync<WeatherAlerts>();

            // Migrate old data if available
            await DataMigrations.PerformDBMigrations(weatherDB, locationDB);

            if (!String.IsNullOrWhiteSpace(LastGPSLocation))
            {
                try
                {
                    using (var jsonTextReader = new JsonTextReader(new StringReader(LastGPSLocation)))
                    {
                        lastGPSLocData = LocationData.FromJson(jsonTextReader);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: Settings.Load(): LastGPSLocation");
                }
                finally
                {
                    if (lastGPSLocData == null || String.IsNullOrWhiteSpace(lastGPSLocData.tz_long))
                        lastGPSLocData = new LocationData();
                }
            }

            await DataMigrations.PerformVersionMigrations(weatherDB, locationDB);
        }

        public static async Task<List<LocationData>> GetFavorites()
        {
            await LoadIfNeeded();
            var query = from loc in await locationDB.Table<LocationData>().ToListAsync()
                        join favs in await locationDB.Table<Favorites>().ToListAsync()
                        on loc.query equals favs.query
                        orderby favs.position
                        select new LocationData
                        {
                            query = loc.query,
                            name = loc.name,
                            latitude = loc.latitude,
                            longitude = loc.longitude,
                            locationType = loc.locationType,
                            tz_long = loc.tz_long,
                            weatherSource = loc.weatherSource,
                            locationSource = loc.locationSource
                        };
            return query.ToList();
        }

        public static async Task<List<LocationData>> GetLocationData()
        {
            await LoadIfNeeded();
            return await locationDB.Table<LocationData>().ToListAsync();
        }

        public static async Task<LocationData> GetLocation(string key)
        {
            await LoadIfNeeded();
            return await locationDB.FindAsync<LocationData>(key);
        }

        public static async Task<Weather> GetWeatherData(string key)
        {
            await LoadIfNeeded();
            return await weatherDB.FindWithChildrenAsync<Weather>(key);
        }

        public static async Task<Weather> GetWeatherDataByCoordinate(LocationData location)
        {
            await LoadIfNeeded();
            List<Weather> weatherData = await weatherDB.GetAllWithChildrenAsync<Weather>(weather => 
                    location.latitude.Equals(weather.location.latitude) && location.longitude.Equals(weather.location.longitude));

            return weatherData.FirstOrDefault();
        }

        public static async Task<List<WeatherAlert>> GetWeatherAlertData(string key)
        {
            await LoadIfNeeded();

            List<WeatherAlert> alerts = null;

            try
            {
                var weatherAlertData = await weatherDB.FindWithChildrenAsync<WeatherAlerts>(key);

                if (weatherAlertData != null && weatherAlertData.alerts != null)
                    alerts = weatherAlertData.alerts;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: Settings.GetWeatherAlertData()");
            }
            finally
            {
                if (alerts == null)
                    alerts = new List<WeatherAlert>();
            }

            return alerts;
        }

        public static async Task<LocationData> GetLastGPSLocData()
        {
            await LoadIfNeeded();

            if (lastGPSLocData != null && lastGPSLocData.locationType != LocationType.GPS)
                lastGPSLocData.locationType = LocationType.GPS;

            return lastGPSLocData;
        }

        public static async Task SaveWeatherData(Weather weather)
        {
            if (weather != null && weather.IsValid())
            {
                await weatherDB.InsertOrReplaceAsync(weather);
                await WriteOperations.UpdateWithChildrenAsync(weatherDB, weather);
            }

            if (await weatherDB.Table<Weather>().CountAsync() > CACHE_LIMIT)
                CleanupWeatherData();
        }

        public static async Task SaveWeatherAlerts(LocationData location, List<WeatherAlert> alerts)
        {
            if (location != null && location.IsValid())
            {
                var alertdata = new WeatherAlerts(location.query, alerts);
                await weatherDB.InsertOrReplaceAsync(alertdata);
                await weatherDB.UpdateWithChildrenAsync(alertdata);
            }

            if (await weatherDB.Table<WeatherAlerts>().CountAsync() > CACHE_LIMIT)
                CleanupWeatherAlertData();
        }

        private static void CleanupWeatherData()
        {
            Task.Run(async () =>
            {
                var locs = await locationDB.Table<LocationData>().ToListAsync();
                if (FollowGPS) locs.Add(lastGPSLocData);
                var data = await weatherDB.Table<Weather>().ToListAsync();
                var weatherToDelete = data.Where(w => locs.All(l => l.query != w.query));

                foreach (Weather weather in data)
                {
                    await weatherDB.DeleteAsync<Weather>(weather.query);
                }
            });
        }

        private static void CleanupWeatherAlertData()
        {
            Task.Run(async () =>
            {
                var locs = await locationDB.Table<LocationData>().ToListAsync();
                if (FollowGPS) locs.Add(lastGPSLocData);
                var data = await weatherDB.Table<WeatherAlerts>().ToListAsync();
                var weatherToDelete = data.Where(w => locs.All(l => l.query != w.query));

                foreach (WeatherAlerts alertdata in data)
                {
                    await weatherDB.DeleteAsync<WeatherAlerts>(alertdata.query);
                }
            });
        }

        public static async Task SaveLocationData(List<LocationData> locationData)
        {
            if (locationData != null)
            {
                var favs = new List<Favorites>(locationData.Count);
                foreach (LocationData loc in locationData)
                {
                    if (loc != null && loc.IsValid())
                    {
                        await locationDB.InsertOrReplaceAsync(loc);
                        var fav = new Favorites() { query = loc.query, position = locationData.IndexOf(loc) };
                        favs.Add(fav);
                        await locationDB.InsertOrReplaceAsync(fav);
                    }
                }

                var locs = await locationDB.Table<LocationData>().ToListAsync();
                var locToDelete = locs.Where(l => locationData.All(l2 => !l2.Equals(l)));
                int count = locToDelete.Count();

                if (count > 0)
                {
                    foreach (LocationData loc in locToDelete)
                    {
                        await locationDB.DeleteAsync<LocationData>(loc.query);
                        await locationDB.DeleteAsync<Favorites>(loc.query);
                    }
                }
            }
        }

        public static async Task AddLocation(LocationData location)
        {
            if (location != null && location.IsValid())
            {
                await locationDB.InsertOrReplaceAsync(location);
                int pos = await locationDB.Table<LocationData>().CountAsync();
                await locationDB.InsertOrReplaceAsync(new Favorites() { query = location.query, position = pos });
            }
        }

        public static async Task UpdateLocation(LocationData location)
        {
            if (location != null && location.IsValid())
            {
                await locationDB.UpdateAsync(location);
            }
        }

        public static async Task UpdateLocationWithKey(LocationData location, string oldKey)
        {
            if (location != null && location.IsValid() && !String.IsNullOrWhiteSpace(oldKey))
            {
                // Get position from favorites table
                var favs = await locationDB.Table<Favorites>().ToListAsync();
                var fav = favs.Find(f => f.query == oldKey);

                if (fav == null)
                {
                    return;
                }

                int pos = fav.position;

                // Remove location from table
                await locationDB.DeleteAsync<LocationData>(oldKey);
                await locationDB.QueryAsync<Favorites>("delete from favorites where query = ?", oldKey);

                // Add updated location with new query (pkey)
                await locationDB.InsertOrReplaceAsync(location);
                await locationDB.InsertOrReplaceAsync(new Favorites() { query = location.query, position = pos });
            }
        }

        public static async Task DeleteLocations()
        {
            await locationDB.DeleteAllAsync<LocationData>();
            await locationDB.DeleteAllAsync<Favorites>();
        }

        public static async Task DeleteLocation(string key)
        {
            if (!String.IsNullOrWhiteSpace(key))
            {
                await locationDB.DeleteAsync<LocationData>(key);
                await locationDB.QueryAsync<Favorites>("delete from favorites where query = ?", key);
                await ResetPosition();
            }
        }

        public static async Task MoveLocation(string key, int toPos)
        {
            if (!String.IsNullOrWhiteSpace(key))
            {
                await locationDB.QueryAsync<Favorites>("update favorites set position = ? where query = ?",
                    toPos, key);
            }
        }

        private static async Task ResetPosition()
        {
            var favs = await locationDB.Table<Favorites>().OrderBy(f => f.position).ToListAsync();
            foreach(Favorites fav in favs)
            {
                fav.position = favs.IndexOf(fav);
                await locationDB.UpdateAsync(fav);
            }
        }

        public static void SaveLastGPSLocData(LocationData data)
        {
            lastGPSLocData = data;
            LastGPSLocation = lastGPSLocData?.ToJson();
        }

        private static LocationData GetHomeData()
        {
            LocationData homeData = null;

            if (FollowGPS)
                homeData = Task.Run(() => GetLastGPSLocData()).Result;
            else
                homeData = Task.Run(async () => (await GetFavorites()).FirstOrDefault()).Result;

            return homeData;
        }
    }
}
