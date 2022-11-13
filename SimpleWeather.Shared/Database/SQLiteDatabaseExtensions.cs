using SQLite;
using System.Threading.Tasks;

namespace SimpleWeather.Database
{
    public static class SQLiteDatabaseExtensions
    {
        public static void SetDatabaseToVersion(this SQLiteConnection conn, int version)
        {
            conn.ExecuteScalar<int>($"PRAGMA user_version = {version}");
        }

        public static int GetDatabaseVersion(this SQLiteConnection conn)
        {
            return conn.ExecuteScalar<int>("PRAGMA user_version");
        }

        public static Task SetDatabaseToVersion(this SQLiteAsyncConnection conn, int version)
        {
            return conn.ExecuteScalarAsync<int>($"PRAGMA user_version = {version}");
        }

        public static Task<int> GetDatabaseVersion(this SQLiteAsyncConnection conn)
        {
            return conn.ExecuteScalarAsync<int>("PRAGMA user_version");
        }
    }
}
