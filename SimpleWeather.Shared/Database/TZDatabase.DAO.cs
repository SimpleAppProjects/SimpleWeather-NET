using System.Threading.Tasks;

namespace SimpleWeather.Database
{
    public partial class TZDatabase
    {
        public Task InsertTZData(TZDB.TZDB tzdb)
        {
            return _database.InsertOrReplaceAsync(tzdb);
        }

        public Task DeleteTZData(TZDB.TZDB tzdb)
        {
            return _database.DeleteAsync(tzdb);
        }

        public Task<string> GetTimeZoneData(double lat, double lon)
        {
            return _database.ExecuteScalarAsync<string>(
                    "select tz_long from tzdb where latitude = ? AND longitude = ?",
                    lat, lon
            );
        }
    }
}
