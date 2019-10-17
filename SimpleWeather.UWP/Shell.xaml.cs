using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        public Frame AppFrame { get { return FrameContent; } }
        public static Shell Instance { get; set; }

        private Color appbarColor = Color.FromArgb(0xff, 0x0, 0x70, 0xc0);
        public Color AppBarColor
        {
            get { return appbarColor; }
            set
            {
                appbarColor = value;
                (CommandBar.Background as SolidColorBrush).Color = value;
            }
        }

        private List<AppBarButton> NavBarItems { get; }

        public Shell()
        {
            Instance = this;
            this.InitializeComponent();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested += Shell_BackRequested;

            NavBarItems = new List<AppBarButton>
            {
                new AppBarButton()
                {
                    Icon = new FontIcon()
                    {
                        Glyph = WeatherIcons.DAY_CLOUDY,
                        FontFamily = new FontFamily("/Assets/WeatherIcons/weathericons-regular-webfont.ttf#Weather Icons"),
                        FontSize = 15
                    },
                    Label = App.ResLoader.GetString("Nav_WeatherNow/Label"),
                    Margin = new Thickness(5, 0, 5, 0),
                    Tag = typeof(WeatherNow),
                    Width = 100,
                    Style = this.Resources["AppBarButtonStyle"] as Style
                },
                new AppBarButton()
                {
                    Icon = new FontIcon()
                    {
                        Glyph = "\uEA37",
                        FontFamily = new FontFamily("Segoe MDL2 Assets")
                    },
                    Label = App.ResLoader.GetString("Nav_Locations/Label"),
                    Margin = new Thickness(5, 0, 5, 0),
                    Tag = typeof(LocationsPage),
                    Width = 100,
                    Style = this.Resources["AppBarButtonStyle"] as Style
                },
                new AppBarButton()
                {
                    Icon = new SymbolIcon()
                    {
                        Symbol = Symbol.Setting
                    },
                    Label = App.ResLoader.GetString("Nav_Settings/Label"),
                    Margin = new Thickness(5, 0, 5, 0),
                    Tag = typeof(SettingsPage),
                    Width = 100,
                    Style = this.Resources["AppBarButtonStyle"] as Style
                }
            };
            FindName("NavBar");
            NavBar.Children.Clear();
            NavBarItems.ForEach(btn => 
            {
                btn.Click += NavBarItem_Click;
                NavBar.Children.Add(btn);
            });
            AppFrame.Navigated += AppFrame_Navigated;
            AppFrame.CacheSize = 1;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
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
            Windows.ApplicationModel.Background.BackgroundExecutionManager.RemoveAccess();
            await WeatherUpdateBackgroundTask.RegisterBackgroundTask();
        }

        private void AppFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (AppFrame.CanGoBack)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            int idx = NavBarItems.FindIndex(item => item.Tag == e.SourcePageType);
            if (idx >=0)
            {
                NavBar.Children.Clear();
                NavBarItems.ForEach(btn => 
                {
                    if (NavBarItems.IndexOf(btn) == idx)
                        btn.Foreground = new SolidColorBrush(Colors.White);
                    else
                        btn.Foreground = new SolidColorBrush(Color.FromArgb(0x80, 0xff, 0xff, 0xff));
                    NavBar.Children.Add(btn);
                });
            }

            if (e.SourcePageType != typeof(WeatherNow))
            {
                AppBarColor = App.AppColor;
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    // Mobile
                    StatusBar.GetForCurrentView().BackgroundColor = AppBarColor;
                }
                else
                {
                    // Desktop
                    var titlebar = ApplicationView.GetForCurrentView().TitleBar;
                    titlebar.BackgroundColor = AppBarColor;
                    titlebar.ButtonBackgroundColor = titlebar.BackgroundColor;
                }
            }

            UpdateCommandBar();
        }

        private async void Shell_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (AppFrame == null)
                return;

            // Navigate back if possible, and if the event has not 
            // already been handled.
            bool PageRequestedToStay = e.Handled = AppFrame.Content is IBackRequestedPage backPage && await backPage.OnBackRequested();

            if (!PageRequestedToStay)
            {
                if (AppFrame.BackStackDepth > 0)
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
        }

        private void NavBarItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarButton btn && btn.Tag is Type pageType && pageType != AppFrame.SourcePageType)
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