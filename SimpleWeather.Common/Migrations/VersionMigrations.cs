using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.WeatherData;
using SQLite;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace SimpleWeather.Common.Migrations
{
    internal partial class DataMigrations
    {
        internal static async Task PerformVersionMigrations(SQLiteAsyncConnection weatherDB, SQLiteAsyncConnection locationDB)
        {
            var SettingsMgr = DI.Utils.SettingsManager;

            var PackageVersion = Windows.ApplicationModel.Package.Current.Id.Version;
            var version = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}0",
                PackageVersion.Major, PackageVersion.Minor, PackageVersion.Build); // Exclude revision number used by Xbox & others
            var CurrentVersionCode = int.Parse(version, CultureInfo.InvariantCulture);

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

                        locData.query = WeatherModule.Instance.WeatherManager.GetWeatherProvider(locData.weatherSource)
                            .UpdateLocationQuery(locData);
                        await SettingsMgr.SaveLastGPSLocData(locData);

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
                }
                AnalyticsLogger.LogEvent("App upgrading", new Dictionary<string, string>()
                    {
                        { "API", SettingsMgr.API },
                        { "API_IsInternalKey", (!SettingsMgr.UsePersonalKey).ToString() },
                        { "VersionCode", SettingsMgr.VersionCode.ToString() },
                        { "CurrentVersionCode", CurrentVersionCode.ToString() }
                    });
            }
            // TZ Refresh
            if (SettingsMgr.VersionCode < 5700)
            {
                var locations = new List<LocationData.LocationData>(await SettingsMgr.GetLocationData());
                (await SettingsMgr.GetLastGPSLocData())?.Let(l => locations.Add(l));

                foreach (var location in locations)
                {
                    if (Equals(location.tz_long, "unknown") || Equals(location.tz_long, "UTC"))
                    {
                        if (location.latitude != 0 && location.longitude != 0)
                        {
                            var tzId = await WeatherModule.Instance.TZDBService.GetTimeZone(location.latitude, location.longitude);
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
#if WINDOWS_UWP && !UNIT_TEST
            if (SettingsMgr.VersionCode < CurrentVersionCode)
            {
                FeatureSettings.IsUpdateAvailable = false;
            }
#endif
            SettingsMgr.VersionCode = CurrentVersionCode;
        }
    }
}
