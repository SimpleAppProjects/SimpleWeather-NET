using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using Windows.System.UserProfile;
using static SimpleWeather.WeatherData.Beaufort;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
        public static String GetLastBuildDate(Weather weather)
        {
            if (weather is null)
            {
                return String.Empty;
            }

            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            String date;
            String prefix;
            DateTime update_time = weather.update_time.DateTime;
            String timeformat = update_time.ToString("t", culture);

            timeformat = string.Format("{0} {1}", timeformat, weather.location.tz_short);

            if (update_time.DayOfWeek == DateTime.Now.DayOfWeek)
            {
                prefix = SimpleLibrary.ResLoader.GetString("Update_PrefixDay");
                date = string.Format("{0} {1}", prefix, timeformat);
            }
            else
            {
                prefix = SimpleLibrary.ResLoader.GetString("Update_Prefix");
                date = string.Format("{0} {1} {2}",
                    prefix, update_time.ToString("ddd", culture), timeformat);
            }

            return date;
        }

        public static String GetPressureStateIcon(string state)
        {
            switch (state)
            {
                // Steady
                case "0":
                default:
                    return string.Empty;
                // Rising
                case "1":
                case "+":
                case "Rising":
                    return "\uf058\uf058";
                // Falling
                case "2":
                case "-":
                case "Falling":
                    return "\uf044\uf044";
            }
        }

        public static float GetFeelsLikeTemp(float tempF, float windMph, int humidityPercent)
        {
            float feelslikeTemp;

            if (tempF < 50)
                feelslikeTemp = CalculateWindChill(tempF, windMph);
            else if (tempF > 80)
                feelslikeTemp = CalculateHeatIndex(tempF, humidityPercent);
            else
                feelslikeTemp = tempF;

            return feelslikeTemp;
        }

        public static float CalculateWindChill(float tempF, float windMph)
        {
            if (tempF < 50)
                return (float)(35.74f + (0.6215f * tempF) - (35.75f * Math.Pow(windMph, 0.16f)) + (0.4275f * tempF * Math.Pow(windMph, 0.16f)));
            else
                return tempF;
        }

        public static float CalculateHeatIndex(float tempF, int humidity)
        {
            if (tempF > 80)
            {
                double HI = -42.379
                            + (2.04901523 * tempF)
                            + (10.14333127 * humidity)
                            - (0.22475541 * tempF * humidity)
                            - (0.00683783 * Math.Pow(tempF, 2))
                            - (0.05481717 * Math.Pow(humidity, 2))
                            + (0.00122874 * Math.Pow(tempF, 2) * humidity)
                            + (0.00085282 * tempF * Math.Pow(humidity, 2))
                            - (0.00000199 * Math.Pow(tempF, 2) * Math.Pow(humidity, 2));

                if (humidity < 13 && (tempF > 80 && tempF < 112))
                {
                    double adj = ((13 - humidity) / 4f) * Math.Sqrt((17 - Math.Abs(tempF - 95)) / 17);
                    HI -= adj;
                }
                else if (humidity > 85 && (tempF > 80 && tempF < 87))
                {
                    double adj = ((humidity - 85) / 10f) * ((87 - tempF) / 5);
                    HI += adj;
                }

                if (HI > 80 && HI > tempF)
                    return (float)HI;
                else
                    return tempF;
            }
            else
                return tempF;
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

        public static BeaufortScale GetBeaufortScale(int mph)
        {
            if (mph >= 1 && mph <= 3)
            {
                return BeaufortScale.B1;
            }
            else if (mph >= 4 && mph <= 7)
            {
                return BeaufortScale.B2;
            }
            else if (mph >= 8 && mph <= 12)
            {
                return BeaufortScale.B3;
            }
            else if (mph >= 13 && mph <= 18)
            {
                return BeaufortScale.B4;
            }
            else if (mph >= 19 && mph <= 24)
            {
                return BeaufortScale.B5;
            }
            else if (mph >= 25 && mph <= 31)
            {
                return BeaufortScale.B6;
            }
            else if (mph >= 32 && mph <= 38)
            {
                return BeaufortScale.B7;
            }
            else if (mph >= 39 && mph <= 46)
            {
                return BeaufortScale.B8;
            }
            else if (mph >= 47 && mph <= 54)
            {
                return BeaufortScale.B9;
            }
            else if (mph >= 55 && mph <= 63)
            {
                return BeaufortScale.B10;
            }
            else if (mph >= 64 && mph <= 72)
            {
                return BeaufortScale.B11;
            }
            else if (mph >= 73)
            {
                return BeaufortScale.B12;
            }
            else
            {
                return BeaufortScale.B0;
            }
        }

        public static BeaufortScale GetBeaufortScale(float mps)
        {
            mps = (float)Math.Round(mps, 1);

            if (mps >= 0.5f && mps <= 1.5f)
            {
                return BeaufortScale.B1;
            }
            else if (mps >= 1.6f && mps <= 3.3f)
            {
                return BeaufortScale.B2;
            }
            else if (mps >= 3.4f && mps <= 5.5f)
            {
                return BeaufortScale.B3;
            }
            else if (mps >= 5.5f && mps <= 7.9f)
            {
                return BeaufortScale.B4;
            }
            else if (mps >= 8f && mps <= 10.7f)
            {
                return BeaufortScale.B5;
            }
            else if (mps >= 10.8f && mps <= 13.8f)
            {
                return BeaufortScale.B6;
            }
            else if (mps >= 13.9f && mps <= 17.1f)
            {
                return BeaufortScale.B7;
            }
            else if (mps >= 17.2f && mps <= 20.7f)
            {
                return BeaufortScale.B8;
            }
            else if (mps >= 20.8f && mps <= 24.4f)
            {
                return BeaufortScale.B9;
            }
            else if (mps >= 24.5 && mps <= 28.4f)
            {
                return BeaufortScale.B10;
            }
            else if (mps >= 28.5f && mps <= 32.6f)
            {
                return BeaufortScale.B11;
            }
            else if (mps >= 32.7f)
            {
                return BeaufortScale.B12;
            }
            else
            {
                return BeaufortScale.B0;
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