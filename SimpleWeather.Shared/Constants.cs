#if WINUI
using Windows.ApplicationModel;
#else
using Microsoft.Maui.ApplicationModel;
#endif

namespace SimpleWeather
{
    public static class Constants
    {
        public const string KEY_DATA = "data";
        public const string KEY_ACTION = "action";

        public const string KEY_GPS = "GPS";
        public const string KEY_SEARCH = "Search";

        public const string WIDGETKEY_OLDKEY = "oldKey";
        public const string WIDGETKEY_LOCATION = "location";
        public const string WIDGETKEY_LOCATIONQUERY = "locationQuery";
        public const string WIDGETKEY_WEATHER = "weather";

        // Mapsui
        public const string TILE_CACHE_DIR = "TileCache";

        public const string SUPPORT_EMAIL_ADDRESS
#if WINUI
            = "thewizrd.dev+SimpleWeatherWindows@gmail.com";
#else
            = "thewizrd.dev+SimpleWeatherMaui@gmail.com";
#endif

        public static string GetUserAgentString()
        {
#if WINUI
            var version = string.Format("v{0}.{1}.{2}",
                Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);
#else
            var version = string.Format("v{0}.{1}.{2}",
                AppInfo.Version.Major, AppInfo.Version.Minor, AppInfo.Version.Build);
#endif

            return $"SimpleWeather/{version} (thewizrd.dev@gmail.com)";
        }
    }
}
