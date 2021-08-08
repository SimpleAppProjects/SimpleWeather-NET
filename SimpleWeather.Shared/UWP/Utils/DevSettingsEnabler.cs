using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.Utils
{
    public static partial class DevSettingsEnabler
    {
        // Shared Settings
        private static readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private static readonly ApplicationDataContainer devSettings =
            localSettings.CreateContainer("devsettings", ApplicationDataCreateDisposition.Always);

        private static bool IsDevSettingsEnabled()
        {
            return devSettings.Values[KEY_DEVSETTINGSENABLED] as bool? ?? false;
        }

        private static void SetDevSettingsEnabled(bool value)
        {
            devSettings.Values[KEY_DEVSETTINGSENABLED] = value;
        }

        public static partial string GetAPIKey(string key)
        {
            return devSettings.Values[key] as string;
        }

        public static partial void SetAPIKey(string key, string value)
        {
            devSettings.Values[key] = value;
        }
    }
}
