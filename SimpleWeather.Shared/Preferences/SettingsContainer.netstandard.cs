#if !__IOS__ && !__MACCATALYST__ && !WINDOWS && !__ANDROID__
using System;

namespace SimpleWeather.Preferences
{
    public partial class SettingsContainer
    {
        private partial int GetPreferenceCount()
        {
            throw new NotImplementedException();
        }
    }
}
#endif