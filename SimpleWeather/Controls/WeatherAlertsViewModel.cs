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
            await AsyncTask.RunOnUIThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }).ConfigureAwait(true);
        }

        public Task UpdateAlerts(LocationData location)
        {
            return Task.Run(() =>
            {
                if (!Equals(this.locationKey, location.query))
                {
                    this.locationKey = location.query;

                    // Update alerts from database
                    RefreshAlerts();
                }
            });
        }

        private void DbConn_TableChanged(object sender, SQLite.NotifyTableChangedEventArgs e)
        {
            if (e?.Table?.TableName == WeatherAlerts.TABLE_NAME)
            {
                RefreshAlerts();
            }
        }

        private void RefreshAlerts()
        {
            Alerts.Clear();

            ICollection<WeatherAlert> alertData = null;
            try
            {
                var db = Settings.GetWeatherDBConnection();
                var dbConn = db.GetConnection();
                using (dbConn.Lock())
                {
                    dbConn.TableChanged -= DbConn_TableChanged;
                    var weatherAlertData = dbConn.FindWithChildren<WeatherAlerts>(locationKey);

                    if (weatherAlertData != null && weatherAlertData.alerts != null)
                        alertData = weatherAlertData.alerts;

                    dbConn.TableChanged += DbConn_TableChanged;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "{0}: error refreshing alerts", nameof(WeatherAlertsViewModel));
            }

            if (alerts?.Count > 0)
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
        }
    }
}