using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Windows.System.UserProfile;

namespace SimpleWeather.Utils
{
    public static class CultureUtils
    {
        public static CultureInfo UserCulture
        {
            get
            {
                var userlang = GlobalizationPreferences.Languages[0];
                var culture = new CultureInfo(userlang);
                return culture;
            }
        }
    }
}
