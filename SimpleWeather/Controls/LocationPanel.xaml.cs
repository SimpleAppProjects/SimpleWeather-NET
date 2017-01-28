using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.Controls
{
    public sealed partial class LocationPanel : Button
    {
        public LocationPanel()
        {
            this.InitializeComponent();

            LoadIt();
        }

        private async void LoadIt()
        {
            LoadingRing.IsActive = true;
            this.IsEnabled = false;

            while (this.LocationName.Text == null || this.LocationName.Text == string.Empty)
            {
                await Task.Delay(100);
            }

            LoadingRing.IsActive = false;
            this.IsEnabled = true;
        }
    }
}
