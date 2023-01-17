using CommunityToolkit.WinUI.Notifications;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace SimpleWeather.NET.Notifications
{
    public static class WeatherAlertCreator
    {
        private const string TAG = "WeatherAlerts";

        private static async Task CreateToastCollection()
        {
            string displayName = App.Current.ResLoader.GetString("label_nav_alerts");
            var icon = new Uri("ms-appx:///SimpleWeather.Shared/Resources/Images/WeatherIcons/png/dark/ic_error.png");

            ToastCollection toastCollection = new ToastCollection(TAG, displayName,
                new ToastArguments()
                {
                    { "action", "view-alerts" }
                }.ToString(),
                icon);

            await ToastNotificationManager.GetDefault()
                .GetToastCollectionManager()
                .SaveToastCollectionAsync(toastCollection);
        }

        public static async Task CreateAlerts(LocationData.LocationData location, IEnumerable<WeatherAlert> alerts)
        {
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
                        BaseUri = new Uri(WeatherIconsManager.GetPNGBaseUri(), UriKind.Absolute),
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
                        }
                    },
                    Launch = new ToastArguments()
                    {
                        { "action", "view-alerts" },
                        { "data", await JSONParser.SerializerAsync(location) },
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
