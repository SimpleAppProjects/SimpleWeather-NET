using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SimpleWeather.Droid.Notifications;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Droid.WeatherAlerts
{
    public static class WeatherAlertHandler
    {
        public static async Task PostAlerts(LocationData location, List<WeatherAlert> alerts)
        {
            var wm = WeatherManager.GetInstance();

            if (wm.SupportsAlerts && alerts != null && alerts.Count > 0)
            {
                // Only alert if we're in the background
                if (App.ApplicationState != AppState.Foreground)
                {
                    // Check if any of these alerts have been posted before
                    // or are past the expiration date
                    var unotifiedAlerts = alerts.Where(alert => alert.Notified == false && alert.ExpiresDate > DateTimeOffset.Now);

                    // Post any un-notified alerts
                    await WeatherAlertNotificationBuilder.CreateNotifications(location, unotifiedAlerts.ToList());

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