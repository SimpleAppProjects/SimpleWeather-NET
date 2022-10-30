using SimpleWeather.Location;
using SimpleWeather.TZDB;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SQLite;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace SimpleWeather.Migrations
{
    internal partial class DataMigrations
    {
        internal static async Task PerformVersionMigrations(SQLiteAsyncConnection weatherDB, SQLiteAsyncConnection locationDB)
        {
            var PackageVersion = Windows.ApplicationModel.Package.Current.Id.Version;
            var version = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}0",
                PackageVersion.Major, PackageVersion.Minor, PackageVersion.Build); // Exclude revision number used by Xbox & others
            var CurrentVersionCode = int.Parse(version, CultureInfo.InvariantCulture);

            if (Settings.WeatherLoaded && Settings.VersionCode < CurrentVersionCode)
            {
                // v4.2.0+ (Units)
                if (Settings.VersionCode < 4201)
                {
                    string tempUnit = Settings.TemperatureUnit;
                    if (Units.CELSIUS.Equals(tempUnit))
                    {
                        Settings.SetDefaultUnits(Units.CELSIUS);
                    }
                    else
                    {
                        Settings.SetDefaultUnits(Units.FAHRENHEIT);
                    }
                }
                // v4.3.3 (OWM)
                // Temporarily disable OWM for now; we're going over the quota
                if (Settings.VersionCode < 4330)
                {
                    if (WeatherAPI.OpenWeatherMap.Equals(Settings.API) && Settings.UsePersonalKey)
                    {
                        Settings.API = WeatherAPI.MetNo;
                        var wm = WeatherManager.GetInstance();
                        wm.UpdateAPI();
                        Settings.UsePersonalKey = false;
                        Settings.KeyVerified = true;
                    }
                }
                if (Settings.VersionCode < 5010)
                {
                    if (WeatherAPI.Here.Equals(Settings.API) || WeatherAPI.Yahoo.Equals(Settings.API))
                    {
                        Settings.API = WeatherAPI.WeatherUnlocked;
                        var wm = WeatherManager.GetInstance();
                        wm.UpdateAPI();
                        Settings.UsePersonalKey = false;
                        Settings.KeyVerified = true;
                    }

                    await DBUtils.UpdateLocationKey(locationDB);
                    if (Settings.lastGPSLocData is LocationData locData && locData.IsValid())
                    {
                        var oldKey = locData.query;

                        locData.query = WeatherManager.GetProvider(locData.weatherSource)
                            .UpdateLocationQuery(locData);
                        Settings.SaveLastGPSLocData(locData);

#if WINDOWS_UWP && !UNIT_TEST
                        // Update tile id for location
                        SharedModule.Instance.RequestAction(
                            CommonActions.ACTION_WEATHER_UPDATETILELOCATION,
                            new Dictionary<string, object>
                            {
                                        { Constants.TILEKEY_OLDKEY, oldKey },
                                        { Constants.TILEKEY_LOCATION, Constants.KEY_GPS },
                            });
#endif
                    }
                    else
                    {
                        Settings.SaveLastGPSLocData(new LocationData());
                    }
                }
                // API_KEY -> GetAPIKey(String)
                if (Settings.VersionCode < 5520)
                {
                    // API_KEY -> GetAPIKey(String)
                    var weatherAPI = Settings.API;
                    if (weatherAPI != null)
                    {
                        Settings.APIKeys[weatherAPI] = Settings.API_KEY;
                        Settings.KeysVerified[weatherAPI] = Settings.KeyVerified;
                    }

                    // DevSettings -> Settings.SetAPIKey
                    var devSettingsMap = DevSettingsEnabler.GetPreferenceMap();
                    devSettingsMap.ForEach((kvp) =>
                    {
                        if (kvp.Value is string)
                        {
                            Settings.APIKeys[kvp.Key] = kvp.Value.ToString();
                            Settings.KeysVerified[kvp.Key] = true;
                        }
                    });
                    DevSettingsEnabler.ClearPreferences();
                }
                AnalyticsLogger.LogEvent("App upgrading", new Dictionary<string, string>()
                    {
                        { "API", Settings.API },
                        { "API_IsInternalKey", (!Settings.UsePersonalKey).ToString() },
                        { "VersionCode", Settings.VersionCode.ToString() },
                        { "CurrentVersionCode", CurrentVersionCode.ToString() }
                    });
            }
            // TZ Refresh
            if (Settings.VersionCode < 5700)
            {
                var locations = new List<LocationData>(await Settings.GetLocationData());
                (await Settings.GetLastGPSLocData())?.Let(l => locations.Add(l));

                foreach (var location in locations)
                {
                    if (Equals(location.tz_long, "unknown") || Equals(location.tz_long, "UTC"))
                    {
                        if (location.latitude != 0 && location.longitude != 0)
                        {
                            var tzId = await TZDBCache.GetTimeZone(location.latitude, location.longitude);
                            if (!Equals("unknown", tzId))
                            {
                                location.tz_long = tzId;
                                // Update DB here or somewhere else
                                await Settings.UpdateLocation(location);
                            }
                        }
                    }
                }
            }
#if WINDOWS_UWP && !UNIT_TEST
            if (Settings.VersionCode < CurrentVersionCode)
            {
                FeatureSettings.IsUpdateAvailable = false;
            }
#endif
            Settings.VersionCode = CurrentVersionCode;
        }
    }
}
