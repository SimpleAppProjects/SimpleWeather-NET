using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Weather_API.BrightSky
{
    public static class BrightSkyExtensions
    {
        public static ICollection<WeatherAlert> CreateWeatherAlerts(this BrightSkyProvider _, AlertsRootobject alertRoot)
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

        public static WeatherAlert CreateWeatherAlert(this BrightSkyProvider _, Alert alertItem)
        {
            var alert = new WeatherAlert();

            alert.Attribution = "DWD";
            alert.Date = alertItem.effective;
            alert.ExpiresDate = alertItem.expires ?? alertItem.onset.AddDays(1);
            alert.Severity = alertItem.severity switch
            {
                "extreme" => WeatherAlertSeverity.Extreme,
                "severe" => WeatherAlertSeverity.Severe,
                "moderate" => WeatherAlertSeverity.Moderate,
                "minor" => WeatherAlertSeverity.Minor,
                _ => WeatherAlertSeverity.Unknown,
            };
            alert.Type = alertItem.event_code switch
            {
                /* 22 - Frost */
                22 => WeatherAlertType.WinterWeather,
                /* Thunderstorm watches / warnings */
                31 or 33 or 34 or 36 or 38 or 40 or 41 or 42 or 44 or 45 or 46 or 48 or 49 => WeatherAlertType.SevereThunderstormWarning,
                /* Strong wind warnings */
                51 or 52 or 53 or 57 => WeatherAlertType.HighWind,
                /* Hurricane winds */
                54 or 55 or 56 => WeatherAlertType.HurricaneWindWarning,
                /* Storm */
                58 => WeatherAlertType.SevereThunderstormWatch,
                /* Fog */
                59 => WeatherAlertType.DenseFog,
                /* Heavy Rain */
                61 or 62 or 63 or 64 or 65 or 66 => WeatherAlertType.SevereWeather,
                /* Winter weather */
                70 or 71 or 72 or 73 or 74 or 75 or 76 or 82 or 84 or 85 or 86 or 87 or 88 or 89 => WeatherAlertType.WinterWeather,
                /* Thunderstorms */
                90 or 91 or 92 or 93 or 95 or 96 => WeatherAlertType.SevereThunderstormWarning,
                /* Coastal warnings */
                11 or 12 => WeatherAlertType.GaleWarning,
                13 => WeatherAlertType.StormWarning,
                /* Open Sea Warnings */
                14 or 15 or 16 => WeatherAlertType.GaleWarning,
                /* Heat warnings */
                246 or 247 or 248 => WeatherAlertType.Heat,
                /* Misc (79 or 98 or 99) */
                _ => WeatherAlertType.SpecialWeatherAlert,
            };

            alert.Title = alertItem.event_de ?? alertItem.headline_de ?? alertItem.event_en ?? alertItem.headline_en;
            alert.Message = new StringBuilder()
                .Apply(sb =>
                {
                    if (!string.IsNullOrWhiteSpace(alertItem.headline_de))
                    {
                        sb.AppendLine("Deutsch:");
                        sb.AppendLine(alertItem.headline_de);
                        sb.AppendLine();
                        sb.AppendLine(alertItem.description_de);

                        if (!string.IsNullOrWhiteSpace(alertItem.instruction_de))
                        {
                            sb.AppendLine().AppendLine(alertItem.instruction_de);
                        }
                    }

                    sb.AppendLine();

                    if (!string.IsNullOrWhiteSpace(alertItem.headline_en))
                    {
                        sb.AppendLine("English:");
                        sb.AppendLine(alertItem.headline_en);
                        sb.AppendLine();
                        sb.AppendLine(alertItem.description_en);

                        if (!string.IsNullOrWhiteSpace(alertItem.instruction_en))
                        {
                            sb.AppendLine().AppendLine(alertItem.instruction_en);
                        }
                    }
                })
                .ToString()
                .TrimEnd();

            return alert;
        }
    }
}