using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.ComponentModel;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Maui.Preferences;
using SimpleWeather.Maui.ViewModels;
using SimpleWeather.NET.Controls;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Main;

public sealed partial class AppShell : ViewModelShell, IViewModelProvider
{
    public static AppShell Instance { get; private set; }

    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    public AppShell()
    {
        Instance = this;
        InitializeComponent();
        // TODO: add support for passing arguments
        AnalyticsLogger.LogEvent("AppShell");

        App.Current.UpdateAppTheme();
    }

    protected override async void OnNavigating(ShellNavigatingEventArgs args)
    {
        if (args.CanCancel)
        {
            if (CurrentPage is IBackRequestedPage backRequestedPage && await backRequestedPage.OnBackRequested())
            {
                args.Cancel();
            }
        }

        base.OnNavigating(args);
    }

    protected override void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);

        /*
        var currentPageType = this.CurrentPage?.GetType();
        if (currentPageType == typeof(WeatherNow) || currentPageType == typeof(WeatherDetailsPage))
        {
            if (this.ShellTabBar.Items.FirstOrDefault(it => it.Route.EndsWith("weather")) is ShellSection item)
            {
                this.CurrentItem = item;
            }
        }
        else if (currentPageType == typeof(WeatherAlertPage))
        {
            if (this.ShellTabBar.Items.FirstOrDefault(it => it.Route.EndsWith("alerts")) is ShellSection item)
            {
                this.CurrentItem = item;
            }
        }
        else if (currentPageType == typeof(WeatherRadarPage))
        {
            if (this.ShellTabBar.Items.FirstOrDefault(it => it.Route.EndsWith("radar")) is ShellSection item)
            {
                this.CurrentItem = item;
            }
        }
        else if (currentPageType == typeof(LocationsPage))
        {
            if (this.ShellTabBar.Items.FirstOrDefault(it => it.Route.EndsWith("locations")) is ShellSection item)
            {
                this.CurrentItem = item;
            }
        }
        else if (currentPageType == typeof(Settings_General))
        {
            if (this.ShellTabBar.Items.FirstOrDefault(it => it.Route.EndsWith("settings")) is ShellSection item)
            {
                this.CurrentItem = item;
            }
        }
        */
    }
}
