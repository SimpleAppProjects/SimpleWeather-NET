using Microsoft.Toolkit.Parsers.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SimpleWeather.Utils
{
    public static class StringValueAttributeUtils
    {
        /// <summary>
        /// Converts between enumeration value and string value.
        /// </summary>
        /// <returns>Returns string value.</returns>
        public static string GetStringValue(this Enum @enum)
        {
            return GetEnumStringValue(@enum);
        }

        /// <summary>
        /// Converts between enumeration value and string value.
        /// </summary>
        /// <param name="@enum">Enumeration.</param>
        /// <returns>Returns string value.</returns>
        public static string GetEnumStringValue(Enum @enum)
        {
            string value = null;
            Type type = @enum.GetType();

            var fi = type.GetRuntimeField(@enum.ToString());
            var attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
            if (attrs?.Length > 0)
            {
                value = attrs[0].Value;
            }

            return value;
        }
    }
}
