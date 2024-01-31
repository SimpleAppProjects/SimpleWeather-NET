using SimpleWeather.WeatherData;
using System.Collections.Generic;

namespace SimpleWeather.Weather_API.NWS
{
    public static partial class NWSAlertProviderExtensions
    {
        public static ICollection<WeatherAlert> CreateWeatherAlerts(this NWSAlertProvider _, AlertRootobject root)
        {
            var alerts = new List<WeatherAlert>(root?.graph?.Length ?? 0);

            if (root?.graph != null)
            {
                foreach (AlertGraph result in root.graph)
                {
                    alerts.Add(_.CreateWeatherAlert(result));
                }
            }

            return alerts;
        }

        // NWS Alerts
        public static WeatherAlert CreateWeatherAlert(this NWSAlertProvider _, NWS.AlertGraph alert)
        {
            var weatherAlert = new WeatherAlert();

            // Alert Type
            switch (alert._event)
            {
                case "Hurricane Local Statement":
                    weatherAlert.Type = WeatherAlertType.HurricaneLocalStatement;
                    break;

                case "Hurricane Force Wind Watch":
                case "Hurricane Watch":
                case "Hurricane Force Wind Warning":
                case "Hurricane Warning":
                    weatherAlert.Type = WeatherAlertType.HurricaneWindWarning;
                    break;

                case "Tornado Warning":
                    weatherAlert.Type = WeatherAlertType.TornadoWarning;
                    break;

                case "Tornado Watch":
                    weatherAlert.Type = WeatherAlertType.TornadoWatch;
                    break;

                case "Severe Thunderstorm Warning":
                    weatherAlert.Type = WeatherAlertType.SevereThunderstormWarning;
                    break;

                case "Severe Thunderstorm Watch":
                    weatherAlert.Type = WeatherAlertType.SevereThunderstormWatch;
                    break;

                case "Excessive Heat Warning":
                case "Excessive Heat Watch":
                case "Heat Advisory":
                    weatherAlert.Type = WeatherAlertType.Heat;
                    break;

                case "Dense Fog Advisory":
                    weatherAlert.Type = WeatherAlertType.DenseFog;
                    break;

                case "Dense Smoke Advisory":
                    weatherAlert.Type = WeatherAlertType.DenseSmoke;
                    break;

                case "Extreme Fire Danger":
                case "Fire Warning":
                case "Fire Weather Watch":
                    weatherAlert.Type = WeatherAlertType.Fire;
                    break;

                case "Volcano Warning":
                    weatherAlert.Type = WeatherAlertType.Volcano;
                    break;

                case "Extreme Wind Warning":
                case "High Wind Warning":
                case "High Wind Watch":
                case "Lake Wind Advisory":
                case "Wind Advisory":
                    weatherAlert.Type = WeatherAlertType.HighWind;
                    break;

                case "Lake Effect Snow Advisory":
                case "Lake Effect Snow Warning":
                case "Lake Effect Snow Watch":
                case "Snow Squall Warning":
                case "Ice Storm Warning":
                case "Winter Storm Warning":
                case "Winter Storm Watch":
                case "Winter Weather Advisory":
                    weatherAlert.Type = WeatherAlertType.WinterWeather;
                    break;

                case "Earthquake Warning":
                    weatherAlert.Type = WeatherAlertType.EarthquakeWarning;
                    break;

                case "Gale Warning":
                case "Gale Watch":
                    weatherAlert.Type = WeatherAlertType.GaleWarning;
                    break;

                default:
                    if (alert._event.Contains("Flood Warning"))
                        weatherAlert.Type = WeatherAlertType.FloodWarning;
                    else if (alert._event.Contains("Flood"))
                        weatherAlert.Type = WeatherAlertType.FloodWatch;
                    else if (alert._event.Contains("Snow") || alert._event.Contains("Blizzard") ||
                        alert._event.Contains("Winter") || alert._event.Contains("Ice") ||
                        alert._event.Contains("Avalanche") || alert._event.Contains("Cold") ||
                        alert._event.Contains("Freez") || alert._event.Contains("Frost") ||
                        alert._event.Contains("Chill"))
                    {
                        weatherAlert.Type = WeatherAlertType.WinterWeather;
                    }
                    else if (alert._event.Contains("Dust"))
                        weatherAlert.Type = WeatherAlertType.DustAdvisory;
                    else if (alert._event.Contains("Small Craft"))
                        weatherAlert.Type = WeatherAlertType.SmallCraft;
                    else if (alert._event.Contains("Storm"))
                        weatherAlert.Type = WeatherAlertType.StormWarning;
                    else if (alert._event.Contains("Tsunami"))
                        weatherAlert.Type = WeatherAlertType.TsunamiWarning;
                    else
                        weatherAlert.Type = WeatherAlertType.SpecialWeatherAlert;
                    break;
            }

            weatherAlert.Severity = alert.severity switch
            {
                "Minor" => WeatherAlertSeverity.Minor,
                "Moderate" => WeatherAlertSeverity.Moderate,
                "Severe" => WeatherAlertSeverity.Severe,
                "Extreme" => WeatherAlertSeverity.Extreme,
                _ => WeatherAlertSeverity.Unknown,
            };

            weatherAlert.Title = alert._event;
            weatherAlert.Message = string.Format("{0}\n{1}", alert.description, alert.instruction);

            weatherAlert.Date = alert.sent;
            weatherAlert.ExpiresDate = alert.expires;

            weatherAlert.Attribution = "U.S. National Weather Service";

            return weatherAlert;
        }
    }
}