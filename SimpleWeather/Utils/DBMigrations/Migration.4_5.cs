using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleWeather.Location;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.Utils
{
    public static partial class DBMigrations
    {
        public static void Migrate4_5(WeatherDBContext weatherDB)
        {
            // Create the new table
            weatherDB.Database.ExecuteSqlRaw(
                "CREATE TABLE `weatherdata_new` (`ttl` varchar, `source` varchar, `query` varchar NOT NULL, `locale` varchar, `locationblob` varchar, `update_time` varchar, `conditionblob` varchar, `atmosphereblob` varchar, `astronomyblob` varchar, `precipitationblob` varchar, PRIMARY KEY(`query`))");
            //weatherDB.CreateTable<Forecasts>();
            //weatherDB.CreateTable<HourlyForecasts>();
            // Copy the data
            weatherDB.Database.ExecuteSqlRaw(
                "INSERT INTO weatherdata_new (`ttl`, `source`, `query`, `locale`, `locationblob`, `update_time`, `conditionblob`, `atmosphereblob`, `astronomyblob`, `precipitationblob`) SELECT `ttl`, `source`, `query`, `locale`, `locationblob`, `update_time`, `conditionblob`, `atmosphereblob`, `astronomyblob`, `precipitationblob` from weatherdata");
            weatherDB.Database.ExecuteSqlRaw(
                "INSERT INTO forecasts (`query`, `forecast`, `txt_forecast`) SELECT `query`, `forecastblob`, `txtforecastblob` from weatherdata");
            using (var dbConn = weatherDB.Database.GetDbConnection())
            {
                foreach (var weather in weatherDB.WeatherData)
                {
                    var command = dbConn.CreateCommand();
                    command.CommandText = "SELECT hrforecastblob from weatherdata WHERE query = @query";
                    var param = command.CreateParameter();
                    param.ParameterName = "@query";
                    param.Value = weather.query;
                    command.Parameters.Add(param);

                    using (var result = command.ExecuteReader())
                    {
                        if (result.Read())
                        {
                            var blobs = result["hrforecastblob"]?.ToString();

                            if (blobs != null)
                            {
                                var jsonArr = JArray.Parse(blobs);

                                foreach (var fcastBlob in jsonArr)
                                {
                                    try
                                    {
                                        var json = fcastBlob?.ToString();
                                        var child = JObject.Parse(json);
                                        var date = child?.GetValue("date").Value<string>();

                                        if (json != null && date != null)
                                        {
                                            weatherDB.Database.ExecuteSqlRaw(
                                                "INSERT INTO hr_forecasts (`query`, `dateblob`, `hr_forecast`) VALUES ({0}, {1}, {2})",
                                                weather.query, date, json);
                                        }
                                    }
                                    catch (JsonReaderException e)
                                    {
                                        Logger.WriteLine(LoggerLevel.Error, e, "Error parsing json!");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // Remove the old table
            weatherDB.Database.ExecuteSqlRaw("DROP TABLE weatherdata");
            // Change the table name to the correct one
            weatherDB.Database.ExecuteSqlRaw(
                "ALTER TABLE weatherdata_new RENAME TO weatherdata");
            // Save changes
            weatherDB.SaveChanges();
        }
    }
}