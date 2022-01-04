using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static class NumberUtils
    {
        public static int? TryParseInt(string number)
        {
            return int.TryParse(number, out int result) ? result : null;
        }

        public static int TryParseInt(string number, int defaultValue)
        {
            return int.TryParse(number, out int result) ? result : defaultValue;
        }

        public static float? TryParseFloat(string number)
        {
            return float.TryParse(number, out float result) ? result : null;
        }

        public static float TryParseFloat(string number, float defaultValue)
        {
            return float.TryParse(number, out float result) ? result : defaultValue;
        }

        public static double? TryParseDouble(string number)
        {
            return double.TryParse(number, out double result) ? result : null;
        }

        public static double TryParseDouble(string number, double defaultValue)
        {
            return double.TryParse(number, out double result) ? result : defaultValue;
        }

        /* Number extensions */
        public static int RoundToInt(this float @num)
        {
            return (int)MathF.Round(num);
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
