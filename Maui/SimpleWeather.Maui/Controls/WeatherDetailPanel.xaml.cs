using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Maui.Markup;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SkiaSharp;

namespace SimpleWeather.Maui.Controls;

public partial class WeatherDetailPanel : ContentView
{
    public WeatherDetailViewModel ViewModel { get; }

    private bool isExpanded = false;

    public bool IsExpanded
    {
        get => isExpanded;
        set
        {
            if (isExpanded != value)
            {
                Toggle();
            }
        }
    }

    public bool IsExpandable { get; set; } = true;

    private bool ShouldShowBodyText { get; set; } = false;

    public WeatherDetailPanel()
	{
        ViewModel = new WeatherDetailViewModel();
        InitializeComponent();
        this.BindingContextChanged += (sender, args) =>
        {
            // Reset expanded state
            IsExpandable = true;
            IsExpanded = false;

            if (BindingContext is HourlyForecastItemViewModel)
            {
                ViewModel.SetForecast(BindingContext as HourlyForecastItemViewModel);
                ShouldShowBodyText = false;

                Dispatcher.Dispatch(() =>
                {
                    MeasureConditionDescTest(this.Width);
                });

                IsExpandable = ViewModel.Extras?.Count > 0;
            }
            else if (BindingContext is ForecastItemViewModel)
            {
                ViewModel.SetForecast(BindingContext as ForecastItemViewModel);
                ShouldShowBodyText = !string.IsNullOrWhiteSpace(ViewModel.ConditionLongDesc);
                IsExpandable = ShouldShowBodyText || ViewModel.Extras?.Count > 0;
            }
            else
            {
                IsExpandable = false;
            }

            ExpandIcon.IsVisible = IsExpandable;
            ApplyBindings();
        };
        HeaderFrame.TapGesture(Toggle);
        SetFrameColor();
	}

    private void SetFrameColor()
    {
        App.Current.Resources.TryGetValue("SimpleBlue", out var SimpleBlue);
        App.Current.Resources.TryGetValue("SimpleBlueLight", out var SimpleBlueLight);
        App.Current.Resources.TryGetValue("LightSurface", out var LightSurface);
        App.Current.Resources.TryGetValue("DarkSurface", out var DarkSurface);

        var simpleBlueLightColor = (Color)SimpleBlueLight;
        var simpleBlueColor = (Color)SimpleBlue;
        var lightSurfaceColor = (Color)LightSurface;
        var darkSurfaceColor = (Color)DarkSurface;

        if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
        {
            HeaderFrame.AppThemeColorBinding(BackgroundColorProperty, ColorUtils.CompositeColors(lightSurfaceColor, simpleBlueColor.WithAlpha(0x10)), ColorUtils.CompositeColors(Color.FromArgb("#242424"), simpleBlueLightColor.WithAlpha(0x10)));
        }
        else
        {
            HeaderFrame.AppThemeColorBinding(BackgroundColorProperty, ColorUtils.CompositeColors(lightSurfaceColor, simpleBlueColor.WithAlpha(0x10)), ColorUtils.CompositeColors(darkSurfaceColor, simpleBlueLightColor.WithAlpha(0x10)));
        }
    }

    private void Toggle()
    {
        if (IsExpandable && IsEnabled)
        {
            var entering = !isExpanded;

            isExpanded = !isExpanded;

            ConditionDescription.IsVisible = entering && ShouldShowBodyText;
            DetailsLayout.IsVisible = entering;
            ExpandIcon.RotateTo(entering ? 180 : 0, 250, Easing.CubicInOut);
            HeaderFrame.Shadow.Radius = entering ? 2 : 0.5f;

            Dispatcher.Dispatch(() =>
            {
                var elements = DetailsLayout.GetVisualTreeDescendants();
                elements.OfType<IconControl>().ForEach(ico =>
                {
                    ico.UpdateWeatherIcon();
                });
            });
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        MeasureConditionDescTest(width);
    }

    private void MeasureConditionDescTest(double width)
    {
        if (IsLoaded)
        {
            var textPaint = new SKPaint(new SKFont(SKTypeface.Default, 14) { Edging = SKFontEdging.SubpixelAntialias });
            var desiredWidth = textPaint.MeasureText(ConditionDescription?.Text ?? string.Empty);
            ShouldShowBodyText = !string.IsNullOrWhiteSpace(ViewModel.ConditionLongDesc);

            if (BindingContext is HourlyForecastItemViewModel)
            {
                if (desiredWidth >= (width - 60 - IconBox.Margin.HorizontalThickness - 36))
                    ShouldShowBodyText = true;
                else if (ViewModel.Extras?.Count <= 0)
                    IsExpandable = false;
            }

            if (ConditionDescription != null)
                ConditionDescription.IsVisible = IsExpanded && ShouldShowBodyText;

            if (ExpandIcon != null)
                ExpandIcon.IsVisible = IsExpandable || ShouldShowBodyText;
        }
    }
}
