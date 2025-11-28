using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Markup;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Controls;

public partial class ForecastRangeBar : ContentView
{
	public View InnerBarView => InnerBar;
	public View RangeBarView => RangeBar;
	public IconControl BarIconView => BarIcon;
	public Label BarDateBlock => BarDate;
	public View BarPoPView => BarPoP;
	
	public double BarHiLoHeight => BarHi.Height + BarLo.Height;

	public ForecastRangeBarEntry Data => BindingContext as ForecastRangeBarEntry;
	
	private ForecastType? _forecastType = null;
	public ForecastType? ForecastType
	{
		get => _forecastType;
		set
		{
			_forecastType = value;
			BarPoP.IsVisible(_forecastType == NET.Controls.ForecastType.Temperature);
		}
	}

	public ForecastRangeBar()
	{
		InitializeComponent();
		this.RangeBar.SizeChanged += RangeBar_SizeChanged;
	}

	private void RangeBar_SizeChanged(object sender, EventArgs e)
	{
		var largestSize = Math.Min(RangeBar.Height, RangeBar.Width);
		RangeBar.CornerRadius = new CornerRadius(largestSize / 2);
	}

	protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
		ApplyBindings();
		BarIcon.UpdateWeatherIcon();
    }

    public void SetColors() => SetColors((Colors.OrangeRed, Colors.LightSkyBlue));

	public void SetColors((Color hiColor, Color loColor) colors)
	{
		RangeBar.Background = new LinearGradientBrush()
		{
			StartPoint = new Point(0.5, 0),
			EndPoint = new Point(0.5, 1),
			GradientStops = new GradientStopCollection().Apply(c =>
			{
				c.Add(new GradientStop()
				{
					Color = colors.hiColor,
					Offset = 0
				});
				c.Add(new GradientStop()
				{
					Color = colors.loColor,
					Offset = 1
				});
			})
		};
	}
}
