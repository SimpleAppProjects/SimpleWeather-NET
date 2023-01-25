﻿using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common;
using SimpleWeather.Extras;
using SimpleWeather.Maui.Resources.Styles;
using SimpleWeather.Maui.Setup;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.WeatherData.Images;
using SimpleWeather.Maui.Main;
using CommunityToolkit.Maui;
#if !MACCATALYST
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using AppCenterLogLevel = Microsoft.AppCenter.LogLevel;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
#endif

namespace SimpleWeather.Maui;

public partial class App : Application
{
    private readonly SettingsManager SettingsManager;

    public AppState AppState { get; private set; } = AppState.Closed;
    private static bool Initialized { get; set; } = false;

    public bool IsSystemDarkTheme
    {
        get
        {
            return Current.RequestedTheme == AppTheme.Dark;
        }
    }

    public AppTheme CurrentTheme => this.UserAppTheme == AppTheme.Unspecified ? this.RequestedTheme : this.UserAppTheme;

    private IExtrasService ExtrasService { get; set; }

    public static new App Current => (App)Application.Current;
    public INavigation Navigation => this.MainPage?.Navigation;
    public Page? CurrentPage => Shell.Current?.CurrentPage ?? (this.MainPage as NavigationPage)?.CurrentPage ?? this.MainPage;
    public void SendBackButtonPressed() => this.CurrentPage?.SendBackButtonPressed();

    public App()
    {
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        InitializeComponent();

#if !MACCATALYST
        AppCenter.LogLevel = AppCenterLogLevel.Verbose;
        AppCenter.Start(APIKeys.GetAppCenterSecret(), typeof(Analytics), typeof(Crashes));
#endif

        // Initialize depencies for library
        InitializeDependencies();

        SettingsManager = Ioc.Default.GetService<SettingsManager>();

        RegisterSettingsListener();

        this.RequestedThemeChanged += App_RequestedThemeChanged;
        UpdateAppTheme(this.CurrentTheme);

        if (SettingsManager.WeatherLoaded && SettingsManager.OnBoardComplete)
        {
            MainPage = new AppShell();
        }
        else
        {
            MainPage = new NavigationPage(Ioc.Default.GetService<SetupWelcomePage>());
        }
    }

    private void InitializeDependencies()
    {
        SharedModule.Instance.OnCommonActionChanged += App_OnCommonActionChanged;
        SharedModule.Instance.Initialize();

        // Set UTF8Json Resolver
        Utf8Json.Resolvers.CompositeResolver.RegisterAndSetAsDefault(
            JSONParser.Resolver,
            Weather_API.Utf8JsonGen.Resolvers.GeneratedResolver.Instance//,
                                                                        //UWP.Utf8JsonGen.Resolvers.GeneratedResolver.Instance
        );

        // Build DI Services
        SharedModule.Instance.GetServiceCollection().Apply(collection =>
        {
            WeatherModule.Instance.ConfigureServices(collection);
            ExtrasModule.Instance.ConfigureServices(collection);
            ConfigureServices(collection);
        });
        SharedModule.Instance.BuildServiceProvider();

        // Initialize post-DI setup; Migrations require rely on DI
        CommonModule.Instance.Initialize();
        ExtrasModule.Instance.Initialize();

        ExtrasService = Ioc.Default.GetService<IExtrasService>();
    }

    private void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ImageDataHelperImpl, Backgrounds.ImageDataHelperRes>();
        // Add Setup Pages
        serviceCollection.AddScoped<SetupViewModel>()
            .AddTransient<SetupWelcomePage>()
            .AddTransient<SetupLocationsPage>()
            .AddTransient<SetupSettingsPage>();
    }

    private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        Logger.WriteLine(LoggerLevel.Fatal, e.Exception, "Unobserved Task Exception: Observed = {0}", e.Observed);

        // Log inner exceptions
        foreach (Exception inner in e.Exception.InnerExceptions)
        {
            Logger.WriteLine(LoggerLevel.Fatal, inner, "Unobserved Task Exception: {0}", inner.Message);
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
        AppState = AppState.Background;
    }

    protected override void OnResume()
    {
        base.OnResume();
        AppState = AppState.Foreground;
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        AppState = AppState.Background;
    }

    protected override void CleanUp()
    {
        base.CleanUp();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        window.MinimumHeight = 500;
        window.MinimumWidth = 500;

        return window;
    }

    protected override void OnAppLinkRequestReceived(Uri uri)
    {
        base.OnAppLinkRequestReceived(uri);
    }

    private void App_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
    {
        UpdateAppTheme(e.RequestedTheme);
    }

    public void UpdateAppTheme()
    {
        UpdateAppTheme(CurrentTheme);
    }

    public void UpdateAppTheme(AppTheme requestedTheme)
    {
        var themeDictionary = this.ThemeDictionary.MergedDictionaries;

        themeDictionary.Clear();
        if (requestedTheme == AppTheme.Dark)
        {
            themeDictionary.Add(new DarkTheme());
        }
        else
        {
            themeDictionary.Add(new LightTheme());
        }
    }

    private void App_OnSettingsChanged(SettingsChangedEventArgs e)
    {
        var isWeatherLoaded = SettingsManager.WeatherLoaded;

        switch (e.Key)
        {
            case SettingsManager.KEY_API:
                WeatherModule.Instance.WeatherManager.UpdateAPI();
                if (isWeatherLoaded)
                {
                    SharedModule.Instance.RequestAction(CommonActions.ACTION_SETTINGS_UPDATEAPI);
                }
                break;
            case SettingsManager.KEY_USEPERSONALKEY:
                WeatherModule.Instance.WeatherManager.UpdateAPI();
                break;
            case SettingsManager.KEY_FOLLOWGPS:
                SharedModule.Instance.RequestAction(CommonActions.ACTION_SETTINGS_UPDATEGPS);
                break;
            case SettingsManager.KEY_REFRESHINTERVAL:
                SharedModule.Instance.RequestAction(CommonActions.ACTION_SETTINGS_UPDATEREFRESH);
                break;
            case SettingsManager.KEY_ICONSSOURCE:
                SharedModule.Instance.WeatherIconsManager.UpdateIconProvider();
                break;
            case SettingsManager.KEY_DAILYNOTIFICATION:
                SharedModule.Instance.RequestAction(CommonActions.ACTION_SETTINGS_UPDATEDAILYNOTIFICATION);
                break;
        }
    }

    public void RegisterSettingsListener()
    {
        SettingsManager.OnSettingsChanged += App_OnSettingsChanged;
    }

    public void UnregisterSettingsListener()
    {
        SettingsManager.OnSettingsChanged -= App_OnSettingsChanged;
    }
}
