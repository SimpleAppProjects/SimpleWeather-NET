using SimpleWeather.WeatherData;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleWeather.Weather_API.TomorrowIO
{
    public static partial class TomorrowIOWeatherProviderExtensions
    {
        public static ICollection<WeatherAlert> CreateWeatherAlerts(this TomorrowIOWeatherProvider _, AlertRootobject alertRoot)
        {
            ICollection<WeatherAlert> weatherAlerts = null;

            if (alertRoot?.data?.events?.Length > 0)
            {
                weatherAlerts = new HashSet<WeatherAlert>(alertRoot.data.events.Length);

                foreach (var @event in alertRoot.data.events)
                {
                    // Skip "Active Fire" alerts (not enough info)
                    if (@event.eventValues.origin != "VIIRS")
                    {
                        weatherAlerts.Add(_.CreateWeatherAlert(@event));
                    }
                }
            }

            return weatherAlerts;
        }

        public static WeatherAlert CreateWeatherAlert(this TomorrowIOWeatherProvider _, Event @event)
        {
            var alert = new WeatherAlert();

            alert.Date = @event.startTime;
            alert.ExpiresDate = @event.endTime;

            alert.Type = @event.insight switch
            {
                "fires" => WeatherAlertType.Fire,
                "wind" => WeatherAlertType.HighWind,
                "winter" => WeatherAlertType.WinterWeather,
                "thunderstorms" => WeatherAlertType.SevereThunderstormWarning,
                "floods" => WeatherAlertType.FloodWarning,
                "tropical" => WeatherAlertType.HurricaneWindWarning,
                "fog" => WeatherAlertType.DenseFog,
                "tornado" => WeatherAlertType.TornadoWarning,
                _ => WeatherAlertType.SpecialWeatherAlert
            };

            if (alert.Type == WeatherAlertType.SpecialWeatherAlert && !string.IsNullOrWhiteSpace(@event.eventValues.title))
            {
                if (@event.eventValues.title.Contains("Heat"))
                {
                    alert.Type = WeatherAlertType.Heat;
                }
                else if (@event.eventValues.title.Contains("Cold") || @event.eventValues.title.Contains("Freeze") || @event.eventValues.title.Contains("Frost"))
                {
                    alert.Type = WeatherAlertType.WinterWeather;
                }
                else if (@event.eventValues.title.Contains("Smoke"))
                {
                    alert.Type = WeatherAlertType.DenseSmoke;
                }
                else if (@event.eventValues.title.Contains("Dust"))
                {
                    alert.Type = WeatherAlertType.DustAdvisory;
                }
                else if (@event.eventValues.title.Contains("Small Craft"))
                {
                    alert.Type = WeatherAlertType.SmallCraft;
                }
                else if (@event.eventValues.title.Contains("Gale"))
                {
                    alert.Type = WeatherAlertType.GaleWarning;
                }
                else if (@event.eventValues.title.Contains("Storm"))
                {
                    alert.Type = WeatherAlertType.StormWarning;
                }
                else if (@event.eventValues.title.Contains("Tsunami"))
                {
                    alert.Type = WeatherAlertType.TsunamiWarning;
                }
            }

            alert.Severity = @event.severity switch
            {
                "minor" => WeatherAlertSeverity.Minor,
                "moderate" => WeatherAlertSeverity.Moderate,
                "severe" => WeatherAlertSeverity.Severe,
                "extreme" => WeatherAlertSeverity.Extreme,
                _ => WeatherAlertSeverity.Unknown,
            };

            alert.Title = @event.eventValues.title;

            var messageStr = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(@event.eventValues.headline))
            {
                messageStr.AppendLine(@event.eventValues.headline);
            }

            if (!string.IsNullOrWhiteSpace(@event.eventValues.description))
            {
                if (messageStr.Length > 0)
                    messageStr.AppendLine();

                messageStr.AppendLine(@event.eventValues.description);
            }

            var instruction = @event.eventValues.response?.FirstOrDefault()?.instruction;
            if (!string.IsNullOrWhiteSpace(instruction))
            {
                if (messageStr.Length > 0)
                    messageStr.AppendLine();

                messageStr.AppendLine(instruction);
            }

            alert.Message = messageStr.ToString();

            alert.Attribution = @event.eventValues.origin ?? "tomorrow.io";

            return alert;
        }
    }
}
