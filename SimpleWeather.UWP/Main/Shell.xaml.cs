using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.ComponentModel;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Preferences;
using SimpleWeather.UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

using muxc = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : ViewModelPage, IViewModelProvider, ISnackbarManager, IBannerManager
    {
        public Frame AppFrame { get { return FrameContent; } }
        public static Shell Instance { get; private set; }
        private SnackbarManager SnackMgr { get; set; }
        private BannerManager BannerMgr { get; set; }

        private readonly UISettings UISettings;
        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        // List of ValueTuple holding the Navigation Tag and the relative Navigation Page
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("WeatherNow", typeof(WeatherNow)),
            ("WeatherAlertPage", typeof(WeatherAlertPage)),
            ("WeatherRadarPage", typeof(WeatherRadarPage)),
            ("LocationsPage", typeof(LocationsPage)),
            ("SettingsPage", typeof(SettingsPage)),
        };

        public PageHeader PageHeader
        {
            get
            {
                return VisualTreeHelperExtensions.FindChild<PageHeader>(NavView);
            }
        }

        public Shell()
        {
            Instance = this;
            this.InitializeComponent();
            AnalyticsLogger.LogEvent("Shell");

            InitSnackManager();
            InitBannerManager();

            NavView.PaneDisplayMode = muxc.NavigationViewPaneDisplayMode.Auto;
            NavView.IsPaneOpen = false;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            AppFrame.CacheSize = 1;

            UISettings = new UISettings();
            UISettings.ColorValuesChanged += UISettings_ColorValuesChanged;
            App.Current.UpdateAppTheme();
            UpdateAppTheme();

            Window.Current.SetTitleBar(AppTitleBar);
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += (s, e) => UpdateAppTitle(s);

            // remove the solid-colored backgrounds behind the caption controls and system back button if we are in left mode
            // This is done when the app is loaded since before that the actual theme that is used is not "determined" yet
            Loaded += delegate (object sender, RoutedEventArgs e)
            {
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;

                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            };

            NavView.RegisterPropertyChangedCallback(muxc.NavigationView.PaneDisplayModeProperty, new DependencyPropertyChangedCallback(OnPaneDisplayModeChanged));
        }

        public void InitSnackManager()
        {
            if (SnackMgr == null)
            {
                SnackMgr = new SnackbarManager(SnackbarContainer);
            }
        }

        public void ShowSnackbar(Snackbar snackbar)
        {
            Dispatcher.RunOnUIThread(() =>
            {
                SnackMgr?.Show(snackbar);
            });
        }

        public void DismissAllSnackbars()
        {
            Dispatcher.RunOnUIThread(() =>
            {
                SnackMgr?.DismissAll();
            });
        }

        public void UnloadSnackManager()
        {
            DismissAllSnackbars();
            SnackMgr = null;
        }

        public void InitBannerManager()
        {
            if (BannerMgr == null)
            {
                BannerMgr = new BannerManager(BannerContainer);
            }
        }

        public void ShowBanner(Banner banner)
        {
            Dispatcher.RunOnUIThread(() =>
            {
                BannerMgr?.Show(banner);
            });
        }

        public void DismissBanner()
        {
            Dispatcher.RunOnUIThread(() =>
            {
                BannerMgr?.Dismiss();
            });
        }

        public void UnloadBannerManager()
        {
            DismissBanner();
            BannerMgr = null;
        }

        private async void UISettings_ColorValuesChanged(UISettings sender, object args)
        {
            // NOTE: Run on UI Thread since this may be called off the main thread
            await Dispatcher?.RunOnUIThread(() =>
            {
                if (AppFrame == null)
                    return;

                if (SettingsManager.UserTheme == UserThemeMode.System)
                {
                    var isDarkTheme = App.Current.IsSystemDarkTheme;

                    AnalyticsLogger.LogEvent("Shell: UISettings_ColorValuesChanged",
                        new Dictionary<string, string>()
                        {
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

            switch (SettingsManager.UserTheme)
            {
                case UserThemeMode.System:
                    isDarkTheme = App.Current.IsSystemDarkTheme;
                    break;

                case UserThemeMode.Light:
                    isDarkTheme = false;
                    break;

                case UserThemeMode.Dark:
                    isDarkTheme = true;
                    break;
            }

            AppFrame.RequestedTheme = isDarkTheme ? ElementTheme.Dark : ElementTheme.Light;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InitSnackManager();
            InitBannerManager();

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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            UnloadSnackManager();
            UnloadBannerManager();
        }

        private void FrameContent_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (e.Exception != null)
            {
                Logger.WriteLine(LoggerLevel.Error, e.Exception, "Failed to load page {0}", e.SourcePageType.FullName);
            }
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Add handler for ContentFrame navigation.
            AppFrame.Navigated += On_Navigated;

            // NavView doesn't load any page by default, so load home page.
            OnNavigated(AppFrame.SourcePageType);

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

            SystemNavigationManager.GetForCurrentView().BackRequested += Shell_BackRequested;

            NavView.PaneDisplayMode = muxc.NavigationViewPaneDisplayMode.Auto;
            NavView.IsPaneOpen = false;
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
            if (_page is not null && !Type.Equals(preNavPageType, _page))
            {
                AppFrame?.Navigate(_page, null, transitionInfo);
            }
        }

        private async void Shell_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = await TryGoBack().ConfigureAwait(true);
            }
        }

        private async void NavView_BackRequested(muxc.NavigationView sender, muxc.NavigationViewBackRequestedEventArgs args)
        {
            await TryGoBack().ConfigureAwait(true);
        }

        private async void BackInvoked(KeyboardAccelerator sender,
                         KeyboardAcceleratorInvokedEventArgs args)
        {
            if (!args.Handled)
            {
                args.Handled = await TryGoBack().ConfigureAwait(true);
            }
        }

        private async Task<bool> TryGoBack()
        {
            if (AppFrame?.CanGoBack == false)
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

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            OnNavigated(e.SourcePageType);
            DismissBanner();
            DismissAllSnackbars();
        }

        private void OnNavigated(Type sourcePageType)
        {
            NavView.IsBackEnabled = AppFrame.CanGoBack ? true : false;

            if (sourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavView.SelectedItem = (muxc.NavigationViewItem)NavView.SettingsItem;
            }
            else if (sourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == sourcePageType);

                NavView.SelectedItem = NavView.MenuItems
                    .OfType<muxc.NavigationViewItem>()
                    .FirstOrDefault(n => n.Tag.Equals(item.Tag));
            }

            UpdateCommandBar();
        }

        public void RequestCommandBarUpdate()
        {
            UpdateCommandBar();
        }

        private void UpdateCommandBar()
        {
            if (PageHeader is PageHeader header)
            {
                if (AppFrame.Content is ICommandBarPage cmdBarPage)
                {
                    header.Title = cmdBarPage.CommandBarLabel;
                    header.Commands = cmdBarPage.PrimaryCommands;
                }
                else
                {
                    header.Title = App.Current.ResLoader.GetString("app_name");
                    header.Commands = null;
                }
            }
        }

        private void OnPaneDisplayModeChanged(DependencyObject sender, DependencyProperty dp)
        {
            var navigationView = sender as muxc.NavigationView;
            AppTitleBar.Visibility = navigationView.PaneDisplayMode == muxc.NavigationViewPaneDisplayMode.Top ? Visibility.Collapsed : Visibility.Visible;
        }

        void UpdateAppTitle(CoreApplicationViewTitleBar coreTitleBar)
        {
            //ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void NavView_PaneOpening(muxc.NavigationView sender, object args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavView_PaneClosing(muxc.NavigationView sender, muxc.NavigationViewPaneClosingEventArgs args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavView_DisplayModeChanged(muxc.NavigationView sender, muxc.NavigationViewDisplayModeChangedEventArgs args)
        {
            Thickness currMargin = AppTitleBar.Margin;
            if (sender.DisplayMode == muxc.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness(sender.CompactPaneLength * 2, currMargin.Top, currMargin.Right, currMargin.Bottom);

            }
            else
            {
                AppTitleBar.Margin = new Thickness(sender.CompactPaneLength, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }

            UpdateAppTitleMargin(sender);
            UpdateHeaderMargin(sender);
        }

        private void UpdateAppTitleMargin(muxc.NavigationView sender)
        {
            const int smallLeftIndent = 4, largeLeftIndent = 24;

            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                AppTitle.TranslationTransition = new Vector3Transition();

                if ((sender.DisplayMode == muxc.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                         sender.DisplayMode == muxc.NavigationViewDisplayMode.Minimal)
                {
                    AppTitle.Translation = new System.Numerics.Vector3(smallLeftIndent, 0, 0);
                }
                else
                {
                    AppTitle.Translation = new System.Numerics.Vector3(largeLeftIndent, 0, 0);
                }
            }
            else
            {
                Thickness currMargin = AppTitle.Margin;

                if ((sender.DisplayMode == muxc.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                         sender.DisplayMode == muxc.NavigationViewDisplayMode.Minimal)
                {
                    AppTitle.Margin = new Thickness(smallLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
                else
                {
                    AppTitle.Margin = new Thickness(largeLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
            }
        }

        private void UpdateHeaderMargin(muxc.NavigationView sender)
        {
            if (PageHeader != null)
            {
                Thickness currMargin = PageHeader.Margin;

                if (sender.DisplayMode == muxc.NavigationViewDisplayMode.Minimal)
                {
                    PageHeader.Margin = new Thickness(currMargin.Left, sender.CompactPaneLength, currMargin.Right, currMargin.Bottom);
                }
                else
                {
                    PageHeader.Margin = new Thickness(currMargin.Left, 0, currMargin.Right, currMargin.Bottom);
                }
            }
        }
    }
}