using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SQLite;
using System;
using System.Text.Json;

namespace SimpleWeather.Database
{
    public partial class WeatherDatabase
    {
        private static void Migrate_4_5(SQLiteConnection weatherDB)
        {
            if (weatherDB.Table<Weather>().Count() <= 0)
            {
                return;
            }

            weatherDB.RunInTransaction(() =>
            {
                // Remove the old table
                weatherDB.Execute("DROP TABLE IF EXISTS weatherdata_new");

                // Create the new table
                weatherDB.Execute(
                    "CREATE TABLE `weatherdata_new` (`ttl` varchar, `source` varchar, `query` varchar NOT NULL, `locale` varchar, `locationblob` varchar, `update_time` varchar, `conditionblob` varchar, `atmosphereblob` varchar, `astronomyblob` varchar, `precipitationblob` varchar, PRIMARY KEY(`query`))");
                weatherDB.CreateTable<Forecasts>();
                weatherDB.CreateTable<HourlyForecasts>();
                // Copy the data
                weatherDB.Execute(
                    "INSERT INTO weatherdata_new (`ttl`, `source`, `query`, `locale`, `locationblob`, `update_time`, `conditionblob`, `atmosphereblob`, `astronomyblob`, `precipitationblob`) SELECT `ttl`, `source`, `query`, `locale`, `locationblob`, `update_time`, `conditionblob`, `atmosphereblob`, `astronomyblob`, `precipitationblob` from weatherdata");
                weatherDB.Execute(
                    "INSERT INTO forecasts (`query`, `forecastblob`, `txtforecastblob`) SELECT `query`, `forecastblob`, `txtforecastblob` from weatherdata");
                foreach (var weather in weatherDB.Table<Weather>().ToList())
                {
                    var result = weatherDB.ExecuteScalar<string>("SELECT hrforecastblob from weatherdata WHERE query = ?", weather.query);

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

                                            weatherDB.Execute(
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
                weatherDB.Execute("DROP TABLE weatherdata");
                // Change the table name to the correct one
                weatherDB.Execute(
                    "ALTER TABLE weatherdata_new RENAME TO weatherdata");
            });
        }
    }
}