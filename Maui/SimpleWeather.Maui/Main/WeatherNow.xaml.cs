using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common.Controls;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Preferences;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;

namespace SimpleWeather.Maui.Main;

public partial class WeatherNow : ScopePage, ISnackbarPage, IBannerManager, IBannerPage
{
	private BannerManager BannerMgr { get; set; }

    private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    private RadarViewProvider radarViewProvider;

    private WeatherNowArgs args;

    private WeatherNowViewModel WNowViewModel { get; } = AppShell.Instance.GetViewModel<WeatherNowViewModel>();
    private ForecastsNowViewModel ForecastView { get; } = AppShell.Instance.GetViewModel<ForecastsNowViewModel>();
    private WeatherAlertsViewModel AlertsView { get; } = AppShell.Instance.GetViewModel<WeatherAlertsViewModel>();

    private bool ClearGraphIconCache = false;

    public WeatherNow()
	{
		InitializeComponent();
	}
}