using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Maui.IncrementalLoadingCollection;
using SimpleWeather.Maui.ViewModels;
using SimpleWeather.NET.Controls;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;

namespace SimpleWeather.Maui.Main;

public partial class WeatherDetailsPage : ViewModelPage
{
	private LocationData.LocationData locationData { get; set; }
	public WeatherNowViewModel WNowViewModel { get; } = AppShell.Instance.GetViewModel<WeatherNowViewModel>();
	public ForecastsListViewModel ForecastsView { get; private set; }
	private DetailsPageArgs Args { get; set; }

	private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;

	public WeatherDetailsPage()
	{
		InitializeComponent();
		AnalyticsLogger.LogEvent("WeatherDetailsPage");
        ListControl.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ListControl.ItemsSource))
            {
                if (ListControl.ItemsSource is ISupportIncrementalLoading loadingCollection)
                {
                    Dispatcher.Dispatch(async () =>
                    {
                        await loadingCollection.LoadMoreItemsAsync((uint)ListControl.RemainingItemsThreshold / 2);
                    });
                }
            }
        };
	}

    internal WeatherDetailsPage(DetailsPageArgs detailsPageArgs) : this()
    {
        Args = detailsPageArgs;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        ForecastsView = this.GetViewModel<ForecastsListViewModel>();

        if (Args != null)
        {
            locationData = Args.Location;
            Initialize(Args.ScrollToPosition);
        }
        else
        {
            WNowViewModel.PropertyChanged += WNowViewModel_PropertyChanged;
            Initialize();
        }
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
            Initialize(Args?.ScrollToPosition ?? 0);
        }
    }

    private void Initialize(int scrollToPosition = 0)
    {
        if (locationData == null)
        {
            locationData = WNowViewModel.UiState.LocationData;
        }

        if (locationData != null)
        {
            ForecastsView.UpdateForecasts(locationData);
        }

        ForecastsView.SelectForecast(Args?.IsHourly ?? false);

        Dispatcher.Dispatch(() =>
        {
            ForecastsView.SelectForecast(Args?.IsHourly ?? false);
            ListControl.Bind(CollectionView.ItemsSourceProperty, mode: BindingMode.OneWay, source: ForecastsView.SelectedForecasts);
            ContentIndicator.Bind(
                ActivityIndicator.IsRunningProperty, mode: BindingMode.OneWay, source: ForecastsView.SelectedForecasts,
                path: "Count", converter: Resources["invValueBooleanConverter"] as IValueConverter);
            IncrementalIndicator.Bind(
                IndeterminateProgressBar.IsActiveProperty, mode: BindingMode.OneWay, source: ForecastsView.SelectedForecasts,
                path: "IsLoading");

            ListControl.ScrollTo(scrollToPosition);
        });
    }

    private async void ListControl_RemainingItemsThresholdReached(object sender, EventArgs e)
    {
        if (ListControl.ItemsSource is ISupportIncrementalLoading loadingCollection)
        {
            await loadingCollection.LoadMoreItemsAsync((uint)ListControl.RemainingItemsThreshold);
        }
    }
}