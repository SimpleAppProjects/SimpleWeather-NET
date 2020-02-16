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
    public static partial class DBMigrations
    {
        public static void SetLocationData(LocationDBContext locationDB, String API)
        {
            AsyncTask.Run(async () =>
            {
                foreach (LocationData location in locationDB.Locations)
                {
                    await WeatherManager.GetProvider(API)
                        .UpdateLocationData(location)
                        .ConfigureAwait(false);
                }

                await locationDB.SaveChangesAsync();
            });
        }
    }
}