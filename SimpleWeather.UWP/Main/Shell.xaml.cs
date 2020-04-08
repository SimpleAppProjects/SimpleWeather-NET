using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Preferences;
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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

        public static readonly DependencyProperty AppBarColorProperty =
            DependencyProperty.Register("AppBarColor", typeof(Color),
            typeof(Shell), new PropertyMetadata(Color.FromArgb(0xff, 0x0, 0x70, 0xc0)));

        public Color AppBarColor
        {
            get { return (Color)GetValue(AppBarColorProperty); }
            set { SetValue(AppBarColorProperty, value); }
        }

        private List<BottomNavigationBarItem> NavBarItems { get; }

        private UISettings UISettings;

        public Shell()
        {
            Instance = this;
            this.InitializeComponent();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested += Shell_BackRequested;

            NavBarItems = new List<BottomNavigationBarItem>
            {
                new BottomNavigationBarItem()
                {
                    Icon = new FontIcon()
                    {
                        Glyph = WeatherIcons.DAY_CLOUDY,
                        FontFamily = new FontFamily("/Assets/WeatherIcons/weathericons-regular-webfont.ttf#Weather Icons"),
                        FontSize = 15
                    },
                    Label = App.ResLoader.GetString("Nav_WeatherNow/Label"),
                    Tag = typeof(WeatherNow),
                },
                new BottomNavigationBarItem()
                {
                    Icon = new FontIcon()
                    {
                        Glyph = "\uEA37",
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        FontSize = 15
                    },
                    Label = App.ResLoader.GetString("Nav_Locations/Label"),
                    Tag = typeof(LocationsPage),
                },
                new BottomNavigationBarItem()
                {
                    Icon = new FontIcon()
                    {
                        Glyph = "\uE713",
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        FontSize = 15
                    },
                    Label = App.ResLoader.GetString("Nav_Settings/Label"),
                    Tag = typeof(SettingsPage),
                }
            };
            FindName("NavBar");
            NavBar.ItemsSource = NavBarItems;
            AppFrame.Navigated += AppFrame_Navigated;
            AppFrame.CacheSize = 1;

            UISettings = new UISettings();
            UISettings.ColorValuesChanged += UISettings_ColorValuesChanged;
            UpdateAppTheme();
            RegisterPropertyChangedCallback(AppBarColorProperty, Shell_PropertyChanged);
        }

        private async void UISettings_ColorValuesChanged(UISettings sender, object args)
        {
            // NOTE: Run on UI Thread since this may be called off the main thread
            await AsyncTask.RunOnUIThread(() =>
            {
                if (AppFrame == null)
                    return;

                if (Settings.UserTheme == UserThemeMode.System)
                {
                    var uiTheme = sender.GetColorValue(UIColorType.Background).ToString();
                    bool isDarkTheme = uiTheme == "#FF000000";
                    AppFrame.RequestedTheme = isDarkTheme ? ElementTheme.Dark : ElementTheme.Light;
                }
            }).ConfigureAwait(false);
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
            if (AppFrame.SourcePageType != typeof(WeatherNow))
                AppBarColor = App.AppColor;
        }

        private void Shell_PropertyChanged(DependencyObject sender, DependencyProperty property)
        {
            if (AppBarColorProperty == property)
            {
                Color color = AppBarColor;
                bool isLightBackground = ColorUtils.IsSuperLight(color);

                CommandBar.RequestedTheme = isLightBackground ? ElementTheme.Light : ElementTheme.Dark;
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    // Mobile
                    var statusBar = StatusBar.GetForCurrentView();
                    if (statusBar != null)
                    {
                        statusBar.BackgroundColor = color;
                        statusBar.ForegroundColor = isLightBackground ? Colors.Black : Colors.White;
                    }
                }
                else
                {
                    // Desktop
                    var titlebar = ApplicationView.GetForCurrentView()?.TitleBar;
                    if (titlebar != null)
                    {
                        titlebar.BackgroundColor = color;
                        titlebar.ButtonBackgroundColor = titlebar.BackgroundColor;
                        titlebar.ForegroundColor = isLightBackground ? Colors.Black : Colors.White;
                    }
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            bool suppressNavigate = false;

            if ("suppressNavigate".Equals(e?.Parameter?.ToString()))
                suppressNavigate = true;

            // Navigate to WeatherNow page
            if (AppFrame.Content == null && !suppressNavigate)
            {
                AppFrame.Navigate(typeof(WeatherNow), e.Parameter);
            }

            // Setup background task
            Task.Run(WeatherUpdateBackgroundTask.RegisterBackgroundTask);
        }

        private void AppFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (AppFrame.CanGoBack)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            int idx = NavBarItems.FindIndex(item => item.Tag as Type == e.SourcePageType);
            if (idx >= 0) NavBar.SelectedIndex = idx;

            if (e.SourcePageType != typeof(WeatherNow))
                AppBarColor = App.AppColor;

            UpdateCommandBar();
        }

        private async void Shell_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (AppFrame == null)
                return;

            // Navigate back if possible, and if the event has not
            // already been handled.
            bool PageRequestedToStay = e.Handled = AppFrame.Content is IBackRequestedPage backPage &&
                await backPage.OnBackRequested().ConfigureAwait(true);

            if (!PageRequestedToStay && AppFrame.BackStackDepth > 0)
            {
                try
                {
                    // Remove all from backstack except home
                    var home = AppFrame.BackStack.ElementAt(0);
                    AppFrame.BackStack.Clear();
                    AppFrame.BackStack.Add(home);
                    AppFrame.GoBack();
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "Exception!!");
                }

                e.Handled = true;
            }
        }

        private void NavBarItem_Seletected(object sender, SelectionChangedEventArgs e)
        {
            var navBar = sender as Selector;

            if (navBar?.SelectedItem is BottomNavigationBarItem btn && btn.Tag is Type pageType && pageType != AppFrame.SourcePageType)
            {
                if (pageType == typeof(WeatherNow) && AppFrame.BackStackDepth >= 1)
                {
                    try
                    {
                        // Remove all from backstack except home
                        var home = AppFrame.BackStack.ElementAt(0);
                        AppFrame.BackStack.Clear();
                        AppFrame.BackStack.Add(home);
                        AppFrame.GoBack();
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(LoggerLevel.Error, ex, "Exception!!");
                    }
                }
                else
                {
                    AppFrame.Navigate(pageType, null);
                }
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
                CommandBar.PrimaryCommands.Clear();
                if (cmdBarPage.PrimaryCommands != null)
                    cmdBarPage.PrimaryCommands.ForEach(cmd => CommandBar.PrimaryCommands.Add(cmd));
            }
            else
            {
                CommandBarTitle.Text = "SimpleWeather";
                CommandBar.PrimaryCommands.Clear();
            }
        }
    }
}