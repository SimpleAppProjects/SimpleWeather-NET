using SimpleWeather.Database;
using SimpleWeather.Icons;
using SimpleWeather.LocationData;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
#if WINDOWS
using Windows.Foundation.Metadata;
#else
using System.ComponentModel.DataAnnotations;
#endif

namespace SimpleWeather.Preferences
{
    public sealed partial class SettingsManager : ISettingsService
    {
        // Settings Members
        public bool WeatherLoaded { get => IsWeatherLoaded(); set => SetWeatherLoaded(value); }
        public bool OnBoardComplete { get { return IsOnBoardingComplete(); } set { SetOnBoardingComplete(value); } }
        public string API { get { return GetAPI(); } set { SetAPI(value); } }
#if WINDOWS
        [Deprecated(
#else
        [Obsolete(
#endif
            "Replace with SettingsManager.APIKey or SettingsManager.APIKeys[String]"
#if WINDOWS
            , DeprecationType.Deprecate, 5530)]
#else
            )]
#endif
        public string API_KEY { get { return GetAPIKEY(); } set { SetAPIKey(value); } }
        public string APIKey { get { return API?.Let(it => APIKeys[it]); } set { API?.Let(it => APIKeys[it] = value); } }
        public IAPIKeyMap APIKeys { get; }
#if WINDOWS
        [Deprecated(
#else
        [Obsolete(
#endif
            "Replace with SettingsManager.IsKeyVerified(String) or SettingsManager.SetKeyVerified(String, Boolean)"
#if WINDOWS
            , DeprecationType.Deprecate, 5530)]
#else
            )]
#endif
        public bool KeyVerified { get { return IsKeyVerified(); } set { SetKeyVerified(value); } }
        public IKeyVerifiedMap KeysVerified { get; }
        public bool FollowGPS { get { return UseFollowGPS(); } set { SetFollowGPS(value); } }
        private string LastGPSLocation { get { return GetLastGPSLocation(); } set { SetLastGPSLocation(value); } }
        public DateTime UpdateTime { get { return GetUpdateTime(); } set { SetUpdateTime(value); } }
        public int RefreshInterval { get { return GetRefreshInterval(); } set { SetRefreshInterval(value); } }
        public bool ShowAlerts { get { return UseAlerts(); } set { SetAlerts(value); } }
        public bool UsePersonalKey { get { return IsPersonalKey(); } set { SetPersonalKey(value); } }
        public int VersionCode { get { return GetVersionCode(); } set { SetVersionCode(value); } }
        public bool DevSettingsEnabled { get { return IsDevSettingsEnabled(); } set { SetDevSettingsEnabled(value); } }
        public string IconProvider { get { return GetIconsProvider(); } set { SetIconsProvider(value); } }
        public UserThemeMode UserTheme { get { return GetUserThemeMode(); } set { SetUserThemeMode(value); } }
        public bool DailyNotificationEnabled { get { return IsDailyNotificationEnabled(); } set { SetDailyNotificationEnabled(value); } }
        public TimeSpan DailyNotificationTime { get { return GetDailyNotificationTime(); } set { SetDailyNotificationTime(value); } }
        public bool PoPChanceNotificationEnabled { get { return IsPoPChanceNotificationEnabled(); } set { SetPoPChanceNotificationEnabled(value); } }
        public DateTimeOffset LastPoPChanceNotificationTime { get { return GetLastPoPChanceNotificationTime(); } set { SetLastPoPChanceNotificationTime(value); } }
        public int PoPChanceMinimumPercentage { get { return GetPoPChanceMinimumPercentage(); } set { SetPoPChanceMinimumPercentage(value); } }

        // Units
        public string TemperatureUnit { get { return GetTempUnit(); } set { SetTempUnit(value); } }
        public string SpeedUnit { get { return GetSpeedUnit(); } set { SetSpeedUnit(value); } }
        public string PressureUnit { get { return GetPressureUnit(); } set { SetPressureUnit(value); } }
        public string DistanceUnit { get { return GetDistanceUnit(); } set { SetDistanceUnit(value); } }
        public string PrecipitationUnit { get { return GetPrecipitationUnit(); } set { SetPrecipitationUnit(value); } }
        public string UnitString { get { return string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3};{4}", TemperatureUnit, SpeedUnit, PressureUnit, DistanceUnit, PrecipitationUnit); } }

        // Data
        private const int CACHE_LIMIT = 25;
        public const int MAX_LOCATIONS = 16;

        // Units
        public const int DefaultInterval = 180; // 3 hrs
        public const int READ_TIMEOUT = 10000; // 10s

        // Settings Keys
        public const string KEY_API = "API";
        public const string KEY_APIKEY = "API_KEY";
        private const string KEY_APIKEY_VERIFIED = "API_KEY_VERIFIED";
        private const string KEY_APIKEY_PREFIX = "api_key";
        private const string KEY_APIKEYS = "api_keys";
        private const string KEY_USECELSIUS = "key_usecelsius";
        private const string KEY_UNITS = "Units";
        public const string KEY_WEATHERLOADED = "weatherLoaded";
        public const string KEY_FOLLOWGPS = "key_followgps";
        private const string KEY_LASTGPSLOCATION = "key_lastgpslocation";
        public const string KEY_REFRESHINTERVAL = "key_refreshinterval";
        private const string KEY_UPDATETIME = "key_updatetime";
        private const string KEY_DBVERSION = "key_dbversion";
        public const string KEY_USEALERTS = "key_usealerts";
        public const string KEY_USEPERSONALKEY = "key_usepersonalkey";
        private const string KEY_CURRENTVERSION = "key_currentversion";
        private const string KEY_ONBOARDINGCOMPLETE = "key_onboardcomplete";
        public const string KEY_DEVSETTINGSENABLED = "key_devsettingsenabled";
        public const string KEY_USERTHEME = "key_usertheme";
        public const string KEY_TEMPUNIT = "key_tempunit";
        public const string KEY_SPEEDUNIT = "key_speedunit";
        public const string KEY_DISTANCEUNIT = "key_distanceunit";
        public const string KEY_PRECIPITATIONUNIT = "key_precipitationunit";
        public const string KEY_PRESSUREUNIT = "key_pressureunit";
        public const string KEY_ICONSSOURCE = "key_iconssource";
        public const string KEY_DAILYNOTIFICATION = "key_dailynotification";
        public const string KEY_DAILYNOTIFICATIONTIME = "key_dailynotificationtime";
        public const string KEY_POPCHANCENOTIFICATION = "key_popchancenotification";
        public const string KEY_POPCHANCEPCT = "key_popchancepct";
        public const string KEY_LASTCHANCENOTIFICATIONTIME = "key_lastchancenotificationtime";

        // 8am
        public static readonly TimeSpan DEFAULT_DAILYNOTIFICATION_TIME = new TimeSpan(8, 0, 0);

        // Weather Data
        private static LocationData.LocationData lastGPSLocData = null;
        private static bool loaded = false;

        public static bool IsLoaded => loaded;

        private LocationsDatabase LocationDB => LocationsDatabase.Instance;
        private WeatherDatabase WeatherDB => WeatherDatabase.Instance;

        public SettingsManager()
        {
            APIKeys = new APIKeyMap(this);
            KeysVerified = new KeyVerifiedMap(this);
        }

        public async Task LoadIfNeeded()
        {
            if (!loaded)
            {
                await Load();
                loaded = true;
            }
        }

        private async Task Load()
        {
            if (!String.IsNullOrWhiteSpace(LastGPSLocation))
            {
                try
                {
                    var jsonTextReader = new Utf8Json.JsonReader(System.Text.Encoding.UTF8.GetBytes(LastGPSLocation));
                    lastGPSLocData = new LocationData.LocationData();
                    await Task.Run(() =>
                    {
                        lastGPSLocData.FromJson(ref jsonTextReader);
                    });
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: Settings.Load(): LastGPSLocation");
                }
                finally
                {
                    if (lastGPSLocData?.tz_long == null || !lastGPSLocData.IsValid())
                        lastGPSLocData = LocationData.LocationData.BuildEmptyGPSLocation();
                }
            }
        }

        #region Database
        public Task<IEnumerable<LocationData.LocationData>> GetFavorites()
        {
            return Task.Run<IEnumerable<LocationData.LocationData>>(async () =>
            {
                await LoadIfNeeded();
                return await LocationDB.GetFavorites();
            });
        }

        public Task<IEnumerable<LocationData.LocationData>> GetLocationData()
        {
            return Task.Run<IEnumerable<LocationData.LocationData>>(async () =>
            {
                await LoadIfNeeded();
                return await LocationDB.LoadAllLocationData();
            });
        }

        public Task<LocationData.LocationData> GetLocation(string key)
        {
            return Task.Run(async () =>
            {
                await LoadIfNeeded();
                return await LocationDB.GetLocation(key);
            });
        }

        public Task<Weather> GetWeatherData(string key)
        {
            return Task.Run(async () =>
            {
                await LoadIfNeeded();
                return await WeatherDB.GetWeatherData(key);
            });
        }

        public Task<Weather> GetWeatherDataByCoordinate(LocationData.LocationData location)
        {
            return Task.Run(async () =>
            {
                await LoadIfNeeded();
                var culture = CultureInfo.InvariantCulture;
                var escapedQuery = String.Format(culture, "\\\"latitude\\\":\\\"{0}\\\",\\\"longitude\\\":\\\"{1}\\\"",
                        location.latitude.ToString(culture), location.longitude.ToString(culture));
                var query = String.Format(culture, "\"latitude\":\"{0}\",\"longitude\":\"{1}\"",
                        location.latitude.ToString(culture), location.longitude.ToString(culture));
                return await WeatherDB.GetWeatherDataByCoord(escapedQuery, query);
            });
        }

        public Task<ICollection<WeatherAlert>> GetWeatherAlertData(string key)
        {
            return Task.Run(async () =>
            {
                await LoadIfNeeded();

                ICollection<WeatherAlert> alerts = null;

                try
                {
                    var weatherAlertData = await WeatherDB.GetWeatherAlertData(key);
                    alerts = weatherAlertData?.alerts;
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "Settings: GetWeatherAlertData()");
                }

                return alerts ?? new List<WeatherAlert>();
            });
        }

        public Task<Forecasts> GetWeatherForecastData(string key)
        {
            return Task.Run(async () =>
            {
                await LoadIfNeeded();
                return await WeatherDB.GetForecastData(key);
            });
        }

        public Task<IList<HourlyForecast>> GetHourlyWeatherForecastDataByLimit(string key, int loadSize)
        {
            return Task.Run<IList<HourlyForecast>>(async () =>
            {
                await LoadIfNeeded();
                return await WeatherDB.GetHourlyForecastsByQueryOrderByDateByLimit(key, loadSize);
            });
        }

        public Task<IList<HourlyForecast>> GetHourlyForecastsByQueryOrderByDateByLimitFilterByDate(string key, int loadSize, DateTimeOffset date)
        {
            return Task.Run<IList<HourlyForecast>>(async () =>
            {
                await LoadIfNeeded();
                return await WeatherDB.GetHourlyForecastsByQueryOrderByDateByLimitFilterByDate(key, loadSize, date);
            });
        }

        public Task<IList<HourlyForecast>> GetHourlyWeatherForecastData(string key)
        {
            return Task.Run<IList<HourlyForecast>>(async () =>
            {
                await LoadIfNeeded();
                return await WeatherDB.GetHourlyForecastsByQueryOrderByDate(key);
            });
        }

        public Task<IList<HourlyForecast>> GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(string key, int pageIndex, int loadSize, DateTimeOffset date)
        {
            return Task.Run<IList<HourlyForecast>>(async () =>
            {
                await LoadIfNeeded();
                return await WeatherDB.GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(key, pageIndex, loadSize, date);
            });
        }

        public Task<HourlyForecast> GetFirstHourlyWeatherForecastDataByDate(string key, DateTimeOffset date)
        {
            return Task.Run<HourlyForecast>(async () =>
            {
                await LoadIfNeeded();
                return await WeatherDB.GetFirstHourlyForecastDataByDate(key, date);
            });
        }

        public Task<LocationData.LocationData> GetLastGPSLocData()
        {
            return Task.Run(async () =>
            {
                await LoadIfNeeded();

                if (lastGPSLocData != null && lastGPSLocData.locationType != LocationType.GPS)
                {
                    lastGPSLocData.locationType = LocationType.GPS;
                }

                return lastGPSLocData;
            });
        }

        public Task SaveWeatherData(Weather weather)
        {
            return Task.Run(async () =>
            {
                if (weather != null && weather.IsValid())
                {
                    await WeatherDB.InsertWeatherData(weather);
                }

                _ = Task.Run(async () =>
                {
                    if (await WeatherDB.GetWeatherDataCount() > CACHE_LIMIT) await CleanupWeatherData();
                });
            });
        }

        public Task SaveWeatherAlerts(LocationData.LocationData location, ICollection<WeatherAlert> alerts)
        {
            return Task.Run(async () =>
            {
                if (location != null && location.IsValid())
                {
                    var alertdata = new WeatherAlerts(location.query, alerts);
                    await WeatherDB.InsertWeatherAlertData(alertdata);
                }

                _ = Task.Run(async () =>
                {
                    if (await WeatherDB.GetWeatherAlertDataCount() > CACHE_LIMIT) await CleanupWeatherAlertData();
                });
            });
        }

        public Task SaveWeatherForecasts(Forecasts forecasts)
        {
            return Task.Run(async () =>
            {
                if (forecasts != null)
                {
                    await WeatherDB.InsertForecast(forecasts);
                }

                _ = Task.Run(async () =>
                {
                    if (await WeatherDB.GetHourlyForecastCountGroupedByQuery() > CACHE_LIMIT / 2) await CleanupWeatherForecastData();
                });
            });
        }

        public Task SaveWeatherForecasts(string key, IEnumerable<HourlyForecasts> forecasts)
        {
            return Task.Run(async () =>
            {
                await WeatherDB.DeleteHourlyForecastsByKey(key);
                if (forecasts != null)
                {
                    await WeatherDB.InsertAllHourlyForecasts(forecasts);
                }

                _ = Task.Run(async () =>
                {
                    if (await WeatherDB.GetHourlyForecastCountGroupedByQuery() > CACHE_LIMIT / 2) await CleanupWeatherForecastData();
                });
            });
        }

        private async Task CleanupWeatherData()
        {
            var locs = await LocationDB.LoadAllLocationData();
            if (FollowGPS) locs.Add(lastGPSLocData);

            var locQueries = locs.Select(l => l.query);
            await WeatherDB.DeleteWeatherDataByKeyNotIn(locQueries);
        }

        private async Task CleanupWeatherForecastData()
        {
            var locs = await LocationDB.LoadAllLocationData();
            if (FollowGPS) locs.Add(lastGPSLocData);

            var locQueries = locs.Select(l => l.query);
            await WeatherDB.DeleteForecastByKeyNotIn(locQueries);
            await WeatherDB.DeleteHourlyForecastByKeyNotIn(locQueries);
        }

        private async Task CleanupWeatherAlertData()
        {
            var locs = await LocationDB.LoadAllLocationData();
            if (FollowGPS) locs.Add(lastGPSLocData);

            var locQueries = locs.Select(l => l.query);
            await WeatherDB.DeleteWeatherAlertDataByKeyNotIn(locQueries);
        }

        public Task AddLocation(LocationData.LocationData location)
        {
            return Task.Run(async () =>
            {
                if (location?.IsValid() == true)
                {
                    await LocationDB.InsertLocationData(location);
                    int pos = await LocationDB.GetLocationCount();
                    var fav = new Favorites()
                    {
                        query = location.query,
                        position = pos
                    };
                    await LocationDB.InsertFavorite(fav);
                }
            });
        }

        public Task UpdateLocation(LocationData.LocationData location)
        {
            return Task.Run(async () =>
            {
                // We only store searched locations in the db
                // GPS location is stored in [the] local settings [container]
                if (location?.locationType == LocationType.GPS && location?.IsValid() == true)
                {
                    await SaveLastGPSLocData(location);
                }
                else if (location?.locationType == LocationType.Search && location?.IsValid() == true)
                {
                    await LocationDB.UpdateLocationData(location);
                }
            });
        }

        public Task UpdateLocationWithKey(LocationData.LocationData location, string oldKey)
        {
            return Task.Run(async () =>
            {
                if (location != null && location.IsValid() && !String.IsNullOrWhiteSpace(oldKey))
                {
                    // Get position from favorites table
                    var fav = await LocationDB.GetFavorite(oldKey);
                    if (fav == null) return;

                    int pos = fav.position;

                    // Remove location from table
                    await LocationDB.DeleteLocationDataByKey(oldKey);
                    await LocationDB.DeleteFavoritesByKey(oldKey);

                    // Add updated location with new query (pkey)
                    await LocationDB.InsertLocationData(location);
                    await LocationDB.InsertFavorite(new Favorites()
                    {
                        query = location.query,
                        position = pos
                    });
                }
            });
        }

        public Task DeleteLocations()
        {
            return Task.Run(async () =>
            {
                await LocationDB.DeleteAllLocationData();
                await LocationDB.DeleteAllFavoriteData();
            });
        }

        public Task DeleteLocation(string key)
        {
            return Task.Run(async () =>
            {
                if (!String.IsNullOrWhiteSpace(key))
                {
                    await LocationDB.DeleteLocationDataByKey(key);
                    await LocationDB.DeleteFavoritesByKey(key);
                    await ResetPosition();
                }
            });
        }

        public Task MoveLocation(string key, int toPos)
        {
            return Task.Run(async () =>
            {
                if (!String.IsNullOrWhiteSpace(key))
                {
                    await LocationDB.UpdateFavPosition(key, toPos);
                }
            });
        }

        private async Task ResetPosition()
        {
            var favs = await LocationDB.LoadAllFavoritesByPosition();
            foreach (Favorites fav in favs)
            {
                fav.position = favs.IndexOf(fav);
                await LocationDB.UpdateFavorite(fav);
            }
        }

        public async Task SaveLastGPSLocData(LocationData.LocationData data)
        {
            lastGPSLocData = data;
            await Task.Run(() =>
            {
                LastGPSLocation = JSONParser.Serializer(lastGPSLocData);
            });
        }

        public Task<LocationData.LocationData> GetHomeData()
        {
            return Task.Run(async () =>
            {
                LocationData.LocationData homeData = null;

                if (FollowGPS)
                {
                    homeData = await GetLastGPSLocData();
                }
                else
                {
                    await LoadIfNeeded();
                    homeData = await LocationDB.GetFirstFavorite();
                }

                return homeData;
            });
        }
        #endregion

        #region Settings
        private string GetTempUnit()
        {
            var defaultUnit = GetValue(KEY_USECELSIUS, false) ? Units.CELSIUS : Units.FAHRENHEIT;
            return GetValue(KEY_TEMPUNIT, defaultUnit);
        }

        private void SetTempUnit(string unit)
        {
            SetValue(KEY_TEMPUNIT, unit);
        }

        private string GetSpeedUnit()
        {
            return GetValue(KEY_SPEEDUNIT, Units.MILES_PER_HOUR);
        }

        private void SetSpeedUnit(string value)
        {
            SetValue(KEY_SPEEDUNIT, value);
        }

        private string GetPressureUnit()
        {
            return GetValue(KEY_PRESSUREUNIT, Units.INHG);
        }

        private void SetPressureUnit(string value)
        {
            SetValue(KEY_PRESSUREUNIT, value);
        }

        private string GetDistanceUnit()
        {
            return GetValue(KEY_DISTANCEUNIT, Units.MILES);
        }

        private void SetDistanceUnit(string value)
        {
            SetValue(KEY_DISTANCEUNIT, value);
        }

        private string GetPrecipitationUnit()
        {
            return GetValue(KEY_PRECIPITATIONUNIT, Units.INCHES);
        }

        private void SetPrecipitationUnit(string value)
        {
            SetValue(KEY_PRECIPITATIONUNIT, value);
        }

        public void SetDefaultUnits(string unit)
        {
            bool isFahrenheit = Units.FAHRENHEIT.Equals(unit, StringComparison.InvariantCulture);
            TemperatureUnit = unit;
            SpeedUnit = isFahrenheit ? Units.MILES_PER_HOUR : Units.KILOMETERS_PER_HOUR;
            PressureUnit = isFahrenheit ? Units.INHG : Units.MILLIBAR;
            DistanceUnit = isFahrenheit ? Units.MILES : Units.KILOMETERS;
            PrecipitationUnit = isFahrenheit ? Units.INCHES : Units.MILLIMETERS;
        }

        private bool IsWeatherLoaded()
        {
            if (!ContainsKey(KEY_WEATHERLOADED))
            {
                SetWeatherLoaded(false);
                return false;
            }
            else
            {
                return GetValue(KEY_WEATHERLOADED, false);
            }
        }

        private void SetWeatherLoaded(bool isLoaded)
        {
            SetValue(KEY_WEATHERLOADED, isLoaded);
        }

        private string GetAPI()
        {
            if (!ContainsKey(KEY_API))
            {
                var api = DI.Utils.RemoteConfigService.GetDefaultWeatherProvider();
                SetAPI(api);
                return api;
            }
            else
            {
                return GetValue<string>(KEY_API, null);
            }
        }

        private void SetAPI(string value)
        {
            SetValue(KEY_API, value);
        }

#if WINDOWS
        [Deprecated(
#else
        [Obsolete(
#endif
            "Replace with SettingsManager.GetAPIKey(String)"
#if WINDOWS
            , DeprecationType.Deprecate, 5530)]
#else
            )]
#endif
        private string GetAPIKEY()
        {
            if (!ContainsKey(KEY_APIKEY))
            {
                return string.Empty;
            }
            else
            {
                return GetValue<string>(KEY_APIKEY, null);
            }
        }

        private string GetAPIKey()
        {
            return GetAPI()?.Let(api => GetAPIKey(api));
        }

        private void SetAPIKey(string key)
        {
            GetAPI()?.Let(api =>
            {
                SetAPIKey(api, key);
            });
        }

        private string GetAPIKey(string provider)
        {
            return GetValue<string>($"{KEY_APIKEY_PREFIX}_{provider}", null);
        }

        private void SetAPIKey(string provider, string key)
        {
            SetValue($"{KEY_APIKEY_PREFIX}_{provider}", key);
        }

        private bool UseFollowGPS()
        {
            if (!ContainsKey(KEY_FOLLOWGPS))
            {
                SetFollowGPS(false);
                return false;
            }
            else
            {
                return GetValue(KEY_FOLLOWGPS, false);
            }
        }

        private void SetFollowGPS(bool value)
        {
            SetValue(KEY_FOLLOWGPS, value);
        }

        private string GetLastGPSLocation()
        {
            return GetValue<string>(KEY_LASTGPSLOCATION, null);
        }

        private void SetLastGPSLocation(string value)
        {
            SetValue(KEY_LASTGPSLOCATION, value);
        }

        private DateTime GetUpdateTime()
        {
            if (!ContainsKey(KEY_UPDATETIME))
            {
                return DateTime.MinValue;
            }
            else
            {
                var value = GetValue<string>(KEY_UPDATETIME, null);

                if (DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
            }

            return DateTime.MinValue;
        }

        private void SetUpdateTime(DateTime value)
        {
            SetValue(KEY_UPDATETIME, value.ToString(CultureInfo.InvariantCulture));
        }

        private int GetRefreshInterval()
        {
            return GetValue(KEY_REFRESHINTERVAL, DefaultInterval);
        }

        private void SetRefreshInterval(int value)
        {
            SetValue(KEY_REFRESHINTERVAL, value);
        }

        private bool UseAlerts()
        {
            if (!ContainsKey(KEY_USEALERTS))
            {
                SetAlerts(false);
                return false;
            }
            else
            {
                return GetValue(KEY_USEALERTS, false);
            }
        }

        private void SetAlerts(bool value)
        {
            SetValue(KEY_USEALERTS, value);
        }

        private UserThemeMode GetUserThemeMode()
        {
            if (!ContainsKey(KEY_USERTHEME))
            {
                SetUserThemeMode(UserThemeMode.System);
                return UserThemeMode.System;
            }
            else
            {
                return (UserThemeMode)GetValue(KEY_USERTHEME, (int)UserThemeMode.System);
            }
        }

        private void SetUserThemeMode(UserThemeMode value)
        {
            SetValue(KEY_USERTHEME, (int)value);
        }

#if WINDOWS
        [Deprecated(
#else
        [Obsolete(
#endif
            "Replace with SettingsManager.IsKeyVerified(String)"
#if WINDOWS
            , DeprecationType.Deprecate, 5530)]
#else
            )]
#endif
        private bool IsKeyVerified()
        {
            if (!WUSharedSettings.ContainsKey(KEY_APIKEY_VERIFIED))
            {
                return false;
            }
            else
            {
                return WUSharedSettings.GetValue(KEY_APIKEY_VERIFIED, false);
            }
        }

#if WINDOWS
        [Deprecated(
#else
        [Obsolete(
#endif
            "Replace with SettingsManager.SetKeyVerified(String, Boolean)"
#if WINDOWS
            , DeprecationType.Deprecate, 5530)]
#else
            )]
#endif
        private void SetKeyVerified(bool value)
        {
            WUSharedSettings.SetValue(KEY_APIKEY_VERIFIED, value);
            OnSettingsChanged?.Invoke(new SettingsChangedEventArgs { Key = KEY_APIKEY_VERIFIED, NewValue = value });

            if (!value) WUSharedSettings.Remove(KEY_APIKEY_VERIFIED);
        }

        private bool IsKeyVerified(string provider)
        {
            var key = $"{KEY_APIKEY_VERIFIED}_{provider}";

            if (!WUSharedSettings.ContainsKey(key))
            {
                return false;
            }
            else
            {
                return WUSharedSettings.GetValue(key, false);
            }
        }

        private void SetKeyVerified(string provider, bool value)
        {
            var key = $"{KEY_APIKEY_VERIFIED}_{provider}";

            WUSharedSettings.SetValue(key, value);
            OnSettingsChanged?.Invoke(new SettingsChangedEventArgs { Key = key, NewValue = value });

            if (!value) WUSharedSettings.Remove(key);
        }

        private bool IsPersonalKey()
        {
            if (!ContainsKey(KEY_USEPERSONALKEY))
            {
                return WUSharedSettings.GetValue(KEY_USEPERSONALKEY, false);
            }
            else
            {
                return GetValue(KEY_USEPERSONALKEY, false);
            }
        }

        private void SetPersonalKey(bool value)
        {
            SetValue(KEY_USEPERSONALKEY, value);
        }

        private int GetVersionCode()
        {
            return VersionSettings.GetValue(KEY_CURRENTVERSION, 0);
        }

        private void SetVersionCode(int value)
        {
            VersionSettings.SetValue(KEY_CURRENTVERSION, value);
            OnSettingsChanged?.Invoke(new SettingsChangedEventArgs { Key = KEY_CURRENTVERSION, NewValue = value });
        }

        private bool IsOnBoardingComplete()
        {
            if (!ContainsKey(KEY_ONBOARDINGCOMPLETE))
            {
                SetValue(KEY_ONBOARDINGCOMPLETE, false);
                return false;
            }
            else
            {
                return GetValue(KEY_ONBOARDINGCOMPLETE, false);
            }
        }

        private void SetOnBoardingComplete(bool value)
        {
            SetValue(KEY_ONBOARDINGCOMPLETE, value);
            OnSettingsChanged?.Invoke(new SettingsChangedEventArgs { Key = KEY_ONBOARDINGCOMPLETE, NewValue = value });
        }

        private bool IsDevSettingsEnabled()
        {
            return DevSettings.GetValue(KEY_DEVSETTINGSENABLED, false);
        }

        private void SetDevSettingsEnabled(bool value)
        {
            DevSettings.SetValue(KEY_DEVSETTINGSENABLED, value);
            OnSettingsChanged?.Invoke(new SettingsChangedEventArgs { Key = KEY_DEVSETTINGSENABLED, NewValue = value });
        }

#if WINDOWS || __ANDROID__
        public IDictionary<string, object> GetDevSettingsPreferenceMap()
        {
            return GetAllDevSettings().WhereNot(kvp => Equals(kvp.Key, KEY_DEVSETTINGSENABLED))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
#endif

        public void ClearDevSettingsPreferences(bool? enable = null)
        {
            var enabled = enable ?? IsDevSettingsEnabled();
            ClearDevSettings();
            SetDevSettingsEnabled(enabled);
        }

        private string GetIconsProvider()
        {
            return GetValue(KEY_ICONSSOURCE, WeatherIconsEFProvider.KEY);
        }

        private void SetIconsProvider(string value)
        {
            SetValue(KEY_ICONSSOURCE, value);
        }

        private bool IsDailyNotificationEnabled()
        {
            return GetValue(KEY_DAILYNOTIFICATION, false);
        }

        private void SetDailyNotificationEnabled(bool value)
        {
            SetValue(KEY_DAILYNOTIFICATION, value);
        }

        private TimeSpan GetDailyNotificationTime()
        {
            if (GetValue<string>(KEY_DAILYNOTIFICATIONTIME, null) is string value && value is not null)
            {
                return TimeSpan.ParseExact(value.ToString(), "hh\\:mm", CultureInfo.InvariantCulture, TimeSpanStyles.None);
            }

            return DEFAULT_DAILYNOTIFICATION_TIME;
        }

        private void SetDailyNotificationTime(TimeSpan value)
        {
            SetValue(KEY_DAILYNOTIFICATIONTIME, value.ToString("hh\\:mm"));
        }

        private bool IsPoPChanceNotificationEnabled()
        {
            return GetValue(KEY_POPCHANCENOTIFICATION, false);
        }

        private void SetPoPChanceNotificationEnabled(bool value)
        {
            SetValue(KEY_POPCHANCENOTIFICATION, value);
        }

        private DateTimeOffset GetLastPoPChanceNotificationTime()
        {
            if (GetValue<string>(KEY_LASTCHANCENOTIFICATIONTIME, null) is string value && value is not null)
            {
                return DateTimeOffset.ParseExact(value.ToString(), DateTimeUtils.ISO8601_DATETIMEOFFSET_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }

            return DateTimeOffset.MinValue;
        }

        private void SetLastPoPChanceNotificationTime(DateTimeOffset value)
        {
            SetValue(KEY_LASTCHANCENOTIFICATIONTIME, value.ToISO8601Format());
        }

        private int GetPoPChanceMinimumPercentage()
        {
            return GetValue<int>(KEY_POPCHANCEPCT, 60);
        }

        private void SetPoPChanceMinimumPercentage([Range(40, 90)] int value)
        {
            if (value >= 40 && value <= 90)
            {
                SetValue(KEY_POPCHANCEPCT, value);
            }
            else
            {
                SetValue(KEY_POPCHANCEPCT, 60);
            }
        }
        #endregion
    }
}