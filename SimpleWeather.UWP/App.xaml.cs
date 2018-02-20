using Microsoft.QueryStringDotNET;
using Newtonsoft.Json;
using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SimpleWeather.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static readonly Color AppColor = Color.FromArgb(255, 0, 111, 191);
        public const int HomeIdx = 0;
        public static ResourceLoader ResLoader;
        public static Frame RootFrame { get; set; }
        public static bool IsInBackground { get; set; } = true;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.EnteredBackground += OnEnteredBackground;
            this.LeavingBackground += OnLeavingBackground;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                //this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            await Initialize(e);

            if (e.PrelaunchActivated == false)
            {
                if (RootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    if (Settings.WeatherLoaded)
                        RootFrame.Navigate(typeof(Shell), e.Arguments);
                    else
                        RootFrame.Navigate(typeof(SetupPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        private async Task Initialize(IActivatedEventArgs e)
        {
            RootFrame = Window.Current.Content as Frame;

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
                ResLoader = new ResourceLoader();

            // Load data if needed
            await Settings.LoadIfNeeded();

            // TitleBar
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                // Mobile
                StatusBar.GetForCurrentView().BackgroundOpacity = 1;
                StatusBar.GetForCurrentView().BackgroundColor = App.AppColor;
                StatusBar.GetForCurrentView().ForegroundColor = Colors.White;

                Window.Current.SizeChanged += async (sender, eventArgs) =>
                {
                    if (ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape)
                        await StatusBar.GetForCurrentView().HideAsync();
                    else
                        await StatusBar.GetForCurrentView().ShowAsync();
                };
            }
            else
            {
                // Desktop
                var titlebar = ApplicationView.GetForCurrentView().TitleBar;
                titlebar.BackgroundColor = App.AppColor;
                titlebar.ButtonBackgroundColor = titlebar.BackgroundColor;
                titlebar.ForegroundColor = Colors.White;
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
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
        }

        protected override async void OnActivated(IActivatedEventArgs e)
        {
            base.OnActivated(e);

            // Handle toast activation
            if (e is ToastNotificationActivatedEventArgs)
            {
                // Get the root frame
                RootFrame = Window.Current.Content as Frame;

                var toastActivationArgs = e as ToastNotificationActivatedEventArgs;

                // Parse the query string (using QueryString.NET)
                QueryString args = QueryString.Parse(toastActivationArgs.Argument);

                if (!args.Contains("action"))
                    return;

                // See what action is being requested 
                switch (args["action"])
                {
                    case "view-alerts":
                        if (Settings.WeatherLoaded)
                        {
                            String key = args["query"];

                            // App loaded for first time
                            await Initialize(e);

                            if (RootFrame.Content == null)
                            {
                                RootFrame.Navigate(typeof(Shell), "suppressNavigate");
                            }

                            if (Shell.Instance != null)
                            {
                                var weather = await Settings.GetWeatherData(key);
                                weather.weather_alerts = await Settings.GetWeatherAlertData(key);

                                // If we're already on WeatherNow navigate to Alert page
                                if (Shell.Instance.AppFrame.Content != null && Shell.Instance.AppFrame.SourcePageType.IsTypeOf(typeof(WeatherNow)))
                                {
                                    Shell.Instance.AppFrame.Navigate(typeof(WeatherAlertPage), new WeatherNowViewModel(weather));
                                }
                                // If not clear backstack and navigate to Alert page
                                // Add a WeatherNow page in backstack to go back to
                                else
                                {
                                    Shell.Instance.AppFrame.Navigate(typeof(WeatherAlertPage), new WeatherNowViewModel(weather));
                                    Shell.Instance.AppFrame.BackStack.Clear();
                                    Shell.Instance.AppFrame.BackStack.Add(new PageStackEntry(typeof(WeatherNow), null, null));
                                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                                }
                            }
                        }
                        break;
                }
            }

            // TODO: Handle other types of activation

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Event fired when a Background Task is activated (in Single Process Model)
        /// </summary>
        /// <param name="args">Arguments that describe the BackgroundTask activated</param>
        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            var deferral = args.TaskInstance.GetDeferral();

            switch (args.TaskInstance.Task.Name)
            {
                case "WeatherUpdateBackgroundTask":
                    new BackgroundTasks.WeatherUpdateBackgroundTask().Run(args.TaskInstance);
                    break;
            }

            deferral.Complete();
        }

        private void OnLeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            IsInBackground = false;
        }

        private void OnEnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            IsInBackground = true;
        }
    }
}
