using SimpleWeather.Common.Controls;

namespace SimpleWeather.Maui.Controls;

public partial class AQIForecastControl : ContentView
{
    public AirQualityViewModel ViewModel => this.BindingContext as AirQualityViewModel;

    public AQIForecastControl()
    {
        InitializeComponent();
        this.BindingContextChanged += (sender, args) => { ApplyBindings(); };
    }
}