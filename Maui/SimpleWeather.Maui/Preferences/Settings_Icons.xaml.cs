using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Extras;
using SimpleWeather.Preferences;
using System;

namespace SimpleWeather.Maui.Preferences;

public partial class Settings_Icons : ContentPage
{
    private readonly IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();
    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    public Settings_Icons()
	{
		InitializeComponent();
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        Initialize();
    }

    private void Initialize()
    {
        IconRadioContainer.Children.Clear();

        var currentProvider = SettingsManager.IconProvider;

        var providers = SharedModule.Instance.WeatherIconsManager.GetIconProviders();

        foreach (var provider in providers)
        {
            var radioBtn = new IconRadioPreference(provider.Value)
            {
                Padding = new Thickness(0, 10, 0, 10),
            };

            IconRadioContainer.Children.Add(radioBtn);

            if (provider.Key == currentProvider)
            {
                radioBtn.IsChecked = true;
            }

            radioBtn.RadioButtonChecked += Preference_RadioButtonChecked;
        }
    }

    private void Preference_RadioButtonChecked(object sender, EventArgs e)
    {
        if (sender is IconRadioPreference selected)
        {
            var selectedKey = selected.Key;
            bool success = SetDefaultKey(selectedKey);
            if (success)
            {
                UpdateCheckedState(selectedKey);
            }
            OnSelectionPerformed(success);
        }
    }

    private bool SetDefaultKey(string key)
    {
        if (String.IsNullOrWhiteSpace(key))
        {
            return false;
        }
        if (!ExtrasService.IsIconPackSupported(key))
        {
            //Frame.Navigate(typeof(Extras.Store.PremiumPage));
            return false;
        }
        SettingsManager.IconProvider = key;
        SharedModule.Instance.WeatherIconsManager.UpdateIconProvider();
        return true;
    }

    private void UpdateCheckedState(string selectedKey)
    {
        foreach (var preference in IconRadioContainer.Children)
        {
            if (preference is IconRadioPreference radioPref)
            {
                bool newCheckedState = Equals(radioPref.Key, selectedKey);
                if (radioPref.IsChecked != newCheckedState)
                {
                    radioPref.IsChecked = Equals(radioPref.Key, selectedKey);
                }
            }
        }
    }

    private void OnSelectionPerformed(bool success)
    {
        if (success)
        {
            // Refresh page
        }
    }
}