#if !WINDOWS
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace SimpleWeather.Uno.Notifications
{
    public static class DailyNotificationCreator
    {
        private const string TAG = "DailyNotfication";

        public static async Task CreateNotification(LocationData.LocationData location)
        {
            var SettingsManager = Ioc.Default.GetService<SettingsManager>();

            if (!SettingsManager.DailyNotificationEnabled || location == null) return;

            var now = DateTimeOffset.Now;

            // Get forecast
            var forecasts = await SettingsManager.GetWeatherForecastData(location.query);

            if (forecasts == null) return;

            var todaysForecast = forecasts.forecast?.FirstOrDefault(f => f.date.Date == now.Date) ?? forecasts.forecast?.FirstOrDefault();

            if (todaysForecast == null) return;
        }

        /*
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

            return new ToastContent()
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
                    { "action", "view-weather" },
                    { "data", await JSONParser.SerializerAsync(location) },
                }.ToString()
            };
        }
        */
    }
}
#endif