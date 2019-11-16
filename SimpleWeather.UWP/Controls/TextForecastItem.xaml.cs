using SimpleWeather.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class TextForecastItem : UserControl
    {
        public TextForecastItemViewModel ViewModel
        {
            get { return (this.DataContext as TextForecastItemViewModel); }
        }

        public TextForecastItem()
        {
            this.InitializeComponent();
            this.SizeChanged += TextForecastItem_SizeChanged;
            this.DataContextChanged += (sender, args) => this.Bindings.Update();
        }

        private void TextForecastItem_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FctTextBox.Width = FctText.Width = e.NewSize.Width - WeatherBox.ActualWidth;
        }
    }
}
