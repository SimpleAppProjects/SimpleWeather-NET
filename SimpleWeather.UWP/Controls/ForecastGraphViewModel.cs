using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace SimpleWeather.UWP.Controls
{
    public class ForecastGraphViewModel : INotifyPropertyChanged, IDisposable
    {
        private CoreDispatcher Dispatcher;

        private LocationData locationData;
        private string tempUnit;

        private SimpleObservableList<GraphItemViewModel> forecasts;
        private SimpleObservableList<GraphItemViewModel> hourlyForecasts;

        private ObservableItem<Forecasts> currentForecastsData;
        private ObservableItem<IList<HourlyForecast>> currentHrForecastsData;

        public SimpleObservableList<GraphItemViewModel> Forecasts
        {
            get { return forecasts; }
            private set { forecasts = value; OnPropertyChanged(nameof(Forecasts)); }
        }

        public SimpleObservableList<GraphItemViewModel> HourlyForecasts
        {
            get { return hourlyForecasts; }
            private set { hourlyForecasts = value; OnPropertyChanged(nameof(HourlyForecasts)); }
        }

        public ForecastGraphViewModel(CoreDispatcher dispatcher)
        {
            Dispatcher = dispatcher;

            currentForecastsData = new ObservableItem<Forecasts>();
            currentForecastsData.ItemValueChanged += CurrentForecastsData_ItemValueChanged;
            currentHrForecastsData = new ObservableItem<IList<HourlyForecast>>();
            currentHrForecastsData.ItemValueChanged += CurrentHrForecastsData_ItemValueChanged;

            Forecasts = new SimpleObservableList<GraphItemViewModel>();
            HourlyForecasts = new SimpleObservableList<GraphItemViewModel>();
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
            return Task.Run(async () =>
            {
                if (this.locationData == null || !Equals(this.locationData?.query, location?.query))
                {
                    Settings.GetWeatherDBConnection().GetConnection().TableChanged -= ForecastGraphViewModel_TableChanged;

                    // Clone location data
                    this.locationData = new LocationData(new LocationQueryViewModel(location));

                    this.tempUnit = Settings.Unit;

                    currentForecastsData.SetValue(await Settings.GetWeatherForecastData(location.query));

                    var dateBlob = DateTimeOffset.Now.ToOffset(location.tz_offset).Trim(TimeSpan.TicksPerHour).ToString("yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture);
                    currentHrForecastsData.SetValue(await Settings.GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(location.query, 0, 24, dateBlob));

                    Settings.GetWeatherDBConnection().GetConnection().TableChanged += ForecastGraphViewModel_TableChanged;
                }
                else if (!Equals(tempUnit, Settings.Unit))
                {
                    this.tempUnit = Settings.Unit;

                    RefreshForecasts(currentForecastsData.GetValue());
                    RefreshHourlyForecasts(currentHrForecastsData.GetValue());
                }
            });
        }

        private void ForecastGraphViewModel_TableChanged(object sender, NotifyTableChangedEventArgs e)
        {
            if (locationData == null) return;

            Task.Run(async () =>
            {
                if (e?.Table?.TableName == WeatherData.Forecasts.TABLE_NAME)
                {
                    currentForecastsData.SetValue(await Settings.GetWeatherForecastData(locationData.query));
                }
                if (e?.Table?.TableName == WeatherData.HourlyForecasts.TABLE_NAME)
                {
                    currentHrForecastsData.SetValue(await Settings.GetHourlyWeatherForecastDataByPageIndexByLimit(locationData.query, 0, 24));
                }
            });
        }

        private void CurrentForecastsData_ItemValueChanged(object sender, ObservableItemChangedEventArgs e)
        {
            if (e.NewValue is Forecasts fcasts)
            {
                RefreshForecasts(fcasts);
            }
        }

        private ConfiguredTaskAwaitable RefreshForecasts(Forecasts fcasts)
        {
            return Dispatcher.RunOnUIThread(() =>
            {
                Forecasts.Clear();

                if (fcasts?.forecast?.Count > 0)
                {
                    for (int i = 0; i < Math.Min(fcasts.forecast.Count, 10); i++)
                    {
                        var dataItem = fcasts.forecast[i];

                        var f = new GraphItemViewModel(dataItem);

                        Forecasts.Add(f);
                    }

                    OnPropertyChanged(nameof(Forecasts));
                    Forecasts.NotifyCollectionChanged();
                }
            }).ConfigureAwait(true);
        }

        private void CurrentHrForecastsData_ItemValueChanged(object sender, ObservableItemChangedEventArgs e)
        {
            if (e.NewValue is IList<HourlyForecast> hrfcasts)
            {
                RefreshHourlyForecasts(hrfcasts);
            }
        }

        private ConfiguredTaskAwaitable RefreshHourlyForecasts(IList<HourlyForecast> hrfcasts)
        {
            return Dispatcher.RunOnUIThread(() =>
            {
                HourlyForecasts.Clear();

                if (hrfcasts?.Count > 0)
                {
                    foreach (var dataItem in hrfcasts)
                    {
                        HourlyForecasts.Add(new GraphItemViewModel(dataItem));
                    }
                }

                OnPropertyChanged(nameof(HourlyForecasts));
                HourlyForecasts.NotifyCollectionChanged();
            }).ConfigureAwait(true);
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
                Settings.GetWeatherDBConnection().GetConnection().TableChanged -= ForecastGraphViewModel_TableChanged;
                Dispatcher = null;
            }

            isDisposed = true;
        }
    }
}
