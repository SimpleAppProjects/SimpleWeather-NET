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
        DependencyObject, INotifyPropertyChanged,
#endif
        IDisposable
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

#if WINDOWS_UWP
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected async void OnPropertyChanged(string name)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }).AsTask().ConfigureAwait(true);
        }
#endif

        public Task UpdateAlerts(LocationData location)
        {
            return Task.Run(async () =>
            {
                if (!Equals(this.locationKey, location?.query))
                {
                    Settings.GetWeatherDBConnection().GetConnection().TableChanged -= WeatherAlertsViewModel_TableChanged;

                    this.locationKey = location?.query;

                    // Update alerts from database
                    currentAlertsData.SetValue(await Settings.GetWeatherAlertData(locationKey));

                    Settings.GetWeatherDBConnection().GetConnection().TableChanged += WeatherAlertsViewModel_TableChanged;
                }
            });
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

        private async void CurrentAlertsData_ItemValueChanged(object sender, ObservableItemChangedEventArgs e)
        {
            if (e.NewValue is ICollection<WeatherAlert> alertData)
            {
                await RefreshAlerts(alertData);
            }
        }

        public ConfiguredTaskAwaitable RefreshAlerts(ICollection<WeatherAlert> alertData)
        {
#if WINDOWS_UWP
            return Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
#else
            return Task.Run(() =>
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
#if WINDOWS_UWP
                OnPropertyChanged(nameof(Alerts));
            }).AsTask().ConfigureAwait(true);
#else
            }).ConfigureAwait(true);
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