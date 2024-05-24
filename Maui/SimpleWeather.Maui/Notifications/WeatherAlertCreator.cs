#if __IOS__ || __MACCATALYST__
using Foundation;
using SimpleWeather.Common.Controls;
using SimpleWeather.WeatherData;
using UserNotifications;

namespace SimpleWeather.Maui.Notifications
{
    public static partial class WeatherAlertCreator
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
                    Subtitle = alertVM.ExpireDate
                };

                if (alertVM.Message?.StartsWith("https://weatherkit.apple.com") == true)
                {
                    content.CategoryIdentifier = CATEGORY_WEATHERKIT_ALERT;
                    content.Body = "";
                    content.UserInfo = NSDictionary.FromObjectAndKey(new NSString(KEY_HYPERLINK), new NSString(alertVM.Message));
                }
                else
                {
                    content.Body = alertVM.Message;
                }

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
                catch { }
            }
        }
    }
}
#endif