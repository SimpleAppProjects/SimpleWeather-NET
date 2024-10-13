using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Controls;

public partial class ForecastRangeBar : ContentView
{
	private readonly Color[] DEFAULT_COLORS = { Colors.OrangeRed, Colors.LightSkyBlue };

	public ForecastRangeBarEntry Data
	{
		get => BindingContext as ForecastRangeBarEntry;
	}

	public FlexLayout InnerBarView => InnerBar;
	public View RangeBarView => RangeBar;

    public ForecastRangeBar()
	{
		InitializeComponent();
	}

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
		ApplyBindings();
    }

    public void SetColors() => SetColors(DEFAULT_COLORS);

	public void SetColors(params Color[] colors)
	{
		RangeBar.Background = new LinearGradientBrush()
		{
			StartPoint = new Point(0.5, 0),
			EndPoint = new Point(0.5, 1),
			GradientStops = new GradientStopCollection().Apply(c =>
			{
                colors.ForEachIndexed((index, color) =>
				{
					c.Add(new GradientStop(color, index / MathF.Max(1, colors.Length - 1)));
				});
			})
		};
	}
}
