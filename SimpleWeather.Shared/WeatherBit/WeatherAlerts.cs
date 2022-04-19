using SimpleWeather.Utils;
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
        public WeatherAlert(WeatherBit.Alert alert)
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
            };

            Severity = alert.severity switch
            {
                "Advisory" => WeatherAlertSeverity.Minor,
                "Watch" => WeatherAlertSeverity.Moderate,
                "Warning" => WeatherAlertSeverity.Severe,
                _ => WeatherAlertSeverity.Minor,
            };

            Title = alert.title;
            Message = new StringBuilder()
                .AppendLine(alert.title)
                .AppendLine()
                .AppendLine(alert.description)
                .ToString();

            Date = DateTime.ParseExact(alert.effective_utc, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            ExpiresDate = DateTime.ParseExact(alert.expires_utc, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }
    }
}
