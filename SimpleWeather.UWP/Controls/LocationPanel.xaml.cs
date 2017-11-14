using SimpleWeather.Controls;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
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

        public LocationPanel()
        {
            this.InitializeComponent();
            this.SizeChanged += LocationPanel_SizeChanged;
            this.DataContextChanged += (sender, args) => this.Bindings.Update();
        }

        private void LocationPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LocationName.Width = e.NewSize.Width - TempBox.Width - IconBox.Width;
        }
    }
}
