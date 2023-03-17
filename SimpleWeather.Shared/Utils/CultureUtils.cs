using System.Globalization;
#if WINDOWS
using Windows.System.UserProfile;
#endif

namespace SimpleWeather.Utils
{
    public static class CultureUtils
    {
        public static string GetNativeDisplayName(this CultureInfo culture, CultureInfo displayCulture = null)
        {
#if __IOS__
            var locale = Foundation.NSLocale.FromLocaleIdentifier((displayCulture ?? LocaleUtils.GetLocale()).Name);
            var str = locale.GetIdentifierDisplayName(culture.Name);
            return str;
#elif __ANDROID__
            return Java.Util.Locale.ForLanguageTag(culture.Name).DisplayName;
#else
            return culture.DisplayName;
#endif
        }
    }
}
