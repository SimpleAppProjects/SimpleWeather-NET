using System.Collections.ObjectModel;
using System.Globalization;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;

namespace SimpleWeather.Maui.Controls;

public partial class WeatherDetailPanel : ContentView
{
    public WeatherDetailViewModel ViewModel { get; }
    private readonly WeatherIconsManager wim;

    public bool UseMonochrome
    {
        get => (bool)GetValue(UseMonochromeProperty);
        set => SetValue(UseMonochromeProperty, value);
    }

    public static readonly BindableProperty UseMonochromeProperty =
        BindableProperty.Create(nameof(UseMonochrome), typeof(bool), typeof(WeatherDetailPanel), false);

    public WeatherDetailPanel()
	{
		InitializeComponent();
        wim = SharedModule.Instance.WeatherIconsManager;
        ViewModel = new WeatherDetailViewModel();
        this.BindingContextChanged += (sender, args) =>
        {
            if (BindingContext is HourlyForecastItemViewModel)
                ViewModel.SetForecast(BindingContext as HourlyForecastItemViewModel);
            else if (BindingContext is ForecastItemViewModel)
                ViewModel.SetForecast(BindingContext as ForecastItemViewModel);

            ApplyBindings();
            UseMonochrome = wim.ShouldUseMonochrome();
        };
        this.SizeChanged += (sender, args) =>
        {
            var txtblk = new Label()
            {
                Text = ConditionDescription.Text,
                FontSize = ConditionDescription.FontSize
            };
            var desiredSize = txtblk.Measure(double.PositiveInfinity, double.PositiveInfinity, MeasureFlags.IncludeMargins);

            if (!string.IsNullOrWhiteSpace(ViewModel.ConditionLongDesc))
            {
                ConditionDescription.IsVisible = true;
            }
            else
            {
                ConditionDescription.IsVisible = (this.Width - 50 - IconBox.Margin.Left - IconBox.Margin.Right) <= desiredSize.Request.Width;
            }
        };
	}

    private void DetailExpander_ExpandedChanged(object sender, CommunityToolkit.Maui.Core.ExpandedChangedEventArgs e)
    {
        (DetailExpanderIcon as VisualElement)?.RotateTo(e.IsExpanded ? 180 : 0);
    }
}
