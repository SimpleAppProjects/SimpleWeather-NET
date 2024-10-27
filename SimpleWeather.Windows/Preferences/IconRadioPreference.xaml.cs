using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleWeather.Extras;
using SimpleWeather.Icons;
using SimpleWeather.NET.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Preferences
{
    public sealed partial class IconRadioPreference : UserControl
    {
        private readonly String[] PREVIEW_ICONS = { WeatherIcons.DAY_SUNNY, WeatherIcons.NIGHT_CLEAR, WeatherIcons.DAY_SUNNY_OVERCAST, WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY, WeatherIcons.RAIN };
        public String Key { get; private set; }

        private IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();
        private bool IsPremiumIcon => !ExtrasService.IsIconPackSupported(Key);

        public IconRadioPreference()
        {
            this.InitializeComponent();
            this.ActualThemeChanged += IconRadioPreference_ActualThemeChanged;
        }

        private void IconRadioPreference_ActualThemeChanged(FrameworkElement sender, object args)
        {
            if (Key != null)
            {
                var provider = SharedModule.Instance.WeatherIconsManager.GetIconProvider(Key);
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
            IconPreference.Text = provider.DisplayName;
            Tag = Key = provider.Key;

            IconContainer.Children.Clear();
            foreach (var icon in PREVIEW_ICONS)
            {
                IconContainer.Children.Add(new IconControl()
                {
                    IconHeight = 30,
                    IconWidth = 30,
                    Margin = new Thickness(5, 0, 5, 0),
                    WeatherIcon = icon,
                    IconProvider = provider.Key,
                    RequestedTheme = this.ActualTheme
                });
            }
        }

        private void UserControl_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            RadioButtonChecked?.Invoke(this, e);
        }

        private void PreferenceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked?.Invoke(this, e);
        }
    }
}