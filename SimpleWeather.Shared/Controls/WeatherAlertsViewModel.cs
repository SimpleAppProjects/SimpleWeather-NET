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
#endif

namespace SimpleWeather.Controls
{
    public class WeatherAlertsViewModel : INotifyPropertyChanged, IDisposable
    {
#if WINDOWS_UWP
        private CoreDispatcher Dispatcher;
#endif

        private String locationKey;

        private List<WeatherAlertViewModel> alerts;
        private ObservableItem<ICollection<WeatherAlert>> currentAlertsData;

        public List<WeatherAlertViewModel> Alerts
        {
            get { return alerts; }
            private set { alerts = value; OnPropertyChanged(nameof(Alerts)); }
        }

#if WINDOWS_UWP
        public WeatherAlertsViewModel(CoreDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            Initialize();
        }
#endif

        public WeatherAlertsViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            Alerts = new List<WeatherAlertViewModel>();
            currentAlertsData = new ObservableItem<ICollection<WeatherAlert>>();
            currentAlertsData.ItemValueChanged += CurrentAlertsData_ItemValueChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected async void OnPropertyChanged(string name)
        {
#if WINDOWS_UWP
            if (Dispatcher != null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
#endif
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
#if WINDOWS_UWP
                }).AsTask().ConfigureAwait(true);
            }
#endif
        }

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

                OnPropertyChanged(nameof(Alerts));
#if WINDOWS_UWP
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
                Dispatcher = null;
            }

            isDisposed = true;
        }
    }
}