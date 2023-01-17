#if __ANDROID__
using Android.Preferences;
using System.Collections.Generic;

namespace SimpleWeather.Preferences
{
    public partial class SettingsContainer
    {
        public partial IDictionary<string, object> GetAllSettings()
        {
            var context = Android.App.Application.Context;

            if (string.IsNullOrWhiteSpace(SharedName))
            {
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CA1422 // Validate platform compatibility
                return PreferenceManager.GetDefaultSharedPreferences(context).All;
#pragma warning restore CA1422 // Validate platform compatibility
#pragma warning restore CA1416 // Validate platform compatibility
            }
            else
            {
                return context.GetSharedPreferences(SharedName, Android.Content.FileCreationMode.Private).All;
            }
        }

        private partial int GetPreferenceCount()
        {
            return GetAllSettings().Keys.Count;
        }
    }
}
#endif