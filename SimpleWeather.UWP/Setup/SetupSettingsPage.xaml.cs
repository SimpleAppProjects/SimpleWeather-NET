using Microsoft.Extensions.DependencyInjection;
using SimpleWeather.Extras;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupSettingsPage : Page, IPageVerification
    {
        private readonly WeatherManager wm;
        private readonly IExtrasService ExtrasService = App.Services.GetService<IExtrasService>();

        private readonly IReadOnlyList<SimpleWeather.Controls.ComboBoxItem> RefreshOptions = new List<SimpleWeather.Controls.ComboBoxItem>
        {
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_60min"), "60"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_2hrs"), "120"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_3hrs"), "180"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_6hrs"), "360"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_12hrs"), "720"),
        };

        private readonly IReadOnlyList<SimpleWeather.Controls.ComboBoxItem> PremiumRefreshOptions = new List<SimpleWeather.Controls.ComboBoxItem>
        {
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_30min"), "30"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_60min"), "60"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_2hrs"), "120"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_3hrs"), "180"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_6hrs"), "360"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_12hrs"), "720"),
        };

        public SetupSettingsPage()
        {
            this.InitializeComponent();

            wm = WeatherManager.GetInstance();
            RestoreSettings();
            AnalyticsLogger.LogEvent("SetupSettingsPage");
        }

        private void RestoreSettings()
        {
            // Temperature
            Fahrenheit.IsChecked = true;
            Celsius.IsChecked = false;

            wm.UpdateAPI();

            // Refresh interval
            if (ExtrasService.IsEnabled())
            {
                RefreshComboBox.ItemsSource = PremiumRefreshOptions;
            }
            else
            {
                RefreshComboBox.ItemsSource = RefreshOptions;
            }
            RefreshComboBox.DisplayMemberPath = "Display";
            RefreshComboBox.SelectedValuePath = "Value";
            RefreshComboBox.SelectedValue = Settings.DefaultInterval.ToInvariantString();

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
            object value = box.SelectedValue;

            if (!int.TryParse(value?.ToString(), out int interval))
            {
                interval = Settings.DefaultInterval;
            }

            if (CoreApplication.Properties.ContainsKey(Settings.KEY_REFRESHINTERVAL))
                CoreApplication.Properties.Remove(Settings.KEY_REFRESHINTERVAL);
            CoreApplication.Properties.Add(Settings.KEY_REFRESHINTERVAL, interval);
        }

        private void Fahrenheit_Checked(object sender, RoutedEventArgs e)
        {
            if (CoreApplication.Properties.ContainsKey(Settings.KEY_TEMPUNIT))
                CoreApplication.Properties.Remove(Settings.KEY_TEMPUNIT);
            CoreApplication.Properties.Add(Settings.KEY_TEMPUNIT, Units.FAHRENHEIT);
        }

        private void Celsius_Checked(object sender, RoutedEventArgs e)
        {
            if (CoreApplication.Properties.ContainsKey(Settings.KEY_TEMPUNIT))
                CoreApplication.Properties.Remove(Settings.KEY_TEMPUNIT);
            CoreApplication.Properties.Add(Settings.KEY_TEMPUNIT, Units.CELSIUS);
        }

        public bool CanContinue()
        {
            return true;
        }
    }
}
