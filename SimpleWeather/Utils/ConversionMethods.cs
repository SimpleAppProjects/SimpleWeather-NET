using System;

namespace SimpleWeather.Utils
{
    public static class ConversionMethods
    {
        // Constants
        private const double KM_TO_MI = 0.621371192;
        private const double MI_TO_KM = 1.609344;
        private const double INHG_TO_MB = 1013.25 / 29.92;
        private const double MB_TO_INHG = 29.92 / 1013.25;

        public static string MBToInHg(String input)
        {
            double result = MB_TO_INHG * double.Parse(input);
            return Math.Round(result, 2).ToString();
        }

        public static string InHgToMB(String input)
        {
            double result = INHG_TO_MB * double.Parse(input);
            return Math.Round(result, 2).ToString();
        }

        public static string KmToMi(String input)
        {
            double result = KM_TO_MI * double.Parse(input);
            return Math.Round(result).ToString();
        }

        public static string MiToKm(String input)
        {
            double result = MI_TO_KM * double.Parse(input);
            return Math.Round(result).ToString();
        }

        public static string MphToKph(String input)
        {
            double result = MI_TO_KM * double.Parse(input);
            return Math.Round(result).ToString();
        }

        public static string KphToMph(String input)
        {
            double result = KM_TO_MI * double.Parse(input);
            return Math.Round(result).ToString();
        }

        public static string FtoC(String input)
        {
            double result = (double.Parse(input) - 32) * ((double)5 / 9);
            return Math.Round(result).ToString();
        }

        public static string CtoF(String input)
        {
            double result = (double.Parse(input) * ((double)9 /5)) + 32;
            return Math.Round(result).ToString();
        }

        public static DateTime ToEpochDateTime(string epoch_time)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(long.Parse(epoch_time));
        }

        public static double toRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double CalculateHaversine(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6372.8; // In kilometers
            var dLat = toRadians(lat2 - lat1);
            var dLon = toRadians(lon2 - lon1);
            lat1 = toRadians(lat1);
            lat2 = toRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Asin(Math.Sqrt(a));

            /* Output in Meters */
            return (R * c) * 1000;
        }

#if __ANDROID__
        public static double CalculateGeopositionDistance(Android.Locations.Location position1, Android.Locations.Location position2)
        {
            double lat1 = position1.Latitude;
            double lon1 = position1.Longitude;
            double lat2 = position2.Latitude;
            double lon2 = position2.Longitude;

            /* Returns value in meters */
            return Math.Abs(CalculateHaversine(lat1, lon1, lat2, lon2));
        }
#elif WINDOWS_UWP
        public static double CalculateGeopositionDistance(Windows.Devices.Geolocation.Geoposition position1, Windows.Devices.Geolocation.Geoposition position2)
        {
            double lat1 = position1.Coordinate.Point.Position.Latitude;
            double lon1 = position1.Coordinate.Point.Position.Longitude;
            double lat2 = position2.Coordinate.Point.Position.Latitude;
            double lon2 = position2.Coordinate.Point.Position.Longitude;

            /* Returns value in meters */
            return Math.Abs(CalculateHaversine(lat1, lon1, lat2, lon2));
        }
#endif
    }
}
