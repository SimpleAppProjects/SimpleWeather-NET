using SimpleWeather.Database;
using SimpleWeather.Utils;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.TZDB
{
    public class TZDBServiceImpl : ITZDBService
    {
        public async Task<string> GetTimeZone(double latitude, double longitude)
        {
            if (latitude != 0 && longitude != 0)
            {
                AnalyticsLogger.LogEvent("TZDBCache: querying");

                // Initialize db if it hasn't been already
                var tzDB = TZDatabase.Instance;

                // Search db if result already exists
                var dbResult = await tzDB.GetTimeZoneData(latitude, longitude);

                if (!string.IsNullOrWhiteSpace(dbResult))
                    return dbResult;

                // Search tz lookup
                var result = await new TimeZoneProviderImpl().GetTimeZone(latitude, longitude);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    // Cache result
                    _ = Task.Run(async () =>
                    {
                        await tzDB.InsertTZData(new SimpleWeather.TZDB.TZDB()
                        {
                            latitude = latitude,
                            longitude = longitude,
                            tz_long = result
                        });
                    });

                    return result;
                }
            }

            return "unknown";
        }
    }
}