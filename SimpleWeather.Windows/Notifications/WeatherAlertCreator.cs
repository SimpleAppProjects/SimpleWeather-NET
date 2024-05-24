using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Notifications;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
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

            ToastCollection toastCollection = new ToastCollection(TAG, displayName,
                new ToastArguments()
                {
                    { Constants.KEY_ACTION, "view-alerts" }
                }.ToString(),
                icon);

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

                var toastContent = new ToastContent()
                {
                    Visual = new ToastVisual()
                    {
                        BaseUri = new Uri(WeatherIconsManager.GetPNGBaseUri(isLight), UriKind.Absolute),
                        BindingGeneric = new ToastBindingGeneric()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = alertVM.Title
                                },
                                new AdaptiveText()
                                {
                                    Text = alertVM.ExpireDate
                                }
                            },
                            AppLogoOverride = new ToastGenericAppLogo()
                            {
                                Source = WeatherUtils.GetAssetFromAlertType(alertVM.AlertType, false)
                            },
                            Attribution = new ToastGenericAttributionText()
                            {
                                Text = alertVM.Attribution
                            }
                        },
                    },
                    Actions = new ToastActionsCustom()
                    {
                        Buttons =
                        {
                            new ToastButton()
                                .SetContent(ResStrings.label_moreinfo)
                                .AddArgument(Constants.KEY_ACTION, "open-link")
                                .AddArgument(Constants.KEY_DATA, alertVM.Message)
                        }
                    },
                    Launch = new ToastArguments()
                    {
                        { Constants.KEY_ACTION, "view-alerts" },
                        { Constants.KEY_DATA, await JSONParser.SerializerAsync(location) },
                    }.ToString()
                };

                // Set a unique ID for the notification
                var notID = string.Format("{0}:{1}", location.query, ((int)alertVM.AlertType).ToString());

                // Create the toast notification
                var toastNotif = new ToastNotification(toastContent.GetXml())
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
