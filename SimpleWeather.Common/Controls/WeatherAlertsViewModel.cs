using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using SimpleWeather.ComponentModel;
using SimpleWeather.Database;
using SimpleWeather.LocationData;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleWeather.Common.Controls
{
    public partial class WeatherAlertsViewModel : BaseViewModel, IDisposable
    {
        private LocationData.LocationData locationData = null;

        private readonly WeatherDatabase WeatherDB = WeatherDatabase.Instance;

        private ObservableItem<ICollection<WeatherAlert>> currentAlertsData;

        [ObservableProperty]
        private SimpleObservableList<WeatherAlertViewModel> alerts;

        public WeatherAlertsViewModel()
        {
            Alerts = new SimpleObservableList<WeatherAlertViewModel>();
            currentAlertsData = new ObservableItem<ICollection<WeatherAlert>>();
            currentAlertsData.ItemValueChanged += CurrentAlertsData_ItemValueChanged;
        }

        public void UpdateAlerts(LocationData.LocationData location)
        {
            if (locationData == null || !Equals(locationData?.query, location?.query))
            {
                DispatcherQueue.EnqueueAsync(async () =>
                {
                    locationData = new LocationQuery(location).ToLocationData();

                    WeatherDB.Connection.GetConnection().TableChanged -= WeatherAlertsViewModel_TableChanged;

                    // Update alerts from database
                    currentAlertsData.SetValue(AlertsMapper(await WeatherDB.GetWeatherAlertData(location.query)));

                    WeatherDB.Connection.GetConnection().TableChanged += WeatherAlertsViewModel_TableChanged;
                });
            }
        }

        private void WeatherAlertsViewModel_TableChanged(object sender, SQLite.NotifyTableChangedEventArgs e)
        {
            Task.Run(async () =>
            {
                if (e?.Table?.TableName == WeatherAlerts.TABLE_NAME)
                {
                    if (locationData?.query == null) return;

                    currentAlertsData.SetValue(AlertsMapper(await WeatherDB.GetWeatherAlertData(locationData.query)));
                }
            });
        }

        private Func<WeatherAlerts, ICollection<WeatherAlert>> AlertsMapper = _alerts =>
        {
            return _alerts?.alerts ?? new List<WeatherAlert>(0);
        };

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
                WeatherDB.Connection.GetConnection().TableChanged -= WeatherAlertsViewModel_TableChanged;
            }

            isDisposed = true;
        }
    }
}