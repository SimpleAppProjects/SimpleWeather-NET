#if __IOS__ && !__MACCATALYST__
using Foundation;
using System.Linq;

namespace SimpleWeather.Preferences
{
    public partial class SettingsContainer
    {
        private partial int GetPreferenceCount()
        {
            return GetUserDefaults().ToDictionary().Keys.Count(i => i is NSString);
        }

        private NSUserDefaults GetUserDefaults()
        {
            if (!string.IsNullOrWhiteSpace(SharedName))
                return new NSUserDefaults(SharedName, NSUserDefaultsType.SuiteName);
            else
                return NSUserDefaults.StandardUserDefaults;
        }
    }
}
#endif