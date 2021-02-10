using SimpleWeather.Controls;
using SimpleWeather.Utils;
using System;
using Windows.UI;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class WeatherAlertPanel : UserControl
    {
        public WeatherAlertViewModel WeatherAlert
        {
            get { return (this.DataContext as WeatherAlertViewModel); }
        }

        public Color AlertHeaderColor { get; set; }
        public String AlertIconSrc { get; set; }

        public WeatherAlertPanel()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) =>
            {
                if (WeatherAlert != null)
                {
                    AlertHeaderColor = WeatherUtils.GetColorFromAlertSeverity(WeatherAlert.AlertSeverity);
                    AlertIconSrc = WeatherUtils.GetAssetFromAlertType(WeatherAlert.AlertType);
                }

                this.Bindings.Update();
            };
        }
    }
}
