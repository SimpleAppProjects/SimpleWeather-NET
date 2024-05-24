#if __IOS__
using Foundation;
using SimpleWeather.Maui.Notifications;
using UserNotifications;

namespace SimpleWeather.Maui
{
    public class NotificationCenterDelegate : NSObject, IUNUserNotificationCenterDelegate
    {
        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            var content = response?.Notification?.Request?.Content;
            var userInfo = content?.UserInfo;

            if (content?.CategoryIdentifier == WeatherAlertCreator.CATEGORY_WEATHERKIT_ALERT)
            {
                if (userInfo?.TryGetValue(new NSString(), out var value) == true)
                {
                    var uri = value as NSString;
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Launcher.TryOpenAsync(new Uri(uri));
                    });
                }
            }

            // Always call the completion handler when done.
            completionHandler();
        }
    }
}
#endif