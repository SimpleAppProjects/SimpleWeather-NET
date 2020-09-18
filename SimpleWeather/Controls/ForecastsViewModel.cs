using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Uwp;
using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace SimpleWeather.Controls
{
    public class ForecastsViewModel : INotifyPropertyChanged, IDisposable
    {
        private CoreDispatcher Dispatcher;

        private LocationData locationData;
        private string tempUnit;

        private IncrementalLoadingCollection<ForecastSource, ForecastItemViewModel> forecasts;
        private IncrementalLoadingCollection<HourlyForecastSource, HourlyForecastItemViewModel> hourlyForecasts;

        public IncrementalLoadingCollection<ForecastSource, ForecastItemViewModel> Forecasts
        {
            get { return forecasts; }
            private set { forecasts = value; OnPropertyChanged(nameof(Forecasts)); }
        }

        public IncrementalLoadingCollection<HourlyForecastSource, HourlyForecastItemViewModel> HourlyForecasts
        {
            get { return hourlyForecasts; }
            private set { hourlyForecasts = value; OnPropertyChanged(nameof(HourlyForecasts)); }
        }

        public ForecastsViewModel(CoreDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected async void OnPropertyChanged(string name)
        {
            await Dispatcher.RunOnUIThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }).ConfigureAwait(true);
        }

        public Task UpdateForecasts(LocationData location)
        {
            return Task.Run(() =>
            {
                if (this.locationData == null || !Equals(this.locationData?.query, location?.query))
                {
                    Settings.GetWeatherDBConnection().GetConnection().TableChanged -= ForecastsViewModel_TableChanged;

                    // Clone location data
                    this.locationData = new LocationData(new LocationQueryViewModel(location));

                    this.tempUnit = Settings.Unit;

                    // Update forecasts from database
                    ResetForecasts();
                    ResetHourlyForecasts();

                    Settings.GetWeatherDBConnection().GetConnection().TableChanged += ForecastsViewModel_TableChanged;
                }
                else if (!Equals(tempUnit, Settings.Unit))
                {
                    this.tempUnit = Settings.Unit;

                    RefreshForecasts();
                    RefreshHourlyForecasts();
                }
            });
        }

        private void ForecastsViewModel_TableChanged(object sender, SQLite.NotifyTableChangedEventArgs e)
        {
            if (locationData == null) return;

            Task.Run(() =>
            {
                if (e?.Table?.TableName == WeatherData.Forecasts.TABLE_NAME)
                {
                    RefreshForecasts();
                }
                if (e?.Table?.TableName == WeatherData.HourlyForecasts.TABLE_NAME)
                {
                    RefreshHourlyForecasts();
                }
            });
        }

        private void ResetForecasts()
        {
            Forecasts = new IncrementalLoadingCollection<ForecastSource, ForecastItemViewModel>(new ForecastSource(locationData), 7);
            OnPropertyChanged(nameof(Forecasts));
        }

        public void RefreshForecasts()
        {
            if (Forecasts == null)
            {
                Forecasts = new IncrementalLoadingCollection<ForecastSource, ForecastItemViewModel>(new ForecastSource(locationData), 7);
                OnPropertyChanged(nameof(Forecasts));
            }
            else
            {
                Dispatcher.RunOnUIThread(() =>
                {
                    Forecasts.RefreshAsync();
                });
            }
        }

        private void ResetHourlyForecasts()
        {
            HourlyForecasts = new IncrementalLoadingCollection<HourlyForecastSource, HourlyForecastItemViewModel>(new HourlyForecastSource(locationData), 24);
            OnPropertyChanged(nameof(HourlyForecasts));
        }

        public void RefreshHourlyForecasts()
        {
            if (HourlyForecasts == null)
            {
                HourlyForecasts = new IncrementalLoadingCollection<HourlyForecastSource, HourlyForecastItemViewModel>(new HourlyForecastSource(locationData), 24);
                OnPropertyChanged(nameof(HourlyForecasts));
            }
            else
            {
                Dispatcher.RunOnUIThread(() =>
                {
                    HourlyForecasts.RefreshAsync();
                });
            }
        }

        private bool isDisposed;
        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                // free managed resources
                Settings.GetWeatherDBConnection().GetConnection().TableChanged -= ForecastsViewModel_TableChanged;
                Dispatcher = null;
            }

            isDisposed = true;
        }
    }

    public class ForecastSource : IIncrementalSource<ForecastItemViewModel>
    {
        private LocationData locationData;

        public ForecastSource(LocationData location) : base()
        {
            this.locationData = location;
        }

        public Task<IEnumerable<ForecastItemViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                if (locationData?.query == null)
                    return new List<ForecastItemViewModel>(0);

                Forecasts fcasts = null;
                try
                {
                    fcasts = await Settings.GetWeatherForecastData(locationData?.query);
                }
                catch (Exception)
                {
                    // Unable to get forecasts
                }

                int totalCount = (int)fcasts?.forecast?.Count;
                var models = new List<ForecastItemViewModel>(Math.Min(totalCount, pageSize));

                if (totalCount > 0)
                {
                    bool isDayAndNt = fcasts.txt_forecast?.Count == (fcasts.forecast?.Count * 2);
                    bool addTextFct = isDayAndNt || (fcasts.txt_forecast?.Count == fcasts.forecast?.Count && fcasts.txt_forecast?.Count > 0);

                    int startPosition = pageIndex * pageSize;

                    for (int i = startPosition; i < Math.Min(totalCount, startPosition + pageSize); i++)
                    {
                        ForecastItemViewModel f;
                        var dataItem = fcasts.forecast[i];

                        if (addTextFct)
                        {
                            if (isDayAndNt)
                                f = new ForecastItemViewModel(dataItem, fcasts.txt_forecast[i * 2], fcasts.txt_forecast[(i * 2) + 1]);
                            else
                                f = new ForecastItemViewModel(dataItem, fcasts.txt_forecast[i]);
                        }
                        else
                        {
                            f = new ForecastItemViewModel(dataItem);
                        }

                        models.Add(f);
                    }
                }

                return models.AsEnumerable();
            });
        }
    }

    public class HourlyForecastSource : IIncrementalSource<HourlyForecastItemViewModel>
    {
        private LocationData locationData;

        public HourlyForecastSource(LocationData location) : base()
        {
            this.locationData = location;
        }

        public Task<IEnumerable<HourlyForecastItemViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                if (locationData?.query == null)
                    return new List<HourlyForecastItemViewModel>(0);

                var dateBlob = DateTimeOffset.Now.ToOffset(locationData.tz_offset).Trim(TimeSpan.TicksPerHour).ToString("yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture);
                var result = await Settings.GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(locationData?.query, pageIndex, pageSize, dateBlob);

                var models = new List<HourlyForecastItemViewModel>(Math.Min(pageSize, (int)result?.Count));

                if (result?.Count > 0)
                {
                    foreach (HourlyForecast forecast in result)
                    {
                        models.Add(new HourlyForecastItemViewModel(forecast));
                    }
                }

                return models.AsEnumerable();
            });
        }
    }
}