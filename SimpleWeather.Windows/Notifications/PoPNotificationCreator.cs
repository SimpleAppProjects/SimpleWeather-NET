using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Notifications;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using Windows.UI.Notifications;

namespace SimpleWeather.NET.Notifications
{
    public static class PoPNotificationCreator
    {
        private const string TAG = "ChanceNotfication";

        private static async Task CreateToastCollection()
        {
            string displayName = App.Current.ResLoader.GetString("not_channel_name_precipnotification");
            var isLight = await SharedModule.Instance.DispatcherQueue.EnqueueAsync(() =>
            {
                return !App.Current.IsSystemDarkTheme;
            });
            var icon = new Uri($"{WeatherIconsManager.GetPNGBaseUri(isLight)}wi-umbrella.png");

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

        public static async Task CreateNotification(LocationData.LocationData location)
        {
            var SettingsManager = Ioc.Default.GetService<SettingsManager>();

            if (!SettingsManager.PoPChanceNotificationEnabled || !SettingsManager.WeatherLoaded || location == null) return;

            var now = DateTimeOffset.UtcNow;
            var lastPostTime = SettingsManager.LastPoPChanceNotificationTime;

            // We already posted recently; skip for now
            if (now < lastPostTime) return;

            var offsetNow = now.ToOffset(location.tz_offset);

            // Get the forecast for the next 12 hours
            var minForecasts = (await SettingsManager.GetWeatherForecastData(location.query))?.min_forecast?.Where(it =>
            {
                return it.date >= offsetNow;
            });

            await CreateToastCollection();
            var toastNotifier = await ToastNotificationManager.GetDefault()
                .GetToastNotifierForToastCollectionIdAsync(TAG);

            if (!await CreateMinutelyToastContent(toastNotifier, location, minForecasts, now))
            {
                // If not fallback to PoP% notification
                var nowHour = offsetNow.Trim(TimeSpan.TicksPerHour);
                var hrForecasts = await SettingsManager.GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(location.query, 0, 12, nowHour);

                if (!await CreatePoPToastContent(toastNotifier, location, hrForecasts, now))
                {
                    return;
                }
            }
        }

        private static async Task<bool> CreatePoPToastContent(ToastNotifier toastNotifier, LocationData.LocationData location, IEnumerable<HourlyForecast> hrForecasts, DateTimeOffset now)
        {
            if (hrForecasts?.Count() <= 0) return false;

            var SettingsManager = Ioc.Default.GetService<SettingsManager>();

            // Find the next hour with a 60% or higher chance of precipitation
            var forecast = hrForecasts?.FirstOrDefault(h =>
            {
                return h.date >= now.Trim(TimeSpan.TicksPerHour).AddHours(1) &&
                    h?.extras?.pop.GetValueOrDefault(0) >= SettingsManager.PoPChanceMinimumPercentage;
            });

            // Proceed if within the next 2hrs
            if (forecast == null || (forecast.date - now.Trim(TimeSpan.TicksPerHour)).TotalHours > 2) return false;

            var wim = SharedModule.Instance.WeatherIconsManager;

            // Should be within 0-3 hours
            var dt = forecast.date.Trim(TimeSpan.TicksPerHour).ToLocalTime();
            var time = dt.ToString("t", LocaleUtils.GetLocale());

            var duraStr = string.Format(App.Current.ResLoader.GetString("Precipitation_Likely_Text_Format"), time, forecast.extras.pop);

            var isLight = await SharedModule.Instance.DispatcherQueue.EnqueueAsync(() =>
            {
                return !App.Current.IsSystemDarkTheme;
            });
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
                Launch = new ToastArguments()
                {
                    { Constants.KEY_ACTION, "view-weather" },
                    { Constants.KEY_DATA, await JSONParser.SerializerAsync(location) },
                }.ToString()
            };

            // Set a unique ID for the notification
            var notID = location.query;

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml())
            {
                Group = TAG,
                Tag = notID,
                ExpirationTime = DateTimeOffset.Now.AddHours(3)
            };

            // And send the notification
            toastNotifier.Show(toastNotif);

            // Find the next hour with < 60% or higher chance of precipitation
            var stopForecast = hrForecasts.FirstOrDefault(h =>
            {
                return h.date > forecast.date && (h.extras?.pop == null || h.extras.pop < SettingsManager.PoPChanceMinimumPercentage);
            });
            // Delay further notifications until this time
            var nextTime = stopForecast?.date.Trim(TimeSpan.TicksPerHour) ?? now.AddHours(1);
            SettingsManager.LastPoPChanceNotificationTime = nextTime.AddMinutes(-5);

            return true;
        }

        private static async Task<bool> CreateMinutelyToastContent(ToastNotifier toastNotifier, LocationData.LocationData location, IEnumerable<MinutelyForecast> minForecasts, DateTimeOffset now)
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

            var formatStrTemplate = isRainingMinute switch
            {
                not null => App.Current.ResLoader.GetString("Precipitation_Likely_Minutely_Stopping_Text_Format"),
                _ => App.Current.ResLoader.GetString("Precipitation_Likely_Minutely_Starting_Text_Format")
            };

            var dt = minute.date.Trim(TimeSpan.TicksPerMinute).ToLocalTime();
            var time = dt.ToString("t", LocaleUtils.GetLocale());
            var formatStr = string.Format(formatStrTemplate, time);

            var duration = (int)Math.Round((minute.date - now).Duration().TotalMinutes);

            var isLight = await SharedModule.Instance.DispatcherQueue.EnqueueAsync(() =>
            {
                return !App.Current.IsSystemDarkTheme;
            });
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
                                Text = formatStr
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
                Launch = new ToastArguments()
                {
                    { Constants.KEY_ACTION, "view-weather" },
                    { Constants.KEY_DATA, await JSONParser.SerializerAsync(location) },
                }.ToString()
            };

            // Set a unique ID for the notification
            var notID = location.query;

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml())
            {
                Group = TAG,
                Tag = notID,
                ExpirationTime = now.AddMinutes(duration * 2), // Expires the next day
            };

            // And send the notification
            toastNotifier.Show(toastNotif);

            // Delay further notifications until this time
            var SettingsManager = Ioc.Default.GetService<SettingsManager>();
            var nextTime = minute.date.Trim(TimeSpan.TicksPerMinute);
            SettingsManager.LastPoPChanceNotificationTime = nextTime;

            return true;
        }
    }
}
