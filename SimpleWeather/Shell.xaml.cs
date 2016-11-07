using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        public Frame AppFrame { get { return Content; } }

        public Shell()
        {
            this.InitializeComponent();

            AppFrame.Navigating += AppFrame_Navigating;

            if (AppFrame.Content == null)
            {
                AppFrame.Navigate(typeof(WeatherNow));
            }
        }

        private void AppFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.SourcePageType == typeof(WeatherNow))
            {
                WeatherButton.Background = new SolidColorBrush(MainPage.AppColor);

                SettingsButton.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            }
            else if (e.SourcePageType == typeof(SettingsPage))
            {
                SettingsButton.Background = new SolidColorBrush(MainPage.AppColor);

                WeatherButton.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            HamBurgerMenu.IsPaneOpen = !HamBurgerMenu.IsPaneOpen;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.GetType() != typeof(SettingsPage))
                AppFrame.Navigate(typeof(SettingsPage), SettingsButton.Tag);
        }

        private void WeatherButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.GetType() != typeof(WeatherNow))
                AppFrame.Navigate(typeof(WeatherNow), WeatherButton.Tag);
        }
    }
}