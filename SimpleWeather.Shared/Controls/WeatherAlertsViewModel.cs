using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using SimpleWeather.ComponentModel;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if WINDOWS_UWP
#endif

namespace SimpleWeather.Controls
{
    public partial class WeatherAlertsViewModel : BaseViewModel, IDisposable
    {
        private string locationKey;

        private ObservableItem<ICollection<WeatherAlert>> currentAlertsData;

        [ObservableProperty]
        private SimpleObservableList<WeatherAlertViewModel> alerts;

        public WeatherAlertsViewModel()
        {
            Alerts = new SimpleObservableList<WeatherAlertViewModel>();
            currentAlertsData = new ObservableItem<ICollection<WeatherAlert>>();
            currentAlertsData.ItemValueChanged += CurrentAlertsData_ItemValueChanged;
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

            DispatcherQueue.EnqueueAsync(() =>
            {
                Alerts.NotifyCollectionChanged();
                OnPropertyChanged(nameof(Alerts));
            });
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