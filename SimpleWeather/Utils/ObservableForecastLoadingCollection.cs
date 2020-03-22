using SimpleWeather.Controls;
using SimpleWeather.WeatherData;
using SQLiteNetExtensionsAsync.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace SimpleWeather.Utils
{
    public class ObservableForecastLoadingCollection<T> : ObservableLoadingCollection<T>
    {
        private Weather weather;

        public ObservableForecastLoadingCollection()
            : base()
        {
            if (typeof(T) != typeof(ForecastItemViewModel) &&
                typeof(T) != typeof(HourlyForecastItemViewModel))
            {
                throw new NotSupportedException("Collection type not supported");
            }
        }

        public ObservableForecastLoadingCollection(Weather weather)
            : this()
        {
            this.weather = weather;
        }

        public async void SetWeather(Weather weather)
        {
            var old = this.weather;
            this.weather = weather;

            if (!Object.Equals(old, weather) || Count == 0)
                await RefreshAsync();
        }

        public override IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return LoadMoreItems(count).AsAsyncOperation();
        }

        private async Task<LoadMoreItemsResult> LoadMoreItems(uint count)
        {
            uint resultCount = 0;

            if (weather != null)
            {
                try
                {
                    var currentCount = Count;
                    IsLoading = true;
                    HasMoreItems = true;

                    if (typeof(T) == typeof(ForecastItemViewModel))
                    {
                        var db = Settings.GetWeatherDBConnection();
                        var fcast = await db.FindWithChildrenAsync<Forecasts>(weather.query);
                        var dataCount = fcast?.forecast?.Count;

                        if (dataCount > 0 && dataCount != currentCount)
                        {
                            bool isDayAndNt = fcast.txt_forecast?.Count == fcast.forecast?.Count * 2;
                            bool addTextFct = isDayAndNt || fcast.txt_forecast?.Count == fcast.forecast?.Count;

                            for (int i = currentCount; i < fcast.forecast.Count; i++)
                            {
                                object f;
                                var dataItem = fcast.forecast[i];
                                await db.GetChildrenAsync(dataItem);
                                if (addTextFct)
                                {
                                    if (isDayAndNt)
                                        f = new ForecastItemViewModel(dataItem, fcast.txt_forecast[i * 2], fcast.txt_forecast[(i * 2) + 1]);
                                    else
                                        f = new ForecastItemViewModel(dataItem, fcast.txt_forecast[i]);
                                }
                                else
                                {
                                    f = new ForecastItemViewModel(dataItem);
                                }

                                await AsyncTask.RunOnUIThread(() => Add((T)f));
                                resultCount++;
                            }
                        }
                        else
                        {
                            HasMoreItems = false;
                        }
                    }
                    else if (typeof(T) == typeof(HourlyForecastItemViewModel))
                    {
                        var db = Settings.GetWeatherDBConnection();
                        var dbSet = db.Table<HourlyForecasts>();
                        var dataCount = await dbSet.CountAsync(hrf => hrf.query == weather.query);

                        if (dataCount > 0 && dataCount != currentCount)
                        {
                            var lastItem = this.LastOrDefault();
                            IEnumerable<HourlyForecasts> data = null;

                            if (lastItem is HourlyForecastItemViewModel hrfcast)
                            {
                                data = await db.QueryAsync<HourlyForecasts>(
                                    "select * from hr_forecasts where query = ? AND dateblob > ? ORDER BY dateblob LIMIT ?",
                                    weather.query, hrfcast.Forecast.date.ToString("yyyy-MM-dd HH:mm:ss zzzz"), (int)count);
                            }
                            else
                            {
                                data = (await dbSet.Where(hrf => hrf.query == weather.query)
                                            .Skip(currentCount)
                                            .Take((int)count)
                                            .ToListAsync());
                            }

                            foreach (var dataItem in data)
                            {
                                await db.GetChildrenAsync(dataItem);
                                object fcast = new HourlyForecastItemViewModel(dataItem.hr_forecast);
                                await AsyncTask.RunOnUIThread(() => Add((T)fcast));
                                resultCount++;
                            }
                        }
                        else
                        {
                            HasMoreItems = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e, "Error!!");
                }
                finally
                {
                    IsLoading = false;

                    if (_refreshOnLoad)
                    {
                        _refreshOnLoad = false;
                        await RefreshAsync();
                    }
                }
            }

            return new LoadMoreItemsResult() { Count = resultCount };
        }
    }
}