using SimpleWeather.Common.Controls;

namespace SimpleWeather.Maui.Controls;

public partial class AQIControl : ContentView
{
    public AirQualityViewModel ViewModel => this.BindingContext as AirQualityViewModel;

    public AQIControl()
    {
        InitializeComponent();
        this.BindingContextChanged += (sender, args) => { ApplyBindings(); };
    }
}