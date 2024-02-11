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
            Colors.Black, Colors.White, Colors.DimGray, Colors.LightGray,
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