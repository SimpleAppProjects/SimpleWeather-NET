using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common.Helpers;
using SimpleWeather.Controls;
using SimpleWeather.Extras;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Setup;

public partial class SetupSettingsPage : BaseSetupPage, IPageVerification
{
    private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
    private readonly IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();
    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    private readonly List<ComboBoxItem> RefreshOptions = new()
    {
        new ComboBoxItem(ResStrings.refresh_60min, "60"),
        new ComboBoxItem(ResStrings.refresh_2hrs, "120"),
        new ComboBoxItem(ResStrings.refresh_3hrs, "180"),
        new ComboBoxItem(ResStrings.refresh_6hrs, "360"),
        new ComboBoxItem(ResStrings.refresh_12hrs, "720"),
    };

    private readonly List<ComboBoxItem> PremiumRefreshOptions = new()
    {
        new ComboBoxItem(ResStrings.refresh_30min, "30"),
        new ComboBoxItem(ResStrings.refresh_60min, "60"),
        new ComboBoxItem(ResStrings.refresh_2hrs, "120"),
        new ComboBoxItem(ResStrings.refresh_3hrs, "180"),
        new ComboBoxItem(ResStrings.refresh_6hrs, "360"),
        new ComboBoxItem(ResStrings.refresh_12hrs, "720"),
    };

    public SetupSettingsPage(SetupViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();

        RestoreSettings();
        AnalyticsLogger.LogEvent("SetupSettingsPage");
    }

    private void RestoreSettings()
    {
        // Temperature
        Fahrenheit.IsChecked = SettingsManager.TemperatureUnit == Units.FAHRENHEIT;
        Celsius.IsChecked = SettingsManager.TemperatureUnit == Units.CELSIUS;

        wm.UpdateAPI();

        // Refresh interval
        RefreshComboBox.ItemDisplayBinding = new Binding("Display");
        if (ExtrasService.IsEnabled())
        {
            RefreshComboBox.ItemsSource = PremiumRefreshOptions;
            RefreshComboBox.SelectedItem = PremiumRefreshOptions.First(it => Equals(it.Value, SettingsManager.RefreshInterval.ToInvariantString()));
        }
        else
        {
            RefreshComboBox.ItemsSource = RefreshOptions;
            RefreshComboBox.SelectedItem = RefreshOptions.First(it => Equals(it.Value, SettingsManager.RefreshInterval.ToInvariantString()));
        }
        RefreshComboBox.MinWidth(260);

        // Alerts
        AlertSwitch.IsEnabled = wm.SupportsAlerts;
        AlertSwitch.IsToggled = SettingsManager.ShowAlerts;

        Fahrenheit.CheckedChanged += Fahrenheit_Checked;
        Celsius.CheckedChanged += Celsius_Checked;
        AlertSwitch.Toggled += AlertSwitch_Toggled;
        RefreshComboBox.SelectedIndexChanged += RefreshComboBox_SelectionChanged;
    }

    private async void AlertSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        var sw = sender as Switch;

        if (e.Value)
        {
            if (!await NotificationPermissionRequestHelper.NotificationPermissionEnabled())
            {
                sw.IsToggled = false;
                await NotificationPermissionRequestHelper.RequestNotificationPermission();
                return;
            }
        }

        SettingsManager.ShowAlerts = e.Value;
    }

    private void RefreshComboBox_SelectionChanged(object sender, EventArgs e)
    {
        Picker box = sender as Picker;

        if (box.SelectedIndex != -1)
        {
            var value = box.ItemsSource.OfType<ComboBoxItem>().ElementAt(box.SelectedIndex);

            if (!int.TryParse(value?.Value?.ToString(), out int interval))
            {
                interval = SettingsManager.DefaultInterval;
            }

            SettingsManager.RefreshInterval = interval;
        }
        else
        {
            SettingsManager.RefreshInterval = SettingsManager.DefaultInterval;
        }
    }

    private void Fahrenheit_Checked(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            SettingsManager.SetDefaultUnits(Units.FAHRENHEIT);
        }
    }

    private void Celsius_Checked(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            SettingsManager.SetDefaultUnits(Units.CELSIUS);
        }
    }

    public bool CanContinue()
    {
        return true;
    }

    private void RadioButton_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is RadioButton button && button.IsEnabled)
        {
            button.IsChecked = true;
        }
    }

    private void RadioButton_Clicked(object sender, EventArgs e)
    {
#if WINDOWS || MACCATALYST
        if (sender is RadioButton button && button.IsEnabled)
        {
            button.IsChecked = true;
        }
#endif
    }
}