using System.Diagnostics;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI.ViewManagement;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI.Helpers;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppLifecycle;
using Newtonsoft.Json;
using Sentry.Extensibility;
using Sentry.Protocol;
using SimpleWeather.Backgrounds;
using SimpleWeather.Common;
using SimpleWeather.Extras;
using SimpleWeather.Keys;
using SimpleWeather.NET.BackgroundTasks;
using SimpleWeather.NET.Helpers;
using SimpleWeather.NET.Json;
using SimpleWeather.NET.Localization;
using SimpleWeather.NET.Main;
using SimpleWeather.NET.Preferences;
using SimpleWeather.NET.Radar;
using SimpleWeather.NET.Setup;
using SimpleWeather.NET.Shared.Helpers;
using SimpleWeather.NET.Widgets.Json;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.Json;
using SimpleWeather.WeatherData.Images;
using SQLite;
using FirebaseAuth = Firebase.Auth;
using FirebaseDb = Firebase.Database;
using JsonException = System.Text.Json.JsonException;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;
using Package = Windows.ApplicationModel.Package;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using SimpleWeather.Extras.BackgroundTasks;
using CommunityToolkit.WinUI;

namespace SimpleWeather.NET
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application, IApplication
    {
        private Window _window;

        private readonly UISettings UISettings;
        private readonly SettingsManager SettingsManager;

        public IResourceLoader ResLoader { get; private set; }
        public AppState AppState { get; private set; } = AppState.Closed;
        private static Frame RootFrame { get; set; }
        private static bool Initialized { get; set; } = false;

        public bool IsSystemDarkTheme
        {
            get
            {
                try
                {
                    return Current.RequestedTheme == ApplicationTheme.Dark;
                }
                catch (AccessViolationException)
                {
                    return UISettings.GetColorValue(UIColorType.Background).ToString() == "#FF000000";
                }
            }
        }

        public ElementTheme CurrentTheme
        {
            get
            {
                if (_window?.Content is FrameworkElement window)
                {
                    return window.RequestedTheme;
                }
                else
                {
                    return IsSystemDarkTheme ? ElementTheme.Dark : ElementTheme.Light;
                }
            }
        }

        private IExtrasService ExtrasService { get; set; }

        public static new App Current => (App)Application.Current;
        //public static IApplication Instance { get; private set; }

        private static Exception LastFirstChanceException;

        private readonly SynchronizationContext syncContext;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.syncContext = SynchronizationContext.Current;
            this.UnhandledException += OnUnhandledException;
            // Handle exceptions on background threads
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += OnDomainFirstChanceException;
            CoreApplication.EnablePrelaunch(true);
            this.InitializeComponent();
            //Instance = this;

            // During the transition from foreground to background the
            // memory limit allowed for the application changes. The application
            // has a short time to respond by bringing its memory usage
            // under the new limit.
            MemoryManager.AppMemoryUsageLimitChanging += MemoryManager_AppMemoryUsageLimitChanging;

            // After an application is backgrounded it is expected to stay
            // under a memory target to maintain priority to keep running.
            // Subscribe to the event that informs the app of this change.
            MemoryManager.AppMemoryUsageIncreased += MemoryManager_AppMemoryUsageIncreased;

            // Sentry
            SentrySdk.Init(options =>
            {
                options.Dsn = SentryConfig.GetDsn();
                options.IsGlobalModeEnabled = true;
                // Disable Sentry's built in UnhandledException handler as this won't work with AOT compilation
                options.DisableWinUiUnhandledExceptionIntegration();

                // Limit exceptions captured
                options.AddExceptionFilter(new ExceptionFilter());
            });

            // Initialize depencies for library
            InitializeDependencies();

            UISettings = new UISettings();
            SettingsManager = Ioc.Default.GetService<SettingsManager>();

            // Init user-preferred locale
            LocaleUtils.Init();

            RegisterSettingsListener();

            AnalyticsLogger.SetUserProperty(AnalyticsProps.DEVICE_TYPE, DeviceTypeHelper.DeviceType.ToString().ToLowerInvariant());
            AnalyticsLogger.SetUserProperty(AnalyticsProps.PLATFORM, "Windows");
            AnalyticsLogger.SetUserProperty(AnalyticsProps.WINDOWS_OS_VERSION, DeviceTypeHelper.OSVersion.ToFormattedString());
            AnalyticsLogger.SetUserProperty(AnalyticsProps.WINDOWS_OS_VERSION_CODE, DeviceTypeHelper.OSVersion.ToVersionCode());
            AnalyticsLogger.SetUserProperty(AnalyticsProps.APP_VERSION, Package.Current.Id.Version.ToFormattedString());
            AnalyticsLogger.SetUserProperty(AnalyticsProps.APP_VERSION_CODE, Package.Current.Id.Version.ToVersionCode());
        }

        ~App()
        {
            UnregisterSettingsListener();
            UnregisterWidgetProvider();
            UnregisterInProcBackgroundTask();
        }

        private void InitializeDependencies()
        {
            SharedModule.Instance.OnCommonActionChanged += App_OnCommonActionChanged;
            SharedModule.Instance.Initialize();

            // Add Json Resolvers
            JSONParser.DefaultSettings
                .AddWeatherAPIContexts()
                .AddJsonContexts()
                .AddWeatherWidgetContexts();

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

            ResLoader = Ioc.Default.GetService<IResourceLoader>();
            ExtrasService = Ioc.Default.GetService<IExtrasService>();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IImageDataService, ImageDataHelperRes>();
            serviceCollection.AddLogging();
            serviceCollection.AddLocalization();
            serviceCollection.AddSingleton<CustomStringLocalizer>();
            serviceCollection.AddSingleton<IResourceLoader, ResourceLoaderImpl>();
        }

        /// <summary>
        /// Handle system notifications that the app has increased its
        /// memory usage level compared to its current target.
        /// </summary>
        /// <remarks>
        /// The app may have increased its usage or the app may have moved
        /// to the background and the system lowered the target for the app
        /// In either case, if the application wants to maintain its priority
        /// to avoid being suspended before other apps, it may need to reduce
        /// its memory usage.
        ///
        /// This is not a replacement for handling AppMemoryUsageLimitChanging
        /// which is critical to ensure the app immediately gets below the new
        /// limit. However, once the app is allowed to continue running and
        /// policy is applied, some apps may wish to continue monitoring
        /// usage to ensure they remain below the limit.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MemoryManager_AppMemoryUsageIncreased(object sender, object e)
        {
            // Obtain the current usage level
            var level = MemoryManager.AppMemoryUsageLevel;

            // Check the usage level to determine whether reducing memory is necessary.
            // Memory usage may have been fine when initially entering the background but
            // the app may have increased its memory usage since then and will need to trim back.
            if (level == AppMemoryUsageLevel.OverLimit || level == AppMemoryUsageLevel.High)
            {
                ReduceMemoryUsage(MemoryManager.AppMemoryUsageLimit);
            }
        }

        /// <summary>
        /// Raised when the memory limit for the app is changing, such as when the app
        /// enters the background.
        /// </summary>
        /// <remarks>
        /// If the app is using more than the new limit, it must reduce memory within 2 seconds
        /// on some platforms in order to avoid being suspended or terminated.
        ///
        /// While some platforms will allow the application
        /// to continue running over the limit, reducing usage in the time
        /// allotted will enable the best experience across the broadest range of devices.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MemoryManager_AppMemoryUsageLimitChanging(object sender, AppMemoryUsageLimitChangingEventArgs e)
        {
            // If app memory usage is over the limit, reduce usage within 2 seconds
            // so that the system does not suspend the app
            if (MemoryManager.AppMemoryUsage >= e.NewLimit)
            {
                ReduceMemoryUsage(e.NewLimit);
            }
        }

        /// <summary>
        /// Reduces application memory usage.
        /// </summary>
        /// <remarks>
        /// When the app enters the background, receives a memory limit changing
        /// event, or receives a memory usage increased event, it can
        /// can optionally unload cached data or even its view content in
        /// order to reduce memory usage and the chance of being suspended.
        ///
        /// This must be called from multiple event handlers because an application may already
        /// be in a high memory usage state when entering the background, or it
        /// may be in a low memory usage state with no need to unload resources yet
        /// and only enter a higher state later.
        /// </remarks>
        public void ReduceMemoryUsage(ulong limit)
        {
            // If the app has caches or other memory it can free, it should do so now.
            // << App can release memory here >>
            AnalyticsLogger.LogEvent("App: ReduceMemoryUsage",
                new Dictionary<string, string>()
                {
                    { "limit (bytes)", limit.ToString() }
                });

            // Additionally, if the application is currently
            // in background mode and still has a view with content
            // then the view can be released to save memory and
            // can be recreated again later when leaving the background.
            if (AppState == AppState.Background && _window?.Content != null)
            {
                // Some apps may wish to use this helper to explicitly disconnect
                // child references.
                // VisualTreeHelper.DisconnectChildrenRecursive(_window.Content);

                // Clear the view content. Note that views should rely on
                // events like Page.Unloaded to further release resources.
                // Release event handlers in views since references can
                // prevent objects from being collected.
                _window.Content = null;
            }

            // Run the GC to collect released resources.
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, false);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Register for toast activation. Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
            ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;
            // Register activation events
            AppInstance.GetCurrent().Activated += (sender, e) =>
            {
                // Use the synchronous option to ensure AppActivationArguments stays alive.
                // Once the Activated event returns the other instance will close and so 
                // will the AppActivationArguments object.
                this.syncContext.Send(
                    state =>
                    {
                        AppActivationArguments arguments = (AppActivationArguments)state;
                        this.OnActivated(arguments);
                    },
                    e);
            };

            // If we weren't launched by an app, launch our window like normal.
            // Otherwise if launched by a toast, our OnActivated callback will be triggered
            if (!ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            {
                var activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
                var activationKind = activatedArgs.Kind;
                var launchArgs = activatedArgs?.Data as ILaunchActivatedEventArgs;

                if (launchArgs?.Arguments?.Contains("RegisterProcessAsComServer") == true)
                {
                    RegisterWidgetProvider();
                }
                else if (launchArgs?.Arguments?.Contains("RegisterForBGTaskServer") == true)
                {
                    RegisterInProcBackgroundTask();
                }
                else
                {
                    LaunchAndBringToForegroundIfNeeded(args.ToCompatArgs());
                }
            }
        }

        private void OnActivated(AppActivationArguments e)
        {
            if (e.Data is Windows.ApplicationModel.Activation.LaunchActivatedEventArgs args)
            {
                LaunchAndBringToForegroundIfNeeded(args.ToCompatArgs());
            }
        }

        private void LaunchAndBringToForegroundIfNeeded(LaunchActivatedEventExArgs? args = null)
        {
            Initialize(args);

#if DEBUG
            if (Debugger.IsAttached)
            {
                // this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Logger.WriteLine(LoggerLevel.Info, "Started logger...");

            // App loaded for first time
            Task.Run(async () =>
            {
                // Register background tasks
                await UpdateTask.RegisterBackgroundTask();
                await AppUpdaterTask.RegisterBackgroundTask();
                await RemoteConfigUpdateTask.RegisterBackgroundTask();
                if (SettingsManager.DailyNotificationEnabled)
                {
                    await DailyNotificationTask.RegisterBackgroundTask();
                }
                await PremiumStatusTask.RegisterBackgroundTask();
            });

            var prelaunchActivated = args?.PrelaunchActivated ?? false;

            if (!prelaunchActivated)
            {
                // args.UWPLaunchActivatedEventArgs throws an InvalidCastException in desktop apps.
                var tileId = args?.TileId;

                if (SettingsManager.WeatherLoaded && SettingsManager.OnBoardComplete && !String.IsNullOrEmpty(tileId) && !tileId.Equals("App", StringComparison.OrdinalIgnoreCase))
                {
                    if (RootFrame.Content == null)
                    {
                        RootFrame.Navigate(typeof(Shell), "suppressNavigate");
                    }

                    // Navigate to WeatherNow page for location
                    if (Shell.Instance != null)
                    {
                        if (tileId != null)
                        {
                            Shell.Instance.AppFrame.Navigate(typeof(WeatherNow), new WeatherNowArgs()
                            {
                                TileId = tileId
                            });
                            Shell.Instance.AppFrame.BackStack.Clear();
                        }

                        // If Shell content is empty navigate to default page
                        if (Shell.Instance.AppFrame.CurrentSourcePageType == null)
                        {
                            Shell.Instance.AppFrame.Navigate(typeof(WeatherNow), new WeatherNowArgs()
                            {
                                IsHome = true
                            });
                            Shell.Instance.AppFrame.BackStack.Clear();
                        }
                    }
                }

                if (RootFrame.Content == null)
                {
                    UpdateAppTheme();

                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    if (SettingsManager.WeatherLoaded && SettingsManager.OnBoardComplete)
                        RootFrame.Navigate(typeof(Shell), args.Arguments);
                    else
                        RootFrame.Navigate(typeof(SetupPage), args.Arguments);
                }

                // Ensure the current window is active
                _window.Activate();
            }
        }

        private void ToastNotificationManagerCompat_OnActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            // Use the dispatcher from the window if present, otherwise the app dispatcher
            var dispatcherQueue = _window?.DispatcherQueue ?? SharedModule.Instance.DispatcherQueue;

            dispatcherQueue.TryEnqueue(async () =>
            {
                Initialize();

                // Get the root frame
                RootFrame = _window?.Content as Frame;

                var args = ToastArguments.Parse(e.Argument);

                if (!args.Contains(Constants.KEY_ACTION))
                    return;

                switch (args[Constants.KEY_ACTION])
                {
                    case "view-alerts":
                        {
                            if (SettingsManager.WeatherLoaded && SettingsManager.OnBoardComplete)
                            {
                                string? data = null;

                                if (args.Contains(Constants.KEY_DATA))
                                {
                                    data = args[Constants.KEY_DATA];
                                }

                                // App loaded for first time
                                if (RootFrame.Content == null)
                                {
                                    RootFrame.Navigate(typeof(Shell), "suppressNavigate");
                                }

                                if (Shell.Instance != null)
                                {
                                    LocationData.LocationData? locData = null;
                                    if (!string.IsNullOrWhiteSpace(data))
                                    {
                                        locData = await JSONParser.DeserializerAsync<LocationData.LocationData>(data);
                                    }

                                    // If we're already on WeatherNow navigate to Alert page
                                    if (Shell.Instance.AppFrame.Content != null && Shell.Instance.AppFrame.SourcePageType.Equals(typeof(WeatherNow)))
                                    {
                                        Shell.Instance.AppFrame.Navigate(typeof(WeatherAlertPage), new WeatherPageArgs()
                                        {
                                            Location = locData
                                        });
                                    }
                                    // If not clear backstack and navigate to Alert page
                                    // Add a WeatherNow page in backstack to go back to
                                    else
                                    {
                                        Shell.Instance.AppFrame.Navigate(typeof(WeatherAlertPage), new WeatherPageArgs()
                                        {
                                            Location = locData
                                        });
                                        Shell.Instance.AppFrame.BackStack.Clear();
                                        Shell.Instance.AppFrame.BackStack.Add(new PageStackEntry(typeof(WeatherNow), new WeatherNowArgs()
                                        {
                                            Location = locData,
                                            IsHome = Equals(locData, await SettingsManager.GetHomeData())
                                        }, null));
                                    }
                                }
                            }
                        }
                        break;

                    case "check-updates":
                        {
                            if (SettingsManager.WeatherLoaded && SettingsManager.OnBoardComplete)
                            {
                                // App loaded for first time
                                if (RootFrame.Content == null)
                                {
                                    RootFrame.Navigate(typeof(Shell), "suppressNavigate");
                                }

                                if (Shell.Instance != null)
                                {
                                    if (Shell.Instance.AppFrame.Content != null && Shell.Instance.AppFrame.SourcePageType.Equals(typeof(WeatherNow)))
                                    {
                                        Shell.Instance.AppFrame.Navigate(typeof(SettingsPage), "About");
                                    }
                                    else
                                    {
                                        Shell.Instance.AppFrame.Navigate(typeof(SettingsPage), "About");
                                        Shell.Instance.AppFrame.BackStack.Clear();
                                        Shell.Instance.AppFrame.BackStack.Add(new PageStackEntry(typeof(WeatherNow), new WeatherNowArgs()
                                        {
                                            IsHome = true
                                        }, null));
                                    }
                                }
                            }
                        }
                        break;

                    case "view-weather":
                        {
                            if (SettingsManager.WeatherLoaded && SettingsManager.OnBoardComplete)
                            {
                                string? data = null;

                                if (args.Contains(Constants.KEY_DATA))
                                {
                                    data = args[Constants.KEY_DATA];
                                }

                                LocationData.LocationData? locData = null;
                                if (!string.IsNullOrWhiteSpace(data))
                                {
                                    locData = await JSONParser.DeserializerAsync<LocationData.LocationData>(data);
                                }

                                // App loaded for first time
                                if (RootFrame.Content == null)
                                {
                                    RootFrame.Navigate(typeof(Shell), new WeatherNowArgs()
                                    {
                                        Location = locData
                                    });
                                }
                                else if (Shell.Instance != null)
                                {
                                    // If we're not on WeatherNow, navigate
                                    if (Shell.Instance.AppFrame.Content != null && !Shell.Instance.AppFrame.SourcePageType.Equals(typeof(WeatherNow)))
                                    {
                                        Shell.Instance.AppFrame.Navigate(typeof(WeatherNow), new WeatherNowArgs()
                                        {
                                            Location = locData
                                        });
                                    }
                                }
                            }
                        }
                        break;

                    case "open-link":
                        {
                            if (args.TryGetValue(Constants.KEY_DATA, out var data))
                            {
                                this.RunCatching(async () =>
                                {
                                    await Launcher.LaunchUriAsync(new Uri(data));
                                });
                                return;
                            }
                        }
                        break;

                    default:
                        break;
                }

                // Ensure the current window is active
                _window?.Activate();

                UpdateAppTheme();
            });
        }

        private void Initialize(LaunchActivatedEventExArgs? e = null)
        {
            if (MainWindow.Current == null || _window == null)
            {
                MainWindow.Current = _window = new MainWindow();
                InitializeWindow(_window);
                _window.Closed += Window_Closed;
                _window.SizeChanged += Window_SizeChanged;
                _window.VisibilityChanged += Window_VisibilityChanged;
            }
            _window.Activate();

            RootFrame = _window?.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (RootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                RootFrame = new Frame();

                RootFrame.NavigationFailed += OnNavigationFailed;

                if (e?.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                _window.Content = RootFrame;
            }

            if (_window.Content is FrameworkElement)
            {
                UpdateAppTheme();
            }

            // Load data if needed
            _ = Task.Run(SettingsManager.LoadIfNeeded);

            _ = Task.Run(DI.Utils.RemoteConfigService.CheckConfig);

            ExtrasService.CheckPremiumStatus();

            Initialized = true;
        }

        private void Initialize(IBackgroundTaskInstance instance)
        {
            // Load data if needed
            _ = Task.Run(SettingsManager.LoadIfNeeded);

            DI.Utils.RemoteConfigService.UpdateWeatherProvider();

            ExtrasService.CheckPremiumStatus();

            Initialized = true;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /* 
         * Source: https://gist.github.com/mattjohnsonpint/7b385b7a2da7059c4a16562bc5ddb3b7
         * Exceptions on background threads are caught by AppDomain.CurrentDomain.UnhandledException,
         *   not by Microsoft.UI.Xaml.Application.Current.UnhandledException
         *   See: https://github.com/microsoft/microsoft-ui-xaml/issues/5221
         */
        [SecurityCritical]
        private void OnDomainUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;

            if (exception is not null)
            {
                // Tell Sentry this was an unhandled exception
                exception.Data[Mechanism.HandledKey] = false;
                exception.Data[Mechanism.MechanismKey] = "AppDomain.CurrentDomain.UnhandledException";

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

            // Flush the event immediately
            SentrySdk.FlushAsync(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
        }

        private void OnDomainFirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            LastFirstChanceException = e.Exception;
        }

        /*
         * Exceptions caught by Microsoft.UI.Xaml.Application.Current.UnhandledException have details removed,
         *   but that can be worked around by saved by trapping first chance exceptions
         *   See: https://github.com/microsoft/microsoft-ui-xaml/issues/7160
         */
        [SecurityCritical]
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;

            if (exception is not null)
            {
                if (exception?.StackTrace is null && LastFirstChanceException != null)
                {
                    exception = LastFirstChanceException;
                }

                // Tell Sentry this was an unhandled exception
                exception.Data[Mechanism.HandledKey] = false;
                exception.Data[Mechanism.MechanismKey] = "Application.UnhandledException";

                Logger.WriteLine(LoggerLevel.Fatal, exception, $"Unhandled Exception: {exception?.Message}");

                // Log inner exceptions
                if (exception is AggregateException agg)
                {
                    foreach (Exception inner in agg.InnerExceptions)
                    {
                        Logger.WriteLine(LoggerLevel.Fatal, inner, $"Unhandled Inner Exception: {inner.Message}");
                    }
                }
            }

            // Flush the event immediately
            SentrySdk.FlushAsync(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
        }

        [SecurityCritical]
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var exception = e.Exception;

            if (exception is not null)
            {
                // Tell Sentry this was an unhandled exception
                exception.Data[Mechanism.HandledKey] = false;
                exception.Data[Mechanism.MechanismKey] = "TaskScheduler.UnobservedTaskException";

                Logger.WriteLine(LoggerLevel.Fatal, exception, "Unobserved Task Exception: Observed = {0}", e.Observed);

                // Log inner exceptions
                foreach (Exception inner in exception.InnerExceptions)
                {
                    Logger.WriteLine(LoggerLevel.Fatal, inner, "Unobserved Task Inner Exception: {0}", inner.Message);
                }
            }

            // Flush the event immediately
            SentrySdk.FlushAsync(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
        }

        private void DefaultTheme_ColorValuesChanged(UISettings sender, object args)
        {
            UpdateColorValues();
        }

        private async void UpdateColorValues()
        {
            try
            {
                // NOTE: Run on UI Thread since this may be called off the main thread
                await SharedModule.Instance.DispatcherQueue.EnqueueAsync(() =>
                {
                    AnalyticsLogger.LogEvent("App: UpdateColorValues",
                        new Dictionary<string, string>()
                        {
                            { "IsSystemDarkTheme", IsSystemDarkTheme.ToString() }
                        });

                    UpdateAppTheme();
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "App: UpdateColorValues: error updating color values");
            }
        }

        public void UpdateAppTheme()
        {
            if (_window?.Content is FrameworkElement window)
            {
                switch (SettingsManager.UserTheme)
                {
                    case UserThemeMode.System:
                        window.RequestedTheme = IsSystemDarkTheme ? ElementTheme.Dark : ElementTheme.Light;
                        break;

                    case UserThemeMode.Light:
                        window.RequestedTheme = ElementTheme.Light;
                        break;

                    case UserThemeMode.Dark:
                        window.RequestedTheme = ElementTheme.Dark;
                        break;
                }
            }
        }

        private void App_OnSettingsChanged(SettingsChangedEventArgs e)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            var isWeatherLoaded = localSettings.Values[SettingsManager.KEY_WEATHERLOADED] as bool? ?? false;

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

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            MapControlCreator.Instance?.RemoveMapControl();
            MainWindow.Current = null;
            if (_window != null)
            {
                _window.Content = null;
                _window = null;
            }
        }

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
        }

        private void Window_VisibilityChanged(object sender, WindowVisibilityChangedEventArgs args)
        {
            if (args.Visible)
            {
                UISettings.ColorValuesChanged += DefaultTheme_ColorValuesChanged;
            }
            else
            {
                UISettings.ColorValuesChanged -= DefaultTheme_ColorValuesChanged;
            }
        }

        private class ExceptionFilter : IExceptionFilter
        {
            public bool Filter(Exception ex)
            {
                // Don't filter unhandled exceptions of any type
                if (ex.Data is not null && ex.Data.Contains(Mechanism.HandledKey) && ex.Data[Mechanism.HandledKey] is false)
                {
                    return false;
                }

                if (ex is IOException && ex.Message?.Contains("HTTP") == true)
                {
                    return true;
                }

                if (ex is ArgumentNullException && (ex.StackTrace?.Contains("FromJson") == true || ex.StackTrace?.Contains("JSONParser") == true))
                {
                    return true;
                }

                if (ex is NullReferenceException && (ex.StackTrace?.Contains("SKXamlCanvas") == true || ex.StackTrace?.Contains("SQLite") == true))
                {
                    return true;
                }

                if (ex is not null && ex.Message?.Contains("tz_long") == true)
                {
                    return true;
                }

                if (ex is JsonException || ex is HttpRequestException ||
                    ex is WebException || ex is COMException ||
                    ex is WeatherException || ex is FileNotFoundException ||
                    ex is TaskCanceledException || ex is TimeoutException ||
                    ex is FirebaseAuth.FirebaseAuthException || ex is FirebaseAuth.FirebaseAuthHttpException ||
                    ex is FirebaseDb.FirebaseException || ex is SQLiteException ||
                    ex is JsonReaderException)
                {
                    return true;
                }

                return false;
            }
        }
    }
}