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

namespace SimpleWeather.UWP.Controls
{
    public class ForecastGraphViewModel : INotifyPropertyChanged, IDisposable
    {
        private LocationData locationData;
        private string tempUnit;

        private SimpleObservableList<ForecastItemViewModel> forecasts;
        private SimpleObservableList<HourlyForecastItemViewModel> hourlyForecasts;

        private ObservableItem<Forecasts> currentForecastsData;
        private ObservableItem<IList<HourlyForecast>> currentHrForecastsData;

        public SimpleObservableList<ForecastItemViewModel> Forecasts
        {
            get { return forecasts; }
            private set { forecasts = value; OnPropertyChanged(nameof(Forecasts)); }
        }

        public SimpleObservableList<HourlyForecastItemViewModel> HourlyForecasts
        {
            get { return hourlyForecasts; }
            private set { hourlyForecasts = value; OnPropertyChanged(nameof(HourlyForecasts)); }
        }

        public ForecastGraphViewModel()
        {
            currentForecastsData = new ObservableItem<Forecasts>();
            currentForecastsData.ItemValueChanged += CurrentForecastsData_ItemValueChanged;
            currentHrForecastsData = new ObservableItem<IList<HourlyForecast>>();
            currentHrForecastsData.ItemValueChanged += CurrentHrForecastsData_ItemValueChanged;

            Forecasts = new SimpleObservableList<ForecastItemViewModel>();
            HourlyForecasts = new SimpleObservableList<HourlyForecastItemViewModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected async void OnPropertyChanged(string name)
        {
            await AsyncTask.TryRunOnUIThread(() =>
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
            return AsyncTask.TryRunOnUIThread(() =>
            {
                Forecasts.Clear();

                if (fcasts?.forecast?.Count > 0)
                {
                    bool isDayAndNt = fcasts.txt_forecast?.Count == fcasts.forecast?.Count * 2;
                    bool addTextFct = isDayAndNt || fcasts.txt_forecast?.Count == fcasts.forecast?.Count;

                    for (int i = 0; i < Math.Min(fcasts.forecast.Count, 24); i++)
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
            return AsyncTask.TryRunOnUIThread(() =>
            {
                HourlyForecasts.Clear();

                if (hrfcasts?.Count > 0)
                {
                    foreach (var dataItem in hrfcasts)
                    {
                        HourlyForecasts.Add(new HourlyForecastItemViewModel(dataItem));
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
            }

            isDisposed = true;
        }
    }
}
