using System;
using System.Collections.Generic;
using System.Text;
using static SimpleWeather.WeatherData.Beaufort;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
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
    }
}
