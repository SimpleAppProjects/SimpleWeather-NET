using SimpleWeather.Common.Controls;

namespace SimpleWeather.Maui.Controls;

public partial class PollenCountControl : ContentView
{
    public PollenViewModel ViewModel
    {
        get => this.BindingContext as PollenViewModel;
    }

    public PollenCountControl()
    {
        InitializeComponent();
        this.BindingContextChanged += (sender, args) =>
        {
            ApplyBindings();
        };
    }
}
