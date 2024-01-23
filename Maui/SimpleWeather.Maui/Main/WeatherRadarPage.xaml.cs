using SimpleWeather.Common.ViewModels;
using SimpleWeather.NET.Radar;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Main;

public partial class WeatherRadarPage : ContentPage
{
	private RadarViewProvider radarViewProvider;
	private WeatherNowViewModel WNowViewModel { get; } = AppShell.Instance.GetViewModel<WeatherNowViewModel>();

	public WeatherRadarPage()
	{
		InitializeComponent();

		AnalyticsLogger.LogEvent("WeatherRadarPage");
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        WNowViewModel.Weather?.Let(it =>
        {
            radarViewProvider?.UpdateCoordinates(it.LocationCoord, true);
        });
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        radarViewProvider?.OnDestroyView();
        AnalyticsLogger.LogEvent("WeatherRadarPage: OnNavigatingFrom");
        base.OnNavigatingFrom(args);
    }

    private void RadarWebViewContainer_Loaded(object sender, EventArgs e)
    {
        if (radarViewProvider == null)
        {
            RadarWebViewContainer.SetAppTheme(ClassIdProperty, "light", "dark");
            radarViewProvider = RadarProvider.GetRadarViewProvider(RadarWebViewContainer);
        }
        radarViewProvider.EnableInteractions(true);
        radarViewProvider.UpdateCoordinates(new WeatherUtils.Coordinate(0,0), true);
        WNowViewModel.Weather?.Let(it =>
        {
            radarViewProvider?.UpdateCoordinates(it.LocationCoord, true);
        });
    }

    private void RefreshBtn_Clicked(object sender, EventArgs e)
    {
        WNowViewModel.Weather?.Let(it =>
        {
            radarViewProvider?.UpdateCoordinates(it.LocationCoord, true);
        });
    }
}