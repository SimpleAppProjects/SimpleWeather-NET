using Microsoft.EntityFrameworkCore;
using SimpleWeather.Controls;
using SimpleWeather.WeatherData;
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
                typeof(T) != typeof(HourlyForecastItemViewModel) &&
                typeof(T) != typeof(TextForecastItemViewModel))
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

                    using (var dBContext = new WeatherDBContext())
                    {
                        if (typeof(T) == typeof(ForecastItemViewModel))
                        {
                            var dbSet = dBContext.Forecasts;
                            var fcast = await dbSet.FindAsync(weather.query);
                            var dataCount = fcast?.forecast?.Count;

                            if (dataCount > 0 && dataCount != currentCount)
                            {
                                var loadedData = fcast.forecast
                                                      .Skip(currentCount)
                                                      .Take((int)count);

                                bool isDayAndNt = fcast.txt_forecast?.Count == fcast.forecast?.Count * 2;
                                bool addTextFct = isDayAndNt || fcast.txt_forecast?.Count == fcast.forecast?.Count;

                                foreach (var dataItem in loadedData)
                                {
                                    object f;
                                    if (addTextFct)
                                    {
                                        if (isDayAndNt)
                                            f = new ForecastItemViewModel(dataItem, new TextForecastItemViewModel(fcast.txt_forecast[(int)resultCount * 2]), new TextForecastItemViewModel(fcast.txt_forecast[((int)resultCount * 2) + 1]));
                                        else
                                            f = new ForecastItemViewModel(dataItem, new TextForecastItemViewModel(fcast.txt_forecast[(int)resultCount]));
                                    }
                                    else
                                    {
                                        f = new ForecastItemViewModel(dataItem);
                                    }

                                    Add((T)f);
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
                            var dbSet = dBContext.HourlyForecasts;
                            var dataCount = await dbSet.CountAsync(hrf => hrf.query == weather.query);

                            if (dataCount > 0 && dataCount != currentCount)
                            {
                                var lastItem = this.LastOrDefault();
                                IQueryable<HourlyForecast> data = null;

                                if (lastItem is HourlyForecast f)
                                {
                                    data = dbSet.Where(hrf => hrf.query == weather.query && hrf.date > f.date)
                                                .OrderBy(hrf => hrf.date)
                                                .Take((int)count)
                                                .Select(hrf => hrf.hr_forecast);
                                }
                                else
                                {
                                    data = dbSet.Where(hrf => hrf.query == weather.query)
                                                .Skip(currentCount)
                                                .Take((int)count)
                                                .Select(hrf => hrf.hr_forecast);
                                }

                                await data.ForEachAsync(dataItem =>
                                {
                                    object fcast = new HourlyForecastItemViewModel(dataItem);
                                    Add((T)fcast);
                                    resultCount++;
                                });
                            }
                            else
                            {
                                HasMoreItems = false;
                            }
                        }
                        else if (typeof(T) == typeof(TextForecastItemViewModel))
                        {
                            var dbSet = dBContext.Forecasts;
                            var fcast = await dbSet.FindAsync(weather.query);
                            var dataCount = fcast?.txt_forecast?.Count;

                            if (dataCount > 0 && dataCount != currentCount)
                            {
                                var loadedData = fcast.txt_forecast
                                                      .Skip(currentCount)
                                                      .Take((int)count);

                                foreach (var dataItem in loadedData)
                                {
                                    object f = new TextForecastItemViewModel(dataItem);
                                    Add((T)f);
                                    resultCount++;
                                }
                            }
                            else
                            {
                                HasMoreItems = false;
                            }
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