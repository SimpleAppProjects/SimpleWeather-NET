using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleWeather.Common.Migrations
{
    internal static partial class DBUtils
    {
        public static async Task UpdateLocationKey(SQLiteAsyncConnection locationDB)
        {
            var SettingsManager = new SettingsManager();

            foreach (LocationData.LocationData location in await locationDB.Table<LocationData.LocationData>().ToListAsync()
                        .ConfigureAwait(false))
            {
                var oldKey = location.query;

                location.query = WeatherModule.Instance.WeatherManager.GetWeatherProvider(location.weatherSource)
                    .UpdateLocationQuery(location);

                await SettingsManager.UpdateLocationWithKey(location, oldKey).ConfigureAwait(false);

#if WINDOWS && !UNIT_TEST
                // Update tile id for location
                if (oldKey != null)
                {
                    // Update tile id for location
                    SharedModule.Instance.RequestAction(
                        CommonActions.ACTION_WEATHER_UPDATETILELOCATION,
                        new Dictionary<string, object>
                        {
                            { Constants.TILEKEY_OLDKEY, oldKey },
                            { Constants.TILEKEY_LOCATION, location },
                        });
                }
#endif
            }
        }
    }
}