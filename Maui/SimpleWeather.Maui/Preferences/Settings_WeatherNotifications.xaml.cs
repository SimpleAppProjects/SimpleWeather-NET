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

public partial class Settings_WeatherNotifications : ContentPage, IRecipient<SettingsChangedMessage>
{
    private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    private readonly IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();

    public Settings_WeatherNotifications()
	{
		InitializeComponent();

		RestoreSettings();

        // Event Listeners
        this.SettingsTable.Model.ItemSelected += Model_ItemSelected;
        DailyNotifPref.OnChanged += DailyNotifPref_OnChanged;
        DailyNotifTimePref.PropertyChanged += DailyNotifTimePref_PropertyChanged;
    }

    private void RestoreSettings()
    {
        // Daily Notification
        DailyNotifPref.On = SettingsManager.DailyNotificationEnabled;
        DailyNotifTimePref.Time = SettingsManager.DailyNotificationTime;
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
        }

        RestoreSettings();
    }

    // TODO: Daily notification task
    private async void DailyNotifPref_OnChanged(object sender, ToggledEventArgs e)
    {
        SwitchCell sw = sender as SwitchCell;

        if (sw.On)
        {
            // if !BackgroundAccessEnabled
        }

        if (sw.On && ExtrasService.IsEnabled())
        {
            SettingsManager.DailyNotificationEnabled = true;
            // Register task
        }
        else
        {
            if (sw.On && !ExtrasService.IsEnabled())
            {
                // TODO: show premium popup                
            }
            SettingsManager.DailyNotificationEnabled = sw.On = false;
            // TODO: unregister task
        }
    }

    private void DailyNotifTimePref_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DailyNotifTimePref.Time))
        {
            SettingsManager.DailyNotificationTime = DailyNotifTimePref.Time;

            if (SettingsManager.DailyNotificationEnabled)
            {
                // TODO: register task
            }
        }
    }
}
