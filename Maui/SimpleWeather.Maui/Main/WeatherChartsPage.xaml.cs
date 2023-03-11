using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls;
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

public partial class WeatherChartsPage : ViewModelPage
{
    private LocationData.LocationData locationData { get; set; }
    public WeatherNowViewModel WNowViewModel { get; } = AppShell.Instance.GetViewModel<WeatherNowViewModel>();
    public ChartsViewModel ChartsView { get; private set; }
    private WeatherPageArgs Args { get; set; }

    public WeatherChartsPage()
    {
        InitializeComponent();
        AnalyticsLogger.LogEvent("WeatherChartsPage");
    }

    internal WeatherChartsPage(WeatherPageArgs pageArgs) : this()
    {
        this.Args = pageArgs;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        ChartsView = this.GetViewModel<ChartsViewModel>();

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
            ChartsView.UpdateForecasts(locationData);
        }

        Dispatcher.Dispatch(() =>
        {
            ListControl.Bind(CollectionView.ItemsSourceProperty, mode: BindingMode.OneWay, source: ChartsView.GraphModels);
            ContentIndicator.Bind(
                ActivityIndicator.IsRunningProperty, mode: BindingMode.OneWay, source: ChartsView.GraphModels,
                path: "Count", converter: Resources["invValueBooleanConverter"] as IValueConverter);
        });
    }
}

public class ChartsDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate LineViewTemplate { get; set; }
    public DataTemplate BarChartTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is LineViewData)
        {
            return LineViewTemplate;
        }
        else if (item is BarGraphData)
        {
            return BarChartTemplate;
        }

        return null;
    }
}