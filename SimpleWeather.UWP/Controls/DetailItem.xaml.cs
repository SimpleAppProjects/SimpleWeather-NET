using SimpleWeather.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class DetailItem : UserControl
    {
        public DetailItemViewModel Details
        {
            get { return (this.DataContext as DetailItemViewModel); }
        }

        private readonly WeatherIconsManager wim;

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
            wim = WeatherIconsManager.GetInstance();
            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();
                IconCtrl.ShowAsMonochrome = wim.ShouldUseMonochrome(IconCtrl.IconProvider ?? Settings.IconProvider);
            };
        }
    }
}
