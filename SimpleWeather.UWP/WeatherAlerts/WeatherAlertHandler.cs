using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Notifications;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.WeatherAlerts
{
    public static class WeatherAlertHandler
    {
        public static async Task PostAlerts(LocationData location, ICollection<WeatherAlert> alerts)
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
                    WeatherAlertCreator.CreateAlerts(location, unotifiedAlerts);

                    // Update all alerts
                    foreach (var alert in alerts)
                    {
                        alert.Notified = true;
                    }

                    // Save alert data
                    await Settings.SaveWeatherAlerts(location, alerts);
                }
            }
        }

        public static async Task SetasNotified(LocationData location, ICollection<WeatherAlert> alerts)
        {
            if (alerts != null)
            {
                // Update all alerts
                foreach (var alert in alerts)
                {
                    alert.Notified = true;
                }

                // Save alert data
                await Settings.SaveWeatherAlerts(location, alerts);
            }
        }
    }
}