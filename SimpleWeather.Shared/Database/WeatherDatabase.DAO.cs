using SimpleWeather.SQLiteNet;
using SimpleWeather.WeatherData;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleWeather.Database
{
    public partial class WeatherDatabase
    {
        #region Weather
        public Task InsertWeatherData(Weather weather)
        {
            return _database.InsertOrReplaceWithChildrenAsync(weather);
        }

        public Task DeleteWeatherData(Weather weather)
        {
            return _database.DeleteAsync<Weather>(weather.query);
        }

        public Task DeleteWeatherDataByKeyNotIn(IEnumerable<string> keyQuery)
        {
            return _database.DeleteAllIdsNotInAsync<Weather>(keyQuery);
        }

        public Task<List<Weather>> LoadAllWeatherData()
        {
            return _database.GetAllWithChildrenAsync<Weather>();
        }

        public Task<Weather> GetWeatherData(string query)
        {
            return _database.FindWithChildrenAsync<Weather>(query);
        }

        public Task<Weather> GetWeatherDataByCoord(string query)
        {
            return _database.FindWithQueryWithChildrenAsync<Weather>("SELECT * FROM weatherdata WHERE `locationblob` LIKE ? LIMIT 1", query);
        }

        public Task<Weather> GetWeatherDataByCoord(string query1, string query2)
        {
            return _database.FindWithQueryWithChildrenAsync<Weather>("SELECT * FROM weatherdata WHERE `locationblob` LIKE ? OR `locationblob` LIKE ? LIMIT 1", query1, query2);
        }

        public Task<int> GetWeatherDataCount()
        {
            return _database.Table<Weather>().CountAsync();
        }

        public Task<int> GetWeatherDataCountByKey(string query)
        {
            return _database.Table<Weather>().CountAsync(w => w.query == query);
        }
        #endregion

        #region Alerts
        public Task InsertWeatherAlertData(WeatherAlerts alert)
        {
            return _database.InsertOrReplaceWithChildrenAsync(alert);
        }

        public Task DeleteWeatherAlertData(WeatherAlerts alert)
        {
            return _database.DeleteAsync<WeatherAlerts>(alert.query);
        }

        public Task DeleteWeatherAlertDataByKeyNotIn(IEnumerable<string> keyQuery)
        {
            return _database.DeleteAllIdsNotInAsync<WeatherAlerts>(keyQuery);
        }

        public Task<List<WeatherAlerts>> LoadAllWeatherAlertData()
        {
            return _database.GetAllWithChildrenAsync<WeatherAlerts>();
        }

        public Task<WeatherAlerts> GetWeatherAlertData(string query)
        {
            return _database.FindWithChildrenAsync<WeatherAlerts>(query);
        }

        public Task<int> GetWeatherAlertDataCount()
        {
            return _database.Table<WeatherAlerts>().CountAsync();
        }
        #endregion

        #region Forecasts
        public Task InsertForecast(Forecasts forecast)
        {
            return _database.InsertOrReplaceWithChildrenAsync(forecast);
        }

        public Task DeleteForecast(Forecasts forecast)
        {
            return _database.DeleteAsync<Forecasts>(forecast.query);
        }

        public Task DeleteForecastByKeyNotIn(IEnumerable<string> keyQuery)
        {
            return _database.DeleteAllIdsNotInAsync<Forecasts>(keyQuery);
        }

        public Task<List<Forecasts>> LoadAllForecasts()
        {
            return _database.GetAllWithChildrenAsync<Forecasts>();
        }

        public Task<Forecasts> GetForecastData(string query)
        {
            return _database.FindWithChildrenAsync<Forecasts>(query);
        }

        public Task<int> GetForecastDataCountGroupedByQuery()
        {
            return _database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM forecasts GROUP BY `query`");
        }
        #endregion

        #region Hourly Forecasts
        public Task InsertAllHourlyForecasts(IEnumerable<HourlyForecasts> forecasts)
        {
            return _database.InsertOrReplaceAllWithChildrenAsync(forecasts);
        }

        public Task InsertForecast(HourlyForecasts forecast)
        {
            return _database.InsertOrReplaceWithChildrenAsync(forecast);
        }

        public Task DeleteHourlyForecast(HourlyForecasts forecast)
        {
            return _database.DeleteAsync<HourlyForecasts>(forecast.query);
        }

        public Task DeleteHourlyForecastsByKey(string key)
        {
            return _database.ExecuteAsync($"delete from {HourlyForecasts.TABLE_NAME} where query = ?", key);
        }

        public Task DeleteHourlyForecastByKeyNotIn(IEnumerable<string> keyQuery)
        {
            return _database.DeleteAllIdsNotInAsync<HourlyForecasts>(keyQuery);
        }

        public Task<List<HourlyForecast>> GetHourlyForecastsByQueryOrderByDate(string key)
        {
            return Task.Run(() =>
            {
                List<HourlyForecasts> items;

                var connectionWithLock = SqliteAsyncConnectionWrapper.Lock(_database);
                using (connectionWithLock.Lock())
                {
                    items = connectionWithLock.Table<HourlyForecasts>()
                                              .Where(hrf => hrf.query == key && hrf.hrforecastblob != null)
                                              .OrderBy(hrf => hrf.dateblob)
                                              .ToList();

                    foreach (var item in items)
                    {
                        connectionWithLock.GetChildren(item);
                    }
                }

                return items.Select(hrf => hrf.hr_forecast).ToList();
            });
        }

        public Task<List<HourlyForecast>> GetHourlyForecastsByQueryOrderByDateByLimit(string key, int loadSize)
        {
            return Task.Run(() =>
            {
                List<HourlyForecasts> items;

                var connectionWithLock = SqliteAsyncConnectionWrapper.Lock(_database);
                using (connectionWithLock.Lock())
                {
                    items = connectionWithLock.Table<HourlyForecasts>()
                                              .Where(hrf => hrf.query == key && hrf.hrforecastblob != null)
                                              .OrderBy(hrf => hrf.dateblob)
                                              .Take(loadSize)
                                              .ToList();

                    foreach (var item in items)
                    {
                        connectionWithLock.GetChildren(item);
                    }
                }

                return items.Select(hrf => hrf.hr_forecast).ToList();
            });
        }

        public Task<List<HourlyForecast>> GetHourlyWeatherForecastDataByPageIndexByLimit(string key, int pageIndex, int loadSize)
        {
            return Task.Run(() =>
            {
                List<HourlyForecasts> items;

                var connectionWithLock = SqliteAsyncConnectionWrapper.Lock(_database);
                using (connectionWithLock.Lock())
                {
                    items = connectionWithLock.Table<HourlyForecasts>()
                                              .Where(hrf => hrf.query == key && hrf.hrforecastblob != null)
                                              .OrderBy(hrf => hrf.dateblob)
                                              .Skip(pageIndex * loadSize)
                                              .Take(loadSize)
                                              .ToList();

                    foreach (var item in items)
                    {
                        connectionWithLock.GetChildren(item);
                    }
                }

                return items.Select(hrf => hrf.hr_forecast).ToList();
            });
        }

        public Task<List<HourlyForecast>> GetHourlyForecastsByQueryOrderByDateByLimitFilterByDate(string key, int loadSize, DateTimeOffset date)
        {
            var dateblob = date.ToString("yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture);

            return Task.Run(() =>
            {
                List<HourlyForecasts> items;

                var connectionWithLock = SqliteAsyncConnectionWrapper.Lock(_database);
                using (connectionWithLock.Lock())
                {
                    items = connectionWithLock.Query<HourlyForecasts>(
                        $"SELECT `hrforecastblob` FROM {HourlyForecasts.TABLE_NAME} WHERE `query` = ? AND `dateblob` >= ? ORDER BY `dateblob` LIMIT ?",
                        key, dateblob, loadSize
                    );

                    foreach (var item in items)
                    {
                        connectionWithLock.GetChildren(item);
                    }
                }

                return items.Select(hrf => hrf.hr_forecast).ToList();
            });
        }

        public Task<List<HourlyForecast>> GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(string key, int pageIndex, int loadSize, DateTimeOffset date)
        {
            var dateblob = date.ToString("yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture);

            return Task.Run(() =>
            {
                List<HourlyForecasts> items;

                var connectionWithLock = SqliteAsyncConnectionWrapper.Lock(_database);
                using (connectionWithLock.Lock())
                {
                    items = connectionWithLock.Query<HourlyForecasts>(
                        $"SELECT * FROM {HourlyForecasts.TABLE_NAME} WHERE `query` = ? AND `hrforecastblob` IS NOT NULL AND `dateblob` >= ? ORDER BY `dateblob` LIMIT ? OFFSET ?",
                        key, dateblob, loadSize, pageIndex * loadSize
                    );

                    foreach (var item in items)
                    {
                        connectionWithLock.GetChildren(item);
                    }
                }

                return items.Select(hrf => hrf.hr_forecast).ToList();
            });
        }

        public Task<int> GetHourlyForecastCountGroupedByQuery()
        {
            return _database.ExecuteScalarAsync<int>($"select count(*) from (select count(*) from {HourlyForecasts.TABLE_NAME} group by query)");
        }

        public Task<HourlyForecast> GetFirstHourlyForecastDataByDate(string key, DateTimeOffset date)
        {
            var dateblob = date.ToString("yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture);

            return Task.Run(() =>
            {
                var connectionWithLock = SqliteAsyncConnectionWrapper.Lock(_database);
                using (connectionWithLock.Lock())
                {
                    var data = connectionWithLock.FindWithQuery<HourlyForecasts>(
                        "SELECT * FROM " + HourlyForecasts.TABLE_NAME + " WHERE `query` = ? AND `dateblob` >= ? ORDER BY `dateblob` LIMIT 1",
                        key, dateblob
                    );

                    return data?.hr_forecast;
                }
            });
        }
        #endregion
    }
}
