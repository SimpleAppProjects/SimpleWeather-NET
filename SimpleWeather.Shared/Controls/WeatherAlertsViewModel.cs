using SimpleWeather.Controls;
using SimpleWeather.ComponentModel;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS_UWP
using Windows.UI.Core;
using Windows.UI.Xaml;
#endif

namespace SimpleWeather.Controls
{
    public class WeatherAlertsViewModel :
#if WINDOWS_UWP
        DependencyObject,
#endif
        IViewModel, IDisposable
    {
        private String locationKey;

        private ObservableItem<ICollection<WeatherAlert>> currentAlertsData;

#if WINDOWS_UWP
        public SimpleObservableList<WeatherAlertViewModel> Alerts
        {
            get { return (SimpleObservableList<WeatherAlertViewModel>)GetValue(AlertsProperty); }
            set { SetValue(AlertsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Alerts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlertsProperty =
            DependencyProperty.Register("Alerts", typeof(SimpleObservableList<WeatherAlertViewModel>), typeof(WeatherAlertsViewModel), new PropertyMetadata(null));
#else
        public List<WeatherAlertViewModel> Alerts { get; set; }
#endif

        public WeatherAlertsViewModel()
        {
            Alerts = new SimpleObservableList<WeatherAlertViewModel>();
            currentAlertsData = new ObservableItem<ICollection<WeatherAlert>>();
            currentAlertsData.ItemValueChanged += CurrentAlertsData_ItemValueChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
#if WINDOWS_UWP
            Dispatcher.LaunchOnUIThread(() =>
            {
#endif
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
#if WINDOWS_UWP
            });
#endif
        }

        public void UpdateAlerts(LocationData location)
        {
            if (!Equals(this.locationKey, location?.query))
            {
                Task.Run(async () =>
                {
                    Settings.GetWeatherDBConnection().GetConnection().TableChanged -= WeatherAlertsViewModel_TableChanged;

                    this.locationKey = location?.query;

                    // Update alerts from database
                    currentAlertsData.SetValue(await Settings.GetWeatherAlertData(locationKey));

                    Settings.GetWeatherDBConnection().GetConnection().TableChanged += WeatherAlertsViewModel_TableChanged;
                });
            }
        }

        private void WeatherAlertsViewModel_TableChanged(object sender, SQLite.NotifyTableChangedEventArgs e)
        {
            if (locationKey == null) return;

            Task.Run(async () =>
            {
                if (e?.Table?.TableName == WeatherData.WeatherAlerts.TABLE_NAME)
                {
                    currentAlertsData.SetValue(await Settings.GetWeatherAlertData(locationKey));
                }
            });
        }

        private void CurrentAlertsData_ItemValueChanged(object sender, ObservableItemChangedEventArgs e)
        {
            if (e.NewValue is ICollection<WeatherAlert> alertData)
            {
                RefreshAlerts(alertData);
            }
        }

        private void RefreshAlerts(ICollection<WeatherAlert> alertData)
        {
#if WINDOWS_UWP
            Dispatcher.LaunchOnUIThread(() =>
            {
#endif
                Alerts.Clear();

                if (alertData?.Count > 0)
                {
                    var now = DateTimeOffset.Now;
                    Alerts.EnsureCapacity(alertData.Count);
                    foreach (var alert in alertData)
                    {
                        // Skip if alert has expired
                        if (alert.ExpiresDate <= now || alert.Date > now)
                            continue;

                        Alerts.Add(new WeatherAlertViewModel(alert));
                    }
                }

                Alerts.NotifyCollectionChanged();
                OnPropertyChanged(nameof(Alerts));
#if WINDOWS_UWP
            });
#endif
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
                Settings.GetWeatherDBConnection().GetConnection().TableChanged -= WeatherAlertsViewModel_TableChanged;
            }

            isDisposed = true;
        }
    }
}