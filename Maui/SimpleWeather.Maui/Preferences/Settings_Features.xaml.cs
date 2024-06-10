using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Preferences;

public partial class Settings_Features : ContentPage
{
	public Settings_Features()
	{
		InitializeComponent();

        App.Current.Resources.TryGetValue("LightPrimary", out var LightPrimary);
        App.Current.Resources.TryGetValue("DarkPrimary", out var DarkPrimary);

        SettingsTable.UpdateCellColors(
            Colors.Black, Colors.White, Color.Parse("#767676"), Color.Parse("#a2a2a2"),
            LightPrimary as Color, DarkPrimary as Color);

        SettingsTable.Root
            .SelectMany(s => s)
            .Where(c => c is CheckBoxCell)
            .ForEach(c =>
            {
                if (c is CheckBoxCell chk)
                {
                    chk.IsCompact = true;
                }
            });
    }
}