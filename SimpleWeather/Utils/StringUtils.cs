using System;
using System.Globalization;
using System.Text;
#if WINDOWS_UWP
using SimpleWeather.UWP.Helpers;
#endif

namespace SimpleWeather.Utils
{
    public static class StringUtils
    {
        public static String ToUpperCase(this String @string)
        {
#if WINDOWS_UWP
            return @string.ToTitleCase();
#else
            var culture = CultureInfo.CurrentCulture;
            return culture.TextInfo.ToTitleCase(@string);
#endif
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
    }
}
