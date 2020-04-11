using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
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
        private LocationData locationData;

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

        public async Task UpdateForecasts(LocationData location)
        {
            this.locationData = location;

            // Update forecasts from database
            Forecasts.Clear();
            HourlyForecasts.Clear();

            var fcasts = await Settings.GetWeatherForecastData(location.query);
            var hourlyFcasts = await Settings.GetHourlyWeatherForecastDataByPageIndexByLimit(location.query, 0, 24);

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

            if (hourlyFcasts?.Count > 0)
            {
                foreach (var dataItem in hourlyFcasts)
                {
                    HourlyForecasts.Add(new HourlyForecastItemViewModel(dataItem));
                }
            }
            OnPropertyChanged(nameof(HourlyForecasts));
        }
    }
}
