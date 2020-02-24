using Microsoft.EntityFrameworkCore;
using SimpleWeather.Location;
using SimpleWeather.WeatherData;
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
        public static async Task Migrate4_5(WeatherDBContext weatherDB)
        {
            // Create the new table
            await weatherDB.Database.ExecuteSqlRawAsync(
                "CREATE TABLE `weatherdata_new` (`ttl` varchar, `source` varchar, `query` varchar NOT NULL, `locale` varchar, `locationblob` varchar, `update_time` varchar, `conditionblob` varchar, `atmosphereblob` varchar, `astronomyblob` varchar, `precipitationblob` varchar, PRIMARY KEY(`query`))");
            //weatherDB.CreateTable<Forecasts>();
            //weatherDB.CreateTable<HourlyForecasts>();
            // Copy the data
            await weatherDB.Database.ExecuteSqlRawAsync(
                "INSERT INTO weatherdata_new (`ttl`, `source`, `query`, `locale`, `locationblob`, `update_time`, `conditionblob`, `atmosphereblob`, `astronomyblob`, `precipitationblob`) SELECT `ttl`, `source`, `query`, `locale`, `locationblob`, `update_time`, `conditionblob`, `atmosphereblob`, `astronomyblob`, `precipitationblob` from weatherdata");
            await weatherDB.Database.ExecuteSqlRawAsync(
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

                    using (var result = await command.ExecuteReaderAsync())
                    {
                        if (await result.ReadAsync())
                        {
                            var blobs = result["hrforecastblob"]?.ToString();

                            if (blobs != null)
                            {
                                try
                                {
                                    using (var jsonArr = JsonDocument.Parse(blobs))
                                    {
                                        foreach (var fcastBlob in jsonArr.RootElement.EnumerateArray())
                                        {
                                            var json = fcastBlob.GetString();
                                            using (var child = JsonDocument.Parse(json))
                                            {
                                                var date = child.RootElement.GetProperty("date").GetString();

                                                if (json != null && date != null)
                                                {
                                                    await weatherDB.Database.ExecuteSqlRawAsync(
                                                        "INSERT INTO hr_forecasts (`query`, `dateblob`, `hr_forecast`) VALUES ({0}, {1}, {2})",
                                                        weather.query, date, json);
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
                    }
                }
            }
            // Remove the old table
            await weatherDB.Database.ExecuteSqlRawAsync("DROP TABLE weatherdata");
            // Change the table name to the correct one
            await weatherDB.Database.ExecuteSqlRawAsync(
                "ALTER TABLE weatherdata_new RENAME TO weatherdata");
            // Save changes
            await weatherDB.SaveChangesAsync();
        }
    }
}