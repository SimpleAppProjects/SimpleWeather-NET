using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
        public static String SpeedUnit
        {
            get
            {
                bool IsFahrenheit = Settings.IsFahrenheit;

                if (IsFahrenheit)
                {
                    return SimpleLibrary.ResLoader.GetString("/Units/unit_mph");
                }
                else
                {
                    return SimpleLibrary.ResLoader.GetString("/Units/unit_kph");
                }
            }
        }

        public static String PressureUnit
        {
            get
            {
                bool IsFahrenheit = Settings.IsFahrenheit;

                if (IsFahrenheit)
                {
                    return SimpleLibrary.ResLoader.GetString("/Units/unit_inHg");
                }
                else
                {
                    return SimpleLibrary.ResLoader.GetString("/Units/unit_mBar");
                }
            }
        }

        public static String DistanceUnit
        {
            get
            {
                bool IsFahrenheit = Settings.IsFahrenheit;

                if (IsFahrenheit)
                {
                    return SimpleLibrary.ResLoader.GetString("/Units/unit_miles");
                }
                else
                {
                    return SimpleLibrary.ResLoader.GetString("/Units/unit_kilometers");
                }
            }
        }

        public static String GetPrecipitationUnit(bool snow)
        {
            bool IsFahrenheit = Settings.IsFahrenheit;

            if (IsFahrenheit)
            {
                return SimpleLibrary.ResLoader.GetString("/Units/unit_in");
            }
            else
            {
                if (snow)
                {
                    return SimpleLibrary.ResLoader.GetString("/Units/unit_cm");
                }
                else
                {
                    return SimpleLibrary.ResLoader.GetString("/Units/unit_mm");
                }
            }
        }
    }
}
