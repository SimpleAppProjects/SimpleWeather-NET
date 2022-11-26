using System;
using System.Linq;

namespace SimpleWeather.Utils
{
    public static class NumberUtils
    {
        public static int? TryParseInt(this string number)
        {
            return int.TryParse(number, out int result) ? result : null;
        }

        public static int TryParseInt(this string number, int defaultValue)
        {
            return int.TryParse(number, out int result) ? result : defaultValue;
        }

        public static float? TryParseFloat(this string number)
        {
            return float.TryParse(number, out float result) ? result : null;
        }

        public static float TryParseFloat(this string number, float defaultValue)
        {
            return float.TryParse(number, out float result) ? result : defaultValue;
        }

        public static double? TryParseDouble(this string number)
        {
            return double.TryParse(number, out double result) ? result : null;
        }

        public static double TryParseDouble(this string number, double defaultValue)
        {
            return double.TryParse(number, out double result) ? result : defaultValue;
        }

        /* Number extensions */
        public static int RoundToInt(this float @num)
        {
#if NETSTANDARD2_0
            return (int)Math.Round(num);
#else
            return (int)MathF.Round(num);
#endif
        }

        public static float Times(this float @num, float value)
        {
            return num * value;
        }

        public static float Div(this float @num, float value)
        {
            return num / value;
        }

        public static float? MaxOf(params float?[] other)
        {
            return Enumerable.Max(other.AsEnumerable());
        }

        public static float MaxOf(params float[] other)
        {
            return Enumerable.Max(other.AsEnumerable());
        }

        public static int? MaxOf(params int?[] other)
        {
            return Enumerable.Max(other.AsEnumerable());
        }

        public static int MaxOf(params int[] other)
        {
            return Enumerable.Max(other.AsEnumerable());
        }
    }
}
