using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static class DBUtils
    {
        public static async Task<bool> WeatherDataExists(SQLite.SQLiteAsyncConnection dbConn)
        {
            try
            {
                int count = await dbConn.Table<WeatherData.Weather>().CountAsync();
                if (count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<bool> LocationDataExists(SQLite.SQLiteAsyncConnection dbConn)
        {
            try
            {
                await dbConn.Table<WeatherData.Favorites>().CountAsync();
                int count = await dbConn.Table<WeatherData.LocationData>().CountAsync();
                if (count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
