using SimpleWeather.Common.Controls;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class PollenCountControl : UserControl
    {
        public PollenViewModel ViewModel
        {
            get { return (this.DataContext as PollenViewModel); }
        }

        public PollenCountControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();
            };
        }
    }
}
