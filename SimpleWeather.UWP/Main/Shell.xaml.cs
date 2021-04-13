using Microsoft.UI.Xaml.Controls;
using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Preferences;
using SimpleWeather.UWP.Shared.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using muxc = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        public Frame AppFrame { get { return FrameContent; } }
        public static Shell Instance { get; set; }

        private UISettings UISettings;

        // List of ValueTuple holding the Navigation Tag and the relative Navigation Page
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("WeatherNow", typeof(WeatherNow)),
            ("WeatherAlertPage", typeof(WeatherAlertPage)),
            ("WeatherRadarPage", typeof(WeatherRadarPage)),
            ("LocationsPage", typeof(LocationsPage)),
            ("SettingsPage", typeof(SettingsPage)),
        };

        public Shell()
        {
            Instance = this;
            this.InitializeComponent();

            AnalyticsLogger.LogEvent("Shell");

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested += Shell_BackRequested;

            AppFrame.Navigated += AppFrame_Navigated;
            AppFrame.CacheSize = 1;

            // Add keyboard accelerators for backwards navigation.
            var goBack = new KeyboardAccelerator { Key = Windows.System.VirtualKey.GoBack };
            goBack.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(goBack);
            if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.UIElement", "KeyboardAcceleratorPlacementMode"))
            {
                this.KeyboardAcceleratorPlacementMode = KeyboardAcceleratorPlacementMode.Hidden;
            }

            // ALT routes here
            var altLeft = new KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.Left,
                Modifiers = Windows.System.VirtualKeyModifiers.Menu
            };
            altLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(altLeft);

            UISettings = new UISettings();
            UISettings.ColorValuesChanged += UISettings_ColorValuesChanged;
            UpdateAppTheme();
        }

        private async void UISettings_ColorValuesChanged(UISettings sender, object args)
        {
            // NOTE: Run on UI Thread since this may be called off the main thread
            await Dispatcher?.RunOnUIThread(() =>
            {
                if (AppFrame == null)
                    return;

                if (Settings.UserTheme == UserThemeMode.System)
                {
                    var uiTheme = sender.GetColorValue(UIColorType.Background).ToString();
                    bool isDarkTheme = uiTheme == "#FF000000";

                    AnalyticsLogger.LogEvent("Shell: UISettings_ColorValuesChanged",
                        new Dictionary<string, string>()
                        {
                            { "UITheme", uiTheme },
                            { "IsSystemDarkTheme", isDarkTheme.ToString() }
                        });

                    AppFrame.RequestedTheme = isDarkTheme ? ElementTheme.Dark : ElementTheme.Light;
                }
            });
        }

        public void UpdateAppTheme()
        {
            if (AppFrame == null)
                return;

            bool isDarkTheme = false;

            switch (Settings.UserTheme)
            {
                case UserThemeMode.System:
                    var uiTheme = UISettings.GetColorValue(UIColorType.Background).ToString();
                    isDarkTheme = uiTheme == "#FF000000";
                    break;

                case UserThemeMode.Light:
                    isDarkTheme = false;
                    break;

                case UserThemeMode.Dark:
                    isDarkTheme = true;
                    break;
            }

            FrameworkElement window = Window.Current.Content as FrameworkElement;
            AppFrame.RequestedTheme = window.RequestedTheme = isDarkTheme ? ElementTheme.Dark : ElementTheme.Light;

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            bool suppressNavigate = false;

            if ("suppressNavigate".Equals(e?.Parameter?.ToString()))
                suppressNavigate = true;

            AnalyticsLogger.LogEvent("Shell: OnNavigatedTo",
                new Dictionary<string, string>()
                {
                    { "suppressNavigate", suppressNavigate.ToString() }
                });

            // Navigate to WeatherNow page
            if (AppFrame.Content == null && !suppressNavigate)
            {
                AppFrame.Navigate(typeof(WeatherNow), e.Parameter, new Windows.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
            }

            // Setup background task
            Task.Run(async () => 
            {
                await WeatherTileUpdaterTask.RegisterBackgroundTask(false);
                await WeatherUpdateBackgroundTask.RegisterBackgroundTask(false);
            });
        }

        private void AppFrame_Navigated(object sender, NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;

            if (e.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavView.SelectedItem = (muxc.NavigationViewItem)NavView.SettingsItem;
            }
            else if (e.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                NavView.SelectedItem = NavView.MenuItems
                    .OfType<muxc.NavigationViewItem>()
                    .FirstOrDefault(n => n.Tag.Equals(item.Tag));
            }

            UpdateCommandBar();
        }

        private async void Shell_BackRequested(object sender, BackRequestedEventArgs e)
        {
            await On_BackRequested().ConfigureAwait(true);
            e.Handled = true;
        }

        private async void NavView_BackRequested(muxc.NavigationView sender, muxc.NavigationViewBackRequestedEventArgs args)
        {
            await On_BackRequested().ConfigureAwait(true);
        }

        private async void BackInvoked(KeyboardAccelerator sender,
                         KeyboardAcceleratorInvokedEventArgs args)
        {
            await On_BackRequested().ConfigureAwait(true);
            args.Handled = true;
        }

        private async Task<bool> On_BackRequested()
        {
            if (!AppFrame.CanGoBack)
                return false;

            // Navigate back if possible, and if the event has not
            // already been handled.
            bool PageRequestedToStay = AppFrame.Content is IBackRequestedPage backPage &&
                await backPage.OnBackRequested().ConfigureAwait(true);

            if (PageRequestedToStay)
                return false;

            // Don't go back if the nav pane is overlayed.
            if (NavView.IsPaneOpen &&
                (NavView.DisplayMode == muxc.NavigationViewDisplayMode.Compact ||
                 NavView.DisplayMode == muxc.NavigationViewDisplayMode.Minimal))
                return false;

            AppFrame.GoBack();
            return true;
        }

        private void NavView_ItemInvoked(muxc.NavigationView sender, muxc.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                NavView_Navigate("SettingsPage", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(
            string navItemTag,
            Windows.UI.Xaml.Media.Animation.NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "SettingsPage")
            {
                _page = typeof(SettingsPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = AppFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                object parameter = null;

                if (Type.Equals(preNavPageType, typeof(WeatherNow)))
                {
                    var wnowPage = AppFrame.Content as WeatherNow;

                    if (Type.Equals(_page, typeof(WeatherAlertPage)))
                    {
                        parameter = new WeatherPageArgs()
                        {
                            Location = wnowPage?.locationData,
                            WeatherNowView = wnowPage?.WeatherView
                        };
                    }
                    else if (Type.Equals(_page, typeof(WeatherRadarPage)))
                    {
                        parameter = wnowPage?.WeatherView?.LocationCoord;
                    }
                }

                AppFrame.Navigate(_page, parameter, transitionInfo);
            }
        }

        public void RequestCommandBarUpdate()
        {
            UpdateCommandBar();
        }

        private void UpdateCommandBar()
        {
            if (AppFrame.Content is ICommandBarPage cmdBarPage)
            {
                CommandBarTitle.Text = cmdBarPage.CommandBarLabel;
                CommandBar.Children.Clear();
                if (cmdBarPage.PrimaryCommands != null)
                    cmdBarPage.PrimaryCommands.ForEach(cmd => CommandBar.Children.Add(cmd));
            }
            else
            {
                CommandBarTitle.Text = App.ResLoader.GetString("AppName/Text");
                CommandBar.Children.Clear();
            }
        }

        private void TogglePaneButton_Click(object sender, RoutedEventArgs e)
        {
            NavView.IsPaneOpen = !NavView.IsPaneOpen;
        }
    }
}