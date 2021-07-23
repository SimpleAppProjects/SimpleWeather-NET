using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using SimpleWeather.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace SimpleWeather.UWP.Notifications
{
    public static class DailyNotificationCreator
    {
        private const string TAG = "DailyNotfication";

        private static async Task CreateToastCollection()
        {
            string displayName = App.ResLoader.GetString("not_channel_name_dailynotification");
            var icon = new Uri("ms-appx:///SimpleWeather.Shared/Assets/WeatherIcons/dark/wi-day-cloudy.png");

            ToastCollection toastCollection = new ToastCollection(TAG, displayName,
                new QueryString()
                {
                    { "action", "view-weather" }
                }.ToString(),
                icon);

            await ToastNotificationManager.GetDefault()
                .GetToastCollectionManager()
                .SaveToastCollectionAsync(toastCollection);
        }

        public static async Task CreateNotification(LocationData location)
        {
            if (!Settings.DailyNotificationEnabled || location == null) return;

            await CreateToastCollection();
            var toastNotifier = await ToastNotificationManager.GetDefault()
                .GetToastNotifierForToastCollectionIdAsync(TAG);

            var now = DateTimeOffset.Now;

            // Get forecast
            var forecasts = await Settings.GetWeatherForecastData(location.query);

            if (forecasts == null) return;

            var todaysForecast = forecasts.forecast?.FirstOrDefault(f => f.date.Date == now.Date) ?? forecasts.forecast?.FirstOrDefault();

            if (todaysForecast == null) return;

            var toastContent = await CreateToastContent(location, todaysForecast);

            // Set a unique ID for the notification
            var notID = location.query;

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml())
            {
                Group = TAG,
                Tag = notID,
                ExpirationTime = DateTime.Now.AddDays(1), // Expires the next day
            };

            // And send the notification
            toastNotifier.Show(toastNotif);
        }

        private static async Task<ToastContent> CreateToastContent(LocationData location, Forecast forecast)
        {
            var wim = WeatherIconsManager.GetInstance();

            var viewModel = new ForecastItemViewModel(forecast);
            var hiTemp = viewModel.HiTemp ?? WeatherIcons.PLACEHOLDER;
            var loTemp = viewModel.LoTemp ?? WeatherIcons.PLACEHOLDER;
            var condition = viewModel.Condition ?? WeatherIcons.EM_DASH;

            DetailItemViewModel chanceModel = null;
            DetailItemViewModel feelsLikeModel = null;
            foreach (var model in viewModel.DetailExtras)
            {
                if (model.DetailsType == WeatherDetailsType.PoPChance)
                {
                    chanceModel = model;
                }
                else if (model.DetailsType == WeatherDetailsType.FeelsLike)
                {
                    feelsLikeModel = model;
                }

                if (chanceModel != null && feelsLikeModel != null) break;
            }

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

            return new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BaseUri = new Uri(WeatherIconsManager.GetBaseUri(false), UriKind.Absolute),
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
                Launch = new QueryString()
                {
                    { "action", "view-weather" },
                    { "data", await JSONParser.SerializerAsync(location) },
                }.ToString()
            };
        }
    }
}