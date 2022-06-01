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
            string displayName = App.ResLoader.GetString("not_channel_name_precipnotification");
            var icon = new Uri("ms-appx:///SimpleWeather.Shared/Assets/WeatherIcons/png/dark/wi-umbrella.png");

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

            var now = DateTimeOffset.UtcNow;
            var lastPostTime = Settings.LastPoPChanceNotificationTime;

            // We already posted today; post any chance tomorrow
            if (now.Date == lastPostTime.ToUniversalTime().Date) return;

            var offsetNow = now.ToOffset(location.tz_offset);

            // Get the forecast for the next 12 hours
            var minForecasts = (await Settings.GetWeatherForecastData(location.query))?.min_forecast?.Where(it =>
            {
                return it.date >= offsetNow;
            });

            await CreateToastCollection();
            var toastNotifier = await ToastNotificationManager.GetDefault()
                .GetToastNotifierForToastCollectionIdAsync(TAG);

            if (!await CreateMinutelyToastContent(toastNotifier, location, minForecasts, offsetNow))
            {
                // If not fallback to PoP% notification
                var nowHour = offsetNow.Trim(TimeSpan.TicksPerHour);
                var hrForecasts = await Settings.GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(location.query, 0, 12, nowHour.ToString("yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture));

                if (!await CreatePoPToastContent(toastNotifier, location, hrForecasts, offsetNow))
                {
                    return;
                }
            }

            Settings.LastPoPChanceNotificationTime = now;
        }

        private static async Task<bool> CreatePoPToastContent(ToastNotifier toastNotifier, LocationData location, IEnumerable<HourlyForecast> hrForecasts, DateTimeOffset now)
        {
            if (hrForecasts?.Count() <= 0) return false;

            // Find the next hour with a 60% or higher chance of precipitation
            var forecast = hrForecasts?.FirstOrDefault(h => h?.extras?.pop >= 60);

            // Proceed if within the next 3hrs
            if (forecast == null || (forecast.date - now.Trim(TimeSpan.TicksPerHour)).TotalHours > 3) return false;

            var wim = SharedModule.Instance.WeatherIconsManager;

            // Should be within 0-3 hours
            var duration = (int)Math.Round((forecast.date - now).TotalMinutes);
            string duraStr = duration switch
            {
                <= 60 => string.Format(App.ResLoader.GetString("Precipitation_NextHour_Text_Format"), forecast.extras.pop),
                < 120 => string.Format(App.ResLoader.GetString("Precipitation_Text_Format"), forecast.extras.pop,
                                            App.ResLoader.GetString("refresh_30min").Replace("30", duration.ToString())),
                _ => string.Format(App.ResLoader.GetString("Precipitation_Text_Format"), forecast.extras.pop,
                                            App.ResLoader.GetString("refresh_12hrs").Replace("12", (duration / 60).ToString())),
            };

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

            return true;
        }

        private static async Task<bool> CreateMinutelyToastContent(ToastNotifier toastNotifier, LocationData location, IEnumerable<MinutelyForecast> minForecasts, DateTimeOffset now)
        {
            if (minForecasts?.Count() <= 0) return false;

            var wim = SharedModule.Instance.WeatherIconsManager;

            var isRainingMinute = minForecasts?.FirstOrDefault(it =>
            {
                return (now.Trim(TimeSpan.TicksPerMinute) - it.date.Trim(TimeSpan.TicksPerMinute)).Duration().TotalMinutes <= 5 && it.rain_mm > 0;
            });

            MinutelyForecast minute = null;

            if (isRainingMinute != null)
            {
                // Find minute where rain stops
                minute = minForecasts?.FirstOrDefault(it =>
                {
                    return it.date.Trim(TimeSpan.TicksPerMinute) > isRainingMinute.date && it.rain_mm <= 0;
                });
            }
            else
            {
                // Find minute where rain starts
                minute = minForecasts?.FirstOrDefault(it =>
                {
                    return it.date.Trim(TimeSpan.TicksPerMinute) > now.Trim(TimeSpan.TicksPerMinute) && it.rain_mm > 0;
                });
            }

            if (minute == null) return false;

            var formatStr = isRainingMinute switch
            {
                not null => App.ResLoader.GetString("Precipitation_Minutely_Stopping_Text_Format"),
                _ => App.ResLoader.GetString("Precipitation_Minutely_Starting_Text_Format")
            };

            var duration = (int)Math.Round((minute.date - now).Duration().TotalMinutes);
            string duraStr = duration switch
            {
                < 120 => string.Format(formatStr, App.ResLoader.GetString("refresh_30min").Replace("30", duration.ToString())),
                _ => string.Format(formatStr, App.ResLoader.GetString("refresh_12hrs").Replace("12", (duration / 60).ToString())),
            };

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

            return true;
        }
    }
}