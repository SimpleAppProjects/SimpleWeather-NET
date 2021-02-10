using SimpleWeather.Controls;
using SimpleWeather.Icons;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class LocationPanel : UserControl
    {
        public LocationPanelViewModel ViewModel
        {
            get { return (this.DataContext as LocationPanelViewModel); }
        }

        private readonly WeatherIconsManager wim = WeatherIconsManager.GetInstance();

        public LocationPanel()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();
            };
        }
    }
}
