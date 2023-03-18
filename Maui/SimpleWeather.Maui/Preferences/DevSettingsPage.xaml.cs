namespace SimpleWeather.Maui.Preferences;

public partial class DevSettingsPage : ContentPage
{
	public DevSettingsPage()
	{
		InitializeComponent();

        UpdateSettingsTableTheme();
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