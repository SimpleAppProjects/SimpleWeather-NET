using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Controls
{
    public class ForecastGraphViewModel : INotifyPropertyChanged
    {
        private Weather weather;
        private String locationKey;
        private String tempUnit;

        private SimpleObservableList<ForecastItemViewModel> forecasts;
        private SimpleObservableList<HourlyForecastItemViewModel> hourlyForecasts;

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

        public Task UpdateForecasts(Weather weather)
        {
            return Task.Run(() =>
            {
                if (!Equals(this.weather, weather) || !Equals(tempUnit, Settings.Unit))
                {
                    this.weather = weather;
                    this.tempUnit = Settings.Unit;

                    // Update forecasts from database
                    RefreshForecasts();
                    RefreshHourlyForecasts();
                }
            });
        }

        public Task UpdateForecasts(LocationData location)
        {
            return Task.Run(() =>
            {
                if (!Equals(this.locationKey, location.query) || !Equals(tempUnit, Settings.Unit))
                {
                    this.locationKey = location.query;
                    this.tempUnit = Settings.Unit;

                    // Update forecasts from database
                    RefreshForecasts();
                    RefreshHourlyForecasts();
                }
            });
        }

        private Task RefreshForecasts()
        {
            return Task.Run(async () =>
            {
                Forecasts fcasts = null;
                try
                {
                    fcasts = await Settings.GetWeatherForecastData(weather.query);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "{0}: error refreshing forecasts", nameof(ForecastGraphViewModel));
                }

                await AsyncTask.TryRunOnUIThread(() =>
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
            });
        }

        private Task RefreshHourlyForecasts()
        {
            return Task.Run(async () =>
            {
                IList<HourlyForecast> hrfcasts = null;
                try
                {
                    hrfcasts = await Settings.GetHourlyWeatherForecastDataByPageIndexByLimit(weather.query, 0, 24);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "{0}: error refreshing hourly forecasts", nameof(ForecastGraphViewModel));
                }

                await AsyncTask.TryRunOnUIThread(() =>
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
            });
        }
    }
}
