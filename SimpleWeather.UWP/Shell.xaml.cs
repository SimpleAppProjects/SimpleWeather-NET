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

        public Color HamburgerButtonColor
        {
            get { return (CommandBar.Background as SolidColorBrush).Color; }
            set { (CommandBar.Background as SolidColorBrush).Color = value; }
        }

        private List<MenuItem> MainMenuItems { get; }
        private List<MenuItem> OptionMenuItems { get; }

        public Shell()
        {
            Instance = this;
            this.InitializeComponent();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested += Shell_BackRequested;

            MainMenuItems = new List<MenuItem>
            {
                new MenuItem()
                {
                    Icon = WeatherIcons.DAY_SUNNY,
                    Name = App.ResLoader.GetString("Nav_WeatherNow/Text"),
                    PageType = typeof(WeatherNow),
                    FontFamily = new FontFamily("/Assets/WeatherIcons/weathericons-regular-webfont.ttf#Weather Icons")
                },
                new MenuItem()
                {
                    Icon = "\uEA37",
                    Name = App.ResLoader.GetString("Nav_Locations/Text"),
                    PageType = typeof(LocationsPage),
                    FontFamily = new FontFamily("Segoe MDL2 Assets")
                },
                new MenuItem()
                {
                    Icon = "\uE713",
                    Name = App.ResLoader.GetString("Nav_Settings/Text"),
                    PageType = typeof(SettingsPage),
                    FontFamily = new FontFamily("Segoe MDL2 Assets")
                },
            };
            OptionMenuItems = null;

            HamBurgerMenu.ItemsSource = MainMenuItems;
            HamBurgerMenu.OptionsItemsSource = OptionMenuItems;

            AppFrame.Navigating += AppFrame_Navigating;
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

            int idx = MainMenuItems.FindIndex(item => item.PageType == e.SourcePageType);
            if (idx >= 0) HamBurgerMenu.SelectedIndex = idx;

            if (e.SourcePageType != typeof(WeatherNow))
            {
                HamburgerButtonColor = App.AppColor;
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    // Mobile
                    StatusBar.GetForCurrentView().BackgroundColor = HamburgerButtonColor;
                }
                else
                {
                    // Desktop
                    var titlebar = ApplicationView.GetForCurrentView().TitleBar;
                    titlebar.BackgroundColor = HamburgerButtonColor;
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

        private void AppFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (HamBurgerMenu.IsPaneOpen)
                HamBurgerMenu.IsPaneOpen = !HamBurgerMenu.IsPaneOpen;
        }

        private void Shell_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (AppFrame == null)
                return;

            // Navigate back if possible, and if the event has not 
            // already been handled .
            if (AppFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                AppFrame.GoBack();
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            HamBurgerMenu.IsPaneOpen = !HamBurgerMenu.IsPaneOpen;
        }

        private void HamBurgerMenu_ItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as MenuItem;

            if (menuItem.PageType != AppFrame.SourcePageType)
            {
                AppFrame.Navigate(menuItem.PageType, null);

                if (menuItem.PageType == typeof(WeatherNow))
                {
                    try
                    {
                        AppFrame.BackStack.Clear();
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    }
                    catch (System.Runtime.InteropServices.COMException ex)
                    {
                        Logger.WriteLine(LoggerLevel.Error, ex, "Exception!!");
                    }
                }
            }
        }
    }

    internal class MenuItem
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public Type PageType { get; set; }
        public FontFamily FontFamily { get; set; }
    }
}