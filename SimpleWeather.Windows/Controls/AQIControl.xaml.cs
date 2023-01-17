using SimpleWeather.Common.Controls;
using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls
{
    public sealed partial class AQIControl : UserControl
    {
        public AirQualityViewModel ViewModel
        {
            get { return (this.DataContext as AirQualityViewModel); }
        }

        public AQIControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();
            };
        }
    }
}
