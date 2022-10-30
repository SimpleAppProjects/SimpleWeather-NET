using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleWeather.Migrations
{
    internal partial class DataMigrations
    {
        internal static async Task PerformDBMigrations(SQLiteAsyncConnection locationDB, SQLiteAsyncConnection weatherDB)
        {
            if (Settings.DBVersion < Settings.CurrentDBVersion)
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
                                await DBMigrations.MigrateDataJsonToDB(locationDB, weatherDB);
                            goto case 1;
                        // Add and set tz_long column in db
                        case 1:
                        // LocationData updates: added new fields
                        case 2:
                        case 3:
                            if (await locationDB.Table<LocationData>().CountAsync() > 0)
                                await DBMigrations.SetLocationData(locationDB, Settings.API);
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
            }
        }
    }
}