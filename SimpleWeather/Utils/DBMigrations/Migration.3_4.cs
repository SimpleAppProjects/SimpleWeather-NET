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
    public static partial class DBMigrations
    {
        public static void SetLocationData(SQLiteAsyncConnection locationDB, String API)
        {
            AsyncTask.Run(async () =>
            {
                foreach (LocationData location in await locationDB.Table<LocationData>().ToListAsync()
                        .ConfigureAwait(false))
                {
                    await WeatherManager.GetProvider(API)
                        .UpdateLocationData(location)
                        .ConfigureAwait(false);
                }
            });
        }
    }
}