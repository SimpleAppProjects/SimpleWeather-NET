#if WINDOWS_UWP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;

namespace SimpleWeather.Utils
{
    public static partial class FeatureSettings
    {
        // Shared Settings
        private static readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private static readonly ApplicationDataContainer featureSettings =
            localSettings.CreateContainer("features", ApplicationDataCreateDisposition.Always);

        private static bool GetUpdateAvailable()
        {
            if (featureSettings.Values.TryGetValue(KEY_UPDATEAVAILABLE, out object value))
            {
                return (bool)value;
            }

            return false;
        }

        private static void SetUpdateAvailable(bool value)
        {
            featureSettings.Values[KEY_UPDATEAVAILABLE] = value;
        }
    }
}
#endif