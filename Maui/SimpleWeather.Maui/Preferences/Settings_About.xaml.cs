using SimpleWeather.Utils;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Preferences;

public partial class Settings_About : ContentPage
{
    private const string KEY_FEEDBACK = "key_feedback";
    private const string KEY_RATEREVIEW = "key_ratereview";
    private const string KEY_TRANSLATE = "key_translate";

    // TODO: Dev Settings controller

    public Settings_About()
	{
		InitializeComponent();

        App.Current.Resources.TryGetValue("LightPrimary", out var LightPrimary);
        App.Current.Resources.TryGetValue("DarkPrimary", out var DarkPrimary);
        SettingsTable.UpdateCellColors(
            Colors.Black, Colors.White, Colors.DimGray, Colors.LightGray,
            LightPrimary as Color, DarkPrimary as Color);

        // Event Listeners
        this.SettingsTable.Model.ItemSelected += Model_ItemSelected;

        // Version info
        VersionPref.Detail = $"v{AppInfo.Current.VersionString}";

        // TODO: init devcontroller
    }

    private async void Model_ItemSelected(object sender, Microsoft.Maui.Controls.Internals.EventArg<object> e)
    {
        if (e.Data is ListViewCell listPref)
        {
            await Navigation.PushAsync(new PreferenceListDialogPage(listPref));
        }
    }

    private async void TextCell_Tapped(object sender, EventArgs e)
    {
        var cell = sender as TextCell;
        if (cell.CommandParameter is Type pageType)
        {
            if (pageType == typeof(Settings_Credits))
            {
                await Navigation.PushAsync(new Settings_Credits());
            }
            else if (pageType == typeof(Settings_OSSLibs))
            {
                await Navigation.PushAsync(new Settings_OSSLibs());
            }
        }
        else if (cell is DialogCell dialogCell)
        {
            switch (dialogCell.PreferenceKey)
            {
                case KEY_FEEDBACK:
                    {
                        try
                        {
                            if (Email.Default.IsComposeSupported)
                            {
                                await Email.Default.ComposeAsync(
                                    subject: $"{ResStrings.pref_title_feedback} - ${ResStrings.app_name}",
                                    body: "",
                                    "thewizrd.dev+SimpleWeatherMaui@gmail.com");
                            }
                        }
                        catch { }
                    }
                    break;
                case KEY_RATEREVIEW:
                    {
                        // TODO: rate and review
                    }
                    break;
                case KEY_TRANSLATE:
                    {
                        try
                        {
                            await Browser.Default.OpenAsync("https://poeditor.com/join/project?hash=x9AzamDQO8");
                        }
                        catch { }
                    }
                    break;
            }
        }
    }
}