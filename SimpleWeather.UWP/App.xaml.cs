using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.QueryStringDotNET;
using SimpleWeather.Controls;
using SimpleWeather.Keys;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Main;
using SimpleWeather.UWP.Preferences;
using SimpleWeather.UWP.Setup;
using SimpleWeather.UWP.Tiles;
using SimpleWeather.UWP.Utils;
using SimpleWeather.UWP.WNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;

namespace SimpleWeather.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        public const int HomeIdx = 0;

        public static ResourceLoader ResLoader { get; private set; }
        private UISettings UISettings;

        public static bool IsSystemDarkTheme { get; private set; }
        public static bool IsInBackground { get; private set; } = true;
        public static Frame RootFrame { get; set; }
        private static bool Initialized { get; set; } = false;

        public static Color AppColor
        {
            get
            {
                if (Window.Current?.Content is FrameworkElement window)
                {
                    if (window.RequestedTheme == ElementTheme.Light)
                    {
                        var brush = Application.Current.Resources["SimpleBlueBrush"] as SolidColorBrush;
                        return brush.Color;
                    }
                    else
                    {
                        var brush = Application.Current.Resources["SimpleBlueDarkBrush"] as SolidColorBrush;
                        return brush.Color;
                    }
                }
                else
                {
                    return Color.FromArgb(0xff, 0x00, 0x70, 0xc0);
                }
            }
        }

        public static ElementTheme CurrentTheme
        {
            get
            {
                if (Window.Current?.Content is FrameworkElement window)
                {
                    return window.RequestedTheme;
                }
                else
                {
                    return ElementTheme.Default;
                }
            }
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            CoreApplication.EnablePrelaunch(true);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.EnteredBackground += OnEnteredBackground;
            this.LeavingBackground += OnLeavingBackground;

            // During the transition from foreground to background the
            // memory limit allowed for the application changes. The application
            // has a short time to respond by bringing its memory usage
            // under the new limit.
            MemoryManager.AppMemoryUsageLimitChanging += MemoryManager_AppMemoryUsageLimitChanging;

            // After an application is backgrounded it is expected to stay
            // under a memory target to maintain priority to keep running.
            // Subscribe to the event that informs the app of this change.
            MemoryManager.AppMemoryUsageIncreased += MemoryManager_AppMemoryUsageIncreased;

            // Initialize depencies for library
            SimpleLibrary.GetInstance().OnCommonActionChanged += App_OnCommonActionChanged;
            Utf8Json.Resolvers.CompositeResolver.RegisterAndSetAsDefault(
                JSONParser.Resolver, UWP.Utf8JsonGen.Resolvers.GeneratedResolver.Instance
                );
            Extras.ExtrasLibrary.Init();

            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(APIKeys.GetAppCenterSecret(), typeof(Analytics), typeof(Crashes));

            UISettings = new UISettings();
            IsSystemDarkTheme = UISettings.GetColorValue(UIColorType.Background).ToString() == "#FF000000";
            switch (Settings.UserTheme)
            {
                case UserThemeMode.System:
                    RequestedTheme = IsSystemDarkTheme ? ApplicationTheme.Dark : ApplicationTheme.Light;
                    break;

                case UserThemeMode.Light:
                    RequestedTheme = ApplicationTheme.Light;
                    break;

                case UserThemeMode.Dark:
                    RequestedTheme = ApplicationTheme.Dark;
                    break;
            }
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
            if (IsInBackground && Window.Current?.Content != null)
            {
                // Some apps may wish to use this helper to explicitly disconnect
                // child references.
                // VisualTreeHelper.DisconnectChildrenRecursive(Window.Current.Content);

                // Clear the view content. Note that views should rely on
                // events like Page.Unloaded to further release resources.
                // Release event handlers in views since references can
                // prevent objects from being collected.
                Window.Current.Content = null;
            }

            // Run the GC to collect released resources.
            GC.Collect();
        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            base.OnActivated(e);
            Initialize(e);

            // Handle toast activation
            if (e is ToastNotificationActivatedEventArgs)
            {
                // Get the root frame
                RootFrame = Window.Current?.Content as Frame;

                var toastActivationArgs = e as ToastNotificationActivatedEventArgs;

                // Parse the query string (using QueryString.NET)
                var args = QueryString.Parse(toastActivationArgs.Argument);

                if (!args.Contains("action"))
                    return;

                // See what action is being requested
                switch (args["action"])
                {
                    case "view-alerts":
                        if (Settings.WeatherLoaded && Settings.OnBoardComplete)
                        {
                            string data = null;

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
                                LocationData locData = null;
                                if (!string.IsNullOrWhiteSpace(data))
                                {
                                    locData = JSONParser.Deserializer<LocationData>(data);
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
                                        IsHome = Object.Equals(locData, Settings.GetHomeData().Result)
                                    }, null));
                                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                                }
                            }
                        }
                        break;

                    case "check-updates":
                        if (Settings.WeatherLoaded && Settings.OnBoardComplete) 
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
                                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            // TODO: Handle other types of activation

            // Ensure the current window is active
            Window.Current.Activate();

            UpdateAppTheme();
        }

        /// <summary>
        /// Event fired when a Background Task is activated (in Single Process Model)
        /// </summary>
        /// <param name="args">Arguments that describe the BackgroundTask activated</param>
        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);
            Initialize(args);

            AnalyticsLogger.LogEvent("App: Background Activated",
                new Dictionary<string, string>()
                {
                    { "Task", args?.TaskInstance?.Task?.Name }
                });

            switch (args?.TaskInstance?.Task?.Name)
            {
                case nameof(WeatherUpdateBackgroundTask):
                    Logger.WriteLine(LoggerLevel.Debug, "App: Starting WeatherUpdateBackgroundTask");
                    new WeatherUpdateBackgroundTask().Run(args.TaskInstance);
                    break;

                case nameof(ImageDatabaseTask):
                    Logger.WriteLine(LoggerLevel.Debug, "App: Starting ImageDatabaseTask");
                    new ImageDatabaseTask().Run(args.TaskInstance);
                    break;

                case nameof(WNSPushBackgroundTask):
                    Logger.WriteLine(LoggerLevel.Debug, "App: Starting WNSPushBackgroundTask");
                    new WNSPushBackgroundTask().Run(args.TaskInstance);
                    break;

                case nameof(WNSWorkerBackgroundTask):
                    Logger.WriteLine(LoggerLevel.Debug, "App: Starting WNSWorkerBackgroundTask");
                    new WNSWorkerBackgroundTask().Run(args.TaskInstance);
                    break;

                case nameof(UpdateTask):
                    Logger.WriteLine(LoggerLevel.Debug, "App: Starting UpdateTask");
                    new UpdateTask().Run(args.TaskInstance);
                    break;

                case nameof(AppUpdaterTask):
                    Logger.WriteLine(LoggerLevel.Debug, "App: Starting AppUpdaterTask");
                    new AppUpdaterTask().Run(args.TaskInstance);
                    break;

                case nameof(RemoteConfigUpdateTask):
                    Logger.WriteLine(LoggerLevel.Debug, "App: Starting RemoteConfigUpdateTask");
                    new RemoteConfigUpdateTask().Run(args.TaskInstance);
                    break;

                default:
                    Logger.WriteLine(LoggerLevel.Debug, "App: Unknown task: {0}", args.TaskInstance.Task.Name);
                    break;
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            base.OnLaunched(e);
            Initialize(e);

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                //this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Logger.WriteLine(LoggerLevel.Info, "Started logger...");

            // App loaded for first time
            await WNSHelper.InitNotificationChannel().ConfigureAwait(true);
            await UpdateTask.RegisterBackgroundTask().ConfigureAwait(true);
            await AppUpdaterTask.RegisterBackgroundTask().ConfigureAwait(true);
            await RemoteConfigUpdateTask.RegisterBackgroundTask().ConfigureAwait(true);

            if (!e.PrelaunchActivated)
            {
                if (Settings.WeatherLoaded && Settings.OnBoardComplete && !String.IsNullOrEmpty(e.TileId) && !e.TileId.Equals("App", StringComparison.OrdinalIgnoreCase))
                {
                    if (RootFrame.Content == null)
                    {
                        RootFrame.Navigate(typeof(Shell), "suppressNavigate");
                    }

                    // Navigate to WeatherNow page for location
                    if (Shell.Instance != null)
                    {
                        if (e.TileId != null)
                        {
                            Shell.Instance.AppFrame.Navigate(typeof(WeatherNow), new WeatherNowArgs()
                            {
                                TileId = e.TileId
                            });
                            Shell.Instance.AppFrame.BackStack.Clear();
                            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                        }

                        // If Shell content is empty navigate to default page
                        if (Shell.Instance.AppFrame.CurrentSourcePageType == null)
                        {
                            Shell.Instance.AppFrame.Navigate(typeof(WeatherNow), new WeatherNowArgs()
                            {
                                IsHome = true
                            });
                            Shell.Instance.AppFrame.BackStack.Clear();
                            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                        }
                    }
                }

                if (RootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    if (Settings.WeatherLoaded && Settings.OnBoardComplete)
                        RootFrame.Navigate(typeof(Shell), e.Arguments);
                    else
                    {
                        UpdateAppTheme();
                        RootFrame.Navigate(typeof(SetupPage), e.Arguments);
                    }
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        private void OnLeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            IsInBackground = false;
            UpdateColorValues();
            UISettings.ColorValuesChanged += DefaultTheme_ColorValuesChanged;
        }

        private void OnEnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            IsInBackground = true;
            UISettings.ColorValuesChanged -= DefaultTheme_ColorValuesChanged;
        }

        private void Initialize(IActivatedEventArgs e)
        {
            RootFrame = Window.Current?.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (RootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                RootFrame = new Frame();

                RootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = RootFrame;
            }

            if (ResLoader == null)
                ResLoader = ResourceLoader.GetForCurrentView();

            // TitleBar
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                Window.Current.SizeChanged += async (sender, eventArgs) =>
                {
                    if (ApplicationView.GetForCurrentView()?.Orientation == ApplicationViewOrientation.Landscape)
                        await StatusBar.GetForCurrentView()?.HideAsync();
                    else
                        await StatusBar.GetForCurrentView()?.ShowAsync();
                };
            }

            // Load data if needed
            Settings.LoadIfNeededAsync();

            InitializeExtras();

            Initialized = true;
        }

        private void Initialize(IBackgroundActivatedEventArgs _)
        {
            if (Initialized) return;

            if (ResLoader == null)
                ResLoader = ResourceLoader.GetForViewIndependentUse();

            // Load data if needed
            Settings.LoadIfNeededAsync();

            InitializeExtras();

            Initialized = true;
        }

        private async void InitializeExtras()
        {
            try
            {
                if (await Extras.Store.WindowsStoreManager.CheckIfUserHasActiveSubscriptionAsync())
                {
                    // Enable extras
                    Extras.ExtrasLibrary.EnableExtras();
                }
                else
                {
                    // Disable extras
                    Extras.ExtrasLibrary.DisableExtras();
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e);
            }
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

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
            Initialized = false;
        }

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
                await DispatcherExtensions.TryRunOnUIThread(() =>
                {
                    var uiTheme = UISettings.GetColorValue(UIColorType.Background).ToString();
                    IsSystemDarkTheme = uiTheme == "#FF000000";

                    AnalyticsLogger.LogEvent("App: UpdateColorValues",
                        new Dictionary<string, string>()
                        {
                            { "UITheme", uiTheme },
                            { "IsSystemDarkTheme", IsSystemDarkTheme.ToString() }
                        });

                    if (Shell.Instance == null && Settings.UserTheme == UserThemeMode.System)
                    {
                        UpdateAppTheme();
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "App: UpdateColorValues: error updating color values");
            }
        }

        private static void UpdateAppTheme()
        {
            if (Shell.Instance != null) return;

            if (Window.Current?.Content is FrameworkElement window)
            {
                window.RequestedTheme = IsSystemDarkTheme ? ElementTheme.Dark : ElementTheme.Light;
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    // Mobile
                    var statusBar = StatusBar.GetForCurrentView();
                    if (statusBar != null)
                    {
                        statusBar.BackgroundColor = App.AppColor;
                        statusBar.ForegroundColor = Colors.White;
                    }
                }
                else
                {
                    // Desktop
                    var titlebar = ApplicationView.GetForCurrentView()?.TitleBar;
                    if (titlebar != null)
                    {
                        titlebar.BackgroundColor = App.AppColor;
                        titlebar.ButtonBackgroundColor = titlebar.BackgroundColor;
                        titlebar.ForegroundColor = Colors.White;
                    }
                }
            }
        }
    }
}