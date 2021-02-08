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
        public static ConfiguredTaskAwaitable Migrate4_5(SQLiteAsyncConnection weatherDB)
        {
            return weatherDB.RunInTransactionAsync((transaction) => 
            {
                // Remove the old table
                transaction.Execute("DROP TABLE IF EXISTS weatherdata_new");

                // Create the new table
                transaction.Execute(
                    "CREATE TABLE `weatherdata_new` (`ttl` varchar, `source` varchar, `query` varchar NOT NULL, `locale` varchar, `locationblob` varchar, `update_time` varchar, `conditionblob` varchar, `atmosphereblob` varchar, `astronomyblob` varchar, `precipitationblob` varchar, PRIMARY KEY(`query`))");
                transaction.CreateTable<Forecasts>();
                transaction.CreateTable<HourlyForecasts>();
                // Copy the data
                transaction.Execute(
                    "INSERT INTO weatherdata_new (`ttl`, `source`, `query`, `locale`, `locationblob`, `update_time`, `conditionblob`, `atmosphereblob`, `astronomyblob`, `precipitationblob`) SELECT `ttl`, `source`, `query`, `locale`, `locationblob`, `update_time`, `conditionblob`, `atmosphereblob`, `astronomyblob`, `precipitationblob` from weatherdata");
                transaction.Execute(
                    "INSERT INTO forecasts (`query`, `forecastblob`, `txtforecastblob`) SELECT `query`, `forecastblob`, `txtforecastblob` from weatherdata");
                foreach (var weather in transaction.Table<Weather>().ToList())
                {
                    var result = transaction.ExecuteScalar<string>("SELECT hrforecastblob from weatherdata WHERE query = ?", weather.query);

                    if (result != null)
                    {
                        try
                        {
                            using (var jsonArr = JsonDocument.Parse(result))
                            {
                                foreach (var fcastBlob in jsonArr.RootElement.EnumerateArray())
                                {
                                    var json = fcastBlob.GetString();
                                    using (var child = JsonDocument.Parse(json))
                                    {
                                        var date = child.RootElement.GetProperty("date").GetString();

                                        if (json != null && date != null)
                                        {
                                            var dto = DateTimeOffset.ParseExact(date, "dd.MM.yyyy HH:mm:ss zzzz", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);
                                            var dtoStr = dto.ToString("yyyy-MM-dd HH:mm:ss zzzz", System.Globalization.CultureInfo.InvariantCulture);

                                            transaction.Execute(
                                                "INSERT INTO hr_forecasts (`id`, `query`, `dateblob`, `hrforecastblob`) VALUES (?, ?, ?, ?)",
                                                weather.query + '|' + dtoStr, weather.query, dtoStr, json);
                                        }
                                    }
                                }
                            }
                        }
                        catch (JsonException e)
                        {
                            Logger.WriteLine(LoggerLevel.Error, e, "Error parsing json!");
                        }
                    }
                }
                // Remove the old table
                transaction.Execute("DROP TABLE weatherdata");
                // Change the table name to the correct one
                transaction.Execute(
                    "ALTER TABLE weatherdata_new RENAME TO weatherdata");
            }).ConfigureAwait(false);
        }
    }
}