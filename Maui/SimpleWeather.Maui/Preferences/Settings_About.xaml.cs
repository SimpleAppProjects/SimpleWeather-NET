using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Maui.Controls;
using SimpleWeather.NET.Controls;
using SimpleWeather.Preferences;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Preferences;

public partial class Settings_About : ContentPage, ISnackbarManager
{
    private const string KEY_FEEDBACK = "key_feedback";
    private const string KEY_RATEREVIEW = "key_ratereview";
    private const string KEY_TRANSLATE = "key_translate";

    private const string KEY_PRIVACYPOLICY = "key_privacypolicy";
    private const string KEY_TERMSEULA = "key_termseula";

    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();
    private DevSettingsController devSettingsController;

    private SnackbarManager SnackMgr;

    public Settings_About()
    {
        InitializeComponent();

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
            Colors.Black, Colors.White, Color.Parse("#767676"), Color.Parse("#a2a2a2"),
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

    private async void TextCell_Tapped(object sender, EventArgs e)
    {
        var commandParam = ((sender as TextCell)?.CommandParameter) ?? ((sender as TextViewCell)?.CommandParameter);

        if (commandParam is Type pageType)
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
        else if (sender is DialogCell dialogCell)
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
                        try
                        {
#if __IOS__
                            var app = UIKit.UIApplication.SharedApplication;
                            var reviewUrl = Foundation.NSUrl.FromString("https://apps.apple.com/app/id6447495788?action=write-review");

                            if (app.CanOpenUrl(reviewUrl))
                            {
                                app.OpenUrl(reviewUrl, new UIKit.UIApplicationOpenUrlOptions(), null);
                            }
#endif
                        }
                        catch { }
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
                case KEY_PRIVACYPOLICY:
                    {
                        try
                        {
                            await Browser.Default.OpenAsync("https://simpleweather-91d01.web.app/privacy-policy");
                        }
                        catch { }
                    }
                    break;
                case KEY_TERMSEULA:
                    {
                        try
                        {
#if __IOS__
                            await Browser.Default.OpenAsync("http://www.apple.com/legal/itunes/appstore/dev/stdeula");
#endif
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