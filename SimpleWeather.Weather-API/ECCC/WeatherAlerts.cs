using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System.Collections.Generic;

namespace SimpleWeather.Weather_API.ECCC
{
    public static partial class ECCCWeatherProviderExtensions
    {
        public static ICollection<WeatherAlert> CreateWeatherAlerts(this ECCCWeatherProvider _, Alert alertRoot)
        {
            ICollection<WeatherAlert> weatherAlerts = null;

            if (alertRoot?.alerts?.Length > 0)
            {
                weatherAlerts = new HashSet<WeatherAlert>(alertRoot.alerts.Length);

                foreach (var alert in alertRoot.alerts)
                {
                    weatherAlerts.Add(_.CreateWeatherAlert(alert));
                }
            }

            return weatherAlerts;
        }

        public static WeatherAlert CreateWeatherAlert(this ECCCWeatherProvider _, AlertsItem alertItem)
        {
            var alert = new WeatherAlert();

            alert.Attribution = "Environment and Climate Change Canada (ECCC)";
            alert.Date = alertItem.eventOnsetTime ?? alertItem.issueTime;
            alert.ExpiresDate = alertItem.eventEndTime;
            alert.Severity = alertItem.type switch
            {
                "warning" => WeatherAlertSeverity.Severe,
                "watch" => WeatherAlertSeverity.Moderate,
                "statement" or "advisory" => WeatherAlertSeverity.Minor,
                _ => WeatherAlertSeverity.Unknown,
            };
            // https://www.canada.ca/en/environment-climate-change/services/types-weather-forecasts-use/public/criteria-alerts.html
            alert.Type = alertItem.alertBannerText?.ToLowerInvariant()?.Let(it =>
            {
                if (it.Contains("arctic") || it.Contains("blizzard") || it.Contains("snow") || it.Contains("extreme cold") || it.Contains("freez") || it.Contains("frost") || it.Contains("winter"))
                {
                    return WeatherAlertType.WinterWeather;
                }
                else if (it.Contains("flooding"))
                {
                    return WeatherAlertType.FloodWarning;
                }
                else if (it.Contains("dust"))
                {
                    return WeatherAlertType.DustAdvisory;
                }
                else if (it.Contains("fog"))
                {
                    return WeatherAlertType.DenseFog;
                }
                else if (it.Contains("heat"))
                {
                    return WeatherAlertType.Heat;
                }
                else if (it.Contains("hurricane"))
                {
                    return WeatherAlertType.HurricaneWindWarning;
                }
                else if (it.Contains("thunderstorm"))
                {
                    return WeatherAlertType.SevereThunderstormWarning;
                }
                else if (it.Contains("tornado"))
                {
                    return WeatherAlertType.TornadoWarning;
                }
                else if (it.Contains("tropical storm"))
                {
                    return WeatherAlertType.StormWarning;
                }
                else
                {
                    return WeatherAlertType.SpecialWeatherAlert;
                }
            }) ?? WeatherAlertType.SpecialWeatherAlert;

            alert.Title = alertItem.alertBannerText;
            alert.Message = alertItem.text;

            return alert;
        }
    }
}