using CommunityToolkit.WinUI;
using Microsoft.Windows.AppNotifications.Builder;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.NET.Notifications
{
    public static class WeatherAlertCreator
    {
        private const string TAG = "WeatherAlerts";

        private static async Task CreateToastCollection()
        {
            string displayName = App.Current.ResLoader.GetString("label_nav_alerts");
            var isLight = await SharedModule.Instance.DispatcherQueue.EnqueueAsync(() =>
            {
                return !App.Current.IsSystemDarkTheme;
            });
            var icon = new Uri($"{WeatherIconsManager.GetPNGBaseUri(isLight)}ic_error.png");

            ToastCollection toastCollection = new ToastCollection(TAG, displayName, $"{Constants.KEY_ACTION}=view-alerts", icon);

            await ToastNotificationManager.GetDefault()
                .GetToastCollectionManager()
                .SaveToastCollectionAsync(toastCollection);
        }

        public static async Task CreateAlerts(LocationData.LocationData location, IEnumerable<WeatherAlert> alerts)
        {
            var isLight = await SharedModule.Instance.DispatcherQueue.EnqueueAsync(() =>
            {
                return !App.Current.IsSystemDarkTheme;
            });
            await CreateToastCollection().ConfigureAwait(true);
            var toastNotifier = await ToastNotificationManager.GetDefault()
                .GetToastNotifierForToastCollectionIdAsync(TAG);

            foreach (WeatherAlert alert in alerts)
            {
                if (alert.Date > DateTimeOffset.Now)
                    continue;

                var alertVM = new WeatherAlertViewModel(alert);

                var notification = new AppNotificationBuilder()
                    .AddText(alertVM.Title)
                    .AddText(alertVM.ExpireDate)
                    .SetAppLogoOverride(new Uri(WeatherUtils.GetAssetFromAlertType(alertVM.AlertType, isAbsoluteUri: true, isLight: isLight)))
                    .SetAttributionText(alertVM.Attribution)
                    .AddButton(new AppNotificationButton(ResStrings.label_moreinfo)
                        .AddArgument(Constants.KEY_ACTION, "open-link")
                        .AddArgument(Constants.KEY_DATA, alertVM.Message)
                    )
                    .AddArgument(Constants.KEY_ACTION, "view-alerts")
                    .AddArgument(Constants.KEY_DATA, await JSONParser.SerializerAsync(location))
                    .BuildNotification();

                // Set a unique ID for the notification
                var notID = string.Format("{0}:{1}", location.query, ((int)alertVM.AlertType).ToString());

                // Create the toast notification
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(notification.Payload);
                var toastNotif = new ToastNotification(xmlDocument)
                {
                    Group = TAG,
                    Tag = notID,
                    ExpirationTime = alert.ExpiresDate,
                };

                // And send the notification
                toastNotifier.Show(toastNotif);
            }
        }
    }
}
