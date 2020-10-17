using SimpleWeather.HERE;
using SimpleWeather.Utils;
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

        // HERE GlobalAlerts
        public WeatherAlert(HERE.Alert alert)
        {
            // Alert Type
            switch (alert.type)
            {
                case "1": // Strong Thunderstorms Anticipated
                    Type = WeatherAlertType.SevereThunderstormWatch;
                    Severity = WeatherAlertSeverity.Moderate;
                    break;

                case "2": // Severe Thunderstorms Anticipated
                    Type = WeatherAlertType.SevereThunderstormWarning;
                    Severity = WeatherAlertSeverity.Extreme;
                    break;

                case "3": // Tornadoes Possible
                    Type = WeatherAlertType.TornadoWarning;
                    Severity = WeatherAlertSeverity.Extreme;
                    break;

                case "4": // Heavy Rain Anticipated
                    Type = WeatherAlertType.FloodWatch;
                    Severity = WeatherAlertSeverity.Severe;
                    break;

                case "5": // Floods Anticipated
                case "6": // Flash Floods Anticipated
                    Type = WeatherAlertType.FloodWarning;
                    Severity = WeatherAlertSeverity.Extreme;
                    break;

                case "7": // High Winds Anticipated
                    Type = WeatherAlertType.HighWind;
                    Severity = WeatherAlertSeverity.Moderate;
                    break;

                case "8": // Heavy Snow Anticipated
                case "11": // Freezing Rain Anticipated
                case "12": // Ice Storm Anticipated
                    Type = WeatherAlertType.WinterWeather;
                    Severity = WeatherAlertSeverity.Severe;
                    break;

                case "9": // Blizzard Conditions Anticipated
                case "10": // Blowing Snow Anticipated
                    Type = WeatherAlertType.WinterWeather;
                    Severity = WeatherAlertSeverity.Extreme;
                    break;

                case "13": // Snow Advisory
                case "14": // Winter Weather Advisory

                case "17": // Wind Chill Alert
                case "18": // Frost Advisory
                case "19": // Freeze Advisory

                    Type = WeatherAlertType.WinterWeather;
                    Severity = WeatherAlertSeverity.Moderate;
                    break;

                case "15": // Heat Advisory
                    Type = WeatherAlertType.Heat;
                    Severity = WeatherAlertSeverity.Moderate;
                    break;

                case "16": // Excessive Heat Alert
                    Type = WeatherAlertType.Heat;
                    Severity = WeatherAlertSeverity.Extreme;
                    break;

                case "20": // Fog Anticipated
                case "22": // Smog Anticipated
                    Type = WeatherAlertType.DenseFog;
                    Severity = WeatherAlertSeverity.Moderate;
                    break;

                case "21": // Dense Fog Anticipated
                    Type = WeatherAlertType.DenseFog;
                    Severity = WeatherAlertSeverity.Severe;
                    break;

                case "30": // Tropical Cyclone Conditions Anticipated
                    Type = WeatherAlertType.HurricaneWindWarning;
                    Severity = WeatherAlertSeverity.Severe;
                    break;

                case "31": // Hurricane Conditions Anticipated
                    Type = WeatherAlertType.HurricaneWindWarning;
                    Severity = WeatherAlertSeverity.Extreme;
                    break;

                case "32": // Small Craft Advisory Anticipated
                    Type = WeatherAlertType.SmallCraft;
                    Severity = WeatherAlertSeverity.Moderate;
                    break;

                case "33": // Gale Warning Anticipated
                    Type = WeatherAlertType.GaleWarning;
                    Severity = WeatherAlertSeverity.Severe;
                    break;

                case "34": // High Winds Anticipated (Winds greater than 35 || 50 mph anticipated)
                    Type = WeatherAlertType.HighWind;
                    Severity = WeatherAlertSeverity.Severe;
                    break;

                case "23": // Unknown
                case "24": // Unknown
                case "25": // Unknown
                case "26": // Unknown
                case "27": // Unknown
                case "28": // Unknown
                case "29": // Unknown
                case "35": // Heavy Surf Advisory
                case "36": // Beach Erosion Advisory
                default:
                    Type = WeatherAlertType.SpecialWeatherAlert;
                    Severity = WeatherAlertSeverity.Severe;
                    break;
            }

            Title = alert.description;
            Message = alert.description;

            SetDateTimeFromSegment(alert.timeSegment);

            Attribution = "HERE Weather";
        }

        // HERE NWS Alerts
        public WeatherAlert(Watch alert)
        {
            int type = -1;
            if (int.TryParse(alert.type, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedType))
            {
                type = parsedType;
            }

            Type = GetAlertType(type, alert.description);
            Severity = GetAlertSeverity(alert.severity);

            Title = alert.description;
            Message = alert.message;

            Date = alert.validFromTimeLocal;
            ExpiresDate = alert.validUntilTimeLocal;

            Attribution = "HERE Weather";
        }

        public WeatherAlert(Warning alert)
        {
            int type = -1;
            if (int.TryParse(alert.type, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedType))
            {
                type = parsedType;
            }

            Type = GetAlertType(type, alert.description);
            Severity = GetAlertSeverity(alert.severity);

            Title = alert.description;
            Message = alert.message;

            Date = alert.validFromTimeLocal;
            ExpiresDate = alert.validUntilTimeLocal;

            Attribution = "HERE Weather";
        }

        private WeatherAlertType GetAlertType(int type, String alertDescription)
        {
            switch (type)
            {
                case 0: // Aviation Weather Warning
                case 1: // Civil Emergency Message
                case 10: // Lakeshore Warning or Statement
                case 11: // Marine Weather Statement
                case 12: // Non Precipitation Warning, Watch, or Statement
                case 13: // Public Severe Weather Alert
                case 14: // Red Flag Warning
                case 16: // River Recreation Statement
                case 17: // River Statement
                case 19: // Preliminary Notice of Watch Cancellation - Aviation Message
                case 20: // Special Dispersion Statement
                case 22: // SPC Watch Point Information Message
                case 25: // Special Marine Warning
                case 27: // Special Weather Statement
                case 38: // Air Stagnation Advisory
                default:
                    {
                        // Try to get a more detailed alert type
                        if (alertDescription.Contains("Hurricane", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return WeatherAlertType.HurricaneWindWarning;
                        }
                        else if (alertDescription.Contains("Tornado", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return WeatherAlertType.TornadoWarning;
                        }
                        else if (alertDescription.Contains("Heat", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return WeatherAlertType.Heat;
                        }
                        else if (alertDescription.Contains("Dense Fog", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return WeatherAlertType.DenseFog;
                        }
                        else if (alertDescription.Contains("Dense Smoke", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return WeatherAlertType.DenseSmoke;
                        }
                        else if (alertDescription.Contains("Fire", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return WeatherAlertType.Fire;
                        }
                        else if (alertDescription.Contains("Wind", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return WeatherAlertType.HighWind;
                        }
                        else if (alertDescription.Contains("Snow", StringComparison.InvariantCultureIgnoreCase) || alertDescription.Contains("Blizzard", StringComparison.InvariantCultureIgnoreCase) ||
                              alertDescription.Contains("Winter", StringComparison.InvariantCultureIgnoreCase) || alertDescription.Contains("Ice", StringComparison.InvariantCultureIgnoreCase) ||
                              alertDescription.Contains("Ice", StringComparison.InvariantCultureIgnoreCase) || alertDescription.Contains("Ice", StringComparison.InvariantCultureIgnoreCase) ||
                              alertDescription.Contains("Avalanche", StringComparison.InvariantCultureIgnoreCase) || alertDescription.Contains("Cold", StringComparison.InvariantCultureIgnoreCase) ||
                              alertDescription.Contains("Freez", StringComparison.InvariantCultureIgnoreCase) || alertDescription.Contains("Frost", StringComparison.InvariantCultureIgnoreCase) ||
                              alertDescription.Contains("Chill", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return WeatherAlertType.WinterWeather;
                        }
                        else if (alertDescription.Contains("Earthquake", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return WeatherAlertType.EarthquakeWarning;
                        }
                        else if (alertDescription.Contains("Gale", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return WeatherAlertType.GaleWarning;
                        }
                        else if (alertDescription.Contains("Dust", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return WeatherAlertType.DustAdvisory;
                        }
                        else if (alertDescription.Contains("Small Craft", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return WeatherAlertType.SmallCraft;
                        }
                        else if (alertDescription.Contains("Storm", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return WeatherAlertType.StormWarning;
                        }
                        else if (alertDescription.Contains("Tsunami", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return WeatherAlertType.TsunamiWarning;
                        }

                        return WeatherAlertType.SpecialWeatherAlert;
                    }
                case 2: // Coastal Flood Warning, Watch, or Statement
                case 5: // Flash Flood Warning
                case 7: // Flood Warning
                case 8: // Urban and Small Stream Flood Advisory
                    return WeatherAlertType.FloodWarning;
                case 3: // Flash Flood Watch
                case 4: // Flash Flood Statement
                case 6: // Flood Statement
                    return WeatherAlertType.FloodWatch;
                case 9: // Hurricane Local Statement
                    return WeatherAlertType.HurricaneLocalStatement;
                case 15: // River Ice Statement
                case 18: // Snow Avalanche Bulletin
                case 37: // Winter Weather Warning, Watch, or Advisory
                    return WeatherAlertType.WinterWeather;
                case 21: // Severe Local Storm Watch or Watch Cancellation
                case 23: // Severe Local Storm Watch and Areal Outline
                case 26: // Storm Strike Probability Bulletin from the TPC
                    return WeatherAlertType.SevereThunderstormWatch;
                case 24: // Marine Subtropical Storm Advisory
                    return WeatherAlertType.StormWarning;
                case 28: // Severe Thunderstorm Warning
                    return WeatherAlertType.SevereThunderstormWarning;
                case 29: // Severe Weather Statement
                    return WeatherAlertType.SevereWeather;
                case 30: // Tropical Cyclone Advisory
                case 31: // Tropical Cyclone Advisory for Marine and Aviation Interests
                case 32: // Public Tropical Cyclone Advisory
                case 33: // Tropical Cyclone Update
                    return WeatherAlertType.HurricaneWindWarning;
                case 34: // Tornado Warning
                    return WeatherAlertType.TornadoWarning;
                case 35: // Tsunami Watch or Warning
                    return WeatherAlertType.TsunamiWarning;
                case 36: // Volcanic Activity Advisory
                    return WeatherAlertType.Volcano;
            }
        }

        private WeatherAlertSeverity GetAlertSeverity(int severity)
        {
            if (severity >= 75)
            {
                return WeatherAlertSeverity.Extreme;
            }
            else if (severity >= 50)
            {
                return WeatherAlertSeverity.Severe;
            }
            else if (severity >= 25)
            {
                return WeatherAlertSeverity.Moderate;
            }
            else
            {
                return WeatherAlertSeverity.Minor;
            }
        }

        private void SetDateTimeFromSegment(HERE.Timesegment[] timeSegment)
        {
            if (timeSegment.Length > 1)
            {
                var startDate = DateTimeUtils.GetClosestWeekday((DayOfWeek)(int.Parse(timeSegment[0].day_of_week, CultureInfo.InvariantCulture) - 1));
                var endDate = DateTimeUtils.GetClosestWeekday((DayOfWeek)(int.Parse(timeSegment.Last().day_of_week, CultureInfo.InvariantCulture) - 1));

                Date = new DateTimeOffset(startDate.Add(GetTimeFromSegment(timeSegment[0].segment)), TimeSpan.Zero);
                ExpiresDate = new DateTimeOffset(endDate.Add(GetTimeFromSegment(timeSegment.Last().segment)), TimeSpan.Zero);
            }
            else
            {
                var today = DateTimeUtils.GetClosestWeekday((DayOfWeek)(int.Parse(timeSegment[0].day_of_week, CultureInfo.InvariantCulture) - 1));

                switch (timeSegment[0].segment)
                {
                    case "M": // Morning
                    default:
                        Date = new DateTimeOffset(today.Add(GetTimeFromSegment("M")), TimeSpan.Zero);
                        ExpiresDate = new DateTimeOffset(today.Add(GetTimeFromSegment("A")), TimeSpan.Zero);
                        break;

                    case "A": // Afternoon
                        Date = new DateTimeOffset(today.Add(GetTimeFromSegment("A")), TimeSpan.Zero);
                        ExpiresDate = new DateTimeOffset(today.Add(GetTimeFromSegment("E")), TimeSpan.Zero);
                        break;

                    case "E": // Evening
                        Date = new DateTimeOffset(today.Add(GetTimeFromSegment("E")), TimeSpan.Zero);
                        ExpiresDate = new DateTimeOffset(today.Add(GetTimeFromSegment("N")), TimeSpan.Zero);
                        break;

                    case "N": // Night
                        Date = new DateTimeOffset(today.Add(GetTimeFromSegment("N")), TimeSpan.Zero);
                        ExpiresDate = new DateTimeOffset(today.AddDays(1).Add(GetTimeFromSegment("M")), TimeSpan.Zero); // The next morning
                        break;
                }
            }
        }

        private TimeSpan GetTimeFromSegment(String segment)
        {
            TimeSpan span = TimeSpan.Zero;

            switch (segment)
            {
                case "M": // Morning
                    span = new TimeSpan(5, 0, 0); // hh:mm:ss
                    break;

                case "A": // Afternoon
                    span = new TimeSpan(12, 0, 0); // hh:mm:ss
                    break;

                case "E": // Evening
                    span = new TimeSpan(17, 0, 0); // hh:mm:ss
                    break;

                case "N": // Night
                    span = new TimeSpan(21, 0, 0); // hh:mm:ss
                    break;

                default:
                    break;
            }

            return span;
        }
    }
}