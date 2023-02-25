using SimpleWeather.NET.Controls;

namespace SimpleWeather.Maui.Controls;

public partial class HourlyForecastItem : ContentView
{
	public HourlyForecastNowViewModel ViewModel
	{
		get => BindingContext as HourlyForecastNowViewModel;
	}

	public HourlyForecastItem()
	{
		InitializeComponent();
	}

    private void HourlyForecastItem_BindingContextChanged(object sender, EventArgs e)
    {
		ApplyBindings();
    }
}
