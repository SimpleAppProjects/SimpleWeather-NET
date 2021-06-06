using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using SimpleWeather.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace SimpleWeather.UWP.Notifications
{
    public static class PoPNotificationCreator
    {
        private const string TAG = "ChanceNotfication";

        private static async Task CreateToastCollection()
        {
            string displayName = App.ResLoader.GetString("Pref_Title_PrecipNotification/Text").ReplaceFirst(":", "");
            var icon = new Uri("ms-appx:///SimpleWeather.Shared/Assets/WeatherIcons/dark/wi-umbrella.png");

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
            if (!Settings.PoPChanceNotificationEnabled || !Settings.WeatherLoaded || location == null) return;

            await CreateToastCollection();
            var toastNotifier = await ToastNotificationManager.GetDefault()
                .GetToastNotifierForToastCollectionIdAsync(TAG);

            var now = DateTimeOffset.UtcNow;
            var lastPostTime = Settings.LastPoPChanceNotificationTime;

            // We already posted today; post any chance tomorrow
            if (now.Date == lastPostTime.ToUniversalTime().Date) return;

            // Get the forecast for the next 12 hours
            var nowHour = now.ToOffset(location.tz_offset).Trim(TimeSpan.TicksPerHour);
            var hrForecasts = await Settings.GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(location.query, 0, 12, nowHour.ToString("yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture));

            if (hrForecasts == null || hrForecasts.Count <= 0) return;

            // Find the next hour with a 60% or higher chance of precipitation
            var hrf = hrForecasts?.FirstOrDefault(h => h?.extras?.pop >= 60);

            // Proceed if within the next 3hrs
            if (hrf == null || (hrf.date - nowHour).TotalHours > 3) return;

            var toastContent = await CreateToastContent(location, hrf, nowHour);

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

            Settings.LastPoPChanceNotificationTime = now;
        }

        private static async Task<ToastContent> CreateToastContent(LocationData location, HourlyForecast forecast, DateTimeOffset now)
        {
            var wim = WeatherIconsManager.GetInstance();

            // Should be within 0-3 hours
            var duration = (forecast.date - now).TotalMinutes;
            string duraStr;
            if (duration <= 60)
            {
                duraStr = string.Format(App.ResLoader.GetString("Precipitation_NextHour_Text_Format"), forecast.extras.pop);
            }
            else if (duration < 120)
            {
                duraStr = string.Format(App.ResLoader.GetString("Precipitation_Text_Format"), forecast.extras.pop,
                    App.ResLoader.GetString("Pref_Refresh30Min/Text").Replace("30", ((int)Math.Round(duration)).ToString()));
            }
            else
            {
                duraStr = string.Format(App.ResLoader.GetString("Precipitation_Text_Format"), forecast.extras.pop,
                    App.ResLoader.GetString("Pref_Refresh12Hrs/Text").Replace("12", ((int)Math.Round(duration / 60)).ToString()));
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
                                Text = duraStr
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = wim.GetWeatherIconURI(WeatherIcons.UMBRELLA, false)
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