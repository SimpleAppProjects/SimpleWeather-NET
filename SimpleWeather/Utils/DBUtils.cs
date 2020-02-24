using Microsoft.EntityFrameworkCore;
using SimpleWeather.Location;
using SimpleWeather.WeatherData;
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
        public static bool WeatherDataExists(WeatherDBContext dbConn)
        {
            try
            {
                return dbConn?.WeatherData?.Count() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool LocationDataExists(LocationDBContext dbConn)
        {
            try
            {
                return dbConn?.Locations?.Count() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}