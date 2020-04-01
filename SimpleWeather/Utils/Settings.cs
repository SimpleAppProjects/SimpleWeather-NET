using SimpleWeather.Location;
using SimpleWeather.SQLiteNet;
using SimpleWeather.WeatherData;
using SQLite;
using SQLiteNetExtensions.Extensions.TextBlob;
using SQLiteNetExtensionsAsync.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
        public static DateTime UpdateTime { get { return GetUpdateTime(); } set { SetUpdateTime(value); } }
        public static int RefreshInterval { get { return GetRefreshInterval(); } set { SetRefreshInterval(value); } }
        public static bool ShowAlerts { get { return UseAlerts(); } set { SetAlerts(value); } }
        public static bool UsePersonalKey { get { return IsPersonalKey(); } set { SetPersonalKey(value); } }
        public static int VersionCode { get { return GetVersionCode(); } set { SetVersionCode(value); } }

        // Database
        internal static int DBVersion { get { return GetDBVersion(); } set { SetDBVersion(value); } }

        // Data
        internal const int CurrentDBVersion = 5;
        private static SQLiteAsyncConnection locationDB;
        private static SQLiteAsyncConnection weatherDB;
        private static string tzDBConnStr;
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

        internal static SQLiteAsyncConnection GetWeatherDBConnection() => weatherDB;
        internal static SQLiteAsyncConnection GetLocationDBConnection() => locationDB;
        internal static String GetTZDBConnectionString() => tzDBConnStr;

        internal class DBTextBlobSerializer : ITextBlobSerializer
        {
            private Utf8Json.IJsonFormatterResolver Resolver;

            public DBTextBlobSerializer()
            {
                Resolver = new EF.Utf8JsonGen.AttrFirstUtf8JsonResolver();
            }

            public object Deserialize(string text, Type type)
            {
                bool useAttrResolver;
                string str;

                // Use our own resolver (custom deserializer) if json string is escaped
                // since the Utf8Json deserializer is alot more strict
                if (text.Contains("\"{\\\""))
                {
                    str = text;
                    useAttrResolver = true;
                }
                else
                {
                    var unescape = new System.Text.StringBuilder(System.Text.RegularExpressions.Regex.Unescape(text));
                    if (unescape.Length > 1 && unescape[0] == '"' && unescape[unescape.Length - 1] == '"')
                    {
                        unescape.Remove(0, 1);
                        unescape.Remove(unescape.Length - 1, 1);
                    }
                    str = unescape.ToString();
                    useAttrResolver = str.Contains("\\") || str.Contains("[\"{\"") || str.Contains("\"{\"");
                }

                var method = typeof(Utf8Json.JsonSerializer).GetMethod("Deserialize", new Type[] { typeof(string), typeof(Utf8Json.IJsonFormatterResolver) });
                var genMethod = method.MakeGenericMethod(type);
                return genMethod.Invoke(null, new object[] { str, useAttrResolver ? Resolver : JSONParser.Resolver });
            }

            public string Serialize(object element)
            {
                var method = typeof(Utf8Json.JsonSerializer).GetMethods().Single(m =>
                    m.Name == "ToJsonString" &&
                    m.GetParameters().Length == 2 &&
                    m.IsGenericMethod &&
                    m.ContainsGenericParameters && m.GetParameters()[1].ParameterType == typeof(Utf8Json.IJsonFormatterResolver));
                var genMethod = method.MakeGenericMethod(new Type[] { element.GetType() });
                return genMethod.Invoke(null, new object[] { element, JSONParser.Resolver }) as string;
            }
        }

        public static void LoadIfNeeded()
        {
            if (!loaded)
            {
                Load();
                loaded = true;
            }
        }

        internal static void CreateDatabase()
        {
            var locDBConn = locationDB.GetConnection();
            var weatherDBConn = weatherDB.GetConnection();

            using (locDBConn.Lock())
            using (weatherDBConn.Lock())
            {
                locDBConn.CreateTable<LocationData>();
                locDBConn.CreateTable<Favorites>();
                weatherDBConn.CreateTable<Weather>();
                weatherDBConn.CreateTable<WeatherAlerts>();
                weatherDBConn.CreateTable<Forecasts>();
                weatherDBConn.CreateTable<HourlyForecasts>();
            }
        }

        internal static void DestroyDatabase()
        {
            var locDBConn = locationDB.GetConnection();
            var weatherDBConn = weatherDB.GetConnection();

            using (locDBConn.Lock())
            using (weatherDBConn.Lock())
            {
                locDBConn.DropTable<Favorites>();
                locDBConn.DropTable<LocationData>();
                weatherDBConn.DropTable<HourlyForecasts>();
                weatherDBConn.DropTable<Forecasts>();
                weatherDBConn.DropTable<WeatherAlerts>();
                weatherDBConn.DropTable<Weather>();
            }
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="AggregateException"></exception>
        private static void Load()
        {
            Task.Run(async () =>
            {
                // Create DB tables
                TextBlobOperations.SetTextSerializer(new DBTextBlobSerializer());
                CreateDatabase();

                // Migrate old data if available
                await DataMigrations.PerformDBMigrations(locationDB, weatherDB);

                if (!String.IsNullOrWhiteSpace(LastGPSLocation))
                {
                    try
                    {
                        var jsonTextReader = new Utf8Json.JsonReader(System.Text.Encoding.UTF8.GetBytes(LastGPSLocation));
                        lastGPSLocData = new LocationData();
                        lastGPSLocData.FromJson(ref jsonTextReader);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: Settings.Load(): LastGPSLocation");
                    }
                    finally
                    {
                        if (lastGPSLocData == null || !lastGPSLocData.IsValid())
                            lastGPSLocData = new LocationData();
                    }
                }

                await DataMigrations.PerformVersionMigrations(weatherDB, locationDB);
            }).Wait();
        }

        public static ConfiguredTaskAwaitable<LocationData> GetFirstFavorite()
        {
            return AsyncTask.CreateTask<LocationData>(async () =>
            {
                LoadIfNeeded();

                var query = await locationDB.QueryAsync<LocationData>(
                    "select locations.* from locations INNER JOIN favorites on locations.query = favorites.query ORDER by favorites.position LIMIT 1");
                return query?.FirstOrDefault();
            });
        }

        public static ConfiguredTaskAwaitable<IEnumerable<LocationData>> GetFavorites()
        {
            return AsyncTask.CreateTask<IEnumerable<LocationData>>(async () =>
            {
                LoadIfNeeded();

                var query = await locationDB.QueryAsync<LocationData>(
                    "select locations.* from locations INNER JOIN favorites on locations.query = favorites.query ORDER by favorites.position");
                return query;
            });
        }

        public static ConfiguredTaskAwaitable<IEnumerable<LocationData>> GetLocationData()
        {
            return AsyncTask.CreateTask<IEnumerable<LocationData>>(async () =>
            {
                LoadIfNeeded();
                return await locationDB.Table<LocationData>().ToListAsync();
            });
        }

        public static ConfiguredTaskAwaitable<LocationData> GetLocation(string key)
        {
            return AsyncTask.CreateTask(async () =>
            {
                LoadIfNeeded();
                return await locationDB.FindAsync<LocationData>(key);
            });
        }

        public static ConfiguredTaskAwaitable<Weather> GetWeatherData(string key)
        {
            return AsyncTask.CreateTask(async () =>
            {
                LoadIfNeeded();
                return await weatherDB.FindWithChildrenAsync<Weather>(key);
            });
        }

        public static ConfiguredTaskAwaitable<Weather> GetWeatherDataByCoordinate(LocationData location)
        {
            return AsyncTask.CreateTask(async () =>
            {
                LoadIfNeeded();
                var culture = System.Globalization.CultureInfo.InvariantCulture;
                var escapedQuery = String.Format(culture, "\\\"latitude\\\":\\\"{0}\\\",\\\"longitude\\\":\\\"{1}\\\"",
                        location.latitude.ToString(culture), location.longitude.ToString(culture));
                var query = String.Format(culture, "\"latitude\":\"{0}\",\"longitude\":\"{1}\"",
                        location.latitude.ToString(culture), location.longitude.ToString(culture));
                var filteredData = (await weatherDB.QueryAsync<Weather>(
                    "SELECT * FROM weatherdata WHERE `locationblob` LIKE {0} OR `locationblob` LIKE {1} LIMIT 1",
                        "%" + escapedQuery + "%", "%" + query + "%")).FirstOrDefault();
                if (filteredData != null)
                    await weatherDB.GetChildrenAsync(filteredData);
                return filteredData;
            });
        }

        public static ConfiguredTaskAwaitable<ICollection<WeatherAlert>> GetWeatherAlertData(string key)
        {
            return AsyncTask.CreateTask(async () =>
            {
                LoadIfNeeded();

                ICollection<WeatherAlert> alerts = null;

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
            });
        }

        public static ConfiguredTaskAwaitable<Forecasts> GetWeatherForecastData(string key)
        {
            return AsyncTask.CreateTask(async () =>
            {
                LoadIfNeeded();
                return await weatherDB.FindWithChildrenAsync<Forecasts>(key);
            });
        }

        public static ConfiguredTaskAwaitable<IList<HourlyForecast>> GetHourlyWeatherForecastData(string key)
        {
            return AsyncTask.CreateTask<IList<HourlyForecast>>(async () =>
            {
                LoadIfNeeded();

                var list = (await weatherDB.Table<HourlyForecasts>()
                                                        .Where(hrf => hrf.query == key)
                                                        .OrderBy(hrf => hrf.dateblob)
                                                        .ToListAsync());
                foreach (var item in list)
                {
                    await weatherDB.GetChildrenAsync(item);
                }

                return list.Select(hrf => hrf.hr_forecast).ToList();
            });
        }

        public static ConfiguredTaskAwaitable<LocationData> GetLastGPSLocData()
        {
            return AsyncTask.CreateTask(async () =>
            {
                LoadIfNeeded();

                if (lastGPSLocData != null && lastGPSLocData.locationType != LocationType.GPS)
                    lastGPSLocData.locationType = LocationType.GPS;

                return lastGPSLocData;
            });
        }

        public static ConfiguredTaskAwaitable SaveWeatherData(Weather weather)
        {
            return AsyncTask.CreateTask(async () =>
            {
                if (weather != null && weather.IsValid())
                {
                    await weatherDB.InsertOrReplaceWithChildrenAsync(weather);
                }

                int count = await weatherDB.Table<Weather>().CountAsync();
                if (count > CACHE_LIMIT) CleanupWeatherData();
            });
        }

        public static ConfiguredTaskAwaitable SaveWeatherAlerts(LocationData location, ICollection<WeatherAlert> alerts)
        {
            return AsyncTask.CreateTask(async () =>
            {
                if (location != null && location.IsValid())
                {
                    var alertdata = new WeatherAlerts(location.query, alerts);
                    await weatherDB.InsertOrReplaceWithChildrenAsync(alertdata);
                }

                int count = await weatherDB.Table<WeatherAlerts>().CountAsync();
                if (count > CACHE_LIMIT) CleanupWeatherAlertData();
            });
        }

        public static ConfiguredTaskAwaitable SaveWeatherForecasts(Forecasts forecasts)
        {
            return AsyncTask.CreateTask(async () =>
            {
                if (forecasts != null)
                {
                    await weatherDB.InsertOrReplaceWithChildrenAsync(forecasts);
                }

                int count = await weatherDB.Table<Forecasts>().CountAsync();
                if (count > CACHE_LIMIT / 2)
                {
                    CleanupWeatherForecastData();
                }
            });
        }

        public static ConfiguredTaskAwaitable SaveWeatherForecasts(LocationData location, IEnumerable<HourlyForecasts> forecasts)
        {
            return AsyncTask.CreateTask(async () =>
            {
                await weatherDB.ExecuteAsync("delete from hr_forecasts where query = ?", location.query);
                if (forecasts != null)
                {
                    foreach (var fcast in forecasts)
                    {
                        await weatherDB.InsertOrReplaceWithChildrenAsync(fcast);
                    }
                }

                int count = await weatherDB.ExecuteScalarAsync<int>("select count(*) from (select count(*) from hr_forecasts group by query)");
                if (count > CACHE_LIMIT / 2) CleanupWeatherForecastData();
            });
        }

        private static void CleanupWeatherData()
        {
            AsyncTask.Run(async () =>
            {
                var locs = await locationDB.Table<LocationData>().ToListAsync();
                if (FollowGPS) locs.Add(lastGPSLocData);
                var locQueries = locs.Select(l => l.query);

                await weatherDB.DeleteAllIdsNotInAsync<Weather>(locQueries);
            });
        }

        private static void CleanupWeatherForecastData()
        {
            AsyncTask.Run(async () =>
            {
                var locs = await locationDB.Table<LocationData>().ToListAsync();
                if (FollowGPS) locs.Add(lastGPSLocData);
                var locQueries = locs.Select(l => l.query);

                await weatherDB.DeleteAllIdsNotInAsync<Forecasts>(locQueries);
                await weatherDB.DeleteAllIdsNotInAsync<HourlyForecasts>("query", locQueries);
            });
        }

        private static void CleanupWeatherAlertData()
        {
            AsyncTask.Run(async () =>
            {
                var locs = await locationDB.Table<LocationData>().ToListAsync();
                if (FollowGPS) locs.Add(lastGPSLocData);
                var locQueries = locs.Select(l => l.query);

                await weatherDB.DeleteAllIdsNotInAsync<WeatherAlerts>(locQueries);
            });
        }

        public static ConfiguredTaskAwaitable SaveLocationData(List<LocationData> locationData)
        {
            return AsyncTask.CreateTask(async () =>
            {
                if (locationData != null)
                {
                    for (int i = 0; i < locationData.Count; i++)
                    {
                        var loc = locationData[i];

                        if (loc != null && loc.IsValid())
                        {
                            await locationDB.InsertOrReplaceAsync(loc);
                            var fav = new Favorites() { query = loc.query, position = i };
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
            });
        }

        public static ConfiguredTaskAwaitable AddLocation(LocationData location)
        {
            return AsyncTask.CreateTask(async () =>
            {
                // We only store searched locations in the db
                // GPS location is stored in [the] local settings [container]
                if (location?.locationType == LocationType.Search && location?.IsValid() == true)
                {
                    await locationDB.InsertOrReplaceAsync(location);
                    int pos = await locationDB.Table<LocationData>().CountAsync();
                    await locationDB.InsertOrReplaceAsync(new Favorites() { query = location.query, position = pos });
                }
            });
        }

        public static ConfiguredTaskAwaitable UpdateLocation(LocationData location)
        {
            return AsyncTask.CreateTask(async () =>
            {
                // We only store searched locations in the db
                // GPS location is stored in [the] local settings [container]
                if (location.locationType == LocationType.GPS && location?.IsValid() == true)
                    Settings.SaveLastGPSLocData(location);
                else if (location?.locationType == LocationType.Search && location?.IsValid() == true)
                {
                    await locationDB.UpdateAsync(location);
                }
            });
        }

        public static ConfiguredTaskAwaitable UpdateLocationWithKey(LocationData location, string oldKey)
        {
            return AsyncTask.CreateTask(async () =>
            {
                if (location != null && location.IsValid() && !String.IsNullOrWhiteSpace(oldKey))
                {
                    // Get position from favorites table
                    var fav = await locationDB.FindAsync<Favorites>(oldKey);

                    if (fav == null)
                    {
                        return;
                    }

                    int pos = fav.position;

                    // Remove location from table
                    await locationDB.DeleteAsync<LocationData>(oldKey);
                    await locationDB.DeleteAsync<Favorites>(oldKey);

                    // Add updated location with new query (pkey)
                    await locationDB.InsertOrReplaceAsync(location);
                    await locationDB.InsertOrReplaceAsync(new Favorites() { query = location.query, position = pos });
                }
            });
        }

        public static ConfiguredTaskAwaitable DeleteLocations()
        {
            return AsyncTask.CreateTask(async () =>
            {
                await locationDB.DeleteAllAsync<LocationData>();
                await locationDB.DeleteAllAsync<Favorites>();
            });
        }

        public static ConfiguredTaskAwaitable DeleteLocation(string key)
        {
            return AsyncTask.CreateTask(async () =>
            {
                if (!String.IsNullOrWhiteSpace(key))
                {
                    await locationDB.DeleteAsync<LocationData>(key);
                    await locationDB.DeleteAsync<Favorites>(key);
                    await ResetPosition();
                }
            });
        }

        public static ConfiguredTaskAwaitable MoveLocation(string key, int toPos)
        {
            return AsyncTask.CreateTask(async () =>
            {
                if (!String.IsNullOrWhiteSpace(key))
                {
                    await locationDB.ExecuteAsync("update favorites set position = ? where query = ?", toPos, key);
                }
            });
        }

        private static ConfiguredTaskAwaitable ResetPosition()
        {
            return AsyncTask.CreateTask(async () =>
            {
                var favs = await locationDB.Table<Favorites>().OrderBy(f => f.position).ToListAsync();
                foreach (Favorites fav in favs)
                {
                    fav.position = favs.IndexOf(fav);
                    await locationDB.UpdateAsync(fav);
                }
            });
        }

        public static void SaveLastGPSLocData(LocationData data)
        {
            lastGPSLocData = data;
            LastGPSLocation = lastGPSLocData?.ToJson();
        }

        public static Task<LocationData> GetHomeData()
        {
            return Task.Run(async () =>
            {
                LocationData homeData = null;

                if (FollowGPS)
                    homeData = await GetLastGPSLocData();
                else
                    homeData = await GetFirstFavorite();

                return homeData;
            });
        }
    }
}