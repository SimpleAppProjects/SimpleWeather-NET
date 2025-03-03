using System.Collections.Immutable;
using System.ComponentModel;
using System.Globalization;
using SimpleWeather.Common.Controls;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.Maui.ViewModels;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Main;

public partial class WeatherAQIPage : ViewModelPage
{
    private LocationData.LocationData locationData { get; set; }
    public WeatherNowViewModel WNowViewModel { get; } = AppShell.Instance.GetViewModel<WeatherNowViewModel>();

    public AirQualityForecastViewModel AQIView
    {
        get => (AirQualityForecastViewModel)GetValue(AQIViewProperty);
        private set => SetValue(AQIViewProperty, value);
    }

    public static readonly BindableProperty AQIViewProperty =
        BindableProperty.Create(nameof(AQIView), typeof(AirQualityForecastViewModel), typeof(WeatherAQIPage), null);

    private WeatherPageArgs Args { get; set; }

    public int AQIItemCount
    {
        get
        {
            if (AQIContainer != null)
                return BindableLayout.GetItemsSource(AQIContainer)?.Cast<object>()?.Count() ?? 0;

            return 0;
        }
    }

    public WeatherAQIPage()
    {
        InitializeComponent();
        AnalyticsLogger.LogEvent(nameof(WeatherAQIPage));

        AQILayout.SizeChanged += (s, e) =>
        {
            var it = s as View;

            if (it.Width > it.MaximumWidthRequest)
            {
                it.WidthRequest = it.MaximumWidthRequest;
            }
        };
    }

    internal WeatherAQIPage(WeatherPageArgs pageArgs) : this()
    {
        this.Args = pageArgs;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        AQIView = this.GetViewModel<AirQualityForecastViewModel>();

        if (Args != null)
        {
            locationData = Args.Location;
        }
        else
        {
            WNowViewModel.PropertyChanged += WNowViewModel_PropertyChanged;
        }

        Initialize();
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        WNowViewModel.PropertyChanged -= WNowViewModel_PropertyChanged;
    }

    private void WNowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WNowViewModel.UiState))
        {
            locationData = WNowViewModel.UiState.LocationData;
            Initialize();
        }
    }

    private void Initialize()
    {
        locationData ??= WNowViewModel.UiState.LocationData;

        if (locationData != null)
        {
            AQIView.UpdateForecasts(locationData);
        }
    }
}

public class AQIDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate AQIGraphTemplate { get; set; }
    public DataTemplate AQIForecastTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is BarGraphData)
        {
            return AQIGraphTemplate;
        }
        else if (item is AirQualityViewModel)
        {
            return AQIForecastTemplate;
        }

        return null;
    }
}