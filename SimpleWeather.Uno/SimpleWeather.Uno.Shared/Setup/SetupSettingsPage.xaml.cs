using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Extras;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Uno.Helpers;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.Uno.Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupSettingsPage : Page, IPageVerification
    {
        private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
        private readonly IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();
        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        private readonly IReadOnlyList<SimpleWeather.Controls.ComboBoxItem> RefreshOptions = new List<SimpleWeather.Controls.ComboBoxItem>
        {
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_60min"), "60"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_2hrs"), "120"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_3hrs"), "180"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_6hrs"), "360"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_12hrs"), "720"),
        };

        private readonly IReadOnlyList<SimpleWeather.Controls.ComboBoxItem> PremiumRefreshOptions = new List<SimpleWeather.Controls.ComboBoxItem>
        {
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_30min"), "30"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_60min"), "60"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_2hrs"), "120"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_3hrs"), "180"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_6hrs"), "360"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_12hrs"), "720"),
        };

        public SetupSettingsPage()
        {
            this.InitializeComponent();

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
            RefreshComboBox.SelectedValue = SettingsManager.DefaultInterval.ToInvariantString();

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
            SettingsManager.ShowAlerts = sw.IsOn;
        }

        private void RefreshComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            object value = box.SelectedValue;

            if (!int.TryParse(value?.ToString(), out int interval))
            {
                interval = SettingsManager.DefaultInterval;
            }

            SettingsManager.RefreshInterval = interval;
        }

        private void Fahrenheit_Checked(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetDefaultUnits(Units.FAHRENHEIT);
        }

        private void Celsius_Checked(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetDefaultUnits(Units.CELSIUS);
        }

        public bool CanContinue()
        {
            return true;
        }
    }
}
