using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Preferences;

public partial class DevSettingsPage : ContentPage
{
    public DevSettingsPage()
    {
        InitializeComponent();

        UpdateSettingsTableTheme();

#if DEBUG
        // This should always be true in debug mode
        DebugSwitch.On = Logger.IsDebugLoggerEnabled();
        DebugSwitch.IsEnabled = false;
#else
        DebugSwitch.On = Logger.IsDebugLoggerEnabled();
        DebugSwitch.IsEnabled = true;
#endif

        DebugSwitch.OnChanged += DebugSwitch_OnChanged;
    }

    private void DebugSwitch_OnChanged(object sender, ToggledEventArgs e)
    {
        Logger.EnableDebugLogger(e.Value);
    }

    private void UpdateSettingsTableTheme()
    {
        App.Current.Resources.TryGetValue("LightPrimary", out var LightPrimary);
        App.Current.Resources.TryGetValue("DarkPrimary", out var DarkPrimary);
        SettingsTable.UpdateCellColors(
            Colors.Black, Colors.White, Colors.DimGray, Colors.LightGray,
            LightPrimary as Color, DarkPrimary as Color);
    }
}