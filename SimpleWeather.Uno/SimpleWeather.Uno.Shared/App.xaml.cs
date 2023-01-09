using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
#if !__MACCATALYST__ && !HAS_UNO_SKIA_GTK
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
#endif
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleWeather.Common;
using SimpleWeather.Extras;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Uno.Main;
using SimpleWeather.Uno.Preferences;
using SimpleWeather.Uno.Setup;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.WeatherData.Images;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.Resources;
#if !__MACCATALYST__
using AppCenterLogLevel = Microsoft.AppCenter.LogLevel;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
#endif
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;
#if WINDOWS
using CommunityToolkit.WinUI.Notifications;
using SimpleWeather.Extras.BackgroundTasks;
using SimpleWeather.Uno.BackgroundTasks;
using System.Diagnostics;
#endif
#if HAS_UNO
using Uno.UI;
using Uno.UI.Xaml.Controls;
#endif

namespace SimpleWeather.Uno
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application, IApplication
    {
        private Window _window;

        public ResourceLoader ResLoader => SharedModule.Instance.ResLoader;
        private readonly UISettings UISettings;
        private readonly SettingsManager SettingsManager;

        public AppState AppState { get; private set; } = AppState.Closed;
        private static Frame RootFrame { get; set; }
        private static bool Initialized { get; set; } = false;

        public bool IsSystemDarkTheme
        {
            get
            {
                return Current.RequestedTheme == ApplicationTheme.Dark;
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

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
#if WINDOWS
            Windows.ApplicationModel.Core.CoreApplication.EnablePrelaunch(true);
#endif
#if HAS_UNO
            FeatureConfiguration.ContentPresenter.UseImplicitContentFromTemplatedParent = true;
            FeatureConfiguration.Control.UseDeferredOnApplyTemplate = true;
            // FeatureConfiguration.Font.SymbolsFont = "";
            // FeatureConfiguration.ListViewBase.DefaultCacheLength = null; // default UWP = 4.0
            FeatureConfiguration.Page.IsPoolingEnabled = true;
            // FeatureConfiguration.Style.UseUWPDefaultStyles = false;
            FeatureConfiguration.Style.ConfigureNativeFrameNavigation();
            FeatureConfiguration.ScrollViewer.DefaultUpdatesMode = ScrollViewerUpdatesMode.Synchronous;
#if ANDROID
            FeatureConfiguration.Popup.UseNativePopup = true;
#endif
#endif
            InitializeLogging();
            this.InitializeComponent();
            //Instance = this;
#if HAS_UNO || NETFX_CORE
            this.Suspending += OnSuspending;
            this.EnteredBackground += OnEnteredBackground;
            this.LeavingBackground += OnLeavingBackground;
#endif

#if WINDOWS
            // During the transition from foreground to background the
            // memory limit allowed for the application changes. The application
            // has a short time to respond by bringing its memory usage
            // under the new limit.
            MemoryManager.AppMemoryUsageLimitChanging += MemoryManager_AppMemoryUsageLimitChanging;

            // After an application is backgrounded it is expected to stay
            // under a memory target to maintain priority to keep running.
            // Subscribe to the event that informs the app of this change.
            MemoryManager.AppMemoryUsageIncreased += MemoryManager_AppMemoryUsageIncreased;
#endif

#if !__MACCATALYST__ && !HAS_UNO_SKIA_GTK
            AppCenter.LogLevel = AppCenterLogLevel.Verbose;
            AppCenter.Start(APIKeys.GetAppCenterSecret(), typeof(Analytics), typeof(Crashes));
#endif

            // Initialize depencies for library
            InitializeDependencies();

            UISettings = new UISettings();
            SettingsManager = Ioc.Default.GetService<SettingsManager>();

            RegisterSettingsListener();
        }

        private void InitializeDependencies()
        {
            SharedModule.Instance.OnCommonActionChanged += App_OnCommonActionChanged;
            SharedModule.Instance.Initialize();

            // Set UTF8Json Resolver
            Utf8Json.Resolvers.CompositeResolver.RegisterAndSetAsDefault(
                JSONParser.Resolver,
                Weather_API.Utf8JsonGen.Resolvers.GeneratedResolver.Instance,
                Uno.Utf8JsonGen.Resolvers.GeneratedResolver.Instance
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
        }

#if WINDOWS
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
#endif

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
#if !HAS_UNO
            // Register for toast activation. Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
            ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;

            // If we weren't launched by an app, launch our window like normal.
            // Otherwise if launched by a toast, our OnActivated callback will be triggered
            if (!ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            {
                var activatedArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
                var activationKind = activatedArgs.Kind;
#endif
            LaunchAndBringToForegroundIfNeeded(args);
#if !HAS_UNO
            }
#endif
        }

        private async void LaunchAndBringToForegroundIfNeeded(Microsoft.UI.Xaml.LaunchActivatedEventArgs? args = null)
        {
            await Initialize(args);

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Logger.WriteLine(LoggerLevel.Info, "Started logger...");

            // App loaded for first time
#if WINDOWS
            _ = Task.Run(async () =>
            {
                // Register background tasks
                await UpdateTask.RegisterBackgroundTask();
                await AppUpdaterTask.RegisterBackgroundTask();
                await RemoteConfigUpdateTask.RegisterBackgroundTask();
            });
#endif

#if WINDOWS
            var prelaunchActivated = false;

            // args.UWPLaunchActivatedEventArgs throws an InvalidCastException in desktop apps.
            prelaunchActivated = this.RunCatching(() =>
            {
                return args?.UWPLaunchActivatedEventArgs?.PrelaunchActivated ?? false;
            }).GetOrDefault(prelaunchActivated);

            if (!prelaunchActivated)
#endif
            {
#if WINDOWS
                // args.UWPLaunchActivatedEventArgs throws an InvalidCastException in desktop apps.
                var tileId = this.RunCatching(() =>
                {
                    return args?.UWPLaunchActivatedEventArgs?.TileId;
                }).GetOrNull();

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
                            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                        }

                        // If Shell content is empty navigate to default page
                        if (Shell.Instance.AppFrame.CurrentSourcePageType == null)
                        {
                            Shell.Instance.AppFrame.Navigate(typeof(WeatherNow), new WeatherNowArgs()
                            {
                                IsHome = true
                            });
                            Shell.Instance.AppFrame.BackStack.Clear();
                            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                        }
                    }
                }
#endif

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

#if WINDOWS
        private void ToastNotificationManagerCompat_OnActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            // Use the dispatcher from the window if present, otherwise the app dispatcher
            var dispatcherQueue = _window?.DispatcherQueue ?? SharedModule.Instance.DispatcherQueue;

            dispatcherQueue.TryEnqueue(async () =>
            {
                // Get the root frame
                RootFrame = _window?.Content as Frame;

                var args = ToastArguments.Parse(e.Argument);

                if (!args.Contains("action"))
                    return;

                switch (args["action"])
                {
                    case "view-alerts":
                        if (SettingsManager.WeatherLoaded && SettingsManager.OnBoardComplete)
                        {
                            string? data = null;

                            if (args.Contains("data"))
                            {
                                data = args["data"];
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
                                        IsHome = Object.Equals(locData, await SettingsManager.GetHomeData())
                                    }, null));
                                    //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                                }
                            }
                        }
                        break;

                    case "check-updates":
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
                                    //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                                }
                            }
                        }
                        break;

                    case "view-weather":
                        if (SettingsManager.WeatherLoaded && SettingsManager.OnBoardComplete)
                        {
                            string? data = null;

                            if (args.Contains("data"))
                            {
                                data = args["data"];
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
                        break;

                    default:
                        break;
                }
            });
        }
#endif

#if HAS_UNO || NETFX_CORE
        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);
            args.Window.Closed += Window_Closed;
        }

        private void Window_Closed(object sender, CoreWindowEventArgs e)
        {
            Radar.MapControlCreator.Instance?.RemoveMapControl();
        }
#endif

#if HAS_UNO || NETFX_CORE
        private void OnLeavingBackground(object sender, Windows.ApplicationModel.LeavingBackgroundEventArgs e)
        {
            AppState = AppState.Foreground;
            UpdateColorValues();
            UISettings.ColorValuesChanged += DefaultTheme_ColorValuesChanged;
        }

        private void OnEnteredBackground(object sender, Windows.ApplicationModel.EnteredBackgroundEventArgs e)
        {
            AppState = AppState.Background;
            UISettings.ColorValuesChanged -= DefaultTheme_ColorValuesChanged;
        }
#endif

        private async Task Initialize(Microsoft.UI.Xaml.LaunchActivatedEventArgs? e)
        {
#if WINDOWS && !HAS_UNO
            _window = new MainWindow();
            _window.Activate();
            MainWindow.Current = _window;
#else
            _window = MainWindow.Current;
#endif

            RootFrame = _window?.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (RootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                RootFrame = new Frame();

                RootFrame.NavigationFailed += OnNavigationFailed;

#if WINDOWS
                if (e?.UWPLaunchActivatedEventArgs?.PreviousExecutionState == Windows.ApplicationModel.Activation.ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }
#endif

                // Place the frame in the current Window
                _window.Content = RootFrame;
            }

            if (_window.Content is FrameworkElement)
            {
                UpdateAppTheme();
            }

#if WINDOWS_UWP
            // TitleBar
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                _window.SizeChanged += async (sender, eventArgs) =>
                {
                    if (ApplicationView.GetForCurrentView()?.Orientation == ApplicationViewOrientation.Landscape)
                        await StatusBar.GetForCurrentView()?.HideAsync();
                    else
                        await StatusBar.GetForCurrentView()?.ShowAsync();
                };
            }
#endif

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

#if HAS_UNO || NETFX_CORE
        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
            Initialized = false;
        }
#endif

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.WriteLine(LoggerLevel.Fatal, e.Exception, "Unhandled Exception: {0}", e.Message);

            // Log inner exceptions
            if (e.Exception is AggregateException agg)
            {
                foreach (Exception inner in agg.InnerExceptions)
                {
                    Logger.WriteLine(LoggerLevel.Fatal, inner, "Unhandled Exception: {0}", inner.Message);
                }
            }
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
                var isDarkTheme = false;

                switch (SettingsManager.UserTheme)
                {
                    case UserThemeMode.System:
                        window.RequestedTheme = IsSystemDarkTheme ? ElementTheme.Dark : ElementTheme.Light;
                        isDarkTheme = IsSystemDarkTheme;
                        break;

                    case UserThemeMode.Light:
                        window.RequestedTheme = ElementTheme.Light;
                        isDarkTheme = false;
                        break;

                    case UserThemeMode.Dark:
                        window.RequestedTheme = ElementTheme.Dark;
                        isDarkTheme = true;
                        break;
                }
            }
        }

        private void App_OnSettingsChanged(SettingsChangedEventArgs e)
        {
#if WINDOWS
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var isWeatherLoaded = localSettings.Values[SettingsManager.KEY_WEATHERLOADED] as bool? ?? false;
#else
            var isWeatherLoaded = SettingsManager.WeatherLoaded;
#endif

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

        /// <summary>
        /// Configures global Uno Platform logging
        /// </summary>
        private static void InitializeLogging()
        {
#if DEBUG
            // Logging is disabled by default for release builds, as it incurs a significant
            // initialization cost from Microsoft.Extensions.Logging setup. If startup performance
            // is a concern for your application, keep this disabled. If you're running on web or 
            // desktop targets, you can use url or command line parameters to enable it.
            //
            // For more performance documentation: https://platform.uno/docs/articles/Uno.WinUI-Performance.html

            var factory = LoggerFactory.Create(builder =>
            {
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                builder.AddDebug();
#else
                builder.AddConsole();
#endif

                // Exclude logs below this level
                builder.SetMinimumLevel(LogLevel.Information);

                // Default filters for Uno Platform namespaces
                builder.AddFilter("Uno", LogLevel.Debug);
                builder.AddFilter("Windows", LogLevel.Warning);
                builder.AddFilter("Microsoft", LogLevel.Warning);

                // Generic Xaml events
                builder.AddFilter("Microsoft.UI.Xaml", LogLevel.Warning);
                builder.AddFilter("Microsoft.UI.Xaml.VisualStateGroup", LogLevel.Warning);
                builder.AddFilter("Microsoft.UI.Xaml.StateTriggerBase", LogLevel.Warning);
                builder.AddFilter("Microsoft.UI.Xaml.UIElement", LogLevel.Warning);
                builder.AddFilter("Microsoft.UI.Xaml.FrameworkElement", LogLevel.Warning); // Default: Trace

                // Layouter specific messages
                builder.AddFilter("Microsoft.UI.Xaml.Controls", LogLevel.Warning);
                builder.AddFilter("Microsoft.UI.Xaml.Controls.Layouter", LogLevel.Warning);
                builder.AddFilter("Microsoft.UI.Xaml.Controls.Panel", LogLevel.Warning);

                builder.AddFilter("Windows.Storage", LogLevel.Warning);

                // Binding related messages
                builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug);

                // Binder memory references tracking
                // builder.AddFilter("Uno.WinUI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

                // RemoteControl and HotReload related
                builder.AddFilter("Uno.WinUI.RemoteControl", LogLevel.Warning);

                // Debug JS interop
                // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
            });

            global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
            global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
        }
    }
}