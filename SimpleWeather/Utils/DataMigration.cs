using SimpleWeather.Location;
using SimpleWeather.WeatherData;
using SQLite;
using System;
using System.Linq;
using System.Threading.Tasks;
#if !DEBUG && !UNIT_TEST
using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;
#endif

namespace SimpleWeather.Utils
{
    internal partial class DataMigrations
    {
        internal static Task PerformDBMigrations(SQLiteAsyncConnection locationDB, SQLiteAsyncConnection weatherDB)
        {
            if (Settings.DBVersion < Settings.CurrentDBVersion)
            {
                return Task.Run(async () =>
                {
                    try
                    {
                        switch (Settings.DBVersion)
                        {
                            // Move data from json to db
                            case 0:
                                if (await locationDB.Table<LocationData>().CountAsync() == 0)
                                    DBMigrations.MigrateDataJsonToDB(locationDB, weatherDB);
                                goto case 1;
                            // Add and set tz_long column in db
                            case 1:
                            // LocationData updates: added new fields
                            case 2:
                            case 3:
                                if (await locationDB.Table<LocationData>().CountAsync() > 0)
                                    DBMigrations.SetLocationData(locationDB, Settings.API);
                                goto case 4;
                            case 4:
                                if (await weatherDB.Table<Weather>().CountAsync() > 0)
                                {
                                    await DBMigrations.Migrate4_5(weatherDB);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        // Allow recovery if  migration fails since weatherdata is expendable
                        Logger.WriteLine(LoggerLevel.Error, e, "Migration v{0} -> v{1} failed; Purging weather database!!", Settings.DBVersion, Settings.CurrentDBVersion);

                        Settings.DestroyDatabase();
                        Settings.CreateDatabase();
                    }
                    finally
                    {
                        Settings.DBVersion = Settings.CurrentDBVersion;
                    }
                });
            }
            else
                return Task.CompletedTask;
        }

        internal static Task PerformVersionMigrations(SQLiteAsyncConnection weatherDB, SQLiteAsyncConnection locationDB)
        {
            return Task.Run(async () =>
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
                        await DBMigrations.SetLocationData(locationDB, WeatherAPI.Here);
                        Settings.SaveLastGPSLocData(new LocationData());
                    }
                    // v1.3.8+ - Yahoo API is back in service (but updated)
                    // Update location data from current geocoder service
                    if (WeatherAPI.Yahoo.Equals(Settings.API) && Settings.VersionCode < 1380)
                    {
                        await DBMigrations.SetLocationData(locationDB, WeatherAPI.Yahoo);
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

                            if (wm.KeyRequired && String.IsNullOrWhiteSpace(wm.GetAPIKey()))
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
#if !DEBUG && !UNIT_TEST
                    Analytics.TrackEvent("App upgrading", new Dictionary<string, string>()
                    {
                        { "API", Settings.API },
                        { "API_IsInternalKey", (!Settings.UsePersonalKey).ToString() }
                    });
#endif
                }
                Settings.VersionCode = CurrentVersionCode;
            });
        }
    }
}