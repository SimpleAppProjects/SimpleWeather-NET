using System.Globalization;
using Windows.System.UserProfile;

namespace SimpleWeather.Utils
{
    public static class CultureUtils
    {
        public static CultureInfo UserCulture
        {
            get
            {
#if WINDOWS_UWP
                var userlang = GlobalizationPreferences.Languages[0];
                var culture = new CultureInfo(userlang);
                return culture;
#else
                return CultureInfo.CurrentUICulture;
#endif
            }
        }
    }
}
