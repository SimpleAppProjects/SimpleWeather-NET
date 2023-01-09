using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.Uno.Controls
{
    public sealed partial class DetailItem : UserControl
    {
        public DetailItemViewModel Details
        {
            get { return (this.DataContext as DetailItemViewModel); }
        }

        private readonly WeatherIconsManager wim;
        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public bool UseMonochrome
        {
            get { return (bool)GetValue(UseMonochromeProperty); }
            set { SetValue(UseMonochromeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseMonochrome.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseMonochromeProperty =
            DependencyProperty.Register("UseMonochrome", typeof(bool), typeof(DetailItem), new PropertyMetadata(false));

        public DetailItem()
        {
            this.InitializeComponent();
            wim = SharedModule.Instance.WeatherIconsManager;
            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();
                IconCtrl.ShowAsMonochrome = wim.ShouldUseMonochrome(IconCtrl.IconProvider ?? SettingsManager.IconProvider);
            };
        }
    }
}
