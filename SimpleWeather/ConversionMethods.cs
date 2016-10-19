using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather
{
    public static class ConversionMethods
    {
        public const double KM_TO_MI = 0.621371192;
        public const double MI_TO_KM = 1.609344;

        public static string mbToInHg(String input)
        {
            double result = 29.92 * double.Parse(input) / 1013.25;
            return result.ToString("0.00");
        }

        public static string mbToInHg(double input)
        {
            double result = 29.92 * input / 1013.25;
            return result.ToString("0.00");
        }

        public static string kmToMi(String input)
        {
            double result = KM_TO_MI * double.Parse(input);
            return Math.Round(result, 2).ToString();
        }

        public static string kmToMi(double input)
        {
            double result = KM_TO_MI * input;
            return Math.Round(result, 2).ToString();
        }

        public static string miToKm(String input)
        {
            double result = MI_TO_KM * double.Parse(input);
            return Math.Round(result, 2).ToString();
        }

        public static string miToKm(double input)
        {
            double result = MI_TO_KM * input;
            return Math.Round(result, 2).ToString();
        }

        public static string mphTokph(String input)
        {
            double result = MI_TO_KM * double.Parse(input);
            return Math.Round(result).ToString();
        }

        public static string mphTokph(double input)
        {
            double result = MI_TO_KM * input;
            return Math.Round(result).ToString();
        }

        public static string kphTomph(String input)
        {
            double result = KM_TO_MI * double.Parse(input);
            return Math.Round(result).ToString();
        }

        public static string kphTomph(double input)
        {
            double result = KM_TO_MI * input;
            return Math.Round(result).ToString();
        }
    }
}
