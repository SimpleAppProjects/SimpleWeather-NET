using SimpleWeather.Controls;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class HourlyForecastItem : UserControl
    {
        public HourlyForecastItemViewModel HourlyForecasts
        {
            get { return (this.DataContext as HourlyForecastItemViewModel); }
        }

        public HourlyForecastItem()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();

                if (Utils.Settings.API.Equals(WeatherData.WeatherAPI.OpenWeatherMap) || 
                    Utils.Settings.API.Equals(WeatherData.WeatherAPI.MetNo))
                {
                    // Use cloudiness
                    PoPIcon.Text = "\uf013";
                }
                else
                {
                    PoPIcon.Text = "\uf078";
                }
            };
        }
    }
}
