using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common.Utils;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.WeatherData;
using SimpleWeather.WeatherData.Images;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using Version = SimpleWeather.Utils.Version;

namespace SimpleWeather.Common.Migrations
{
    internal partial class DataMigrations
    {
        internal static async Task PerformVersionMigrations(SQLiteAsyncConnection weatherDB,
            SQLiteAsyncConnection locationDB)
        {
            var SettingsMgr = DI.Utils.SettingsManager;

#if WINUI
            var CurrentVersion = Windows.ApplicationModel.Package.Current.Id.Version.ToVersion();
#else
            var CurrentVersion = Microsoft.Maui.ApplicationModel.AppInfo.Version.ToVersion();
#endif

            // Perform migrations for v0-v5.9.9
            if (SettingsMgr.VersionCode < 5990)
            {
                await PerformLegacyVersionMigrations(SettingsMgr, CurrentVersion, weatherDB, locationDB);
            }

            // v5.12.0
            // Update settings for OWM
            if (SettingsMgr.Version.IsNotAtLeast(5, 12))
            {
                if (WeatherAPI.OpenWeatherMap.Equals(SettingsMgr.API, StringComparison.InvariantCultureIgnoreCase))
                {
                    var oldKey = SettingsMgr.API;
                    SettingsMgr.APIKeys[WeatherAPI.OpenWeatherMap] = SettingsMgr.APIKeys[oldKey];
                    SettingsMgr.KeysVerified[WeatherAPI.OpenWeatherMap] = SettingsMgr.KeysVerified[oldKey];
                    SettingsMgr.UsePersonalKeys[WeatherAPI.OpenWeatherMap] = SettingsMgr.UsePersonalKeys[oldKey];
                    SettingsMgr.API = WeatherAPI.OpenWeatherMap;
                }
            }

#if !UNIT_TEST
            if (SettingsMgr.Version < CurrentVersion)
            {
                UpdateSettings.IsUpdateAvailable = false;
            }
#endif
            SettingsMgr.Version = CurrentVersion;
        }

        private static async Task PerformLegacyVersionMigrations(
            Preferences.SettingsManager SettingsMgr, Version CurrentVersion,
            SQLiteAsyncConnection weatherDB, SQLiteAsyncConnection locationDB)
        {
            var version = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}0",
                CurrentVersion.Major, CurrentVersion.Minor,
                CurrentVersion.Build); // Exclude revision number used by Xbox & others
            var CurrentVersionCode = long.Parse(version, CultureInfo.InvariantCulture);

            if (SettingsMgr.WeatherLoaded && SettingsMgr.VersionCode < CurrentVersionCode)
            {
                // v4.2.0+ (Units)
                if (SettingsMgr.VersionCode < 4201)
                {
                    string tempUnit = SettingsMgr.TemperatureUnit;
                    if (Units.CELSIUS.Equals(tempUnit))
                    {
                        SettingsMgr.SetDefaultUnits(Units.CELSIUS);
                    }
                    else
                    {
                        SettingsMgr.SetDefaultUnits(Units.FAHRENHEIT);
                    }
                }

                // v4.3.3 (OWM)
                // Temporarily disable OWM for now; we're going over the quota
                if (SettingsMgr.VersionCode < 4330)
                {
                    if (WeatherAPI.OpenWeatherMap.Equals(SettingsMgr.API) && SettingsMgr.UsePersonalKey)
                    {
                        SettingsMgr.API = WeatherAPI.MetNo;
                        var wm = WeatherModule.Instance.WeatherManager;
                        wm.UpdateAPI();
                        SettingsMgr.UsePersonalKey = false;
                        SettingsMgr.KeyVerified = true;
                    }
                }

                if (SettingsMgr.VersionCode < 5010)
                {
                    if (WeatherAPI.Here.Equals(SettingsMgr.API) || WeatherAPI.Yahoo.Equals(SettingsMgr.API))
                    {
                        SettingsMgr.API = WeatherAPI.WeatherUnlocked;
                        var wm = WeatherModule.Instance.WeatherManager;
                        wm.UpdateAPI();
                        SettingsMgr.UsePersonalKey = false;
                        SettingsMgr.KeyVerified = true;
                    }

                    await DBUtils.UpdateLocationKey(locationDB);
                    if (await SettingsMgr.GetLastGPSLocData() is LocationData.LocationData locData && locData.IsValid())
                    {
                        var oldKey = locData.query;

                        locData.query = await WeatherModule.Instance.WeatherManager
                            .GetWeatherProvider(locData.weatherSource)
                            .UpdateLocationQuery(locData);
                        await SettingsMgr.SaveLastGPSLocData(locData);

#if (WINDOWS || __IOS__) && !UNIT_TEST
                        // Update tile id for location
                        SharedModule.Instance.RequestAction(
                            CommonActions.ACTION_WEATHER_UPDATEWIDGETLOCATION,
                            new Dictionary<string, object>
                            {
                                { Constants.WIDGETKEY_OLDKEY, oldKey },
                                { Constants.WIDGETKEY_LOCATION, Constants.KEY_GPS },
                            });
#endif
                    }
                    else
                    {
                        await SettingsMgr.SaveLastGPSLocData(new LocationData.LocationData());
                    }
                }

                // API_KEY -> GetAPIKey(String)
                if (SettingsMgr.VersionCode < 5520)
                {
                    // API_KEY -> GetAPIKey(String)
                    var weatherAPI = SettingsMgr.API;
                    if (weatherAPI != null)
                    {
                        SettingsMgr.APIKeys[weatherAPI] = SettingsMgr.API_KEY;
                        SettingsMgr.KeysVerified[weatherAPI] = SettingsMgr.KeyVerified;
                    }

                    // DevSettings -> Settings.SetAPIKey
#if WINDOWS || __ANDROID__
                    var devSettingsMap = SettingsMgr.GetDevSettingsPreferenceMap();
                    devSettingsMap.ForEach((kvp) =>
                    {
                        if (kvp.Value is string)
                        {
                            SettingsMgr.APIKeys[kvp.Key] = kvp.Value.ToString();
                            SettingsMgr.KeysVerified[kvp.Key] = true;
                        }
                    });
                    SettingsMgr.ClearDevSettingsPreferences();
#endif
                }

                AnalyticsLogger.LogEvent("App upgrading", new Dictionary<string, string>()
                {
                    { "API", SettingsMgr.API },
                    { "API_IsInternalKey", (!SettingsMgr.UsePersonalKeys[SettingsMgr.API]).ToString() },
                    { "VersionCode", SettingsMgr.VersionCode.ToString() },
                    { "CurrentVersionCode", CurrentVersionCode.ToString() }
                });

                // Capture user props on every update
                AnalyticsLogger.SetUserProperty(AnalyticsProps.WEATHER_PROVIDER, SettingsMgr.API);
                AnalyticsLogger.SetUserProperty(AnalyticsProps.USING_PERSONAL_KEY,
                    SettingsMgr.UsePersonalKeys[SettingsMgr.API]);
                AnalyticsLogger.SetUserProperty(AnalyticsProps.USER_LOCALE, LocaleUtils.GetLocale()?.Name);
            }

            // TZ Refresh
            if (SettingsMgr.VersionCode < 5700)
            {
                var locations = new List<LocationData.LocationData>(await SettingsMgr.GetLocationData());
                (await SettingsMgr.GetLastGPSLocData())?.Let(l => locations.Add(l));

                foreach (var location in locations)
                {
                    if (string.IsNullOrWhiteSpace(location.tz_long) || Equals(location.tz_long, "unknown") ||
                        Equals(location.tz_long, "UTC"))
                    {
                        if (location.latitude != 0 && location.longitude != 0)
                        {
                            var tzId = await WeatherModule.Instance.TZDBService.GetTimeZone(location.latitude,
                                location.longitude);
                            if (!Equals("unknown", tzId))
                            {
                                location.tz_long = tzId;
                                // Update DB here or somewhere else
                                await SettingsMgr.UpdateLocation(location);
                            }
                        }
                    }
                }
            }

            // v5.8.0
            // Clear image cache due to file path change
            // Windows: unregister all bg tasks
            if (SettingsMgr.VersionCode < 5801)
            {
                var ImageDataContainer = new Preferences.SettingsContainer("images");
                ImageDataContainer.Clear();

#if WINDOWS || WINUI
                try
                {
                    var tasks = Windows.ApplicationModel.Background.BackgroundTaskRegistration.AllTasks;

                    foreach (var task in tasks)
                    {
                        try
                        {
                            task.Value.Unregister(true);

                            AnalyticsLogger.LogEvent("BGTasks: unregistered task", new Dictionary<string, string>()
                            {
                                { "task", task.Value.Name },
                            });
                        }
                        catch { }
                    }
                }
                catch { }
#endif
            }

            // v5.8.1
            // Settings.UsePersonalKey
            if (SettingsMgr.VersionCode < 5810)
            {
                SettingsMgr.UsePersonalKeys[SettingsMgr.API] = SettingsMgr.UsePersonalKey;
            }

            // v5.8.6
            if (SettingsMgr.VersionCode < 5860)
            {
                // Windows: unregister all bg tasks
#if WINDOWS || WINUI
                try
                {
                    var tasks = Windows.ApplicationModel.Background.BackgroundTaskRegistration.AllTasks;

                    foreach (var task in tasks)
                    {
                        try
                        {
                            task.Value.Unregister(true);

                            AnalyticsLogger.LogEvent("BGTasks: unregistered task", new Dictionary<string, string>()
                            {
                                { "task", task.Value.Name },
                            });
                        }
                        catch { }
                    }
                }
                catch { }
#endif
            }

            // v5.9.3
            // Reset ImageDatabase
            if (SettingsMgr.VersionCode < 5930)
            {
#if __IOS__
                var imageDataService = Ioc.Default.GetService<IImageDataService>();
                await imageDataService.ClearCachedImageData();
#endif
            }
        }
    }
}