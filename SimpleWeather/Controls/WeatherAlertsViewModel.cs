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

namespace SimpleWeather.Controls
{
    public class WeatherAlertsViewModel : INotifyPropertyChanged
    {
        private LocationData LocationData { get; set; }

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
            await AsyncTask.RunOnUIThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }).ConfigureAwait(true);
        }

        public async Task UpdateAlerts(LocationData location)
        {
            this.LocationData = location;

            // Update alerts from database
            var alerts = await Settings.GetWeatherAlertData(location.query);

            Alerts.Clear();
            if (alerts != null && alerts.Count > 0)
            {
                foreach (var alert in alerts)
                {
                    // Skip if alert has expired
                    if (alert.ExpiresDate <= DateTimeOffset.Now)
                        continue;

                    WeatherAlertViewModel alertView = new WeatherAlertViewModel(alert);
                    Alerts.Add(alertView);
                }
            }
        }
    }
}
