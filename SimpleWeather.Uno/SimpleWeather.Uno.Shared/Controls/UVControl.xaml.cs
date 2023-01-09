using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.Uno.Controls
{
    public sealed partial class UVControl : UserControl
    {
        public UVIndexViewModel ViewModel
        {
            get { return (this.DataContext as UVIndexViewModel); }
        }

        private readonly WeatherIconsManager wim;

        public bool UseMonochrome
        {
            get { return (bool)GetValue(UseMonochromeProperty); }
            set { SetValue(UseMonochromeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseMonochrome.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseMonochromeProperty =
            DependencyProperty.Register("UseMonochrome", typeof(bool), typeof(UVControl), new PropertyMetadata(false));

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public UVControl()
        {
            this.InitializeComponent();
            wim = SharedModule.Instance.WeatherIconsManager;
            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();
                UVIcon.ShowAsMonochrome = wim.ShouldUseMonochrome(UVIcon.IconProvider ?? SettingsManager.IconProvider);
            };
        }
    }
}
