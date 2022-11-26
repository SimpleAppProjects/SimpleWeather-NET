using SimpleWeather.Helpers;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleWeather.Utils
{
    public static partial class StringUtils
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

        public static String RemoveDigitChars(this String @string)
        {
            if (String.IsNullOrWhiteSpace(@string))
                return "";
            else
                return Regex.Replace(@string, "[0-9]", "").Trim();
        }

        public static bool ContainsDigits(this String @string)
        {
            if (String.IsNullOrWhiteSpace(@string))
                return false;
            else
                return Regex.IsMatch(@string, ".*[0-9].*");
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

        public static String SubstringByIndex(this String @string, int startIndex, int endIndex)
        {
            int subLen = endIndex - startIndex;
            return new string(@string.ToCharArray(), startIndex, subLen);
        }

        public static String UnescapeUnicode(this String @string)
        {
            if (String.IsNullOrWhiteSpace(@string))
            {
                return @string;
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                int seqEnd = @string.Length;
                for (int i = 0; i < @string.Length; i++)
                {
                    // Uses -2 to ensure there is something after the &#
                    char c = @string[i];
                    if (@string[i] == '&' && i < seqEnd - 2 && @string[i + 1] == '#')
                    {
                        int start = i + 2;
                        bool isHex = false;

                        char firstChar = @string[start];
                        if (firstChar == 'x' || firstChar == 'X')
                        {
                            start++;
                            isHex = true;

                            if (start == seqEnd)
                            {
                                sb.Append(@string.Substring(i));
                                break;
                            }
                        }

                        int end = start;
                        while (end < seqEnd && @string[end] != ';')
                        {
                            end++;
                        }

                        int value;
                        try
                        {
                            var substr = @string.SubstringByIndex(start, end);

                            if (isHex)
                            {
                                value = Convert.ToInt32(substr, 16);
                            }
                            else
                            {
                                value = Convert.ToInt32(substr, 10);
                            }
                        }
                        catch (FormatException)
                        {
                            sb.Append(@string.Substring(i));
                            break;
                        }

                        var chars = Convert.ToChar(value, CultureInfo.InvariantCulture);
                        sb.Append(chars);

                        i = end;
                    }
                    else
                    {
                        sb.Append(@string[i]);
                    }
                }

                return sb.ToString();
            }
        }

        public static string EscapeUnicode(this string @string)
        {
            // https://stackoverflow.com/a/48706264
            return Native2Ascii(@string);
        }

        /// <summary>
        /// Encode a String like äöü to \u00e4\u00f6\u00fc
        /// </summary>
        /// <param name="string">Input string</param>
        /// <returns></returns>
        private static string Native2Ascii(string @string)
        {
            var sb = new StringBuilder();
            foreach (var ch in @string)
            {
                sb.Append(Native2Ascii(ch));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Encode a Character like ä to \u00e4
        /// </summary>
        /// <param name="ch">Input character</param>
        /// <returns></returns>
        private static string Native2Ascii(char ch)
        {
            if (ch > '\u007f')
            {
                var hex = ((int)ch).ToString("X4", CultureInfo.InvariantCulture);
                return $"\\u{hex}";
            }
            else
            {
                return ch.ToString();
            }
        }

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static bool Matches(this string text, string pattern)
        {
            return Regex.IsMatch(text, pattern, RegexOptions.CultureInvariant);
        }
    }
}