using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SimpleWeather.Weather_API.HERE
{
    public static partial class HEREWeatherProviderExtensions
    {
        public static ICollection<WeatherAlert> CreateWeatherAlerts(this HEREWeatherProvider _, Rootobject root, float lat, float lon)
        {
            ICollection<WeatherAlert> weatherAlerts = null;

            if (root.alerts?.alerts?.Length > 0)
            {
                weatherAlerts = new List<WeatherAlert>(root.alerts.alerts.Length);

                foreach (Alert result in root.alerts.alerts)
                {
                    weatherAlerts.Add(_.CreateWeatherAlert(result));
                }
            }
            else if (root.nwsAlerts?.watch?.Length > 0 || root.nwsAlerts?.warning?.Length > 0)
            {
                int numOfAlerts = (root.nwsAlerts?.watch?.Length ?? 0) + (root.nwsAlerts?.warning?.Length ?? 0);

                weatherAlerts = new HashSet<WeatherAlert>(
#if !NETSTANDARD2_0
                    numOfAlerts
#endif
                    );

                if (root.nwsAlerts.watch != null)
                {
                    foreach (var watchItem in root.nwsAlerts.watch)
                    {
                        // Add watch item if location is within 20km of the center of the alert zone
                        if (Math.Abs(ConversionMethods.CalculateHaversine(lat, lon, double.Parse(watchItem.latitude), double.Parse(watchItem.longitude))) < 20000)
                        {
                            weatherAlerts.Add(_.CreateWeatherAlert(watchItem));
                        }
                    }
                }
                if (root.nwsAlerts.warning != null)
                {
                    foreach (var warningItem in root.nwsAlerts.warning)
                    {
                        // Add warning item if location is within 25km of the center of the alert zone
                        if (Math.Abs(ConversionMethods.CalculateHaversine(lat, lon, double.Parse(warningItem.latitude), double.Parse(warningItem.longitude))) < 25000)
                        {
                            weatherAlerts.Add(_.CreateWeatherAlert(warningItem));
                        }
                    }
                }
            }

            return weatherAlerts;
        }

        // HERE GlobalAlerts
        public static WeatherAlert CreateWeatherAlert(this HEREWeatherProvider _, HERE.Alert alert)
        {
            var weatherAlert = new WeatherAlert();

            // Alert Type
            switch (alert.type)
            {
                case "1": // Strong Thunderstorms Anticipated
                    weatherAlert.Type = WeatherAlertType.SevereThunderstormWatch;
                    weatherAlert.Severity = WeatherAlertSeverity.Moderate;
                    break;

                case "2": // Severe Thunderstorms Anticipated
                    weatherAlert.Type = WeatherAlertType.SevereThunderstormWarning;
                    weatherAlert.Severity = WeatherAlertSeverity.Extreme;
                    break;

                case "3": // Tornadoes Possible
                    weatherAlert.Type = WeatherAlertType.TornadoWarning;
                    weatherAlert.Severity = WeatherAlertSeverity.Extreme;
                    break;

                case "4": // Heavy Rain Anticipated
                    weatherAlert.Type = WeatherAlertType.FloodWatch;
                    weatherAlert.Severity = WeatherAlertSeverity.Severe;
                    break;

                case "5": // Floods Anticipated
                case "6": // Flash Floods Anticipated
                    weatherAlert.Type = WeatherAlertType.FloodWarning;
                    weatherAlert.Severity = WeatherAlertSeverity.Extreme;
                    break;

                case "7": // High Winds Anticipated
                    weatherAlert.Type = WeatherAlertType.HighWind;
                    weatherAlert.Severity = WeatherAlertSeverity.Moderate;
                    break;

                case "8": // Heavy Snow Anticipated
                case "11": // Freezing Rain Anticipated
                case "12": // Ice Storm Anticipated
                    weatherAlert.Type = WeatherAlertType.WinterWeather;
                    weatherAlert.Severity = WeatherAlertSeverity.Severe;
                    break;

                case "9": // Blizzard Conditions Anticipated
                case "10": // Blowing Snow Anticipated
                    weatherAlert.Type = WeatherAlertType.WinterWeather;
                    weatherAlert.Severity = WeatherAlertSeverity.Extreme;
                    break;

                case "13": // Snow Advisory
                case "14": // Winter Weather Advisory

                case "17": // Wind Chill Alert
                case "18": // Frost Advisory
                case "19": // Freeze Advisory

                    weatherAlert.Type = WeatherAlertType.WinterWeather;
                    weatherAlert.Severity = WeatherAlertSeverity.Moderate;
                    break;

                case "15": // Heat Advisory
                    weatherAlert.Type = WeatherAlertType.Heat;
                    weatherAlert.Severity = WeatherAlertSeverity.Moderate;
                    break;

                case "16": // Excessive Heat Alert
                    weatherAlert.Type = WeatherAlertType.Heat;
                    weatherAlert.Severity = WeatherAlertSeverity.Extreme;
                    break;

                case "20": // Fog Anticipated
                case "22": // Smog Anticipated
                    weatherAlert.Type = WeatherAlertType.DenseFog;
                    weatherAlert.Severity = WeatherAlertSeverity.Moderate;
                    break;

                case "21": // Dense Fog Anticipated
                    weatherAlert.Type = WeatherAlertType.DenseFog;
                    weatherAlert.Severity = WeatherAlertSeverity.Severe;
                    break;

                case "30": // Tropical Cyclone Conditions Anticipated
                    weatherAlert.Type = WeatherAlertType.HurricaneWindWarning;
                    weatherAlert.Severity = WeatherAlertSeverity.Severe;
                    break;

                case "31": // Hurricane Conditions Anticipated
                    weatherAlert.Type = WeatherAlertType.HurricaneWindWarning;
                    weatherAlert.Severity = WeatherAlertSeverity.Extreme;
                    break;

                case "32": // Small Craft Advisory Anticipated
                    weatherAlert.Type = WeatherAlertType.SmallCraft;
                    weatherAlert.Severity = WeatherAlertSeverity.Moderate;
                    break;

                case "33": // Gale Warning Anticipated
                    weatherAlert.Type = WeatherAlertType.GaleWarning;
                    weatherAlert.Severity = WeatherAlertSeverity.Severe;
                    break;

                case "34": // High Winds Anticipated (Winds greater than 35 || 50 mph anticipated)
                    weatherAlert.Type = WeatherAlertType.HighWind;
                    weatherAlert.Severity = WeatherAlertSeverity.Severe;
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
                    weatherAlert.Type = WeatherAlertType.SpecialWeatherAlert;
                    weatherAlert.Severity = WeatherAlertSeverity.Severe;
                    break;
            }

            // NOTE: Alert description may be encoded; unescape encoded characters
            weatherAlert.Title = weatherAlert.Message = alert.description?.UnescapeUnicode();

            weatherAlert.SetDateTimeFromSegment(alert.timeSegment);

            weatherAlert.Attribution = "HERE Weather";

            return weatherAlert;
        }

        // HERE NWS Alerts
        public static WeatherAlert CreateWeatherAlert(this HEREWeatherProvider _, Watch alert)
        {
            var weatherAlert = new WeatherAlert();

            int type = -1;
            if (int.TryParse(alert.type, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedType))
            {
                type = parsedType;
            }

            weatherAlert.Type = GetAlertType(type, alert.description);
            weatherAlert.Severity = GetAlertSeverity(alert.severity);

            weatherAlert.Title = alert.description;
            weatherAlert.Message = alert.message;

            weatherAlert.Date = alert.validFromTimeLocal;
            weatherAlert.ExpiresDate = alert.validUntilTimeLocal;

            weatherAlert.Attribution = "HERE Weather";

            return weatherAlert;
        }

        public static WeatherAlert CreateWeatherAlert(this HEREWeatherProvider _, Warning alert)
        {
            var weatherAlert = new WeatherAlert();

            int type = -1;
            if (int.TryParse(alert.type, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedType))
            {
                type = parsedType;
            }

            weatherAlert.Type = GetAlertType(type, alert.description);
            weatherAlert.Severity = GetAlertSeverity(alert.severity);

            weatherAlert.Title = alert.description;
            weatherAlert.Message = alert.message;

            weatherAlert.Date = alert.validFromTimeLocal;
            weatherAlert.ExpiresDate = alert.validUntilTimeLocal;

            weatherAlert.Attribution = "HERE Weather";

            return weatherAlert;
        }

        private static WeatherAlertType GetAlertType(int type, String alertDescription)
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

        private static WeatherAlertSeverity GetAlertSeverity(int severity)
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

        private static void SetDateTimeFromSegment(this WeatherAlert _, HERE.Timesegment[] timeSegment)
        {
            if (timeSegment.Length > 1)
            {
                var startDate = DateTimeUtils.GetClosestWeekday((DayOfWeek)(int.Parse(timeSegment[0].day_of_week, CultureInfo.InvariantCulture) - 1));
                var endDate = DateTimeUtils.GetClosestWeekday((DayOfWeek)(int.Parse(timeSegment.Last().day_of_week, CultureInfo.InvariantCulture) - 1));

                _.Date = new DateTimeOffset(startDate.Add(GetTimeFromSegment(timeSegment[0].segment)), TimeSpan.Zero);
                _.ExpiresDate = new DateTimeOffset(endDate.Add(GetTimeFromSegment(timeSegment.Last().segment)), TimeSpan.Zero);
            }
            else
            {
                var today = DateTimeUtils.GetClosestWeekday((DayOfWeek)(int.Parse(timeSegment[0].day_of_week, CultureInfo.InvariantCulture) - 1));

                switch (timeSegment[0].segment)
                {
                    case "M": // Morning
                    default:
                        _.Date = new DateTimeOffset(today.Add(GetTimeFromSegment("M")), TimeSpan.Zero);
                        _.ExpiresDate = new DateTimeOffset(today.Add(GetTimeFromSegment("A")), TimeSpan.Zero);
                        break;

                    case "A": // Afternoon
                        _.Date = new DateTimeOffset(today.Add(GetTimeFromSegment("A")), TimeSpan.Zero);
                        _.ExpiresDate = new DateTimeOffset(today.Add(GetTimeFromSegment("E")), TimeSpan.Zero);
                        break;

                    case "E": // Evening
                        _.Date = new DateTimeOffset(today.Add(GetTimeFromSegment("E")), TimeSpan.Zero);
                        _.ExpiresDate = new DateTimeOffset(today.Add(GetTimeFromSegment("N")), TimeSpan.Zero);
                        break;

                    case "N": // Night
                        _.Date = new DateTimeOffset(today.Add(GetTimeFromSegment("N")), TimeSpan.Zero);
                        _.ExpiresDate = new DateTimeOffset(today.AddDays(1).Add(GetTimeFromSegment("M")), TimeSpan.Zero); // The next morning
                        break;
                }
            }
        }

        private static TimeSpan GetTimeFromSegment(String segment)
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