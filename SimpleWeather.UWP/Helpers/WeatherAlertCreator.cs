using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using Windows.System.UserProfile;
using System.Globalization;
using SimpleWeather.Controls;
using Microsoft.QueryStringDotNET;

namespace SimpleWeather.UWP.Helpers
{
    public static class WeatherAlertCreator
    {
        private static WeatherManager wm = WeatherManager.GetInstance();
        private const string Tag = "WeatherAlerts";

        public static void CreateAlerts(String query, List<WeatherAlert> alerts)
        {
            var toastNotifier = ToastNotificationManager.CreateToastNotifier();

            foreach (WeatherAlert alert in alerts)
            {
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
                                Source = GetAssetFromAlertType(alert.Type)
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
                        { "query", query },
                    }.ToString()
                };

                // Create the toast notification
                var toastNotif = new ToastNotification(toastContent.GetXml())
                {
                    Group = Tag,
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
