using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SimpleWeather.Weather_API.WeatherBit
{
    public static partial class WeatherBitIOProviderExtensions
    {
        public static ICollection<WeatherAlert> CreateWeatherAlerts(this WeatherBitIOProvider _, IList<Alert> alerts, TimeSpan tzOffset)
        {
            if (alerts?.Any() != true) return null;

            var weatherAlerts = new HashSet<WeatherAlert>(
#if !NETSTANDARD2_0
                alerts.Count
#endif
                );

            foreach (var alert in alerts)
            {
                weatherAlerts.Add(_.CreateWeatherAlert(alert).Apply(it =>
                {
                    it.Date = it.Date.ToOffset(tzOffset);
                    it.ExpiresDate = it.ExpiresDate.ToOffset(tzOffset);
                }));
            }

            return weatherAlerts;
        }

        public static WeatherAlert CreateWeatherAlert(this WeatherBitIOProvider _, WeatherBit.Alert alert)
        {
            return new WeatherAlert()
            {
                Type = alert.title switch
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

                Severity = alert.severity switch
                {
                    "Advisory" => WeatherAlertSeverity.Minor,
                    "Watch" => WeatherAlertSeverity.Moderate,
                    "Warning" => WeatherAlertSeverity.Severe,
                    _ => WeatherAlertSeverity.Minor,
                },

                Title = alert.title,
                Message = new StringBuilder()
                .AppendLine(alert.title)
                .AppendLine()
                .AppendLine(alert.description)
                .ToString(),

                Date = DateTime.ParseExact(alert.effective_utc, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                ExpiresDate = DateTime.ParseExact(alert.expires_utc, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
            };
        }
    }
}
