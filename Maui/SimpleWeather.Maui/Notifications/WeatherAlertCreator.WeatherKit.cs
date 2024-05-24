#if __IOS__ || __MACCATALYST__
using Foundation;
using UserNotifications;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Notifications
{
    public static partial class WeatherAlertCreator
    {
        public const string CATEGORY_WEATHERKIT_ALERT = "WEATHERKIT_ALERT";
        public const string ACTION_MORE_INFO = "ACTION_MOREINFO";
        public const string KEY_HYPERLINK = "KEY_HYPERLINK";

        public static void AddWeatherKitNotificationCategory()
        {
            var notificationCenter = UNUserNotificationCenter.Current;

            var weatherKitCategory = UNNotificationCategory.FromIdentifier(
                identifier: CATEGORY_WEATHERKIT_ALERT,
                actions: [
                    UNNotificationAction.FromIdentifier(
                        identifier: ACTION_MORE_INFO,
                        title: ResStrings.label_moreinfo,
                        options: UNNotificationActionOptions.None
                    )
                ],
                intentIdentifiers: [],
                options: UNNotificationCategoryOptions.None
            );

            notificationCenter.SetNotificationCategories(categories: new NSSet<UNNotificationCategory>(weatherKitCategory));
        }
    }
}
#endif