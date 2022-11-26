using System;

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
                return countryCode.Equals("us", StringComparison.InvariantCultureIgnoreCase) || countryCode.Equals("usa", StringComparison.InvariantCultureIgnoreCase) || countryCode.Contains("united states", StringComparison.InvariantCultureIgnoreCase);
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
                return IsUS(countryCode) || countryCode.Equals("CA", StringComparison.InvariantCultureIgnoreCase) || countryCode.Contains("canada", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public static bool IsFrance(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return false;
            }
            else
            {
                return countryCode.Equals("fr", StringComparison.InvariantCultureIgnoreCase) || countryCode.Equals("france", StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
