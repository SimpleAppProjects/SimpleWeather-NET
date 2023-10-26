#if __IOS__ || __MACCATALYST__
using CommunityToolkit.Mvvm.DependencyInjection;
using Foundation;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System.Text;
using UserNotifications;

namespace SimpleWeather.Maui.Notifications
{
    public static class DailyNotificationCreator
    {
        private const string TAG = "DailyNotfication";

        public static async Task CreateNotification(LocationData.LocationData location, UNNotificationTrigger notificationTrigger = null)
        {
            var SettingsManager = Ioc.Default.GetService<SettingsManager>();

            if (!SettingsManager.DailyNotificationEnabled || location == null) return;

            var now = DateTimeOffset.Now;

            // Get forecast
            var forecasts = await SettingsManager.GetWeatherForecastData(location.query);

            if (forecasts == null) return;

            var todaysForecast = forecasts.forecast?.FirstOrDefault(f => f.date.Date == now.Date) ?? forecasts.forecast?.FirstOrDefault();

            if (todaysForecast == null) return;

            var toastContent = CreateToastContent(location, todaysForecast);

            // Set a unique ID for the notification
            var notID = location.query;

            // Create the toast notification
            var identifier = $"{TAG}:{notID}";
            var notificationCenter = UNUserNotificationCenter.Current;
            var request = UNNotificationRequest.FromIdentifier(
                identifier, toastContent, notificationTrigger
            );

            try
            {
                notificationCenter.RemovePendingNotificationRequests(new string[] { identifier });
                await notificationCenter.AddNotificationRequestAsync(request);
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e, "Error requesting daily notification");
            }
        }

        public static Task ScheduleNotification(LocationData.LocationData location, double timeIntervalInSeconds)
        {
            return CreateNotification(location, UNTimeIntervalNotificationTrigger.CreateTrigger(timeIntervalInSeconds, false));
        }

        private static UNNotificationContent CreateToastContent(LocationData.LocationData location, Forecast forecast)
        {
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

            return new UNMutableNotificationContent()
            {
                Title = location.name,
                Subtitle = string.Format("{0} / {1} - {2}", hiTemp, loTemp, viewModel.Date),
                Body = contentText.ToString()
            };
        }
    }
}
#endif