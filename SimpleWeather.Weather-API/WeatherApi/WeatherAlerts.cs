using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SimpleWeather.Weather_API.WeatherApi
{
    public static partial class WeatherApiProviderExtensions
    {
        public static ICollection<WeatherAlert> CreateWeatherAlerts(this WeatherApiProvider _, Alerts alerts)
        {
            if (alerts?.alert?.Any() != true) return null;

            var weatherAlerts = new List<WeatherAlert>(alerts.alert.Length);

            foreach (var alert in alerts.alert)
            {
                weatherAlerts.Add(_.CreateWeatherAlert(alert));
            }

            return weatherAlerts;
        }

        public static WeatherAlert CreateWeatherAlert(this WeatherApiProvider _, Alert alert)
        {
            var weatherAlert = new WeatherAlert();

            if (alert._event.Contains("Hurricane"))
            {
                weatherAlert.Type = WeatherAlertType.HurricaneWindWarning;
            }
            else if (alert._event.Contains("Tornado"))
            {
                weatherAlert.Type = WeatherAlertType.TornadoWarning;
            }
            else if (alert._event.Contains("Thunderstorm"))
            {
                weatherAlert.Type = WeatherAlertType.SevereThunderstormWarning;
            }
            else if (alert._event.Contains("Flood"))
            {
                weatherAlert.Type = WeatherAlertType.FloodWarning;
            }
            else if (alert._event.Contains("Wind"))
            {
                weatherAlert.Type = WeatherAlertType.HighWind;
            }
            else if (alert._event.Contains("Fog"))
            {
                weatherAlert.Type = WeatherAlertType.DenseFog;
            }
            else if (alert._event.Contains("Volcano"))
            {
                weatherAlert.Type = WeatherAlertType.Volcano;
            }
            else if (alert._event.Contains("Earthquake"))
            {
                weatherAlert.Type = WeatherAlertType.EarthquakeWarning;
            }
            else if (alert._event.Contains("Storm"))
            {
                weatherAlert.Type = WeatherAlertType.StormWarning;
            }
            else if (alert._event.Contains("Tsunami"))
            {
                weatherAlert.Type = WeatherAlertType.TsunamiWarning;
            }
            else
            {
                weatherAlert.Type = WeatherAlertType.SpecialWeatherAlert;
            }

            weatherAlert.Severity = alert.severity switch
            {
                "Moderate" => WeatherAlertSeverity.Moderate,
                "Severe" => WeatherAlertSeverity.Severe,
                "Extreme" => WeatherAlertSeverity.Extreme,
                _ => WeatherAlertSeverity.Minor,
            };

            weatherAlert.Title = alert._event;
            weatherAlert.Message = new StringBuilder()
                .AppendLine(alert.headline)
                .AppendLine()
                .AppendLine(alert.desc)
                .AppendLine()
                .AppendLine(alert.instruction)
                .ToString();

            weatherAlert.Attribution = alert.note;

            if (DateTimeOffset.TryParse(alert.effective, CultureInfo.InvariantCulture, out var effective))
            {
                weatherAlert.Date = effective;
            }
            if (DateTimeOffset.TryParse(alert.expires, CultureInfo.InvariantCulture, out var expires))
            {
                weatherAlert.ExpiresDate = expires;
            }

            return weatherAlert;
        }
    }
}
