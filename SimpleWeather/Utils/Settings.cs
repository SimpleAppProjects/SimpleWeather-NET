using Newtonsoft.Json;
using SimpleWeather.Location;
using SimpleWeather.WeatherData;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

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
        private const int CACHE_LIMIT = 25;
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
        private const string KEY_USERTHEME = "key_usertheme";

        // Weather Data
        private static LocationData lastGPSLocData = new LocationData();
        private static bool loaded = false;

        static Settings()
        {
            Init();
        }

        public static void LoadIfNeeded()
        {
            if (!loaded)
            {
                Load().GetAwaiter().GetResult();
                loaded = true;
            }
        }

        private static ConfiguredTaskAwaitable Load()
        {
            return AsyncTask.RunAsync(async () =>
            {
                // Create DB tables
                await AsyncTask.RunAsync(locationDB.CreateTableAsync<LocationData>());
                await AsyncTask.RunAsync(locationDB.CreateTableAsync<Favorites>());
                await AsyncTask.RunAsync(weatherDB.CreateTableAsync<Weather>());
                await AsyncTask.RunAsync(weatherDB.CreateTableAsync<WeatherAlerts>());

                // Migrate old data if available
                await AsyncTask.RunAsync(DataMigrations.PerformDBMigrations(weatherDB, locationDB));

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

                DataMigrations.PerformVersionMigrations(weatherDB, locationDB);
            });
        }

        public static ConfiguredTaskAwaitable<IEnumerable<LocationData>> GetFavorites()
        {
            return AsyncTask.RunAsync(async () =>
            {
                LoadIfNeeded();
                var query = from loc in await AsyncTask.RunAsync(locationDB.Table<LocationData>().ToListAsync())
                            join favs in await AsyncTask.RunAsync(locationDB.Table<Favorites>().ToListAsync())
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
                return query;
            });
        }

        public static ConfiguredTaskAwaitable<List<LocationData>> GetLocationData()
        {
            return AsyncTask.RunAsync(async () =>
            {
                LoadIfNeeded();
                return await AsyncTask.RunAsync(locationDB.Table<LocationData>().ToListAsync());
            });
        }

        public static ConfiguredTaskAwaitable<LocationData> GetLocation(string key)
        {
            return AsyncTask.RunAsync(async () =>
            {
                LoadIfNeeded();
                return await AsyncTask.RunAsync(locationDB.FindAsync<LocationData>(key));
            });
        }

        public static ConfiguredTaskAwaitable<Weather> GetWeatherData(string key)
        {
            return AsyncTask.RunAsync(async () =>
            {
                LoadIfNeeded();
                return await AsyncTask.RunAsync(weatherDB.FindWithChildrenAsync<Weather>(key));
            });
        }

        public static ConfiguredTaskAwaitable<Weather> GetWeatherDataByCoordinate(LocationData location)
        {
            return AsyncTask.RunAsync(async () =>
            {
                LoadIfNeeded();
                List<Weather> weatherData = await AsyncTask.RunAsync(weatherDB.GetAllWithChildrenAsync<Weather>());
                var filteredData = weatherData.FirstOrDefault(weather =>
                        String.Equals(location.latitude.ToString(), weather.location.latitude) && String.Equals(location.longitude.ToString(), weather.location.longitude));

                return filteredData;
            });
        }

        public static ConfiguredTaskAwaitable<ICollection<WeatherAlert>> GetWeatherAlertData(string key)
        {
            return AsyncTask.RunAsync(async () =>
            {
                LoadIfNeeded();

                ICollection<WeatherAlert> alerts = null;

                try
                {
                    var weatherAlertData = await AsyncTask.RunAsync(weatherDB.FindWithChildrenAsync<WeatherAlerts>(key));

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
            });
        }

        public static ConfiguredTaskAwaitable<LocationData> GetLastGPSLocData()
        {
            return AsyncTask.RunAsync(() =>
            {
                LoadIfNeeded();

                if (lastGPSLocData != null && lastGPSLocData.locationType != LocationType.GPS)
                    lastGPSLocData.locationType = LocationType.GPS;

                return lastGPSLocData;
            });
        }

        public static ConfiguredTaskAwaitable SaveWeatherData(Weather weather)
        {
            return AsyncTask.RunAsync(async () =>
            {
                if (weather != null && weather.IsValid())
                {
                    await AsyncTask.RunAsync(weatherDB.InsertOrReplaceAsync(weather));
                    await AsyncTask.RunAsync(WriteOperations.UpdateWithChildrenAsync(weatherDB, weather));
                }

                if (await AsyncTask.RunAsync(weatherDB.Table<Weather>().CountAsync()) > CACHE_LIMIT)
                    CleanupWeatherData();
            });
        }

        public static ConfiguredTaskAwaitable SaveWeatherAlerts(LocationData location, ICollection<WeatherAlert> alerts)
        {
            return AsyncTask.RunAsync(async () =>
            {
                if (location != null && location.IsValid())
                {
                    var alertdata = new WeatherAlerts(location.query, alerts);
                    await AsyncTask.RunAsync(weatherDB.InsertOrReplaceAsync(alertdata));
                    await AsyncTask.RunAsync(weatherDB.UpdateWithChildrenAsync(alertdata));
                }

                if (await AsyncTask.RunAsync(weatherDB.Table<WeatherAlerts>().CountAsync()) > CACHE_LIMIT)
                    CleanupWeatherAlertData();
            });
        }

        private static void CleanupWeatherData()
        {
            AsyncTask.Run(async () =>
            {
                var locs = await AsyncTask.RunAsync(locationDB.Table<LocationData>().ToListAsync());
                if (FollowGPS) locs.Add(lastGPSLocData);
                var data = await AsyncTask.RunAsync(weatherDB.Table<Weather>().ToListAsync());
                var weatherToDelete = data.Where(w => locs.All(l => l.query != w.query));

                foreach (Weather weather in data)
                {
                    await AsyncTask.RunAsync(weatherDB.DeleteAsync<Weather>(weather.query));
                }
            });
        }

        private static void CleanupWeatherAlertData()
        {
            AsyncTask.Run(async () =>
            {
                var locs = await AsyncTask.RunAsync(locationDB.Table<LocationData>().ToListAsync());
                if (FollowGPS) locs.Add(lastGPSLocData);
                var data = await AsyncTask.RunAsync(weatherDB.Table<WeatherAlerts>().ToListAsync());
                var weatherToDelete = data.Where(w => locs.All(l => l.query != w.query));

                foreach (WeatherAlerts alertdata in data)
                {
                    await AsyncTask.RunAsync(weatherDB.DeleteAsync<WeatherAlerts>(alertdata.query));
                }
            });
        }

        public static ConfiguredTaskAwaitable SaveLocationData(List<LocationData> locationData)
        {
            return AsyncTask.RunAsync(async () =>
            {
                if (locationData != null)
                {
                    var favs = new List<Favorites>(locationData.Count);
                    for (int i = 0; i < locationData.Count; i++)
                    {
                        var loc = locationData[i];

                        if (loc != null && loc.IsValid())
                        {
                            await AsyncTask.RunAsync(locationDB.InsertOrReplaceAsync(loc));
                            var fav = new Favorites() { query = loc.query, position = i };
                            favs.Add(fav);
                            await AsyncTask.RunAsync(locationDB.InsertOrReplaceAsync(fav));
                        }
                    }

                    var locs = await AsyncTask.RunAsync(locationDB.Table<LocationData>().ToListAsync());
                    var locToDelete = locs.Where(l => locationData.All(l2 => !l2.Equals(l)));
                    int count = locToDelete.Count();

                    if (count > 0)
                    {
                        foreach (LocationData loc in locToDelete)
                        {
                            await AsyncTask.RunAsync(locationDB.DeleteAsync<LocationData>(loc.query));
                            await AsyncTask.RunAsync(locationDB.DeleteAsync<Favorites>(loc.query));
                        }
                    }
                }
            });
        }

        public static ConfiguredTaskAwaitable AddLocation(LocationData location)
        {
            return AsyncTask.RunAsync(async () =>
            {
                if (location != null && location.IsValid())
                {
                    await AsyncTask.RunAsync(locationDB.InsertOrReplaceAsync(location));
                    int pos = await AsyncTask.RunAsync(locationDB.Table<LocationData>().CountAsync());
                    await AsyncTask.RunAsync(locationDB.InsertOrReplaceAsync(new Favorites() { query = location.query, position = pos }));
                }
            });
        }

        public static ConfiguredTaskAwaitable UpdateLocation(LocationData location)
        {
            return AsyncTask.RunAsync(async () =>
            {
                if (location != null && location.IsValid())
                {
                    await AsyncTask.RunAsync(locationDB.UpdateAsync(location));
                }
            });
        }

        public static ConfiguredTaskAwaitable UpdateLocationWithKey(LocationData location, string oldKey)
        {
            return AsyncTask.RunAsync(async () =>
            {
                if (location != null && location.IsValid() && !String.IsNullOrWhiteSpace(oldKey))
                {
                    // Get position from favorites table
                    var favs = await AsyncTask.RunAsync(locationDB.Table<Favorites>().ToListAsync());
                    var fav = favs.Find(f => f.query == oldKey);

                    if (fav == null)
                    {
                        return;
                    }

                    int pos = fav.position;

                    // Remove location from table
                    await AsyncTask.RunAsync(locationDB.DeleteAsync<LocationData>(oldKey));
                    await AsyncTask.RunAsync(locationDB.QueryAsync<Favorites>("delete from favorites where query = ?", oldKey));

                    // Add updated location with new query (pkey)
                    await AsyncTask.RunAsync(locationDB.InsertOrReplaceAsync(location));
                    await AsyncTask.RunAsync(locationDB.InsertOrReplaceAsync(new Favorites() { query = location.query, position = pos }));
                }
            });
        }

        public static ConfiguredTaskAwaitable DeleteLocations()
        {
            return AsyncTask.RunAsync(async () =>
            {
                await AsyncTask.RunAsync(locationDB.DeleteAllAsync<LocationData>());
                await AsyncTask.RunAsync(locationDB.DeleteAllAsync<Favorites>());
            });
        }

        public static ConfiguredTaskAwaitable DeleteLocation(string key)
        {
            return AsyncTask.RunAsync(async () =>
            {
                if (!String.IsNullOrWhiteSpace(key))
                {
                    await AsyncTask.RunAsync(locationDB.DeleteAsync<LocationData>(key));
                    await AsyncTask.RunAsync(locationDB.QueryAsync<Favorites>("delete from favorites where query = ?", key));
                    await ResetPosition();
                }
            });
        }

        public static ConfiguredTaskAwaitable MoveLocation(string key, int toPos)
        {
            return AsyncTask.RunAsync(async () =>
            {
                if (!String.IsNullOrWhiteSpace(key))
                {
                    await AsyncTask.RunAsync(locationDB.QueryAsync<Favorites>("update favorites set position = ? where query = ?", toPos, key));
                }
            });
        }

        private static ConfiguredTaskAwaitable ResetPosition()
        {
            return AsyncTask.RunAsync(async () =>
            {
                var favs = await AsyncTask.RunAsync(locationDB.Table<Favorites>().OrderBy(f => f.position).ToListAsync());
                foreach (Favorites fav in favs)
                {
                    fav.position = favs.IndexOf(fav);
                    await AsyncTask.RunAsync(locationDB.UpdateAsync(fav));
                }
            });
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
                homeData = AsyncTask.RunAsync(async () => await GetLastGPSLocData()).GetAwaiter().GetResult();
            else
                homeData = AsyncTask.RunAsync(async () => (await GetFavorites()).FirstOrDefault()).GetAwaiter().GetResult();

            return homeData;
        }
    }
}