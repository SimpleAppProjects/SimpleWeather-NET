using SimpleWeather.Controls;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class LocationQuery : UserControl
    {
        public LocationQueryViewModel ViewModel
        {
            get { return (this.DataContext as LocationQueryViewModel); }
        }

        public LocationQuery()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) => this.Bindings.Update();
        }
    }
}
