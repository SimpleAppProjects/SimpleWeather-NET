using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;

namespace SimpleWeather.Maui.Controls;

public partial class BeaufortControl : ContentView
{
    public BeaufortViewModel ViewModel
    {
        get { return (this.BindingContext as BeaufortViewModel); }
    }

    private readonly WeatherIconsManager wim = SharedModule.Instance.WeatherIconsManager;

    public BeaufortControl()
    {
        this.InitializeComponent();
        this.BindingContextChanged += (sender, args) =>
        {
            ApplyBindings();
            this.BeaufortIcon.UpdateWeatherIcon();
        };
    }
}
