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

        public static Task UpdateLocationKey(SQLiteAsyncConnection locationDB)
        {
            return Task.Run(async () => 
            {
                foreach (LocationData location in await locationDB.Table<LocationData>().ToListAsync()
                        .ConfigureAwait(false))
                {
                    var oldKey = location.query;

                    location.query = WeatherManager.GetProvider(location.weatherSource)
                        .UpdateLocationQuery(location);

                    await Settings.UpdateLocationWithKey(location, oldKey).ConfigureAwait(false);

#if WINDOWS_UWP && !UNIT_TEST
                    // Update tile id for location
                    if (oldKey != null)
                    {
                        // Update tile id for location
                        SimpleLibrary.GetInstance().RequestAction(
                            CommonActions.ACTION_WEATHER_UPDATETILELOCATION,
                            new Dictionary<string, string>
                            {
                                        { Constants.TILEKEY_OLDKEY, oldKey },
                                        { Constants.TILEKEY_LOCATION, location.query },
                            });
                    }
#endif
                }
            });
        }
    }
}