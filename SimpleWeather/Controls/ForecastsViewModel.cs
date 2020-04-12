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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.Controls
{
    public class ForecastsViewModel : INotifyPropertyChanged
    {
        private String locationKey;

        private List<ForecastItemViewModel> forecasts;
        private IncrementalLoadingCollection<HourlyForecastSource, HourlyForecastItemViewModel> hourlyForecasts;

        public List<ForecastItemViewModel> Forecasts
        {
            get { return forecasts; }
            private set { forecasts = value; OnPropertyChanged(nameof(Forecasts)); }
        }

        public IncrementalLoadingCollection<HourlyForecastSource, HourlyForecastItemViewModel> HourlyForecasts
        {
            get { return hourlyForecasts; }
            private set { hourlyForecasts = value; OnPropertyChanged(nameof(HourlyForecasts)); }
        }

        public ForecastsViewModel()
        {
            Forecasts = new List<ForecastItemViewModel>();
            //HourlyForecasts = new IncrementalLoadingCollection<HourlyForecastSource, HourlyForecastItemViewModel>();
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
            return Task.Run(() =>
            {
                if (!Equals(this.locationKey, location.query))
                {
                    this.locationKey = location.query;

                    // Update forecasts from database
                    RefreshForecasts();
                    ResetHourlyForecasts();
                }
            });
        }

        private void DbConn_TableChanged(object sender, SQLite.NotifyTableChangedEventArgs e)
        {
            if (e?.Table?.TableName == WeatherData.Forecasts.TABLE_NAME)
            {
                RefreshForecasts();
            }

            if (e?.Table?.TableName == WeatherData.HourlyForecasts.TABLE_NAME)
            {
                RefreshHourlyForecasts();
            }
        }

        private void RefreshForecasts()
        {
            Forecasts.Clear();

            Forecasts fcasts = null;
            try
            {
                var db = Settings.GetWeatherDBConnection();
                var dbConn = db.GetConnection();
                using (dbConn.Lock())
                {
                    dbConn.TableChanged -= DbConn_TableChanged;
                    fcasts = dbConn.FindWithChildren<Forecasts>(locationKey);
                    dbConn.TableChanged += DbConn_TableChanged;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "{0}: error refreshing forecasts", nameof(ForecastsViewModel));
            }

            if (fcasts?.forecast?.Count > 0)
            {
                bool isDayAndNt = fcasts.txt_forecast?.Count == fcasts.forecast?.Count * 2;
                bool addTextFct = isDayAndNt || fcasts.txt_forecast?.Count == fcasts.forecast?.Count;

                for (int i = 0; i < fcasts.forecast.Count; i++)
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
            }

            OnPropertyChanged(nameof(Forecasts));
        }

        private void ResetHourlyForecasts()
        {
            try
            {
                var db = Settings.GetWeatherDBConnection();
                var dbConn = db.GetConnection();
                using (dbConn.Lock())
                {
                    dbConn.TableChanged -= DbConn_TableChanged;
                    dbConn.TableChanged += DbConn_TableChanged;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "{0}: error refreshing hourly forecasts", nameof(ForecastsViewModel));
            }

            HourlyForecasts = new IncrementalLoadingCollection<HourlyForecastSource, HourlyForecastItemViewModel>(new HourlyForecastSource(locationKey), 24);
            OnPropertyChanged(nameof(HourlyForecasts));
        }

        private void RefreshHourlyForecasts()
        {
            HourlyForecasts.Clear();

            try
            {
                var db = Settings.GetWeatherDBConnection();
                var dbConn = db.GetConnection();
                using (dbConn.Lock())
                {
                    dbConn.TableChanged -= DbConn_TableChanged;
                    dbConn.TableChanged += DbConn_TableChanged;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "{0}: error refreshing hourly forecasts", nameof(ForecastsViewModel));
            }

            if (HourlyForecasts == null)
            {
                HourlyForecasts = new IncrementalLoadingCollection<HourlyForecastSource, HourlyForecastItemViewModel>(new HourlyForecastSource(locationKey), 24);
            }
            else
            {
                HourlyForecasts.RefreshAsync();
            }

            OnPropertyChanged(nameof(HourlyForecasts));
        }
    }

    public class HourlyForecastSource : IIncrementalSource<HourlyForecastItemViewModel>
    {
        private String locationKey;

        public HourlyForecastSource(String locationKey) : base()
        {
            this.locationKey = locationKey;
        }

        public Task<IEnumerable<HourlyForecastItemViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                if (locationKey == null)
                    return new List<HourlyForecastItemViewModel>(0);

                var result = await Settings.GetHourlyWeatherForecastDataByPageIndexByLimit(locationKey, pageIndex, pageSize);

                List<HourlyForecastItemViewModel> models = new List<HourlyForecastItemViewModel>();

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
