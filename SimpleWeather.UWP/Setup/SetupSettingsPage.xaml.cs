using SimpleWeather.Utils;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupSettingsPage : Page, IPageVerification
    {
        private WeatherManager wm;

        public SetupSettingsPage()
        {
            this.InitializeComponent();

            wm = WeatherManager.GetInstance();
            RestoreSettings();
        }

        private void RestoreSettings()
        {
            // Temperature
            Fahrenheit.IsChecked = true;
            Celsius.IsChecked = false;

            wm.UpdateAPI();

            // Update Interval
            RefreshComboBox.SelectedIndex = 0; // 1hr

            // Alerts
            AlertSwitch.IsEnabled = wm.SupportsAlerts;
            AlertSwitch.IsOn = false;

            Fahrenheit.Checked += Fahrenheit_Checked;
            Celsius.Checked += Celsius_Checked;
            AlertSwitch.Toggled += AlertSwitch_Toggled;
            RefreshComboBox.SelectionChanged += RefreshComboBox_SelectionChanged;
        }

        private void AlertSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch sw = sender as ToggleSwitch;
            if (CoreApplication.Properties.ContainsKey(Settings.KEY_USEALERTS))
                CoreApplication.Properties.Remove(Settings.KEY_USEALERTS);
            CoreApplication.Properties.Add(Settings.KEY_USEALERTS, sw.IsOn);
        }

        private void RefreshComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            int index = box.SelectedIndex;
            int interval = 60;

            if (index == 0)
                interval = 60; // 1 hr
            else if (index == 1)
                interval = 120; // 2 hr
            else if (index == 2)
                interval = 180; // 3 hr
            else if (index == 3)
                interval = 360; // 6 hr
            else if (index == 4)
                interval = 720; // 12 hr

            if (CoreApplication.Properties.ContainsKey(Settings.KEY_REFRESHINTERVAL))
                CoreApplication.Properties.Remove(Settings.KEY_REFRESHINTERVAL);
            CoreApplication.Properties.Add(Settings.KEY_REFRESHINTERVAL, interval);
        }

        private void Fahrenheit_Checked(object sender, RoutedEventArgs e)
        {
            if (CoreApplication.Properties.ContainsKey(Settings.KEY_UNITS))
                CoreApplication.Properties.Remove(Settings.KEY_UNITS);
            CoreApplication.Properties.Add(Settings.KEY_UNITS, Settings.Fahrenheit);
        }

        private void Celsius_Checked(object sender, RoutedEventArgs e)
        {
            if (CoreApplication.Properties.ContainsKey(Settings.KEY_UNITS))
                CoreApplication.Properties.Remove(Settings.KEY_UNITS);
            CoreApplication.Properties.Add(Settings.KEY_UNITS, Settings.Celsius);
        }

        public bool CanContinue()
        {
            return true;
        }
    }
}
