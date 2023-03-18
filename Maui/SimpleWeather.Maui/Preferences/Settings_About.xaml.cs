using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Maui.Controls;
using SimpleWeather.NET.Controls;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Preferences;

public partial class Settings_About : ContentPage, ISnackbarManager
{
    private const string KEY_FEEDBACK = "key_feedback";
    private const string KEY_RATEREVIEW = "key_ratereview";
    private const string KEY_TRANSLATE = "key_translate";

    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();
    private DevSettingsController devSettingsController;

    private SnackbarManager SnackMgr;

    public Settings_About()
	{
		InitializeComponent();

        // Event Listeners
        this.SettingsTable.Model.ItemSelected += Model_ItemSelected;

        // Version info
        VersionPref.Detail = $"v{AppInfo.Current.VersionString}";

        // init devcontroller
        devSettingsController = new DevSettingsController(this);
        devSettingsController.DevSettingsEnabled += DevSettingsController_DevSettingsEnabled;
        if (SettingsManager.DevSettingsEnabled)
            devSettingsController.OnCreatePreferences(DevSettingsSection);

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

    public void InitSnackManager()
    {
        if (SnackMgr == null)
        {
            SnackMgr = new SnackbarManager(Content);
        }
    }

    public void ShowSnackbar(Snackbar snackbar)
    {
        SnackMgr?.Show(snackbar);
    }

    public void DismissAllSnackbars()
    {
        SnackMgr?.DismissAll();
    }

    public void UnloadSnackManager()
    {
        DismissAllSnackbars();
        SnackMgr = null;
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

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        InitSnackManager();

        devSettingsController.OnStart();
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        UnloadSnackManager();
    }

    private void VersionPref_Tapped(object sender, EventArgs e)
    {
        devSettingsController.OnClick();
    }

    private void DevSettingsController_DevSettingsEnabled(object sender, EventArgs e)
    {
        DevSettingsSection.Clear();
        devSettingsController.OnCreatePreferences(DevSettingsSection);
    }
}