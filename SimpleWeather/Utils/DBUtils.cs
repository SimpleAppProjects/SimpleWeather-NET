using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        public static async Task<bool> WeatherDataExists(WeatherDBContext dbConn)
        {
            try
            {
                var count = await AsyncTask.RunAsync(dbConn?.WeatherData?.CountAsync());
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

        public static async Task<bool> LocationDataExists(LocationDBContext dbConn)
        {
            try
            {
                var count = await AsyncTask.RunAsync(dbConn?.Locations?.CountAsync());
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