using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Extras;
using SimpleWeather.Preferences;
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
#if WINDOWS
using SimpleWeather.Uno.BackgroundTasks;
#endif

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.Uno.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings_Icons : Page
    {
        private readonly IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();
        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public Settings_Icons()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Initialize();
        }

        private void Initialize()
        {
            IconRadioContainer.Children.Clear();

            var currentProvider = SettingsManager.IconProvider;

            var providers = SharedModule.Instance.WeatherIconsManager.GetIconProviders();

            foreach (var provider in providers)
            {
                var radioBtn = new IconRadioPreference()
                {
                    Padding = new Thickness(0, 10, 0, 10),
                    RequestedTheme = this.ActualTheme
                };
                radioBtn.SetIconProvider(provider.Value);

                IconRadioContainer.Children.Add(radioBtn);

                if (provider.Key == currentProvider)
                {
                    radioBtn.IsChecked = true;
                }

                radioBtn.RadioButtonChecked += Preference_RadioButtonChecked;
            }
        }

        private void Preference_RadioButtonChecked(object sender, RoutedEventArgs e)
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
                Frame.Navigate(typeof(Extras.Store.PremiumPage));
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

        private async void OnSelectionPerformed(bool success)
        {
            if (success)
            {
#if WINDOWS
                await WeatherUpdateBackgroundTask.RequestAppTrigger();
#endif
                // Refresh page
                this.Frame.Navigate(this.GetType());
            }
        }
    }
}
