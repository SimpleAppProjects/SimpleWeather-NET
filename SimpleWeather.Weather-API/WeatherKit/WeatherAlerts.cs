using SimpleWeather.WeatherData;
using System.Collections.Generic;

namespace SimpleWeather.Weather_API.WeatherKit
{
    public static partial class WeatherKitProviderExtensions
    {
        public static ICollection<WeatherAlert> CreateWeatherAlerts(this WeatherKitProvider _, WeatherAlertCollection alerts)
        {
            ICollection<WeatherAlert> weatherAlerts = null;

            if (alerts?.alerts?.Length > 0)
            {
                weatherAlerts = new List<WeatherAlert>(alerts.alerts.Length);

                foreach (var result in alerts.alerts)
                {
                    weatherAlerts.Add(_.CreateWeatherAlert(result));
                }
            }

            return weatherAlerts;
        }

        public static WeatherAlert CreateWeatherAlert(this WeatherKitProvider _, WeatherAlertSummary alert)
        {
            return new WeatherAlert()
            {
                Title = alert.description,
                Message = alert.detailsUrl,
                Attribution = alert.source,
                Date = alert.effectiveTime,
                ExpiresDate = alert.expireTime,
                Severity = alert.severity switch
                {
                    Severity.Extreme => WeatherAlertSeverity.Extreme,
                    Severity.Severe => WeatherAlertSeverity.Severe,
                    Severity.Moderate => WeatherAlertSeverity.Moderate,
                    Severity.Minor => WeatherAlertSeverity.Minor,
                    _ => WeatherAlertSeverity.Unknown,
                },
                Type = alert.description switch
                {
                    string title when title.Contains("Hurricane") => WeatherAlertType.HurricaneWindWarning,
                    string title when title.Contains("Tornado") => WeatherAlertType.TornadoWarning,
                    string title when title.Contains("Thunderstorm") => WeatherAlertType.SevereThunderstormWarning,
                    string title when title.Contains("Flood") => WeatherAlertType.FloodWarning,
                    string title when title.Contains("Wind") => WeatherAlertType.HighWind,
                    string title when title.Contains("Fog") => WeatherAlertType.DenseFog,
                    string title when title.Contains("Volcano") => WeatherAlertType.Volcano,
                    string title when title.Contains("Earthquake") => WeatherAlertType.EarthquakeWarning,
                    string title when title.Contains("Storm") => WeatherAlertType.StormWarning,
                    string title when title.Contains("Tsunami") => WeatherAlertType.TsunamiWarning,
                    _ => WeatherAlertType.SpecialWeatherAlert
                },
            };
        }
    }
}