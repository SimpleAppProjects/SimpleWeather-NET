using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace SimpleWeather.WeatherData
{
    public partial class WeatherAlert
    {
        // NWS Alerts
        public WeatherAlert(NWS.AlertGraph alert)
        {
            // Alert Type
            switch (alert._event)
            {
                case "Hurricane Local Statement":
                    Type = WeatherAlertType.HurricaneLocalStatement;
                    break;

                case "Hurricane Force Wind Watch":
                case "Hurricane Watch":
                case "Hurricane Force Wind Warning":
                case "Hurricane Warning":
                    Type = WeatherAlertType.HurricaneWindWarning;
                    break;

                case "Tornado Warning":
                    Type = WeatherAlertType.TornadoWarning;
                    break;

                case "Tornado Watch":
                    Type = WeatherAlertType.TornadoWatch;
                    break;

                case "Severe Thunderstorm Warning":
                    Type = WeatherAlertType.SevereThunderstormWarning;
                    break;

                case "Severe Thunderstorm Watch":
                    Type = WeatherAlertType.SevereThunderstormWatch;
                    break;

                case "Excessive Heat Warning":
                case "Excessive Heat Watch":
                case "Heat Advisory":
                    Type = WeatherAlertType.Heat;
                    break;

                case "Dense Fog Advisory":
                    Type = WeatherAlertType.DenseFog;
                    break;

                case "Dense Smoke Advisory":
                    Type = WeatherAlertType.DenseSmoke;
                    break;

                case "Extreme Fire Danger":
                case "Fire Warning":
                case "Fire Weather Watch":
                    Type = WeatherAlertType.Fire;
                    break;

                case "Volcano Warning":
                    Type = WeatherAlertType.Volcano;
                    break;

                case "Extreme Wind Warning":
                case "High Wind Warning":
                case "High Wind Watch":
                case "Lake Wind Advisory":
                case "Wind Advisory":
                    Type = WeatherAlertType.HighWind;
                    break;

                case "Lake Effect Snow Advisory":
                case "Lake Effect Snow Warning":
                case "Lake Effect Snow Watch":
                case "Snow Squall Warning":
                case "Ice Storm Warning":
                case "Winter Storm Warning":
                case "Winter Storm Watch":
                case "Winter Weather Advisory":
                    Type = WeatherAlertType.WinterWeather;
                    break;

                case "Earthquake Warning":
                    Type = WeatherAlertType.EarthquakeWarning;
                    break;

                case "Gale Warning":
                case "Gale Watch":
                    Type = WeatherAlertType.GaleWarning;
                    break;

                default:
                    if (alert._event.Contains("Flood Warning"))
                        Type = WeatherAlertType.FloodWarning;
                    else if (alert._event.Contains("Flood"))
                        Type = WeatherAlertType.FloodWatch;
                    else if (alert._event.Contains("Snow") || alert._event.Contains("Blizzard") ||
                        alert._event.Contains("Winter") || alert._event.Contains("Ice") ||
                        alert._event.Contains("Avalanche") || alert._event.Contains("Cold") ||
                        alert._event.Contains("Freez") || alert._event.Contains("Frost") ||
                        alert._event.Contains("Chill"))
                    {
                        Type = WeatherAlertType.WinterWeather;
                    }
                    else if (alert._event.Contains("Dust"))
                        Type = WeatherAlertType.DustAdvisory;
                    else if (alert._event.Contains("Small Craft"))
                        Type = WeatherAlertType.SmallCraft;
                    else if (alert._event.Contains("Storm"))
                        Type = WeatherAlertType.StormWarning;
                    else if (alert._event.Contains("Tsunami"))
                        Type = WeatherAlertType.TsunamiWarning;
                    else
                        Type = WeatherAlertType.SpecialWeatherAlert;
                    break;
            }

            switch (alert.severity)
            {
                case "Minor":
                    Severity = WeatherAlertSeverity.Minor;
                    break;

                case "Moderate":
                    Severity = WeatherAlertSeverity.Moderate;
                    break;

                case "Severe":
                    Severity = WeatherAlertSeverity.Severe;
                    break;

                case "Extreme":
                    Severity = WeatherAlertSeverity.Extreme;
                    break;

                case "Unknown":
                default:
                    Severity = WeatherAlertSeverity.Unknown;
                    break;
            }

            Title = alert._event;
            Message = string.Format("{0}\n{1}", alert.description, alert.instruction);

            Date = alert.sent;
            ExpiresDate = alert.expires;

            Attribution = "U.S. National Weather Service";
        }
    }
}

namespace SimpleWeather.NWS
{
    public partial class NWSAlertProvider
    {
        private ICollection<WeatherAlert> CreateWeatherAlerts(AlertRootobject root)
        {
            var alerts = new List<WeatherAlert>(root.graph.Length);

            foreach (AlertGraph result in root.graph)
            {
                alerts.Add(new WeatherAlert(result));
            }

            return alerts;
        }
    }
}