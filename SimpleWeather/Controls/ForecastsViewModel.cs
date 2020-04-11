using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Uwp;
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
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.Controls
{
    public class ForecastsViewModel : INotifyPropertyChanged
    {
        private LocationData locationData;

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

            var fcasts = await Settings.GetWeatherForecastData(location.query);

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

            HourlyForecasts = new IncrementalLoadingCollection<HourlyForecastSource, HourlyForecastItemViewModel>(new HourlyForecastSource(location), 24);
            OnPropertyChanged(nameof(HourlyForecasts));
        }
    }

    public class HourlyForecastSource : IIncrementalSource<HourlyForecastItemViewModel>
    {
        private LocationData location;

        public HourlyForecastSource(LocationData locationData) : base()
        {
            this.location = locationData;
        }

        public Task<IEnumerable<HourlyForecastItemViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                if (location == null)
                    return new List<HourlyForecastItemViewModel>(0);

                var result = await Settings.GetHourlyWeatherForecastDataByPageIndexByLimit(location.query, pageIndex, pageSize);

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
