using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Markup;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Controls;

public partial class ForecastBar : ContentView
{
	public Grid InnerBarView => InnerBar;
	public View BarView => Bar;
	public IconControl BarIconView => BarIcon;
	public Label BarDateBlock => BarDate;
	public Label BarValueBlock => BarValue;

	public BarGraphEntry Data => BindingContext as BarGraphEntry;

	public ForecastBar()
	{
		InitializeComponent();
		this.Bar.SizeChanged += Bar_SizeChanged;
	}

	private void Bar_SizeChanged(object sender, EventArgs e)
	{
		var largestSize = Math.Min(Bar.Height, Bar.Width);
		Bar.CornerRadius = new CornerRadius(largestSize / 2);
	}

	protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
		ApplyBindings();
		BarIcon.UpdateWeatherIcon();
    }

	public void SetColors(Color color)
	{
		Bar.Fill = new SolidColorBrush(color);
	}
}
