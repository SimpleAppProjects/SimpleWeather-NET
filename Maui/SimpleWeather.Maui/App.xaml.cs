﻿using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common;
using SimpleWeather.Extras;
using SimpleWeather.Keys;
using SimpleWeather.Maui.Main;
using SimpleWeather.Maui.Resources.Styles;
using SimpleWeather.Maui.Setup;
using SimpleWeather.Maui.Updates;
using SimpleWeather.NET.Localization;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.WeatherData.Images;
#if IOS || MACCATALYST
using UIKit;
#endif
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using AppCenterLogLevel = Microsoft.AppCenter.LogLevel;
using SimpleWeather.Weather_API.Json;
using SimpleWeather.NET.Json;

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
#if __IOS__
            return UIKit.UIScreen.MainScreen.TraitCollection.UserInterfaceStyle == UIUserInterfaceStyle.Dark;
#else
            return Current.RequestedTheme == AppTheme.Dark;
#endif
        }
    }

    public AppTheme CurrentTheme => this.UserAppTheme == AppTheme.Unspecified ? (IsSystemDarkTheme ? AppTheme.Dark : AppTheme.Light) : this.UserAppTheme;

    private IExtrasService ExtrasService { get; set; }

    public static new App Current => (App)Application.Current;
    public INavigation Navigation => this.MainPage?.Navigation;
    public Page? CurrentPage => Shell.Current?.CurrentPage ?? (this.MainPage as NavigationPage)?.CurrentPage ?? this.MainPage;
    public void SendBackButtonPressed() => this.CurrentPage?.SendBackButtonPressed();

    public App()
    {
#if __IOS__
        ObjCRuntime.Runtime.MarshalManagedException += (_, args) =>
        {
            args.ExceptionMode = ObjCRuntime.MarshalManagedExceptionMode.UnwindNativeCode;
        };
#endif
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Logger.WriteLine(LoggerLevel.Fatal, e.ExceptionObject as Exception, "UnhandledException: {0}", e.ExceptionObject);
        };
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        InitializeComponent();

        AppCenter.LogLevel = AppCenterLogLevel.Verbose;
        AppCenter.Start($"ios={AppCenterConfig.GetiOSAppCenterSecret()};macos={AppCenterConfig.GetMacOSAppCenterSecret()}", typeof(Analytics), typeof(Crashes));

        // Initialize depencies for library
        InitializeDependencies();

        SettingsManager = Ioc.Default.GetService<SettingsManager>();

        // Init user-preferred locale
        LocaleUtils.Init();

        RegisterSettingsListener();

        this.UserAppTheme = SettingsManager.UserTheme switch
        {
            UserThemeMode.Light => AppTheme.Light,
            UserThemeMode.Dark => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
        this.RequestedThemeChanged += App_RequestedThemeChanged;
        UpdateAppTheme(this.CurrentTheme);

#if __IOS__
        // Register BG Tasks
        BGTaskRegistrar.RegisterBGTasks();
#endif

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

        // Add Json Resolvers
        JSONParser.DefaultSettings
            .AddWeatherAPIContexts()
            .AddJsonContexts();

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
        // Add localization support
        serviceCollection.AddLocalization();
        serviceCollection.AddSingleton<CustomStringLocalizer>();
        serviceCollection.AddSingleton<IResourceLoader, ResourceLoaderImpl>();
#if __IOS__
        serviceCollection.AddSingleton<InAppUpdateManager, IOSInAppUpdateManagerImpl>();
#else
        serviceCollection.AddSingleton<InAppUpdateManager, DefaultInAppUpdateManagerImpl>();
#endif
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
        if (e.RequestedTheme == AppTheme.Unspecified)
        {
            UpdateAppTheme(IsSystemDarkTheme ? AppTheme.Dark : AppTheme.Light);
        }
        else
        {
            UpdateAppTheme(e.RequestedTheme);
        }
    }

    public void UpdateAppTheme()
    {
        var currentTheme = CurrentTheme;

        if (currentTheme == AppTheme.Unspecified)
        {
            UpdateAppTheme(IsSystemDarkTheme ? AppTheme.Dark : AppTheme.Light);
        }
        else
        {
            UpdateAppTheme(currentTheme);
        }
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

        var currentViewController = WindowStateManager.Default.GetCurrentUIViewController();

        if (currentViewController is null)
            return;

        switch (requestedTheme)
        {
            case AppTheme.Light:
                currentViewController.OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
                break;
            case AppTheme.Dark:
                currentViewController.OverrideUserInterfaceStyle = UIUserInterfaceStyle.Dark;
                break;
            default:
                currentViewController.OverrideUserInterfaceStyle = UIUserInterfaceStyle.Unspecified;
                break;
        }

        currentViewController.SetNeedsStatusBarAppearanceUpdate();
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
                if (isWeatherLoaded)
                {
                    SharedModule.Instance.RequestAction(CommonActions.ACTION_SETTINGS_UPDATEGPS);

                    if (Equals(e.NewValue, true))
                        SharedModule.Instance.RequestAction(CommonActions.ACTION_WIDGET_REFRESHWIDGETS);
                    else
                        SharedModule.Instance.RequestAction(CommonActions.ACTION_WIDGET_RESETWIDGETS);
                }
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
