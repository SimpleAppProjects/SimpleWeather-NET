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
        public WeatherAlert(TomorrowIO.Event @event)
        {
            Date = @event.startTime;
            ExpiresDate = @event.endTime;

            Type = @event.insight switch
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

            if (Type == WeatherAlertType.SpecialWeatherAlert)
            {
                if (@event.eventValues.title.Contains("Heat"))
                {
                    Type = WeatherAlertType.Heat;
                }
                else if (@event.eventValues.title.Contains("Cold") || @event.eventValues.title.Contains("Freeze") || @event.eventValues.title.Contains("Frost"))
                {
                    Type = WeatherAlertType.WinterWeather;
                }
                else if (@event.eventValues.title.Contains("Smoke"))
                {
                    Type = WeatherAlertType.DenseSmoke;
                }
                else if (@event.eventValues.title.Contains("Dust"))
                {
                    Type = WeatherAlertType.DustAdvisory;
                }
                else if (@event.eventValues.title.Contains("Small Craft"))
                {
                    Type = WeatherAlertType.SmallCraft;
                }
                else if (@event.eventValues.title.Contains("Gale"))
                {
                    Type = WeatherAlertType.GaleWarning;
                }
                else if (@event.eventValues.title.Contains("Storm"))
                {
                    Type = WeatherAlertType.StormWarning;
                }
                else if (@event.eventValues.title.Contains("Tsunami"))
                {
                    Type = WeatherAlertType.TsunamiWarning;
                }
            }

            Severity = @event.severity switch
            {
                "minor" => WeatherAlertSeverity.Minor,
                "moderate" => WeatherAlertSeverity.Moderate,
                "severe" => WeatherAlertSeverity.Severe,
                "extreme" => WeatherAlertSeverity.Extreme,
                _ => WeatherAlertSeverity.Unknown,
            };

            Title = @event.eventValues.title;
            Message = new StringBuilder()
                .AppendLine(@event.eventValues.headline)
                .AppendLine()
                .AppendLine(@event.eventValues.description)
                .AppendLine()
                .AppendLine(@event.eventValues.response[0].instruction)
                .ToString();

            Attribution = @event.eventValues.origin ?? "tomorrow.io";
        }
    }
}
