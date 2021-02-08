using SimpleWeather.Location;
using SimpleWeather.WeatherData;
using SQLite;
using SQLiteNetExtensionsAsync;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.Utils
{
    public static partial class DBMigrations
    {
        public static ConfiguredTaskAwaitable Migrate5_6(SQLiteAsyncConnection weatherDB)
        {
            return weatherDB.RunInTransactionAsync((transaction) =>
            {
                // Update data
                var weatherToUpdate = transaction.Table<Weather>()
                    .Where(w => w.source == WeatherAPI.OpenWeatherMap || w.source == WeatherAPI.MetNo);

                foreach (var weather in weatherToUpdate)
                {
                    var query = weather.query;

                    // Update forecasts
                    transaction.Execute("UPDATE forecasts SET forecastblob = REPLACE(forecastblob, ?, ?) WHERE query = ?",
                        "\\\"pop\\\"", "\\\"cloudiness\\\"", query);
                    transaction.Execute("UPDATE hr_forecasts SET hrforecastblob = REPLACE(hrforecastblob, ?, ?) WHERE query = ?",
                        "\"pop\"", "\"cloudiness\"", query);

                    // Update weather data
                    transaction.Execute("UPDATE weatherdata SET precipitationblob = REPLACE(precipitationblob, ?, ?) WHERE query = ?",
                        "\"pop\"", "\"cloudiness\"", query);
                }
            }).ConfigureAwait(false);
        }
    }
}