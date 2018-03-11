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

namespace SimpleWeather.Utils
{
    public static partial class Settings
    {
        // Settings Members
        public static bool WeatherLoaded { get { return IsWeatherLoaded(); } set { SetWeatherLoaded(value); } }
        public static string Unit { get { return GetTempUnit(); } set { SetTempUnit(value); } }
        public static string API { get { return GetAPI(); } set { SetAPI(value); } }
        public static string API_KEY { get { return GetAPIKEY(); } set { SetAPIKEY(value); } }
        public static bool KeyVerified { get { return IsKeyVerified(); } set { SetKeyVerified(value); } }
        public static bool IsFahrenheit { get { return Unit == Fahrenheit; } }
        public static bool FollowGPS { get { return UseFollowGPS(); } set { SetFollowGPS(value); } }
        private static string LastGPSLocation { get { return GetLastGPSLocation(); } set { SetLastGPSLocation(value); } }
        public static LocationData HomeData { get { return GetHomeData(); } }
#if !__ANDROID_WEAR__
        public static int RefreshInterval { get { return GetRefreshInterval(); } set { SetRefreshInterval(value); } }
        public static DateTime UpdateTime { get { return GetUpdateTime(); } set { SetUpdateTime(value); } }
        public static bool ShowAlerts { get { return UseAlerts(); } set { SetAlerts(value); } }
#endif

        // Database
        private static int DBVersion { get { return GetDBVersion(); } set { SetDBVersion(value); } }

        // Data
        private const int CurrentDBVersion = 2;
        private static SQLiteAsyncConnection locationDB;
        private static SQLiteAsyncConnection weatherDB;
        private const int CACHE_LIMIT = 10;

        // Units
        public const string Fahrenheit = "F";
        public const string Celsius = "C";
        private const string DEFAULT_UPDATE_INTERVAL = "30"; // 30 minutes

        // Settings Keys
        private const string KEY_API = "API";
        private const string KEY_APIKEY = "API_KEY";
        private const string KEY_APIKEY_VERIFIED = "API_KEY_VERIFIED";
        private const string KEY_USECELSIUS = "key_usecelsius";
        private const string KEY_UNITS = "Units";
        private const string KEY_WEATHERLOADED = "weatherLoaded";
        private const string KEY_FOLLOWGPS = "key_followgps";
        private const string KEY_LASTGPSLOCATION = "key_lastgpslocation";
        private const string KEY_REFRESHINTERVAL = "key_refreshinterval";
        private const string KEY_UPDATETIME = "key_updatetime";
        private const string KEY_DBVERSION = "key_dbversion";
        private const string KEY_USEALERTS = "key_usealerts";

        // Weather Data
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
            // Create DB tables
            await locationDB.CreateTableAsync<LocationData>();
            await locationDB.CreateTableAsync<Favorites>();
            await weatherDB.CreateTableAsync<Weather>();
            await weatherDB.CreateTableAsync<WeatherAlerts>();

            // Migrate old data if available
            if (GetDBVersion() < CurrentDBVersion)
            {
                switch (GetDBVersion())
                {
                    // Move data from json to db
                    case 0:
                        if (await locationDB.Table<LocationData>().CountAsync() == 0)
                            await DBUtils.MigrateDataJsonToDB(locationDB, weatherDB);
                        break;
                    // Add and set tz_long column in db
                    case 1:
                        if (await locationDB.Table<LocationData>().CountAsync() > 0)
                            await DBUtils.SetLocationData(locationDB);
                        break;
                }

                SetDBVersion(CurrentDBVersion);
            }

            if (!String.IsNullOrWhiteSpace(LastGPSLocation))
            {
                try
                {
                    lastGPSLocData = LocationData.FromJson(new JsonTextReader(new StringReader(LastGPSLocation)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    if (lastGPSLocData == null || String.IsNullOrWhiteSpace(lastGPSLocData.tz_long))
                        lastGPSLocData = new LocationData();
                }
            }
        }

        public static async Task<List<LocationData>> GetFavorites()
        {
            await LoadIfNeeded();
            var query = from loc in await locationDB.Table<LocationData>().ToListAsync()
                        join favs in await locationDB.Table<Favorites>().ToListAsync()
                        on loc.query equals favs.query
                        orderby favs.position
                        select new LocationData()
                        {
                            query = loc.query,
                            name = loc.name,
                            latitude = loc.latitude,
                            longitude = loc.longitude,
                            locationType = loc.locationType,
                            tz_long = loc.tz_long,
                            source = loc.source
                        };
            return query.ToList();
        }

        public static async Task<List<LocationData>> GetLocationData()
        {
            await LoadIfNeeded();
            return await locationDB.Table<LocationData>().ToListAsync();
        }

        public static async Task<Weather> GetWeatherData(string key)
        {
            await LoadIfNeeded();
            return await weatherDB.FindWithChildrenAsync<Weather>(key);
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
                Console.WriteLine(ex.StackTrace);
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
            await weatherDB.InsertOrReplaceAsync(weather);
            await WriteOperations.UpdateWithChildrenAsync(weatherDB, weather);

            if (await weatherDB.Table<Weather>().CountAsync() > CACHE_LIMIT)
                CleanupWeatherData();
        }

        public static async Task SaveWeatherAlerts(LocationData location, List<WeatherAlert> alerts)
        {
            var alertdata = new WeatherAlerts(location.query, alerts);
            await weatherDB.InsertOrReplaceAsync(alertdata);
            await WriteOperations.UpdateWithChildrenAsync(weatherDB, alertdata);

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
            var favs = new List<Favorites>(locationData.Count);
            foreach (LocationData loc in locationData)
            {
                await locationDB.InsertOrReplaceAsync(loc);
                var fav = new Favorites() { query = loc.query, position = locationData.IndexOf(loc) };
                favs.Add(fav);
                await locationDB.InsertAsync(fav);
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

        public static async Task AddLocation(LocationData location)
        {
            await locationDB.InsertOrReplaceAsync(location);
            int pos = await locationDB.Table<LocationData>().CountAsync();
            await locationDB.InsertAsync(new Favorites() { query = location.query, position = pos });
        }

        public static async Task UpdateLocation(LocationData location)
        {
            await locationDB.UpdateAsync(location);
        }

        public static async Task UpdateLocationWithKey(LocationData location, string oldKey)
        {
            // Get position from favorites table
            var favs = await locationDB.Table<Favorites>().ToListAsync();
            var fav = favs.Find(f => f.query == oldKey);
            int pos = fav.position;

            // Remove location from table
            await locationDB.DeleteAsync<LocationData>(oldKey);
            await locationDB.QueryAsync<Favorites>("delete from favorites where query = ?", oldKey);

            // Add updated location with new query (pkey)
            await locationDB.InsertOrReplaceAsync(location);
            await locationDB.InsertAsync(new Favorites() { query = location.query, position = pos });
        }

        public static async Task DeleteLocations()
        {
            await locationDB.DeleteAllAsync<LocationData>();
            await locationDB.DeleteAllAsync<Favorites>();
        }

        public static async Task DeleteLocation(string key)
        {
            await locationDB.DeleteAsync<LocationData>(key);
            await locationDB.QueryAsync<Favorites>("delete from favorites where query = ?", key);
            await ResetPosition();
        }

        public static async Task MoveLocation(string key, int toPos)
        {
            await locationDB.QueryAsync<Favorites>("update favorites set position = ? where query = ?",
                toPos, key);
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

        public static void SaveLastGPSLocData()
        {
            LastGPSLocation = lastGPSLocData.ToJson();
        }

        public static void SaveLastGPSLocData(LocationData data)
        {
            lastGPSLocData = data;
            LastGPSLocation = lastGPSLocData.ToJson();
        }

        private static LocationData GetHomeData()
        {
            LocationData homeData = null;

            if (FollowGPS)
                homeData = Task.Run(() => GetLastGPSLocData()).Result;
            else
                homeData = Task.Run(async () => (await GetFavorites()).First()).Result;

            return homeData;
        }
    }
}
