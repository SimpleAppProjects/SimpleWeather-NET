using SimpleWeather.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.TZDB
{
    [Table("tzdb")]
    public class TZDB
    {
        [Indexed(Name = "tz_latidx", Order = 1)]
        public double latitude { get; set; }
        [Indexed(Name = "tz_longidx", Order = 2)]
        public double longitude { get; set; }
        public string tz_long { get; set; }
    }

    public static class TZDBCache
    {
        private static SQLiteAsyncConnection tzDB;

        public static Task<string> GetTimeZone(double latitude, double longitude)
        {
            return Task.Run(async () =>
            {
                if (latitude != 0 && longitude != 0)
                {
                    AnalyticsLogger.LogEvent("TZDBCache: querying");

                    // Initialize db if it hasn't been already
                    if (tzDB == null)
                    {
                        tzDB = new SQLiteAsyncConnection(Settings.GetTZDBConnectionString(), SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);
                        await tzDB.CreateTableAsync<TZDB>();
                    }

                    // Search db if result already exists
                    var dbResult = await tzDB.ExecuteScalarAsync<string>(
                        "select tz_long from tzdb where latitude = ? AND longitude = ?",
                        latitude, longitude);

                    if (!String.IsNullOrWhiteSpace(dbResult))
                        return dbResult;

                    // Search tz lookup
                    var result = await new TimeZoneProvider().GetTimeZone(latitude, longitude);

                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        // Cache result
                        AsyncTask.Run(async () =>
                        {
                            await tzDB.InsertOrReplaceAsync(new TZDB()
                            {
                                latitude = latitude,
                                longitude = longitude,
                                tz_long = result
                            });

                            // Run GC since tz lookup takes up a good chunk of memory
                            GC.Collect();
                        });

                        return result;
                    }
                }

                return "UTC";
            });
        }
    }
}