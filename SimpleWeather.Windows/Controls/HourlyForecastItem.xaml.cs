using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls
{
    public sealed partial class HourlyForecastItem : UserControl
    {
        public HourlyForecastNowViewModel ViewModel
        {
            get => DataContext as HourlyForecastNowViewModel;
        }

        public string WeatherIcon
        {
            get { return (string)GetValue(WeatherIconProperty); }
            set { SetValue(WeatherIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WeatherIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeatherIconProperty =
            DependencyProperty.Register(nameof(WeatherIcon), typeof(string), typeof(HourlyForecastItem), new PropertyMetadata(null));

        public HourlyForecastItem()
        {
            this.InitializeComponent();
        }

        private void HourlyForecastItem_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            this.Bindings.Update();
        }
    }
}
