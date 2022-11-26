// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace SimpleWeather.Utils
{
    public static partial class StringUtils
    {

#if NETSTANDARD2_0
        public static bool Contains(this string text, string value, StringComparison comparison)
        {
            return text.IndexOf(value, comparison) >= 0;
        }

        public static bool EndsWith(this string text, char value)
        {
            int lastPos = text.Length - 1;
            return ((uint)lastPos < (uint)text.Length) && text[lastPos] == value;
        }
#endif
    }
}
