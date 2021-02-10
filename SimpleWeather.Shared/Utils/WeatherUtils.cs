using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using Windows.System.UserProfile;
using Windows.UI;
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

            var culture = CultureUtils.UserCulture;

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

        public static float CalculateDewpointF(float temp_f, int humidity)
        {
            return ConversionMethods.CtoF(CalculateDewpointC(ConversionMethods.FtoC(temp_f), humidity));
        }

        public static float CalculateDewpointC(float temp_c, int humidity)
        {
            return 243.04f * (MathF.Log(humidity / 100f) + ((17.625f * temp_c) / (243.04f + temp_c))) / (17.625f - MathF.Log(humidity / 100f) - ((17.625f * temp_c) / (243.04f + temp_c)));
        }

        public static Color GetColorFromTempF(float temp_f)
        {
            return GetColorFromTempF(temp_f, Color.FromArgb(0xff, 0x00, 0x70, 0xc0));
        }

        public static Color GetColorFromTempF(float temp_f, Color defaultColor)
        {
            Color color;

            if (temp_f <= 47.5)
            {
                color = Colors.LightSkyBlue;
            }
            else if (temp_f >= 85)
            {
                color = Colors.Red;
            }
            else if (temp_f >= 70)
            {
                color = Colors.Orange;
            }
            else
            {
                color = defaultColor;
            }

            return color;
        }

        ////
    }
}