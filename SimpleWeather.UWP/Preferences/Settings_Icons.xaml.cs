using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace SimpleWeather.UWP.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings_Icons : Page
    {
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

            var currentProvider = Settings.IconProvider;

            var providers = SimpleLibrary.GetInstance().GetIconProviders().Values
                .Cast<WeatherIconProvider>();

            foreach (var provider in providers)
            {
                var radioBtn = new IconRadioPreference()
                {
                    Padding = new Thickness(0, 10, 0, 10),
                    RequestedTheme = this.ActualTheme
                };
                radioBtn.SetIconProvider(provider);

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
            Settings.IconProvider = key;
            WeatherIconsManager.GetInstance().UpdateIconProvider();
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
                await WeatherUpdateBackgroundTask.RequestAppTrigger();
                // Refresh page
                this.Frame.Navigate(this.GetType());
            }
        }
    }
}
