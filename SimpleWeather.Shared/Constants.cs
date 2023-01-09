using System;
using Windows.ApplicationModel;

namespace SimpleWeather
{
    public static class Constants
    {
        public const String KEY_GPS = "GPS";
        public const String KEY_SEARCH = "Search";

        public const String TILEKEY_OLDKEY = "oldKey";
        public const String TILEKEY_LOCATION = "location";

        public const String TILE_CACHE_DIR = "TileCache";

        public static string GetUserAgentString()
        {
            var version = string.Format("v{0}.{1}.{2}",
                Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

            return $"SimpleWeather/{version} (thewizrd.dev@gmail.com)";
        }
    }
}
