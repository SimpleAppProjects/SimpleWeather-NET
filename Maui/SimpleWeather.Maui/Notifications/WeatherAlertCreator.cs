#if __IOS__ || __MACCATALYST__
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserNotifications;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Notifications
{
    public static class WeatherAlertCreator
    {
        private const string TAG = "WeatherAlerts";

        public static async Task CreateAlerts(LocationData.LocationData location, IEnumerable<WeatherAlert> alerts)
        {
            foreach (WeatherAlert alert in alerts)
            {
                if (alert.Date > DateTimeOffset.Now)
                    continue;

                var alertVM = new WeatherAlertViewModel(alert);

                var content = new UNMutableNotificationContent()
                {
                    Title = alertVM.Title,
                    Subtitle = alertVM.ExpireDate,
                    Body = alertVM.Message
                };

                // Set a unique ID for the notification
                var notID = string.Format("{0}:{1}", location.query, ((int)alertVM.AlertType).ToString());

                // Create the toast notification
                var notificationCenter = UNUserNotificationCenter.Current;
                var request = UNNotificationRequest.FromIdentifier(
                    $"{TAG}:{notID}", content, null
                );

                try
                {
                    await notificationCenter.AddNotificationRequestAsync(request);
                }
                catch (Exception e)
                {

                }
            }
        }
    }
}
#endif