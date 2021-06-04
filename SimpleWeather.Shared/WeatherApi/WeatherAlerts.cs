using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public partial class WeatherAlert
    {
        public WeatherAlert(WeatherApi.Alert alert)
        {
            if (alert._event.Contains("Hurricane"))
            {
                Type = WeatherAlertType.HurricaneWindWarning;
            }
            else if (alert._event.Contains("Tornado"))
            {
                Type = WeatherAlertType.TornadoWarning;
            }
            else if (alert._event.Contains("Thunderstorm"))
            {
                Type = WeatherAlertType.SevereThunderstormWarning;
            }
            else if (alert._event.Contains("Flood"))
            {
                Type = WeatherAlertType.FloodWarning;
            }
            else if (alert._event.Contains("Wind"))
            {
                Type = WeatherAlertType.HighWind;
            }
            else if (alert._event.Contains("Fog"))
            {
                Type = WeatherAlertType.DenseFog;
            }
            else if (alert._event.Contains("Volcano"))
            {
                Type = WeatherAlertType.Volcano;
            }
            else if (alert._event.Contains("Earthquake"))
            {
                Type = WeatherAlertType.EarthquakeWarning;
            }
            else if (alert._event.Contains("Storm"))
            {
                Type = WeatherAlertType.StormWarning;
            }
            else if (alert._event.Contains("Tsunami"))
            {
                Type = WeatherAlertType.TsunamiWarning;
            }
            else
            {
                Type = WeatherAlertType.SpecialWeatherAlert;
            }

            switch (alert.severity)
            {
                case "Moderate":
                    Severity = WeatherAlertSeverity.Moderate;
                    break;
                case "Severe":
                    Severity = WeatherAlertSeverity.Severe;
                    break;
                case "Extreme":
                    Severity = WeatherAlertSeverity.Extreme;
                    break;
                default:
                    Severity = WeatherAlertSeverity.Minor;
                    break;
            }

            Title = alert._event;
            Message = new StringBuilder()
                .AppendLine(alert.headline)
                .AppendLine()
                .AppendLine(alert.desc)
                .AppendLine()
                .AppendLine(alert.instruction)
                .ToString();

            Attribution = alert.note;

            Date = alert.effective;
            ExpiresDate = alert.expires;
        }
    }
}
