using CommunityToolkit.Maui;
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
#if __IOS__
using UIKit;
#endif
using SimpleWeather.Weather_API.Json;
using SimpleWeather.NET.Json;
using System.Security;
using SimpleWeather.Maui.Images;

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
        AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        InitializeComponent();

        // Initialize depencies for library
        InitializeDependencies();

        SettingsManager = Ioc.Default.GetService<SettingsManager>();

        // Init user-preferred locale
        LocaleUtils.Init();

        RegisterSettingsListener();

        if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
        {
            AnalyticsLogger.SetUserProperty(AnalyticsProps.DEVICE_TYPE, "desktop");
        }
        else if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
        {
            AnalyticsLogger.SetUserProperty(AnalyticsProps.DEVICE_TYPE, "tablet");
        }
        else if (DeviceInfo.Idiom == DeviceIdiom.Phone)
        {
            AnalyticsLogger.SetUserProperty(AnalyticsProps.DEVICE_TYPE, "phone");
        }

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

#if __IOS__
        FirebaseConfigurator.Initialize();
#endif

        // Initialize post-DI setup; Migrations require rely on DI
        CommonModule.Instance.Initialize();
        ExtrasModule.Instance.Initialize();

        ExtrasService = Ioc.Default.GetService<IExtrasService>();
    }

    private void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IImageDataService, ImageDataServiceImpl>();
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

    [SecurityCritical]
    private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;

        if (exception is not null)
        {
#if __MACCATALYST__
            // Tell Sentry this was an unhandled exception
            exception.Data[Sentry.Protocol.Mechanism.HandledKey] = false;
            exception.Data[Sentry.Protocol.Mechanism.MechanismKey] = "AppDomain.CurrentDomain.UnhandledException";
#endif

            Logger.WriteLine(LoggerLevel.Fatal, exception, "Unhandled Exception: {0}", exception);

            // Log inner exceptions
            if (exception is AggregateException agg)
            {
                foreach (Exception inner in agg.InnerExceptions)
                {
                    Logger.WriteLine(LoggerLevel.Fatal, inner, "Unhandled Inner Exception: {0}", inner.Message);
                }
            }
        }

#if __MACCATALYST__
        // Flush the event immediately
        SentrySdk.FlushAsync(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
#endif
    }

    [SecurityCritical]
    private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        var exception = e.Exception;

        if (exception is not null)
        {
#if __MACCATALYST__
            // Tell Sentry this was an unhandled exception
            exception.Data[Sentry.Protocol.Mechanism.HandledKey] = false;
            exception.Data[Sentry.Protocol.Mechanism.MechanismKey] = "TaskScheduler.UnobservedTaskException";
#endif

            Logger.WriteLine(LoggerLevel.Fatal, exception, "Unobserved Task Exception: Observed = {0}", e.Observed);

            // Log inner exceptions
            foreach (Exception inner in exception.InnerExceptions)
            {
                Logger.WriteLine(LoggerLevel.Fatal, inner, "Unobserved Task Exception: {0}", inner.Message);
            }
        }

#if __MACCATALYST__
        SentrySdk.FlushAsync(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
#endif
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
