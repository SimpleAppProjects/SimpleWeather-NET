#if __ANDROID__
using Android;
using Android.App;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Microsoft.Maui.ApplicationModel;
#endif
#if __IOS__ || __MACCATALYST__
using UserNotifications;
#endif
using System.Threading.Tasks;
using SimpleWeather.Utils;

namespace SimpleWeather.Common.Helpers
{
    public static class NotificationPermissionRequestHelper
    {
        public static async Task<bool> NotificationPermissionEnabled()
        {
#if WINDOWS
            return true;
#elif __ANDROID__
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                return PermissionChecker.CheckSelfPermission(Platform.AppContext, Manifest.Permission.PostNotifications) == PermissionChecker.PermissionGranted;
            }
            else
            {
                return true;
            }
#elif __IOS__ || __MACCATALYST__
            var notificationSettings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();
            return notificationSettings.AuthorizationStatus == UNAuthorizationStatus.Authorized || notificationSettings.AuthorizationStatus == UNAuthorizationStatus.Provisional;
#else
            return true;
#endif
        }

        public static async Task<bool> RequestNotificationPermission()
        {
#if WINDOWS
            return true;
#elif __ANDROID__
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                var result = await Permissions.RequestAsync<NotificationPermission>();
                return result == PermissionStatus.Granted;
            }
            else
            {
                return true;
            }
#elif __IOS__ || __MACCATALYST__
            var notificationCenter = UNUserNotificationCenter.Current;
            var result = await notificationCenter.RequestAuthorizationAsync(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Provisional);
            if (result.Item2 != null)
            {
                Logger.WriteLine(LoggerLevel.Debug, $"NotificationPermission: error - {result.Item2.Description}");
            }
            return result.Item1;
#else
            return true;
#endif
        }

        public static async Task<bool> AreNotificationsEnabled()
        {
#if WINDOWS
            return true;
#elif __ANDROID__
            return NotificationManagerCompat.From(Platform.AppContext).AreNotificationsEnabled();
#elif __IOS__ || __MACCATALYST__
            var notificationSettings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();
            return notificationSettings.AlertStyle != UNAlertStyle.None;
#else
            return true;
#endif
        }
    }
}
