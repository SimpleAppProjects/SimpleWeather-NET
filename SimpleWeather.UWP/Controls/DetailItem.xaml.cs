using SimpleWeather.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
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

        public DetailItem()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();
                IconCtrl.UpdateWeatherIcon();
            };
        }
    }
}
