using System;

namespace SimpleWeather.WeatherData
{
    public partial class WeatherAlert
    {
        public WeatherAlert(OpenWeather.OneCall.Alert alert)
        {
            // OWM does not define alert type as it can come from multiple alert data sources
            // so just define as normal alert
            Type = WeatherAlertType.SpecialWeatherAlert;
            Severity = WeatherAlertSeverity.Moderate;

            Title = alert._event;
            Message = alert.description;

            Date = DateTimeOffset.FromUnixTimeSeconds(alert.start);
            ExpiresDate = DateTimeOffset.FromUnixTimeSeconds(alert.end);

            Attribution = alert.sender_name;
        }
    }
}