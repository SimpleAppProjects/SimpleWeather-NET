using SimpleWeather.UWP.Helpers;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleWeather.Utils
{
    public static class StringUtils
    {
        public static String ToUpperCase(this String @string)
        {
            return @string.ToTitleCase();
        }

        public static String ToPascalCase(this String @string)
        {
            String[] strArray = @string.Split('.');
            StringBuilder sb = new StringBuilder();

            foreach (String str in strArray)
            {
                if (str.Length == 0)
                    continue;

                sb.Append(str.Trim().Substring(0, 1).ToUpper())
                  .Append(str.Trim().Substring(1).ToLower())
                  .Append(". ");
            }

            return sb.ToString().TrimEnd(' ');
        }

        public static String RemoveNonDigitChars(this String @string)
        {
            if (String.IsNullOrWhiteSpace(@string))
                return "";
            else
                return Regex.Replace(@string, "[^\\d.-]", "").Trim();
        }

        public static String Ellipsize(this String @string, int maxLength)
        {
            if (@string.Length > maxLength)
            {
                var substr = @string.Substring(0, maxLength - 1);
                return substr + "\u2026";
            }

            return @string;
        }

        public static String ToInvariantString(this float @num)
        {
            return num.ToString(CultureInfo.InvariantCulture);
        }

        public static String ToInvariantString(this double @num)
        {
            return num.ToString(CultureInfo.InvariantCulture);
        }

        public static String ToInvariantString(this int @num)
        {
            return num.ToString(CultureInfo.InvariantCulture);
        }

        public static String ToInvariantString(this long @num)
        {
            return num.ToString(CultureInfo.InvariantCulture);
        }

        public static String ToInvariantString(this float @num, String format)
        {
            return num.ToString(format, CultureInfo.InvariantCulture);
        }

        public static String ToInvariantString(this double @num, String format)
        {
            return num.ToString(format, CultureInfo.InvariantCulture);
        }

        public static String ToInvariantString(this int @num, String format)
        {
            return num.ToString(format, CultureInfo.InvariantCulture);
        }

        public static String ToInvariantString(this long @num, String format)
        {
            return num.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}