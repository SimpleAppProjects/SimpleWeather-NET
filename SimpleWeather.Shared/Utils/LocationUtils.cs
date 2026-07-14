using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SimpleWeather.Utils
{
    public static class LocationUtils
    {
        // xMin -> lon_min, xMax -> lon_max; yMin -> lat_min, yMax -> lat_max
        // xMax -> East, xMin -> West, yMax -> North, yMin -> South
        private static readonly BoundingBox US_BOUNDING_BOX = new(24.9493, 49.5904, -125.0011, -66.9326);

        // Canada
        private static readonly BoundingBox CA_BOUNDING_BOX = new(41.6765556, 83.3362128, -141.00275, -52.3231981);

        // France
        private static readonly BoundingBox FR_BOUNDING_BOX = new(41.2632185, 51.268318, -5.4534286, 9.8678344);

        // Puerto Rico
        private static readonly BoundingBox PR_BOUNDING_BOX = new(17.7306659963, 18.6663824908, -68.1108798087, -65.1100910828);

        // US Virgin Islands
        private static readonly BoundingBox VI_BOUNDING_BOX = new(17.6234681162, 18.4649841585, -65.1541175321, -64.512674287);

        // Guam & The Northern Mariana Islands
        private static readonly BoundingBox GU_MP_BOUNDING_BOX = new(13.019485113, 20.7560506513, 144.3987098565, 146.3240638604);

        // American Somoa
        private static readonly BoundingBox AS_BOUNDING_BOX = new(-14.6018129466, -10.9972026743, -171.141907163, -168.1016121805);

        // Germany
        private static readonly BoundingBox DE_BOUNDING_BOX = new(47.2701114, 55.099161, 5.8663153, 15.0419319);

        private static readonly BoundingBox CAMS_OPENMETEO_EUROPE_BBOX = new(30.0, 72.0, -25.0, 45.0);

        // United Kingdom and Ireland
        private static readonly BoundingBox UK_IE_BOUNDING_BOX = new(49.674, 61.061, -14.015517, 2.0919117);

        // Korea
        private static readonly BoundingBox KOREA_BOUNDING_BOX = new(32.9104556, 43.0078553939, 124.3017732603, 131.0789644224);

        // Japan
        private static readonly BoundingBox JP_BOUNDING_BOX = new(20.2145811, 45.7563343, 122.7141754, 154.205541);

        // Switzerland
        private static readonly BoundingBox CH_BOUNDING_BOX = new(45.8179579, 47.8084544, 5.9558318, 10.4922941);

        // Met Norway Area
        private static readonly BoundingBox METNO_BOUNDING_BOX = new(54.4504920972, 81.0280176, -9.6846279, 34.6889114);

        // Australia
        private static readonly BoundingBox AU_BOUNDING_BOX = new(-55.3228175, -9.0880125, 72.2461932, 168.2261259);

        // China
        private static readonly BoundingBox CN_BOUNDING_BOX = new(17.917694912, 53.5608154, 73.4997347, 134.7751959);

        // Netherlands
        private static readonly BoundingBox NL_BOUNDING_BOX = new(50.7218810189, 53.7801536938, 3.3388847969, 7.590283575);

        // Denmark
        private static readonly BoundingBox DK_BOUNDING_BOX = new(54.4516667, 57.9524297, 7.7153255, 15.5530641);

        // Italy
        private static readonly BoundingBox IT_BOUNDING_BOX = new(35.2889616, 47.0921485, 6.6272658, 18.7844746);

        // Austria
        private static readonly BoundingBox AT_BOUNDING_BOX = new(46.3722987, 49.0205239, 9.5307487, 17.1607728);

        private static readonly ISet<string> NWS_SUPPORTED_COUNTRIES = ImmutableHashSet.Create("US", "AS", "UM", "GU", "MP", "PR", "VI");
        private static readonly IList<BoundingBox> NWS_SUPPORTED_LOCATIONS = [US_BOUNDING_BOX, PR_BOUNDING_BOX, VI_BOUNDING_BOX, GU_MP_BOUNDING_BOX, AS_BOUNDING_BOX];

        private static readonly ISet<string> OPENMETEO_SUPPORTED_COUNTRIES = ImmutableHashSet.Create(
            "US", "AS", "UM", "GU", "MP", "PR", "VI", // NWS
            "UK", "IE", // UK and Ireland - UK Met Office
            "FR", // France - MeteoFrance
            "KR", "KP", // Korea - KMA Korea
            "JP", // JMA Japan
            "CH", // Switzerland - MeteoSwiss
            "NO", "DK", "SE", "FI", // Norway, Denmark, Sweden, Finland - Met Norway
            "CA", // Canada - GEM Canada
            "AU", // Australia - BOM Australia
            "CN", // China - CMA China
            "NL", // Netherlands - KNMI Netherlands
            "DK", // Denmark - DMI Denmark
            "IT", // Italy - ItaliaMeteo
            "AT"  // Austria - GeoSphere Austria

        );

        private static readonly IList<BoundingBox> OPENMETEO_SUPPORTED_LOCATIONS = [
            // NWS
            US_BOUNDING_BOX,
            PR_BOUNDING_BOX,
            VI_BOUNDING_BOX,
            GU_MP_BOUNDING_BOX,
            AS_BOUNDING_BOX,
            // UK and Ireland
            UK_IE_BOUNDING_BOX,
            // France
            FR_BOUNDING_BOX,
            // Korea
            KOREA_BOUNDING_BOX,
            // Japan
            JP_BOUNDING_BOX,
            // Switzerland
            CH_BOUNDING_BOX,
            // MetNo
            METNO_BOUNDING_BOX,
            // Canada
            CA_BOUNDING_BOX,
            // Australia
            AU_BOUNDING_BOX,
            // China
            CN_BOUNDING_BOX,
            // Netherlands
            NL_BOUNDING_BOX,
            // Denmark
            DK_BOUNDING_BOX,
            // Italy
            IT_BOUNDING_BOX,
            // Austria,
            AT_BOUNDING_BOX,
        ];

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

        private static bool InUSBounds(double lat, double lon) => US_BOUNDING_BOX.Intersects(lat, lon);

        public static bool IsUS(LocationData.LocationData location)
        {
            if (!string.IsNullOrWhiteSpace(location.country_code))
            {
                return IsUS(location.country_code) || InUSBounds(location.latitude, location.longitude);
            }
            else
            {
                return InUSBounds(location.latitude, location.longitude);
            }
        }

        public static bool IsUS(LocationData.LocationQuery location)
        {
            if (!string.IsNullOrWhiteSpace(location.LocationCountry))
            {
                return IsUS(location.LocationCountry) || InUSBounds(location.LocationLat, location.LocationLong);
            }
            else
            {
                return InUSBounds(location.LocationLat, location.LocationLong);
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
                return IsUS(countryCode) || IsCanada(countryCode);
            }
        }

        private static bool InUSorCanadaBounds(double lat, double lon) => InUSBounds(lat, lon) || InCanadaBounds(lat, lon);

        public static bool IsUSorCanada(LocationData.LocationData location)
        {
            if (!string.IsNullOrWhiteSpace(location.country_code))
            {
                return IsUSorCanada(location.country_code) || InUSorCanadaBounds(location.latitude, location.longitude);
            }
            else
            {
                return InUSorCanadaBounds(location.latitude, location.longitude);
            }
        }

        public static bool IsUSorCanada(LocationData.LocationQuery location)
        {
            if (!string.IsNullOrWhiteSpace(location.LocationCountry))
            {
                return IsUSorCanada(location.LocationCountry) || InUSorCanadaBounds(location.LocationLat, location.LocationLong);
            }
            else
            {
                return InUSorCanadaBounds(location.LocationLat, location.LocationLong);
            }
        }

        public static bool IsCanada(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return false;
            }
            else
            {
                return countryCode.Equals("ca", StringComparison.InvariantCultureIgnoreCase) || countryCode.Equals("canada", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        private static bool InCanadaBounds(double lat, double lon) => CA_BOUNDING_BOX.Intersects(lat, lon);

        public static bool IsCanada(LocationData.LocationData location)
        {
            if (!string.IsNullOrWhiteSpace(location.country_code))
            {
                return IsCanada(location.country_code) || InCanadaBounds(location.latitude, location.longitude);
            }
            else
            {
                return InCanadaBounds(location.latitude, location.longitude);
            }
        }

        public static bool IsCanada(LocationData.LocationQuery location)
        {
            if (!string.IsNullOrWhiteSpace(location.LocationCountry))
            {
                return IsCanada(location.LocationCountry) || InCanadaBounds(location.LocationLat, location.LocationLong);
            }
            else
            {
                return InCanadaBounds(location.LocationLat, location.LocationLong);
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

        private static bool InFranceBounds(double lat, double lon) => FR_BOUNDING_BOX.Intersects(lat, lon);

        public static bool IsFrance(LocationData.LocationData location)
        {
            if (!string.IsNullOrWhiteSpace(location.country_code))
            {
                return IsFrance(location.country_code) || InFranceBounds(location.latitude, location.longitude);
            }
            else
            {
                return InFranceBounds(location.latitude, location.longitude);
            }
        }

        public static bool IsFrance(LocationData.LocationQuery location)
        {
            if (!string.IsNullOrWhiteSpace(location.LocationCountry))
            {
                return IsFrance(location.LocationCountry) || InFranceBounds(location.LocationLat, location.LocationLong);
            }
            else
            {
                return InFranceBounds(location.LocationLat, location.LocationLong);
            }
        }

        public static bool IsNWSSupported(string countryCode)
        {
            return NWS_SUPPORTED_COUNTRIES.Contains(countryCode?.ToUpperInvariant());
        }

        public static bool IsNWSSupported(LocationData.LocationData location)
        {
            if (!string.IsNullOrWhiteSpace(location.country_code))
            {
                return IsNWSSupported(location.country_code);
            }
            else
            {
                return NWS_SUPPORTED_LOCATIONS.Any(x => x.Intersects(location.latitude, location.longitude));
            }
        }

        public static bool IsNWSSupported(LocationData.LocationQuery location)
        {
            if (!string.IsNullOrWhiteSpace(location.LocationCountry))
            {
                return IsNWSSupported(location.LocationCountry);
            }
            else
            {
                return NWS_SUPPORTED_LOCATIONS.Any(x => x.Intersects(location.LocationLat, location.LocationLong));
            }
        }

        public static bool IsGermany(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return false;
            }
            else
            {
                return countryCode.Equals("de", StringComparison.InvariantCultureIgnoreCase) || countryCode.Equals("germany", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        private static bool InGermanyBounds(double lat, double lon) => DE_BOUNDING_BOX.Intersects(lat, lon);

        public static bool IsGermany(LocationData.LocationData location)
        {
            if (!string.IsNullOrWhiteSpace(location.country_code))
            {
                return IsGermany(location.country_code) || InGermanyBounds(location.latitude, location.longitude);
            }
            else
            {
                return InGermanyBounds(location.latitude, location.longitude);
            }
        }

        public static bool IsGermany(LocationData.LocationQuery location)
        {
            if (!string.IsNullOrWhiteSpace(location.LocationCountry))
            {
                return IsGermany(location.LocationCountry) || InGermanyBounds(location.LocationLat, location.LocationLong);
            }
            else
            {
                return InGermanyBounds(location.LocationLat, location.LocationLong);
            }
        }

        public static bool IsCAMSEuroCovered(LocationData.LocationData location)
        {
            return CAMS_OPENMETEO_EUROPE_BBOX.Intersects(location.latitude, location.longitude);
        }

        public static bool IsOpenMeteoSupported(string countryCode)
        {
            return OPENMETEO_SUPPORTED_COUNTRIES.Contains(countryCode?.ToUpperInvariant());
        }

        public static bool IsOpenMeteoSupported(LocationData.LocationData location)
        {
            if (!string.IsNullOrWhiteSpace(location.country_code))
            {
                return IsOpenMeteoSupported(location.country_code);
            }
            else
            {
                return OPENMETEO_SUPPORTED_LOCATIONS.Any(x => x.Intersects(location.latitude, location.longitude));
            }
        }

        public static bool IsOpenMeteoSupported(LocationData.LocationQuery location)
        {
            if (!string.IsNullOrWhiteSpace(location.LocationCountry))
            {
                return IsOpenMeteoSupported(location.LocationCountry);
            }
            else
            {
                return OPENMETEO_SUPPORTED_LOCATIONS.Any(x => x.Intersects(location.LocationLat, location.LocationLong));
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
