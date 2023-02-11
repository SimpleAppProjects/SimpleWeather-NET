using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.ComponentModel;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.ViewModels;
using SimpleWeather.NET.Controls;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Main;

public sealed partial class AppShell : ViewModelShell, IViewModelProvider
{
    public static AppShell Instance { get; private set; }

    private SnackbarManager SnackMgr { get; set; }
    private BannerManager BannerMgr { get; set; }

    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    public AppShell()
    {
        Instance = this;
        InitializeComponent();
        // TODO: add support for passing arguments
        AnalyticsLogger.LogEvent("AppShell");

        App.Current.UpdateAppTheme();
    }
}
