using SimpleWeather.Location;
using SimpleWeather.WeatherData;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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
                        AnalyticsLogger.LogEvent("DataMigrations: PerformDBMigrations",
                            new Dictionary<string, string>()
                            {
                                { "Version", Settings.DBVersion.ToString() },
                                { "CurrentDBVersion", Settings.CurrentDBVersion.ToString() }
                            });

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
                                goto case 5;
                            case 5:
                                if (await weatherDB.Table<Weather>().CountAsync() > 0)
                                {
                                    await DBMigrations.Migrate5_6(weatherDB);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        AnalyticsLogger.LogEvent("DataMigrations: Migration failed",
                            new Dictionary<string, string>()
                            {
                                { "Version", Settings.DBVersion.ToString() },
                                { "CurrentDBVersion", Settings.CurrentDBVersion.ToString() }
                            });

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
                var version = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}",
                    PackageVersion.Major, PackageVersion.Minor, PackageVersion.Build, PackageVersion.Revision);
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
                    if (Settings.VersionCode < 4340)
                    {
                        if (WeatherAPI.Here.Equals(Settings.API))
                        {
                            Settings.API = WeatherAPI.Yahoo;
                            var wm = WeatherManager.GetInstance();
                            wm.UpdateAPI();
                            Settings.UsePersonalKey = false;
                            Settings.KeyVerified = true;
                        }

                        await DBUtils.UpdateLocationKey(locationDB);
                        Settings.SaveLastGPSLocData(new LocationData());
                    }
                    AnalyticsLogger.LogEvent("App upgrading", new Dictionary<string, string>()
                    {
                        { "API", Settings.API },
                        { "API_IsInternalKey", (!Settings.UsePersonalKey).ToString() },
                        { "VersionCode", Settings.VersionCode.ToString() },
                        { "CurrentVersionCode", CurrentVersionCode.ToString() }
                    });
                }
#if WINDOWS_UWP && !UNIT_TEST
                if (Settings.VersionCode < CurrentVersionCode)
                {
                    FeatureSettings.IsUpdateAvailable = false;
                }
#endif
                Settings.VersionCode = CurrentVersionCode;
            });
        }
    }
}