using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static partial class DevSettingsEnabler
    {
        private const string KEY_DEVSETTINGSENABLED = "key_devsettingsenabled";

        public static bool DevSettingsEnabled
        {
            get => IsDevSettingsEnabled();
            set
            {
                SetDevSettingsEnabled(value);
                OnDevSettingsChanged?.Invoke(null, new DevSettingsEventArgs() { NewValue = value });
            }
        }

        internal static partial IDictionary<string, object> GetPreferenceMap();

        internal static partial void ClearPreferences(bool? enable = null);

        public static event EventHandler<DevSettingsEventArgs> OnDevSettingsChanged;
    }

    public class DevSettingsEventArgs : EventArgs
    {
        public bool NewValue { get; internal set; }
    }
}
