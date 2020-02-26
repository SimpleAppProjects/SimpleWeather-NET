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
                return AsyncTask.RunAsync(dbConn?.Table<Weather>()?.CountAsync())
                        .GetAwaiter().GetResult() > 0;
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
                return AsyncTask.RunAsync(dbConn?.Table<LocationData>()?.CountAsync())
                        .GetAwaiter().GetResult() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}