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
        internal static async Task PerformDBMigrations(SQLiteAsyncConnection weatherDB, SQLiteAsyncConnection locationDB)
        {
            if (Settings.DBVersion < Settings.CurrentDBVersion)
            {
                Task migrationTask = Task.Run(async () =>
                {
                    switch (Settings.DBVersion)
                    {
                        // Move data from json to db
                        case 0:
                            if (await AsyncTask.RunAsync(locationDB.Table<LocationData>().CountAsync()) == 0)
                                await DBMigrations.MigrateDataJsonToDB(locationDB, weatherDB);
                            goto case 1;
                        // Add and set tz_long column in db
                        case 1:
                        // LocationData updates: added new fields
                        case 2:
                        case 3:
                            if (await AsyncTask.RunAsync(locationDB.Table<LocationData>().CountAsync()) > 0)
                                DBMigrations.SetLocationData(locationDB, Settings.API);
                            goto case 4;
                        case 4:
                            if (await AsyncTask.RunAsync(weatherDB.Table<Weather>().CountAsync()) > 0)
                            {
                                try
                                {
                                    await DBMigrations.Migrate4_5(weatherDB);
                                }
                                catch (Exception e)
                                {
                                    // Allow recovery if this migration fails since weatherdata is expendable
                                    Logger.WriteLine(LoggerLevel.Error, e, "Migrate4_5 failed; Purging weather database!!");

                                    // Purge and recreate database
                                    await weatherDB.DropTableAsync<Weather>();
                                    await weatherDB.DropTableAsync<WeatherAlerts>();
                                    await weatherDB.DropTableAsync<Forecasts>();
                                    await weatherDB.DropTableAsync<HourlyForecasts>();

                                    await weatherDB.CreateTablesAsync<Weather, WeatherAlerts, Forecasts, HourlyForecasts>();
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }).ContinueWith((t) => 
                {
                    if (t.IsFaulted && t.Exception != null)
                        throw t.Exception;

                    if (t.IsCompletedSuccessfully)
                        Settings.DBVersion = Settings.CurrentDBVersion;
                });

                await migrationTask;
            }
        }

        internal static void PerformVersionMigrations(SQLiteAsyncConnection weatherDB, SQLiteAsyncConnection locationDB)
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
                    DBMigrations.SetLocationData(locationDB, WeatherAPI.Here);
                    Settings.SaveLastGPSLocData(new LocationData());
                }
                // v1.3.8+ - Yahoo API is back in service (but updated)
                // Update location data from current geocoder service
                if (WeatherAPI.Yahoo.Equals(Settings.API) && Settings.VersionCode < 1380)
                {
                    DBMigrations.SetLocationData(locationDB, WeatherAPI.Yahoo);
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
        }
    }
}