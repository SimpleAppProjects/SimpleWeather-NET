using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;

namespace SimpleWeather.UWP.WeatherAlerts
{
    public static class WeatherAlertHandler
    {
        public static async Task PostAlerts(LocationData location, List<WeatherAlert> alerts)
        {
            var wm = WeatherManager.GetInstance();

            // Post weather alert notifications
            if (wm.SupportsAlerts && alerts != null && alerts.Count > 0)
            {
                // Only alert if we're in the background
                if (App.IsInBackground)
                {
                    // Check if any of these alerts have been posted before
                    // or are past the expiration date
                    var unotifiedAlerts = alerts.Where(alert => alert.Notified == false && alert.ExpiresDate > DateTimeOffset.Now);

                    // Post any un-notified alerts
                    Helpers.WeatherAlertCreator.CreateAlerts(location, unotifiedAlerts.ToList());

                    // Update all alerts
                    alerts.ForEach(alert => alert.Notified = true);

                    // Save alert data
                    await Settings.SaveWeatherAlerts(location, alerts);
                }
            }
        }

        public static async Task SetasNotified(LocationData location, List<WeatherAlert> alerts)
        {
            if (alerts != null)
            {
                // Update all alerts
                alerts.ForEach(alert => alert.Notified = true);

                // Save alert data
                await Settings.SaveWeatherAlerts(location, alerts);
            }
        }
    }
}
