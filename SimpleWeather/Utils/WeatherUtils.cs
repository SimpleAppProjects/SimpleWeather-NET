using SimpleWeather.Location;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using Windows.System.UserProfile;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
        public static String GetLastBuildDate(Weather weather)
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            String date;
            String prefix;
            DateTime update_time = weather.update_time.DateTime;
            String timeformat = update_time.ToString("t", culture);

            timeformat = string.Format("{0} {1}", timeformat, weather.location.tz_short);

            if (update_time.DayOfWeek == DateTime.Now.DayOfWeek)
            {
                prefix = UWP.App.ResLoader.GetString("Update_PrefixDay");
                date = string.Format("{0} {1}", prefix, timeformat);
            }
            else
            {
                prefix = UWP.App.ResLoader.GetString("Update_Prefix");
                date = string.Format("{0} {1} {2}",
                    prefix, update_time.ToString("ddd", culture), timeformat);
            }

            return date;
        }

        public static String GetFeelsLikeTemp(String temp_f, String wind_mph, String humidity_percent)
        {
            String feelslikeTemp = temp_f;

            if (float.TryParse(temp_f, out float temp))
            {
                if (temp < 50 && float.TryParse(wind_mph, out float windmph))
                {
                    feelslikeTemp = CalculateWindChill(temp, windmph).ToString();
                }
                else if (temp > 80 && int.TryParse(humidity_percent, out int humidity))
                {
                    feelslikeTemp = CalculateHeatIndex(temp, humidity).ToString();
                }
                else
                    feelslikeTemp = temp_f;
            }

            return feelslikeTemp;
        }

        public static float CalculateWindChill(float temp_f, float wind_mph)
        {
            if (temp_f < 50)
                return (float)(35.74f + (0.6215f * temp_f) - (35.75f * Math.Pow(wind_mph, 0.16f)) + (0.4275f * temp_f * Math.Pow(wind_mph, 0.16f)));
            else
                return temp_f;
        }

        public static double CalculateHeatIndex(float temp_f, int humidity)
        {
            if (temp_f > 80)
            {
                double HI = -42.379
                            + (2.04901523 * temp_f)
                            + (10.14333127 * humidity)
                            - (0.22475541 * temp_f * humidity)
                            - (0.00683783 * Math.Pow(temp_f, 2))
                            - (0.05481717 * Math.Pow(humidity, 2))
                            + (0.00122874 * Math.Pow(temp_f, 2) * humidity)
                            + (0.00085282 * temp_f * Math.Pow(humidity, 2))
                            - (0.00000199 * Math.Pow(temp_f, 2) * Math.Pow(humidity, 2));

                if (humidity < 13 && (temp_f > 80 && temp_f < 112))
                {
                    double adj = ((13 - humidity) / 4f) * Math.Sqrt((17 - Math.Abs(temp_f - 95)) / 17);
                    HI -= adj;
                }
                else if (humidity > 85 && (temp_f > 80 && temp_f < 87))
                {
                    double adj = ((humidity - 85) / 10f) * ((87 - temp_f) / 5);
                    HI += adj;
                }

                if (HI > 80 && HI > temp_f)
                    return HI;
                else
                    return temp_f;
            }
            else
                return temp_f;
        }

        public static String GetWindDirection(float angle)
        {
            if (angle >= 348.75 && angle <= 11.25)
            {
                return "N";
            }
            else if (angle >= 11.25 && angle <= 33.75)
            {
                return "NNE";
            }
            else if (angle >= 33.75 && angle <= 56.25)
            {
                return "NE";
            }
            else if (angle >= 56.25 && angle <= 78.75)
            {
                return "ENE";
            }
            else if (angle >= 78.75 && angle <= 101.25)
            {
                return "E";
            }
            else if (angle >= 101.25 && angle <= 123.75)
            {
                return "ESE";
            }
            else if (angle >= 123.75 && angle <= 146.25)
            {
                return "SE";
            }
            else if (angle >= 146.25 && angle <= 168.75)
            {
                return "SSE";
            }
            else if (angle >= 168.75 && angle <= 191.25)
            {
                return "S";
            }
            else if (angle >= 191.25 && angle <= 213.75)
            {
                return "SSW";
            }
            else if (angle >= 213.75 && angle <= 236.25)
            {
                return "SW";
            }
            else if (angle >= 236.25 && angle <= 258.75)
            {
                return "WSW";
            }
            else if (angle >= 258.75 && angle <= 281.25)
            {
                return "W";
            }
            else if (angle >= 281.25 && angle <= 303.75)
            {
                return "WNW";
            }
            else if (angle >= 303.75 && angle <= 326.25)
            {
                return "NW";
            }
            else/* if (angle >= 326.25 && angle <= 348.75)*/
            {
                return "NNW";
            }
        }

        public static int GetWindDirection(String direction)
        {
            if ("N".Equals(direction))
            {
                return 0;
            }
            else if ("NNE".Equals(direction))
            {
                return 22;
            }
            else if ("NE".Equals(direction))
            {
                return 45;
            }
            else if ("ENE".Equals(direction))
            {
                return 67;
            }
            else if ("E".Equals(direction))
            {
                return 90;
            }
            else if ("ESE".Equals(direction))
            {
                return 112;
            }
            else if ("SE".Equals(direction))
            {
                return 135;
            }
            else if ("SSE".Equals(direction))
            {
                return 157;
            }
            else if ("S".Equals(direction))
            {
                return 180;
            }
            else if ("SSW".Equals(direction))
            {
                return 202;
            }
            else if ("SW".Equals(direction))
            {
                return 225;
            }
            else if ("WSW".Equals(direction))
            {
                return 247;
            }
            else if ("W".Equals(direction))
            {
                return 270;
            }
            else if ("WNW".Equals(direction))
            {
                return 292;
            }
            else if ("NW".Equals(direction))
            {
                return 315;
            }
            else
            {
                return 337;
            }
        }

        public class Coordinate
        {
            public double Latitude { get => lat; set { SetLat(value); } }
            public double Longitude { get => _long; set { SetLong(value); } }

            private double lat = 0;
            private double _long = 0;

            private void SetLat(double value) { lat = value; }

            private void SetLong(double value) { _long = value; }

            public Coordinate(string coordinatePair)
            {
                SetCoordinate(coordinatePair);
            }

            public Coordinate(double latitude, double longitude)
            {
                lat = latitude;
                _long = longitude;
            }

            public Coordinate(Windows.Devices.Geolocation.Geoposition geoPos)
            {
                lat = geoPos.Coordinate.Point.Position.Latitude;
                _long = geoPos.Coordinate.Point.Position.Longitude;
            }

            public Coordinate(LocationData location)
            {
                lat = location.latitude;
                _long = location.longitude;
            }

            public void SetCoordinate(string coordinatePair)
            {
                string[] coord = coordinatePair.Split(',');
                lat = double.Parse(coord[0]);
                _long = double.Parse(coord[1]);
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "{0},{1}",
                    lat.ToString(CultureInfo.InvariantCulture), _long.ToString(CultureInfo.InvariantCulture));
            }
        }

        public enum ErrorStatus
        {
            Unknown = -1,
            Success,
            NoWeather,
            NetworkError,
            InvalidAPIKey,
            QueryNotFound,
        }
    }
}
