using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Utils
{
    public class DateTimeUtils
    {
        public static TimeSpan MaxTimeOfDay()
        {
            return new TimeSpan(0, 23, 59, 59, 999);
        }
    }
}
