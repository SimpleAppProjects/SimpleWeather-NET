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

        internal static partial IDictionary<string, object> GetPreferenceMap()
        {
            return devSettings.Values.WhereNot(kvp => Equals(kvp.Key, KEY_DEVSETTINGSENABLED))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        internal static partial void ClearPreferences(bool? enable = null)
        {
            var enabled = enable ?? IsDevSettingsEnabled();

            devSettings.Values.Clear();

            SetDevSettingsEnabled(enabled);
        }
    }
}
