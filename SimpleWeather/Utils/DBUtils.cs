using SimpleWeather.Location;
using SimpleWeather.WeatherData;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.Utils
{
    public static partial class DBUtils
    {
        public static bool WeatherDataExists(SQLiteAsyncConnection dbConn)
        {
            try
            {
                var conn = dbConn.GetConnection();
                using (conn.Lock())
                {
                    return conn.Table<Weather>().Count() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool LocationDataExists(SQLiteAsyncConnection dbConn)
        {
            try
            {
                var conn = dbConn.GetConnection();
                using (conn.Lock())
                {
                    return conn.Table<LocationData>().Count() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}