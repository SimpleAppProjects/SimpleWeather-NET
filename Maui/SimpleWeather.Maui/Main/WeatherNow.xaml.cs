using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common.Controls;
using SimpleWeather.Common.Location;
using SimpleWeather.Common.Utils;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.LocationData;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Maui.MaterialIcons;
using SimpleWeather.Maui.Utils;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Radar;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using System.ComponentModel;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Main;

public partial class WeatherNow : ScopePage, ISnackbarManager, ISnackbarPage, IBannerManager, IBannerPage
{
    private SnackbarManager SnackMgr { get; set; }
    private BannerManager BannerMgr { get; set; }

    private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    private RadarViewProvider radarViewProvider;

    private WeatherNowArgs args;

    private WeatherNowViewModel WNowViewModel { get; } = AppShell.Instance.GetViewModel<WeatherNowViewModel>();
    private ForecastsNowViewModel ForecastView { get; } = AppShell.Instance.GetViewModel<ForecastsNowViewModel>();
    private WeatherAlertsViewModel AlertsView { get; } = AppShell.Instance.GetViewModel<WeatherAlertsViewModel>();

    private bool UpdateBindings = false;
    private bool UpdateTheme = false;
    private bool ClearGraphIconCache = false;

    public WeatherNow()
    {
        InitializeComponent();

        AnalyticsLogger.LogEvent("WeatherNow");

        Utils.FeatureSettings.OnFeatureSettingsChanged += FeatureSettings_OnFeatureSettingsChanged;
        SettingsManager.OnSettingsChanged += Settings_OnSettingsChanged;
        RadarProvider.RadarProviderChanged += RadarProvider_RadarProviderChanged;
        this.Loaded += WeatherNow_Loaded;
        this.Unloaded += WeatherNow_Unloaded;

        BindingContext = WNowViewModel;
    }

    internal WeatherNow(WeatherNowArgs args) : this()
    {
    }

    public void InitSnackManager()
    {
        if (SnackMgr == null)
        {
            SnackMgr = new SnackbarManager(SnackbarContainer);
        }
    }

    public void ShowSnackbar(Snackbar snackbar)
    {
        Dispatcher.Dispatch(() =>
        {
            SnackMgr?.Show(snackbar);
        });
    }
    public void DismissAllSnackbars()
    {
        Dispatcher.Dispatch(() =>
        {
            SnackMgr?.DismissAll();
        });
    }

    public void UnloadSnackManager()
    {
        DismissAllSnackbars();
        SnackMgr = null;
    }

    public void InitBannerManager()
    {
        if (BannerMgr == null)
        {
            BannerMgr = new BannerManager(MainGrid);
        }
    }

    public void ShowBanner(Banner banner)
    {
        Dispatcher.Dispatch(() =>
        {
            BannerMgr?.Show(banner);
        });
    }

    public void DismissBanner()
    {
        Dispatcher.Dispatch(() =>
        {
            BannerMgr?.Dismiss();
        });
    }

    public void UnloadBannerManager()
    {
        DismissBanner();
        BannerMgr = null;
    }

    private void Settings_OnSettingsChanged(SettingsChangedEventArgs e)
    {
        if (e.Key == SettingsManager.KEY_ICONSSOURCE)
        {
            // When page is loaded again from cache, clear icon cache
            ClearGraphIconCache = true;
            UpdateBindings = true;
        }
        else if (e.Key == SettingsManager.KEY_USERTHEME)
        {
            UpdateBindings = true;
            // Update theme
            UpdateTheme = true;
        }
    }

    private void FeatureSettings_OnFeatureSettingsChanged(FeatureSettingsChangedEventArgs e)
    {
        // When page is loaded again from cache, update bindings
        UpdateBindings = true;
        UpdateTheme = true;
    }

    private async void WeatherView_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(WNowViewModel.UiState):
                var uiState = WNowViewModel.UiState;

                AdjustViewLayout();

                if (Utils.FeatureSettings.WeatherRadar)
                {
                    WNowViewModel.Weather?.LocationCoord?.Let(coords =>
                    {
                        radarViewProvider?.UpdateCoordinates(coords, true);
                    });
                }

                if (uiState.NoLocationAvailable)
                {
                    await Dispatcher.DispatchAsync(() =>
                    {
                        var banner = Banner.Make(ResStrings.prompt_location_not_set);
                        banner.Icon = new MaterialIcon(MaterialSymbol.Map);
                        banner.SetAction(ResStrings.label_fab_add_location, async () =>
                        {
                            await Navigation.PushAsync(new LocationsPage());
                        });
                        ShowBanner(banner);
                    });
                }
                else
                {
                    DismissBanner();
                }
                break;

            case nameof(WNowViewModel.Weather):
                WNowViewModel.UiState?.LocationData?.Let(locationData =>
                {
                    ForecastView.UpdateForecasts(locationData);
                    AlertsView.UpdateAlerts(locationData);

                    /*
                    Task.Run(async () =>
                    {
                        // Update home tile if it hasn't been already
                        bool isHome = Equals(locationData, await SettingsManager.GetHomeData());
                        if (isHome && (TimeSpan.FromTicks((long)(DateTime.Now.Ticks - SettingsManager.UpdateTime.Ticks)).TotalMinutes > SettingsManager.RefreshInterval))
                        {
                            await WeatherUpdateBackgroundTask.RequestAppTrigger();
                        }
                        else if (isHome || SecondaryTileUtils.Exists(locationData?.query))
                        {
                            await WeatherTileCreator.TileUpdater(locationData);
                        }
                    });
                    */
                });
                break;

            case nameof(WNowViewModel.Alerts):
                {
                    var weatherAlerts = WNowViewModel.Alerts;
                    var locationData = WNowViewModel.UiState?.LocationData;

                    /*
                    if (wm.SupportsAlerts && locationData != null)
                    {
                        _ = Task.Run(async () =>
                        {
                            // Alerts are posted to the user here. Set them as notified.
#if DEBUG
                            await WeatherAlertHandler.PostAlerts(locationData, weatherAlerts);
#endif
                            await WeatherAlertHandler.SetasNotified(locationData, weatherAlerts);

                        });
                    }
                    */

                    ResizeAlertPanel();
                }
                break;

            case nameof(WNowViewModel.ErrorMessages):
                {
                    var errorMessages = WNowViewModel.ErrorMessages;

                    var error = errorMessages.FirstOrDefault();

                    if (error != null)
                    {
                        OnErrorMessage(error);
                    }
                }
                break;
        }
    }

    private void ResizeAlertPanel()
    {
        double w = this.Width;

        if (w <= 0 || AlertButton == null)
            return;

        if (w <= 640)
            AlertButton.MaximumWidthRequest = w;
        else if (w <= 1080)
            AlertButton.MaximumWidthRequest = w * (0.75);
        else
            AlertButton.MaximumWidthRequest = w * (0.50);
    }

    private void MainViewer_SizeChanged(object sender, EventArgs e)
    {
        AnalyticsLogger.LogEvent("WeatherNow: MainGrid_SizeChanged");
        AdjustViewLayout();
    }

    private void ConditionPanel_SizeChanged(object sender, EventArgs e)
    {
        AdjustViewLayout();
    }

    private void DeferedControl_Loaded(object sender, EventArgs e)
    {
        AdjustViewLayout();
    }

    private void AdjustViewLayout()
    {
        if (MainViewer == null) return;

        double w = MainViewer.Width - 16d - 16d; // Scrollbar padding
        double h = MainViewer.Height - MainViewer.Padding.Top - MainViewer.Padding.Bottom;

        if (w <= 0 || h <= 0) return;

        ResizeAlertPanel();

        if (SpacerRow != null)
        {

        }

        SummaryText?.FontSize(w > 640 ? 14 : 12);
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        radarViewProvider?.OnDestroyView();
        base.OnNavigatingFrom(args);
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs e)
    {
        base.OnNavigatedFrom(e);
        WNowViewModel.PropertyChanged -= WeatherView_PropertyChanged;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs e)
    {
        base.OnNavigatedTo(e);

        AnalyticsLogger.LogEvent("WeatherNow: OnNavigatedTo");

        WNowViewModel.PropertyChanged += WeatherView_PropertyChanged;

        MainViewer.ScrollToAsync(0, 0, false);

        if (UpdateTheme)
        {
            // UpdateControlTheme
            UpdateTheme = false;
        }

        if (ClearGraphIconCache)
        {
            WeatherBox?.UpdateWeatherIcon();
            ClearGraphIconCache = false;
        }

        if (UpdateBindings)
        {
            this.ApplyBindings();
            UpdateBindings = false;
        }

        await Dispatcher.DispatchAsync(async () =>
        {
            await InitializeState();
        });
    }

    private void OnErrorMessage(ErrorMessage error)
    {
        Dispatcher.Dispatch(() =>
        {
            switch (error)
            {
                case ErrorMessage.String err:
                    {
                        ShowSnackbar(Snackbar.MakeError(err.Message, SnackbarDuration.Short));
                    }
                    break;
                case ErrorMessage.WeatherError err:
                    {
                        OnWeatherError(err.Exception);
                    }
                    break;
            }
        });

        WNowViewModel.SetErrorMessageShown(error);
    }

    private void OnWeatherError(WeatherException wEx)
    {
        switch (wEx.ErrorStatus)
        {
            case WeatherUtils.ErrorStatus.NetworkError:
            case WeatherUtils.ErrorStatus.NoWeather:
                // Show error message and prompt to refresh
                Snackbar snackbar = Snackbar.MakeError(wEx.Message, SnackbarDuration.Long);
                snackbar.SetAction(ResStrings.action_retry, () =>
                {
                    WNowViewModel.RefreshWeather(false);
                });
                ShowSnackbar(snackbar);
                break;

            case WeatherUtils.ErrorStatus.QueryNotFound:
                ShowSnackbar(Snackbar.MakeError(wEx.Message, SnackbarDuration.Long));
                break;

            default:
                // Show error message
                ShowSnackbar(Snackbar.MakeError(wEx.Message, SnackbarDuration.Long));
                break;
        }
    }

    private async Task<LocationResult> VerifyLocationData()
    {
        var locationData = WNowViewModel.UiState?.LocationData;
        bool locationChanged = false;

        // Check if current location still exists (is valid)
        if (locationData?.locationType == LocationType.Search)
        {
            if (await SettingsManager.GetLocation(locationData?.query).ConfigureAwait(true) == null)
            {
                locationData = null;
                locationChanged = true;
            }
        }

        // Load new favorite location if argument data is present
        if (args == null || args?.IsHome == true)
        {
            // Check if home location changed
            // For ex. due to GPS setting change
            var homeData = await SettingsManager.GetHomeData().ConfigureAwait(true);
            if (!Equals(locationData, homeData))
            {
                locationData = homeData;
                locationChanged = true;
            }
        }
        else if (args?.Location != null && !Equals(locationData, args?.Location))
        {
            locationData = args?.Location;
            locationChanged = true;
        }

        if (locationChanged)
        {
            if (locationData != null)
            {
                return new LocationResult.Changed(locationData);
            }
            else
            {
                return new LocationResult.ChangedInvalid(null);
            }
        }
        else
        {
            return new LocationResult.NotChanged(locationData);
        }
    }

    private async Task InitializeState()
    {
        var result = await VerifyLocationData();

        await result.Data?.Let(async locationData =>
        {
            if (locationData.locationType == LocationType.GPS && SettingsManager.FollowGPS)
            {
                if (!await this.LocationPermissionEnabled())
                {
                    var snackbar = Snackbar.Make(ResStrings.Msg_LocDeniedSettings, SnackbarDuration.Short);
                    snackbar.SetAction(ResStrings.action_settings, async () =>
                    {
                        await this.LaunchLocationSettings();
                    });
                    ShowSnackbar(snackbar);
                    return;
                }
            }
        });

        if (result is LocationResult.Changed || result is LocationResult.ChangedInvalid)
        {
            WNowViewModel.Initialize(result.Data);
        }
        else
        {
            WNowViewModel.RefreshWeather();
        }
    }

    private void RefreshBtn_Clicked(object sender, EventArgs e)
    {
        AnalyticsLogger.LogEvent("WeatherNow: RefreshButton_Click");
        WNowViewModel.RefreshWeather(true);
    }

    private async void GotoAlertsPage()
    {
        AnalyticsLogger.LogEvent("WeatherNow: GotoAlertsPage");
        await Navigation.PushAsync(new WeatherAlertPage());
    }

    private void AlertButton_Tapped(object sender, TappedEventArgs e)
    {
        GotoAlertsPage();
    }

    private void AlertButton_Clicked(object sender, EventArgs e)
    {
#if WINDOWS || MACCATALYST
        GotoAlertsPage();
#endif
    }

    private async void GotoDetailsPage(bool IsHourly, int Position)
    {
        await Navigation.PushAsync(new WeatherDetailsPage(new DetailsPageArgs()
        {
            IsHourly = IsHourly,
            ScrollToPosition = Position
        }));
    }

    private void RadarWebView_Loaded(object sender, EventArgs e)
    {
        var cToken = GetCancellationToken();

        AsyncTask.Run(async () =>
        {
            await Dispatcher.DispatchAsync(() =>
            {
                if (radarViewProvider == null)
                {
                    radarViewProvider = RadarProvider.GetRadarViewProvider(RadarWebViewContainer);
                }
                radarViewProvider.EnableInteractions(false);
                WNowViewModel.Weather?.Let(it =>
                {
                    radarViewProvider?.UpdateCoordinates(it.LocationCoord, true);
                });
            });
        }, 1000, cToken);
    }

    private async void RadarWebView_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new WeatherRadarPage());
    }

    private async void RadarWebView_Clicked(object sender, EventArgs e)
    {
#if WINDOWS || MACCATALYST
        await Navigation.PushAsync(new WeatherRadarPage());
#endif
    }

    private void RadarProvider_RadarProviderChanged(RadarProviderChangedEventArgs e)
    {
        if (Utils.FeatureSettings.WeatherRadar && RadarWebViewContainer != null)
        {
            radarViewProvider?.OnDestroyView();
            radarViewProvider = RadarProvider.GetRadarViewProvider(RadarWebViewContainer);
        }
    }

    private void WeatherNow_Loaded(object sender, EventArgs e)
    {
    }

    private void WeatherNow_Unloaded(object sender, EventArgs e)
    {
    }
}