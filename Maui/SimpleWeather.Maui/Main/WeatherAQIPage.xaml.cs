using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls;
using SimpleWeather.Common.Controls;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Maui.IncrementalLoadingCollection;
using SimpleWeather.Maui.ViewModels;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;

namespace SimpleWeather.Maui.Main;

public partial class WeatherAQIPage : ViewModelPage
{
    private LocationData.LocationData locationData { get; set; }
    public WeatherNowViewModel WNowViewModel { get; } = AppShell.Instance.GetViewModel<WeatherNowViewModel>();
    public AirQualityForecastViewModel AQIView { get; private set; }
    private WeatherPageArgs Args { get; set; }

    public WeatherAQIPage()
    {
        InitializeComponent();
        AnalyticsLogger.LogEvent("WeatherAQIPage");

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

    private void WNowViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WNowViewModel.UiState))
        {
            locationData = WNowViewModel.UiState.LocationData;
            Initialize();
        }
    }

    private void Initialize()
    {
        if (locationData == null)
        {
            locationData = WNowViewModel.UiState.LocationData;
        }

        if (locationData != null)
        {
            AQIView.UpdateForecasts(locationData);
        }

        Dispatcher.Dispatch(() =>
        {
            AQIContainer.Bind(
                BindableLayout.ItemsSourceProperty, mode: BindingMode.OneWay, source: AQIView,
                path: DeviceInfo.Idiom == DeviceIdiom.Phone ? nameof(AQIView.AQIGraphData) : nameof(AQIView.AQIForecastData));
            ContentIndicator.Bind(
                ActivityIndicator.IsRunningProperty, mode: BindingMode.OneWay, source: BindableLayout.GetItemsSource(AQIContainer),
                path: "Count", converter: Resources["invValueBooleanConverter"] as IValueConverter);
        });

        AQIContainer.ChildAdded += (s, e) =>
        {
            Dispatcher.Dispatch(() =>
            {
                (AQIContainer as IView)?.InvalidateMeasure();
            });
        };
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        Dispatcher.Dispatch(() =>
        {
            (AQIContainer as IView)?.InvalidateMeasure();
        });
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