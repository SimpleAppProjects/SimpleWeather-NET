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
            var icon = new Uri("ms-appx:///Assets/WeatherIcons/png/ic_error_white.png");

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
                                Source = GetAssetFromAlertType(alertVM.AlertType)
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

        private static string GetAssetFromAlertType(WeatherAlertType type)
        {
            string baseuri = "ms-appx:///Assets/WeatherIcons/png/";
            string fileIcon = string.Empty;

            switch (type)
            {
                case WeatherAlertType.DenseFog:
                    fileIcon = "fog.png";
                    break;

                case WeatherAlertType.Fire:
                    fileIcon = "fire.png";
                    break;

                case WeatherAlertType.FloodWarning:
                case WeatherAlertType.FloodWatch:
                    fileIcon = "flood.png";
                    break;

                case WeatherAlertType.Heat:
                    fileIcon = "hot.png";
                    break;

                case WeatherAlertType.HighWind:
                    fileIcon = "strong_wind.png";
                    break;

                case WeatherAlertType.HurricaneLocalStatement:
                case WeatherAlertType.HurricaneWindWarning:
                    fileIcon = "hurricane.png";
                    break;

                case WeatherAlertType.SevereThunderstormWarning:
                case WeatherAlertType.SevereThunderstormWatch:
                    fileIcon = "thunderstorm.png";
                    break;

                case WeatherAlertType.TornadoWarning:
                case WeatherAlertType.TornadoWatch:
                    fileIcon = "tornado.png";
                    break;

                case WeatherAlertType.Volcano:
                    fileIcon = "volcano.png";
                    break;

                case WeatherAlertType.WinterWeather:
                    fileIcon = "snowflake_cold.png";
                    break;

                case WeatherAlertType.SevereWeather:
                case WeatherAlertType.SpecialWeatherAlert:
                default:
                    fileIcon = "ic_error_white.png";
                    break;
            }

            return baseuri + fileIcon;
        }
    }
}