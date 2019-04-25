using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json;
using SimpleWeather.Location;
using SimpleWeather.WeatherData;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.Utils
{
    internal partial class DataMigrations
    {
        internal static async Task PerformDBMigrations(SQLiteAsyncConnection weatherDB, SQLiteAsyncConnection locationDB)
        {
            if (Settings.DBVersion < Settings.CurrentDBVersion)
            {
                switch (Settings.DBVersion)
                {
                    // Move data from json to db
                    case 0:
                        if (await locationDB.Table<LocationData>().CountAsync() == 0)
                            await DBUtils.MigrateDataJsonToDB(locationDB, weatherDB);
                        break;
                    // Add and set tz_long column in db
                    case 1:
                    // LocationData updates: added new fields
                    case 2:
                    case 3:
                        if (await locationDB.Table<LocationData>().CountAsync() > 0)
                            await DBUtils.SetLocationData(locationDB, Settings.API);
                        break;
                    default:
                        break;
                }

                Settings.DBVersion = Settings.CurrentDBVersion;
            }
        }

        internal static async Task PerformVersionMigrations(SQLiteAsyncConnection weatherDB, SQLiteAsyncConnection locationDB)
        {
            var PackageVersion = Windows.ApplicationModel.Package.Current.Id.Version;
            var version = string.Format("{0}{1}{2}{3}",
                PackageVersion.Major, PackageVersion.Minor, PackageVersion.Build, PackageVersion.Revision);
            var CurrentVersionCode = int.Parse(version);

            if (Settings.WeatherLoaded && Settings.VersionCode < CurrentVersionCode)
            {
                // v1.3.7 - Yahoo (YQL) is no longer in service
                // Update location data from HERE Geocoder service
                if (WeatherAPI.Here.Equals(Settings.API) && Settings.VersionCode < 1370)
                {
                    await DBUtils.SetLocationData(locationDB, WeatherAPI.Here);
                    Settings.SaveLastGPSLocData(new LocationData());
                }
                // v1.3.8+ - Yahoo API is back in service (but updated)
                // Update location data from current geocoder service
                if (WeatherAPI.Yahoo.Equals(Settings.API) && Settings.VersionCode < 1380)
                {
                    await DBUtils.SetLocationData(locationDB, WeatherAPI.Yahoo);
                    Settings.SaveLastGPSLocData(new LocationData());
                }
                if (Settings.VersionCode < 1390)
                {
                    // v1.3.8+ - Added onboarding wizard
                    Settings.OnBoardComplete = true;
                    // v1.3.8+
                    // The current WeatherUnderground API is no longer in service
                    // Disable this provider and migrate to HERE
                    if (WeatherAPI.WeatherUnderground.Equals(Settings.API))
                    {
                        // Set default API to HERE
                        Settings.API = WeatherAPI.Here;
                        var wm = WeatherManager.GetInstance();
                        wm.UpdateAPI();

                        if (String.IsNullOrWhiteSpace(wm.GetAPIKey()))
                        {
                            // If (internal) key doesn't exist, fallback to Yahoo
                            Settings.API = WeatherAPI.Yahoo;
                            wm.UpdateAPI();
                            Settings.UsePersonalKey = true;
                            Settings.KeyVerified = false;
                        }
                        else
                        {
                            // If key exists, go ahead
                            Settings.UsePersonalKey = false;
                            Settings.KeyVerified = true;
                        }
                    }
                }
                Analytics.TrackEvent("App upgrading", new Dictionary<string, string>()
                {
                    { "API", Settings.API },
                    { "API_IsInternalKey", (!Settings.UsePersonalKey).ToString() }
                });
            }
            Settings.VersionCode = CurrentVersionCode;
        }
    }
}