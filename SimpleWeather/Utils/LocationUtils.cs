using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Utils
{
    public static class LocationUtils
    {
        public static bool IsUS(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return false;
            }
            else
            {
                return countryCode.Equals("us", StringComparison.InvariantCultureIgnoreCase) || countryCode.Equals("usa", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public static bool IsUSorCanada(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return false;
            }
            else
            {
                return countryCode.Equals("US", StringComparison.InvariantCultureIgnoreCase) || countryCode.Equals("CA", StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
