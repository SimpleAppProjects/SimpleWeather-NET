using System;

namespace SimpleWeather.Utils
{
    public static class ConversionMethods
    {
        private const double KM_TO_MI = 0.621371192;
        private const double MI_TO_KM = 1.609344;

        public static string mbToInHg(String input)
        {
            double result = 29.92 * double.Parse(input) / 1013.25;
            return Math.Round(result, 2).ToString();
        }

        public static string kmToMi(String input)
        {
            double result = KM_TO_MI * double.Parse(input);
            return Math.Round(result).ToString();
        }

        public static string miToKm(String input)
        {
            double result = MI_TO_KM * double.Parse(input);
            return Math.Round(result).ToString();
        }

        public static string mphTokph(String input)
        {
            double result = MI_TO_KM * double.Parse(input);
            return Math.Round(result).ToString();
        }

        public static string kphTomph(String input)
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
    }
}
