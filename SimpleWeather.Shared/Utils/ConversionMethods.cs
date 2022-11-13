using System;

namespace SimpleWeather.Utils
{
    public static class ConversionMethods
    {
        // Constants
        private const float KM_TO_MI = 0.621371192f;
        private const float MI_TO_KM = 1.609344f;
        private const float INHG_TO_MB = 1013.25f / 29.92f;
        private const float MB_TO_INHG = 29.92f / 1013.25f;
        private const float MSEC_TO_MPH = 2.23694f;
        private const float MSEC_TO_KPH = 3.6f;
        private const float MM_TO_IN = 1f / 25.4f;
        private const float IN_TO_MM = 25.4f;
        private const float PA_TO_INHG = 0.0002952998751f;
        private const float PA_TO_MB = 0.01f;

        public static float MBToInHg(float input)
        {
            return MB_TO_INHG * input;
        }

        public static float InHgToMB(float input)
        {
            return INHG_TO_MB * input;
        }

        public static float PaToInHg(float input)
        {
            return PA_TO_INHG * input;
        }

        public static float PaToMB(float input)
        {
            return PA_TO_MB * input;
        }

        public static float KmToMi(float input)
        {
            return KM_TO_MI * input;
        }

        public static float MiToKm(float input)
        {
            return MI_TO_KM * input;
        }

        public static float MMToIn(float input)
        {
            return MM_TO_IN * input;
        }

        public static float InToMM(float input)
        {
            return IN_TO_MM * input;
        }

        public static float MphToKph(float input)
        {
            return MI_TO_KM * input;
        }

        public static float KphToMph(float input)
        {
            return KM_TO_MI * input;
        }

        public static float MSecToMph(float input)
        {
            return input * MSEC_TO_MPH;
        }

        public static float MSecToKph(float input)
        {
            return input * MSEC_TO_KPH;
        }

        public static float KphToMSec(float input)
        {
            return input / MSEC_TO_KPH;
        }

        public static float FtoC(float input)
        {
            return (input - 32) * (5f / 9);
        }

        public static float CtoF(float input)
        {
            return (input * (9f / 5)) + 32;
        }

        public static float KtoC(float input)
        {
            return input - 273.15f;
        }

        public static float KtoF(float input)
        {
            return (input * (9f / 5)) - 459.67f;
        }

        public static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static float ToRadians(float angle)
        {
            return (float)ToRadians((double)angle);
        }

        public static double ToDegrees(double angle)
        {
            return 180.0 * angle / Math.PI;
        }

        public static float ToDegrees(float angle)
        {
            return (float)ToDegrees((double)angle);
        }

        public static double CalculateHaversine(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6372.8; // In kilometers
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Asin(Math.Sqrt(a));

            /* Output in Meters */
            return (R * c) * 1000;
        }

        public static double CalculateGeopositionDistance(LocationData.Location position1, LocationData.Location position2)
        {
            if (position1 is null)
            {
                throw new ArgumentNullException(nameof(position1));
            }

            if (position2 is null)
            {
                throw new ArgumentNullException(nameof(position2));
            }

            double lat1 = position1.Latitude;
            double lon1 = position1.Longitude;
            double lat2 = position2.Latitude;
            double lon2 = position2.Longitude;

            /* Returns value in meters */
            return Math.Abs(CalculateHaversine(lat1, lon1, lat2, lon2));
        }
    }
}