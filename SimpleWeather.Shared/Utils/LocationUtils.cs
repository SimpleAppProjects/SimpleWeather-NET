using System;

namespace SimpleWeather.Utils
{
    public static class LocationUtils
    {
        // Source: https://gist.github.com/graydon/11198540
        private static readonly BoundingBox US_BOUNDING_BOX = new(24.9493, 49.5904, -125.0011, -66.9326);
        private static readonly BoundingBox USCA_BOUNDING_BOX = new(24.4825578966, 71.7611572494, -168.9184947286, -52.2436900411);
        private static readonly BoundingBox FR_BOUNDING_BOX = new(41.2632185, 51.268318, -5.4534286, 9.8678344);

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

        public static bool IsUS(LocationData.LocationData location)
        {
            if (!string.IsNullOrWhiteSpace(location.country_code))
            {
                return IsUS(location.country_code);
            }
            else
            {
                return US_BOUNDING_BOX.Intersects(location.latitude, location.longitude);
            }
        }

        public static bool IsUS(LocationData.LocationQuery location)
        {
            if (!string.IsNullOrWhiteSpace(location.LocationCountry))
            {
                return IsUS(location.LocationCountry);
            }
            else
            {
                return US_BOUNDING_BOX.Intersects(location.LocationLat, location.LocationLong);
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

        public static bool IsUSorCanada(LocationData.LocationData location)
        {
            if (!string.IsNullOrWhiteSpace(location.country_code))
            {
                return IsUSorCanada(location.country_code);
            }
            else
            {
                return USCA_BOUNDING_BOX.Intersects(location.latitude, location.longitude);
            }
        }

        public static bool IsUSorCanada(LocationData.LocationQuery location)
        {
            if (!string.IsNullOrWhiteSpace(location.LocationCountry))
            {
                return IsUSorCanada(location.LocationCountry);
            }
            else
            {
                return USCA_BOUNDING_BOX.Intersects(location.LocationLat, location.LocationLong);
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

        public static bool IsFrance(LocationData.LocationData location)
        {
            if (!string.IsNullOrWhiteSpace(location.country_code))
            {
                return IsFrance(location.country_code);
            }
            else
            {
                return FR_BOUNDING_BOX.Intersects(location.latitude, location.longitude);
            }
        }

        public static bool IsFrance(LocationData.LocationQuery location)
        {
            if (!string.IsNullOrWhiteSpace(location.LocationCountry))
            {
                return IsFrance(location.LocationCountry);
            }
            else
            {
                return FR_BOUNDING_BOX.Intersects(location.LocationLat, location.LocationLong);
            }
        }

        private record BoundingBox(
            double lat_min,
            double lat_max,
            double lon_min,
            double lon_max)
        {
            public bool Intersects(double lat, double lon)
            {
                return (lat >= lat_min && lat <= lat_max) && (lon >= lon_min && lon <= lon_max);
            }
        }
    }
}
