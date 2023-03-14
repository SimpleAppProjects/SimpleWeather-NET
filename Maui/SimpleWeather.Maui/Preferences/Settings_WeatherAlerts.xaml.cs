using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using SimpleWeather.Extras;
using SimpleWeather.Maui.Controls;
using SimpleWeather.NET.Controls;
using SimpleWeather.Preferences;
using SimpleWeather.RemoteConfig;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Preferences;

public partial class Settings_WeatherAlerts : ContentPage, IRecipient<SettingsChangedMessage>
{
    private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    private readonly IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();

    public Settings_WeatherAlerts()
	{
		InitializeComponent();

        RestoreSettings();

        App.Current.Resources.TryGetValue("LightPrimary", out var LightPrimary);
        App.Current.Resources.TryGetValue("DarkPrimary", out var DarkPrimary);
        SettingsTable.UpdateCellColors(
            Colors.Black, Colors.White, Colors.DimGray, Colors.LightGray,
            LightPrimary as Color, DarkPrimary as Color);

        // Event Listeners
        this.SettingsTable.Model.ItemSelected += Model_ItemSelected;
        AlertPref.OnChanged += AlertPref_OnChanged;
        PoPChancePref.OnChanged += PoPChancePref_OnChanged;
        WeakReferenceMessenger.Default.Register(this);
    }

    private void RestoreSettings()
    {
        // Daily Notification
        AlertPref.IsEnabled = wm.SupportsAlerts;
        AlertPref.On = SettingsManager.ShowAlerts;
        PoPChancePref.On = SettingsManager.PoPChanceNotificationEnabled;
        PoPChancePctPref.SelectedItem = SettingsManager.PoPChanceMinimumPercentage.ToInvariantString();
        PoPChancePctPref.Detail = PoPChancePctPref.Items.OfType<PreferenceListItem>().First(it => Equals(it.Value, PoPChancePctPref.SelectedItem)).Display;
    }

    private async void Model_ItemSelected(object sender, Microsoft.Maui.Controls.Internals.EventArg<object> e)
    {
        if (e.Data is ListViewCell listPref)
        {
            await Navigation.PushAsync(new PreferenceListDialogPage(listPref));
        }
    }

    public void Receive(SettingsChangedMessage message)
    {
        switch (message.Value.PreferenceKey)
        {
            case SettingsManager.KEY_POPCHANCEPCT:
                {
                    if (int.TryParse(message.Value.NewValue.ToString(), out int pct))
                    {
                        SettingsManager.PoPChanceMinimumPercentage = pct;
                    }
                }
                break;
        }

        RestoreSettings();
    }

    private void AlertPref_OnChanged(object sender, ToggledEventArgs e)
    {
        SwitchCell sw = sender as SwitchCell;
        SettingsManager.ShowAlerts = sw.On;
    }

    private void PoPChancePref_OnChanged(object sender, ToggledEventArgs e)
    {
        SwitchCell sw = sender as SwitchCell;


        if (sw.On)
        {
            // if !BackgroundAccess
                // alert
                // return
        }

        if (sw.On && ExtrasService.IsEnabled())
        {
            SettingsManager.PoPChanceNotificationEnabled = true;
            // TODO: Re-register background task if needed
        }
        else
        {
            if (sw.On && !ExtrasService.IsEnabled())
            {
                // TODO: show premium popup
                SettingsManager.PoPChanceNotificationEnabled = sw.On = false;
                //App.Current.Navigation.PushAsync(// PremiumPage)
            }
            else
            {
                SettingsManager.PoPChanceNotificationEnabled = sw.On = false;
            }
        }
    }
}
