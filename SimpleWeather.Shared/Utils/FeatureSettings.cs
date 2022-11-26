using SimpleWeather.Preferences;

namespace SimpleWeather.Utils
{
    public static partial class FeatureSettings
    {
        private const string KEY_UPDATEAVAILABLE = "key_updateavailable";

        // Shared Settings
        private static readonly SettingsContainer featureSettings = new SettingsContainer("features");

        public static bool IsUpdateAvailable { get { return GetUpdateAvailable(); } set { SetUpdateAvailable(value); } }

        private static bool GetUpdateAvailable()
        {
            return featureSettings.GetValue<bool>(KEY_UPDATEAVAILABLE, false);
        }

        private static void SetUpdateAvailable(bool value)
        {
            featureSettings.SetValue(KEY_UPDATEAVAILABLE, value);
        }
    }
}