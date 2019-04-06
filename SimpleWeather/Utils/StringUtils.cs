using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SimpleWeather.UWP.Helpers;

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

            foreach(String str in strArray)
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
    }
}
