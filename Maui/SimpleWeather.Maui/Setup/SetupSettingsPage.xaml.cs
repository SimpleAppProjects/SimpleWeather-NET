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
        TempSwitch.IsToggled = SettingsManager.TemperatureUnit == Units.CELSIUS;

        wm.UpdateAPI();

        // Refresh interval
        RefreshComboBox.ItemDisplayBinding = BindingBase.Create(static (ComboBoxItem item) => item.Display, BindingMode.OneWay);
        if (ExtrasService.IsPremiumEnabled())
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
        TempSwitch.Toggled += TempSwitch_Toggled;
        AlertSwitch.Toggled += AlertSwitch_Toggled;
        RefreshComboBox.SelectedIndexChanged += RefreshComboBox_SelectionChanged;
    }

    private async void AlertSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        var sw = sender as Switch;
        var result = e.Value;

        if (result)
        {
            if (!await NotificationPermissionRequestHelper.NotificationPermissionEnabled())
            {
                result = await NotificationPermissionRequestHelper.RequestNotificationPermission();
                sw.IsToggled = result;

                if (!result)
                {
                    return;
                }
            }
        }

        SettingsManager.ShowAlerts = result;
    }

    private void AlertSwitch_Tapped(object sender, TappedEventArgs e)
    {
        AlertSwitch.IsToggled = !AlertSwitch.IsToggled;
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

    private void Scroller_SizeChanged(object sender, EventArgs e)
    {
        if (DeviceInfo.Idiom != DeviceIdiom.Phone)
        {
            StackContainer.WidthRequest = Scroller.Width * 0.75;
        }
    }

    private void TempSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        SettingsManager.SetDefaultUnits(e.Value ? Units.CELSIUS : Units.FAHRENHEIT);
        TempSummary.Text = e.Value ? ResStrings.pref_summary_celsius : ResStrings.pref_summary_fahrenheit;
    }

    private void UnitSwitch_Tapped(object sender, TappedEventArgs e)
    {
        TempSwitch.IsToggled = !TempSwitch.IsToggled;
    }

    private void RefreshComboBox_HandlerChanged(object sender, EventArgs e)
    {
#if __IOS__
        if (sender is VisualElement element && element.Handler?.PlatformView is UIKit.UITextField view)
        {
            view.Layer.MasksToBounds = true;
            view.Layer.CornerRadius = 4f;
            view.Layer.BorderColor = CoreGraphics.CGColor.CreateSrgb(0.53333f, 0.53333f, 0.53333f, 1f);
            view.Layer.BorderWidth = 1f;
        }
#endif
    }
}