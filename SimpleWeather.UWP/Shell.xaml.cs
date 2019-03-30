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
                (NavBar.Background as SolidColorBrush).Color = value;
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
                        Glyph = WeatherIcons.DAY_SUNNY,
                        FontFamily = new FontFamily("/Assets/WeatherIcons/weathericons-regular-webfont.ttf#Weather Icons"),
                        Height = 60,
                        Width = 72,
                        FontSize = 60
                    },
                    Label = App.ResLoader.GetString("Nav_WeatherNow/Label"),
                    Tag = typeof(WeatherNow)
                },
                new AppBarButton()
                {
                    Icon = new FontIcon()
                    {
                        Glyph = "\uEA37",
                        FontFamily = new FontFamily("Segoe MDL2 Assets")
                    },
                    Label = App.ResLoader.GetString("Nav_Locations/Label"),
                    Tag = typeof(LocationsPage)
                },
                new AppBarButton()
                {
                    Icon = new SymbolIcon()
                    {
                        Symbol = Symbol.Setting
                    },
                    Label = App.ResLoader.GetString("Nav_Settings/Label"),
                    Tag = typeof(SettingsPage)
                }
            };
            FindName("NavBar");
            NavBar.PrimaryCommands.Clear();
            NavBarItems.ForEach(btn => 
            {
                btn.Click += NavBarItem_Click;
                NavBar.PrimaryCommands.Add(btn);
            });
            AppFrame.Navigated += AppFrame_Navigated;
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
                NavBar.PrimaryCommands.Clear();
                NavBarItems.ForEach(btn => 
                {
                    if (NavBarItems.IndexOf(btn) == idx)
                        btn.Foreground = new SolidColorBrush(Colors.White);
                    else
                        btn.Foreground = new SolidColorBrush(Color.FromArgb(0x80, 0xff, 0xff, 0xff));
                    NavBar.PrimaryCommands.Add(btn);
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

        private void Shell_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (AppFrame == null)
                return;

            // Navigate back if possible, and if the event has not 
            // already been handled .
            if (e.Handled == false)
            {
                e.Handled = true;
                AppFrame.Navigate(typeof(WeatherNow));
                try
                {
                    AppFrame.BackStack.Clear();
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "Exception!!");
                }
            }
        }

        private void NavBarItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarButton btn && btn.Tag is Type pageType && pageType != AppFrame.SourcePageType)
            {
                AppFrame.Navigate(pageType, null);

                if (pageType == typeof(WeatherNow))
                {
                    try
                    {
                        AppFrame.BackStack.Clear();
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(LoggerLevel.Error, ex, "Exception!!");
                    }
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