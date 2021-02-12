using SimpleWeather.Icons;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Preferences
{
    public sealed partial class IconRadioPreference : UserControl
    {
        private readonly String[] PREVIEW_ICONS = { WeatherIcons.DAY_SUNNY, WeatherIcons.NIGHT_CLEAR, WeatherIcons.DAY_SUNNY_OVERCAST, WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY, WeatherIcons.RAIN };
        public String Key { get; private set; }

        public IconRadioPreference()
        {
            this.InitializeComponent();
            this.ActualThemeChanged += IconRadioPreference_ActualThemeChanged;
        }

        private void IconRadioPreference_ActualThemeChanged(FrameworkElement sender, object args)
        {
            if (Key != null)
            {
                var provider = SimpleLibrary.GetInstance().GetIconProvider(Key);
                if (provider != null)
                {
                    SetIconProvider(provider);
                }
            }
        }

        public IconRadioPreference(WeatherIconProvider provider) : this()
        {
            SetIconProvider(provider);
        }

        public event RoutedEventHandler RadioButtonChecked;

        public bool IsChecked
        {
            get { return PreferenceRadioButton.IsChecked.GetValueOrDefault(); }
            set { PreferenceRadioButton.IsChecked = value; }
        }

        public void SetIconProvider(WeatherIconProvider provider)
        {
            var isLightObj = this.Resources["IsLight"] as Helpers.ObjectContainer;
            bool isLight = false;

            if (isLightObj?.Value is Color paramColor)
            {
                isLight = paramColor == Colors.Black;
            }
            else if (isLightObj?.Value is bool)
            {
                isLight = (bool)isLightObj.Value;
            }

            IconPreference.Text = provider.DisplayName;
            Tag = Key = provider.Key;

            IconContainer.Children.Clear();
            foreach (var icon in PREVIEW_ICONS)
            {
                IconContainer.Children.Add(new BitmapIcon()
                {
                    Height = 30,
                    Width = 30,
                    Margin = new Thickness(5, 0, 5, 0),
                    ShowAsMonochrome = provider.IsFontIcon,
                    UriSource = new Uri(provider.GetWeatherIconURI(icon, true, isLight))
                });
            }
        }

        private void UserControl_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            RadioButtonChecked?.Invoke(this, e);
        }

        private void PreferenceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked?.Invoke(this, e);
        }
    }
}