using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace SimpleWeather.UWP.Notifications
{
    public static class WeatherAlertCreator
    {
        private const string TAG = "WeatherAlerts";

        private static async Task CreateToastCollection()
        {
            string displayName = App.ResLoader.GetString("Nav_WeatherAlerts/Content");
            var icon = new Uri("ms-appx:///SimpleWeather.Shared/Assets/WeatherIcons/dark/ic_error.png");

            ToastCollection toastCollection = new ToastCollection(TAG, displayName,
                new QueryString()
                {
                    { "action", "view-alerts" }
                }.ToString(),
                icon);

            await ToastNotificationManager.GetDefault()
                .GetToastCollectionManager()
                .SaveToastCollectionAsync(toastCollection);
        }

        public static async Task CreateAlerts(LocationData location, IEnumerable<WeatherAlert> alerts)
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
                        BaseUri = new Uri("ms-appx:///SimpleWeather.Shared/Assets/WeatherIcons/dark/", UriKind.Relative),
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
                    Launch = new QueryString()
                    {
                        { "action", "view-alerts" },
                        { "data", JSONParser.Serializer(location) },
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