using Microsoft.UI.Xaml.Controls;
using SimpleWeather.Utils;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.NET.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DevSettingsPage : Page
    {
        public DevSettingsPage()
        {
            this.InitializeComponent();

#if DEBUG
            // This should always be true in debug mode
            DebugSwitch.IsOn = Logger.IsDebugLoggerEnabled();
            DebugSwitch.IsEnabled = false;
#else
            DebugSwitch.IsOn = Logger.IsDebugLoggerEnabled();
            DebugSwitch.IsEnabled = true;
#endif
            DebugSwitch.Toggled += DebugSwitch_Toggled;
        }

        private void DebugSwitch_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var sw = sender as ToggleSwitch;

            Logger.EnableDebugLogger(sw.IsOn);
        }
    }
}
