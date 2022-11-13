using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleWeather.Weather_API.OpenWeather.OneCall
{
    public static partial class OWMOneCallWeatherProviderExtensions
    {
        public static ICollection<WeatherAlert> CreateWeatherAlerts(this OWMOneCallWeatherProvider _, IList<Alert> alerts)
        {
            if (alerts?.Any() != true) return null;

            var weatherAlerts = new List<WeatherAlert>(alerts.Count);
            foreach (var alert in alerts)
            {
                weatherAlerts.Add(_.CreateWeatherAlert(alert));
            }

            return weatherAlerts;
        }

        public static WeatherAlert CreateWeatherAlert(this OWMOneCallWeatherProvider _, Alert alert)
        {
            // OWM does not define alert type as it can come from multiple alert data sources
            // so just define as normal alert
            return new WeatherAlert()
            {
                Type = WeatherAlertType.SpecialWeatherAlert,
                Severity = WeatherAlertSeverity.Moderate,

                Title = alert._event,
                Message = alert.description,

                Date = DateTimeOffset.FromUnixTimeSeconds(alert.start),
                ExpiresDate = DateTimeOffset.FromUnixTimeSeconds(alert.end),

                Attribution = alert.sender_name
            };
        }
    }
}