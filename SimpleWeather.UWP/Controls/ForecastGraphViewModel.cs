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
        private String locationKey;

        private List<ForecastItemViewModel> forecasts;
        private List<HourlyForecastItemViewModel> hourlyForecasts;

        public List<ForecastItemViewModel> Forecasts 
        {
            get { return forecasts; }
            private set { forecasts = value; OnPropertyChanged(nameof(Forecasts)); }
        }

        public List<HourlyForecastItemViewModel> HourlyForecasts
        {
            get { return hourlyForecasts; }
            private set { hourlyForecasts = value; OnPropertyChanged(nameof(HourlyForecasts)); }
        }

        public ForecastGraphViewModel()
        {
            Forecasts = new List<ForecastItemViewModel>(10);
            HourlyForecasts = new List<HourlyForecastItemViewModel>(24);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected async void OnPropertyChanged(string name)
        {
            await AsyncTask.RunOnUIThread(() =>
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
                    RefreshHourlyForecasts();
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
                Logger.WriteLine(LoggerLevel.Error, ex, "{0}: error refreshing forecasts", nameof(ForecastGraphViewModel));
            }

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
            }

            OnPropertyChanged(nameof(Forecasts));
        }

        private void RefreshHourlyForecasts()
        {
            HourlyForecasts.Clear();

            IList<HourlyForecast> hrfcasts = null;
            try
            {
                var db = Settings.GetWeatherDBConnection();
                var dbConn = db.GetConnection();
                using (dbConn.Lock())
                {
                    dbConn.TableChanged -= DbConn_TableChanged;

                    var list = dbConn.Table<HourlyForecasts>()
                                     .Where(hrf => hrf.query == locationKey && hrf.hrforecastblob != null)
                                     .OrderBy(hrf => hrf.dateblob)
                                     .Take(24)
                                     .ToList();

                    foreach (var item in list)
                    {
                        dbConn.GetChildren(item);
                    }

                    hrfcasts = list.Select(hrf => hrf.hr_forecast).ToList();

                    dbConn.TableChanged += DbConn_TableChanged;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "{0}: error refreshing hourly forecasts", nameof(ForecastGraphViewModel));
            }

            if (hrfcasts?.Count > 0)
            {
                foreach (var dataItem in hrfcasts)
                {
                    HourlyForecasts.Add(new HourlyForecastItemViewModel(dataItem));
                }
            }

            OnPropertyChanged(nameof(HourlyForecasts));
        }
    }
}
