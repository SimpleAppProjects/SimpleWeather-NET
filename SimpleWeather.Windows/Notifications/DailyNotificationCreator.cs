﻿using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Notifications;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System.Text;
using Windows.UI.Notifications;

namespace SimpleWeather.NET.Notifications
{
    public static class DailyNotificationCreator
    {
        private const string TAG = "DailyNotfication";

        private static async Task CreateToastCollection()
        {
            string displayName = App.Current.ResLoader.GetString("not_channel_name_dailynotification");
            var isLight = await SharedModule.Instance.DispatcherQueue.EnqueueAsync(() =>
            {
                return !App.Current.IsSystemDarkTheme;
            });
            var icon = new Uri($"{WeatherIconsManager.GetPNGBaseUri(isLight)}wi-day-cloudy.png");

            ToastCollection toastCollection = new ToastCollection(TAG, displayName,
                new ToastArguments()
                {
                    { Constants.KEY_ACTION, "view-weather" }
                }.ToString(),
                icon);

            await ToastNotificationManager.GetDefault()
                .GetToastCollectionManager()
                .SaveToastCollectionAsync(toastCollection);
        }

        public static async Task CreateNotification(LocationData.LocationData location, DateTimeOffset? deliveryTime = null)
        {
            var SettingsManager = Ioc.Default.GetService<SettingsManager>();

            if (!SettingsManager.DailyNotificationEnabled || location == null) return;

            await CreateToastCollection();
            var toastNotifier = await ToastNotificationManager.GetDefault()
                .GetToastNotifierForToastCollectionIdAsync(TAG);

            var now = DateTimeOffset.Now;

            // Get forecast
            var forecasts = await SettingsManager.GetWeatherForecastData(location.query);

            if (forecasts == null) return;

            var todaysForecast = forecasts.forecast?.FirstOrDefault(f => f.date.Date == now.Date) ?? forecasts.forecast?.FirstOrDefault();

            if (todaysForecast == null) return;

            var toastContent = await CreateToastContent(location, todaysForecast);

            // Set a unique ID for the notification
            var notID = location.query;

            // Remove any pending notifications
            try
            {
                var pendingNotifications = toastNotifier.GetScheduledToastNotifications();

                if (pendingNotifications?.Count > 0)
                {
                    pendingNotifications
                        ?.Where(t => t.Group == TAG)
                        ?.ForEach(toastNotifier.RemoveFromSchedule);
                }
            }
            catch { }

            if (deliveryTime == null)
            {
                // Create the toast notification
                var toastNotif = new ToastNotification(toastContent.GetXml())
                {
                    Group = TAG,
                    Tag = notID,
                    ExpirationTime = DateTime.Now.AddDays(0.99), // Expires the next day
                };

                // And send the notification
                toastNotifier.Show(toastNotif);
            }
            else
            {
                // Create the toast notification
                var toastNotif = new ScheduledToastNotification(toastContent.GetXml(), deliveryTime.Value)
                {
                    Group = TAG,
                    Tag = notID,
                    ExpirationTime = deliveryTime.Value.AddDays(0.99), // Expires the next day
                };

                // And schedule the notification
                toastNotifier.AddToSchedule(toastNotif);
            }
        }

        public static Task ScheduleNotification(LocationData.LocationData location, DateTimeOffset deliveryTime)
        {
            return CreateNotification(location, deliveryTime);
        }

        private static async Task<ToastContent> CreateToastContent(LocationData.LocationData location, Forecast forecast)
        {
            var wim = SharedModule.Instance.WeatherIconsManager;

            var viewModel = new ForecastItemViewModel(forecast);
            var hiTemp = viewModel.HiTemp ?? WeatherIcons.PLACEHOLDER;
            var loTemp = viewModel.LoTemp ?? WeatherIcons.PLACEHOLDER;
            var condition = viewModel.Condition ?? WeatherIcons.EM_DASH;

            var chanceModel = viewModel.DetailExtras[WeatherDetailsType.PoPChance];
            var feelsLikeModel = viewModel.DetailExtras[WeatherDetailsType.FeelsLike];

            var contentText = new StringBuilder().Append(condition);
            var appendDiv = false;
            var appendLine = true;

            if (feelsLikeModel != null)
            {
                if (appendLine)
                {
                    contentText.AppendLine();
                    appendLine = false;
                }
                if (appendDiv)
                {
                    contentText.Append("; ");
                }
                contentText.AppendFormat("{0}: {1}", feelsLikeModel.Label, feelsLikeModel.Value);
                appendDiv = true;
            }
            if (chanceModel != null)
            {
                if (appendLine)
                {
                    contentText.AppendLine();
                    appendLine = false;
                }
                if (appendDiv)
                {
                    contentText.Append("; ");
                }
                contentText.AppendFormat("{0}: {1}", chanceModel.Label, chanceModel.Value);
                appendDiv = true;
            }

            var isLight = await SharedModule.Instance.DispatcherQueue.EnqueueAsync(() =>
            {
                return !App.Current.IsSystemDarkTheme;
            });

            return new ToastContent()
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
                                Text = string.Format("{0} / {1} - {2}", hiTemp, loTemp,  viewModel.Date)
                            },
                            new AdaptiveText()
                            {
                                Text = contentText.ToString()
                            },
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = wim.GetWeatherIconURI(forecast.icon, false)
                        },
                        Attribution = new ToastGenericAttributionText()
                        {
                            Text = location.name
                        }
                    }
                },
                Launch = new ToastArguments()
                {
                    { Constants.KEY_ACTION, "view-weather" },
                    { Constants.KEY_DATA, await JSONParser.SerializerAsync(location) },
                }.ToString()
            };
        }
    }
}
