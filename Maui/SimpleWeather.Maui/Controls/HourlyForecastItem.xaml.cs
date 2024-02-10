using SimpleWeather.NET.Controls;

namespace SimpleWeather.Maui.Controls;

public partial class HourlyForecastItem : ContentView
{
	public HourlyForecastNowViewModel ViewModel
	{
		get => BindingContext as HourlyForecastNowViewModel;
	}

	public string IconProvider
	{
		get => (string)GetValue(IconProviderProperty);
		set => SetValue(IconProviderProperty, value);
	}

	public static readonly BindableProperty IconProviderProperty =
		BindableProperty.Create(nameof(IconProvider), typeof(string), typeof(HourlyForecastItem), null);

	public HourlyForecastItem()
	{
		InitializeComponent();
	}

    private void HourlyForecastItem_BindingContextChanged(object sender, EventArgs e)
    {
		ApplyBindings();
    }
}
