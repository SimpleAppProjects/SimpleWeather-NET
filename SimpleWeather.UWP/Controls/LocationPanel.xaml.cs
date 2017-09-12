using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class LocationPanel : UserControl
    {
        public LocationPanel()
        {
            this.InitializeComponent();
            SizeChanged += LocationPanel_SizeChanged;
        }

        private void LocationPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LocationName.Width = e.NewSize.Width - TempBox.ActualWidth - IconBox.ActualWidth;
        }
    }
}
