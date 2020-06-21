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

namespace SimpleWeather.Controls
{
    public class WeatherAlertsViewModel : INotifyPropertyChanged
    {
        private Weather weather;
        private String locationKey;

        private List<WeatherAlertViewModel> alerts;

        public List<WeatherAlertViewModel> Alerts
        {
            get { return alerts; }
            private set { alerts = value; OnPropertyChanged(nameof(Alerts)); }
        }

        public WeatherAlertsViewModel()
        {
            Alerts = new List<WeatherAlertViewModel>();
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

        public Task UpdateAlerts(Weather weather)
        {
            return Task.Run(() =>
            {
                if (!Equals(this.weather, weather))
                {
                    this.weather = weather;
                    this.locationKey = weather.query;

                    // Update alerts from database
                    RefreshAlerts();
                }
            });
        }

        public Task UpdateAlerts(LocationData location)
        {
            return Task.Run(() =>
            {
                if (!Equals(this.locationKey, location?.query))
                {
                    this.locationKey = location?.query;

                    // Update alerts from database
                    RefreshAlerts();
                }
            });
        }

        public Task RefreshAlerts()
        {
            return Task.Run(async () =>
            {
                Alerts.Clear();

                ICollection<WeatherAlert> alertData = null;
                try
                {
                    alertData = await Settings.GetWeatherAlertData(locationKey);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "{0}: error refreshing alerts", nameof(WeatherAlertsViewModel));
                }

                if (alertData?.Count > 0)
                {
                    foreach (var alert in alertData)
                    {
                        // Skip if alert has expired
                        if (alert.ExpiresDate <= DateTimeOffset.Now)
                            continue;

                        Alerts.Add(new WeatherAlertViewModel(alert));
                    }
                }

                OnPropertyChanged(nameof(Alerts));
            });
        }
    }
}