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
        public static LocationData HomeData { get { return GetHomeData(); } }
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

        public static async Task LoadIfNeeded()
        {
            if (!loaded)
            {
                await Load();
                loaded = true;
            }
        }

        private static async Task CreateDatabase()
        {
            await locationDB.CreateTableAsync<LocationData>();
            await locationDB.CreateTableAsync<Favorites>();
            await weatherDB.CreateTableAsync<Weather>();
            await weatherDB.CreateTableAsync<WeatherAlerts>();
            await weatherDB.CreateTableAsync<Forecasts>();
            await weatherDB.CreateTableAsync<HourlyForecasts>();
        }

        private static async Task DestroyDatabase()
        {
            await locationDB.DropTableAsync<Favorites>();
            await locationDB.DropTableAsync<LocationData>();
            await weatherDB.DropTableAsync<WeatherAlerts>();
            await weatherDB.DropTableAsync<Forecasts>();
            await weatherDB.DropTableAsync<HourlyForecasts>();
            await weatherDB.DropTableAsync<Weather>();
        }

        private static async Task Load()
        {
            // Create DB tables
            TextBlobOperations.SetTextSerializer(new DBTextBlobSerializer());
            await AsyncTask.RunAsync(CreateDatabase);

            // Migrate old data if available
            await DataMigrations.PerformDBMigrations(weatherDB, locationDB);

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

            DataMigrations.PerformVersionMigrations(weatherDB, locationDB);
        }

        public static ConfiguredTaskAwaitable<IEnumerable<LocationData>> GetFavorites()
        {
            return AsyncTask.RunAsync<IEnumerable<LocationData>>(async () =>
            {
                await LoadIfNeeded();

                var query = await AsyncTask.RunAsync(locationDB.QueryAsync<LocationData>(
                    "select locations.* from locations INNER JOIN favorites on locations.query = favorites.query ORDER by favorites.position"));
                return query;
            });
        }

        public static ConfiguredTaskAwaitable<IEnumerable<LocationData>> GetLocationData()
        {
            return AsyncTask.RunAsync<IEnumerable<LocationData>>(async () =>
            {
                await LoadIfNeeded();
                return await AsyncTask.RunAsync(locationDB.Table<LocationData>().ToListAsync());
            });
        }

        public static ConfiguredTaskAwaitable<LocationData> GetLocation(string key)
        {
            return AsyncTask.RunAsync(async () =>
            {
                await LoadIfNeeded();
                return await AsyncTask.RunAsync(locationDB.FindAsync<LocationData>(key));
            });
        }

        public static ConfiguredTaskAwaitable<Weather> GetWeatherData(string key)
        {
            return AsyncTask.RunAsync(async () =>
            {
                await LoadIfNeeded();
                return await AsyncTask.RunAsync(weatherDB.FindWithChildrenAsync<Weather>(key));
            });
        }

        public static ConfiguredTaskAwaitable<Weather> GetWeatherDataByCoordinate(LocationData location)
        {
            return AsyncTask.RunAsync(async () =>
            {
                await LoadIfNeeded();
                var culture = System.Globalization.CultureInfo.InvariantCulture;
                var query = String.Format(culture, "\\\"latitude\\\":\\\"{0}\\\",\\\"longitude\\\":\\\"{1}\\\"",
                        location.latitude.ToString(culture), location.longitude.ToString(culture));
                var filteredData = (await weatherDB.QueryAsync<Weather>("SELECT * FROM weatherdata WHERE `locationblob` LIKE {0} LIMIT 1", "%" + query + "%")).FirstOrDefault();
                if (filteredData != null)
                    await weatherDB.GetChildrenAsync(filteredData);
                return filteredData;
            });
        }

        public static ConfiguredTaskAwaitable<ICollection<WeatherAlert>> GetWeatherAlertData(string key)
        {
            return AsyncTask.RunAsync(async () =>
            {
                await LoadIfNeeded();

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

        public static ConfiguredTaskAwaitable<Forecasts> GetWeatherForecastData(string key)
        {
            return AsyncTask.RunAsync(async () =>
            {
                await LoadIfNeeded();
                return await AsyncTask.RunAsync(weatherDB.FindWithChildrenAsync<Forecasts>(key));
            });
        }

        public static ConfiguredTaskAwaitable<IList<HourlyForecast>> GetHourlyWeatherForecastData(string key)
        {
            return AsyncTask.RunAsync<IList<HourlyForecast>>(async () =>
            {
                await LoadIfNeeded();
                return await AsyncTask.RunAsync(async () =>
                {
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
            });
        }

        public static ConfiguredTaskAwaitable<LocationData> GetLastGPSLocData()
        {
            return AsyncTask.RunAsync(async () =>
            {
                await LoadIfNeeded();

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
                    await AsyncTask.RunAsync(weatherDB.InsertOrReplaceWithChildrenAsync(weather));
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
                    await AsyncTask.RunAsync(weatherDB.InsertOrReplaceWithChildrenAsync(alertdata));
                }

                if (await AsyncTask.RunAsync(weatherDB.Table<WeatherAlerts>().CountAsync()) > CACHE_LIMIT)
                    CleanupWeatherAlertData();
            });
        }

        public static ConfiguredTaskAwaitable SaveWeatherForecasts(Forecasts forecasts)
        {
            return AsyncTask.RunAsync(async () =>
            {
                if (forecasts != null)
                {
                    await AsyncTask.RunAsync(weatherDB.InsertOrReplaceWithChildrenAsync(forecasts));
                }

                if ((await AsyncTask.RunAsync(weatherDB.Table<Forecasts>().CountAsync())) > CACHE_LIMIT / 2)
                {
                    CleanupWeatherForecastData();
                }
            });
        }

        public static ConfiguredTaskAwaitable SaveWeatherForecasts(LocationData location, IEnumerable<HourlyForecasts> forecasts)
        {
            return AsyncTask.RunAsync(async () =>
            {
                await AsyncTask.RunAsync(weatherDB.ExecuteAsync("delete from hr_forecasts where query = ?", location.query));
                if (forecasts != null)
                {
                    foreach (var fcast in forecasts)
                    {
                        await AsyncTask.RunAsync(weatherDB.InsertOrReplaceWithChildrenAsync(fcast));
                    }
                }

                if ((await AsyncTask.RunAsync(weatherDB.ExecuteScalarAsync<int>(
                    "select count(*) from (select count(*) from hr_forecasts group by query)"))) > CACHE_LIMIT / 2)
                {
                    CleanupWeatherForecastData();
                }
            });
        }

        private static void CleanupWeatherData()
        {
            AsyncTask.Run(async () =>
            {
                var locs = await AsyncTask.RunAsync(locationDB.Table<LocationData>().ToListAsync());
                if (FollowGPS) locs.Add(lastGPSLocData);
                var locQueries = locs.Select(l => l.query);

                await AsyncTask.RunAsync(weatherDB.DeleteAllIdsNotInAsync<Weather>(locQueries));
            });
        }

        private static void CleanupWeatherForecastData()
        {
            AsyncTask.Run(async () =>
            {
                var locs = await AsyncTask.RunAsync(locationDB.Table<LocationData>().ToListAsync());
                if (FollowGPS) locs.Add(lastGPSLocData);
                var locQueries = locs.Select(l => l.query);

                await AsyncTask.RunAsync(weatherDB.DeleteAllIdsNotInAsync<Forecasts>(locQueries));
                await AsyncTask.RunAsync(weatherDB.DeleteAllIdsNotInAsync<HourlyForecasts>("query", locQueries));
            });
        }

        private static void CleanupWeatherAlertData()
        {
            AsyncTask.Run(async () =>
            {
                var locs = await AsyncTask.RunAsync(locationDB.Table<LocationData>().ToListAsync());
                if (FollowGPS) locs.Add(lastGPSLocData);
                var locQueries = locs.Select(l => l.query);

                await AsyncTask.RunAsync(weatherDB.DeleteAllIdsNotInAsync<WeatherAlerts>(locQueries));
            });
        }

        public static ConfiguredTaskAwaitable SaveLocationData(List<LocationData> locationData)
        {
            return AsyncTask.RunAsync(async () =>
            {
                if (locationData != null)
                {
                    for (int i = 0; i < locationData.Count; i++)
                    {
                        var loc = locationData[i];

                        if (loc != null && loc.IsValid())
                        {
                            await AsyncTask.RunAsync(locationDB.InsertOrReplaceAsync(loc));
                            var fav = new Favorites() { query = loc.query, position = i };
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
                // We only store searched locations in the db
                // GPS location is stored in [the] local settings [container]
                if (location?.locationType == LocationType.Search && location?.IsValid() == true)
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
                // We only store searched locations in the db
                // GPS location is stored in [the] local settings [container]
                if (location.locationType == LocationType.GPS && location?.IsValid() == true)
                    Settings.SaveLastGPSLocData(location);
                else if (location?.locationType == LocationType.Search && location?.IsValid() == true)
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
                    var fav = await locationDB.FindAsync<Favorites>(oldKey);

                    if (fav == null)
                    {
                        return;
                    }

                    int pos = fav.position;

                    // Remove location from table
                    await AsyncTask.RunAsync(locationDB.DeleteAsync<LocationData>(oldKey));
                    await AsyncTask.RunAsync(locationDB.DeleteAsync<Favorites>(oldKey));

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
                    await AsyncTask.RunAsync(locationDB.DeleteAsync<Favorites>(key));
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
                    await AsyncTask.RunAsync(locationDB.ExecuteAsync("update favorites set position = ? where query = ?", toPos, key));
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