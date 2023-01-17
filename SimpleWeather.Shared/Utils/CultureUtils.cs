using System.Globalization;
#if WINDOWS
using Windows.System.UserProfile;
#endif

namespace SimpleWeather.Utils
{
    public static class CultureUtils
    {
        public static CultureInfo UserCulture
        {
            get
            {
#if WINDOWS
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
