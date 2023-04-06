using System.Collections.ObjectModel;
using System.Globalization;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;

namespace SimpleWeather.Maui.Controls;

public partial class WeatherDetailExtraPanel : ContentView
{
    public WeatherDetailExtraViewModel ViewModel { get; }
    private readonly WeatherIconsManager wim;

    public bool UseMonochrome
    {
        get => (bool)GetValue(UseMonochromeProperty);
        set => SetValue(UseMonochromeProperty, value);
    }

    public static readonly BindableProperty UseMonochromeProperty =
        BindableProperty.Create(nameof(UseMonochrome), typeof(bool), typeof(WeatherDetailPanel), false);

    public event EventHandler CloseButtonClicked;

    public WeatherDetailExtraPanel()
    {
        InitializeComponent();
        wim = SharedModule.Instance.WeatherIconsManager;
        ViewModel = new WeatherDetailExtraViewModel();
        this.BindingContextChanged += (sender, args) =>
        {
            if (BindingContext is HourlyForecastItemViewModel)
                ViewModel.SetForecast(BindingContext as HourlyForecastItemViewModel);
            else if (BindingContext is ForecastItemViewModel)
                ViewModel.SetForecast(BindingContext as ForecastItemViewModel);

            ApplyBindings();
            UseMonochrome = wim.ShouldUseMonochrome();
        };
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        CloseButtonClicked?.Invoke(this, EventArgs.Empty);
    }
}
