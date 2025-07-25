﻿using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using SimpleWeather.Common.Controls;
using SimpleWeather.Common.Location;
using SimpleWeather.Common.Utils;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.LocationData;
using SimpleWeather.NET.BackgroundTasks;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.NET.Helpers;
using SimpleWeather.NET.Preferences;
using SimpleWeather.NET.Radar;
using SimpleWeather.NET.Shared.Helpers;
using SimpleWeather.NET.Tiles;
using SimpleWeather.NET.Utils;
using SimpleWeather.NET.WeatherAlerts;
using SimpleWeather.NET.Widgets;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using System.ComponentModel;
using Windows.Devices.Geolocation;
using Windows.Foundation.Metadata;
using Windows.UI.StartScreen;
using muxc = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.NET.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherNow : ScopePage, ICommandBarPage, ISnackbarPage, IBannerManager, IBannerPage
    {
        public String CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }
        private BannerManager BannerMgr { get; set; }

        private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        private RadarViewProvider radarViewProvider;

        private WeatherNowArgs args;

        private WeatherNowViewModel WNowViewModel { get; } = Shell.Instance.GetViewModel<WeatherNowViewModel>();
        private ForecastsNowViewModel ForecastView { get; } = Shell.Instance.GetViewModel<ForecastsNowViewModel>();
        private WeatherAlertsViewModel AlertsView { get; } = Shell.Instance.GetViewModel<WeatherAlertsViewModel>();

        public double ControlShadowOpacity
        {
            get { return (double)GetValue(ControlShadowOpacityProperty); }
            set { SetValue(ControlShadowOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlShadowOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlShadowOpacityProperty =
            DependencyProperty.Register(nameof(ControlShadowOpacity), typeof(double), typeof(WeatherNow), new PropertyMetadata(0d));

        public ElementTheme ControlTheme
        {
            get { return (ElementTheme)GetValue(ControlThemeProperty); }
            set { SetValue(ControlThemeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlTheme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlThemeProperty =
            DependencyProperty.Register(nameof(ControlTheme), typeof(ElementTheme), typeof(WeatherNow), new PropertyMetadata(ElementTheme.Default));

        private bool UpdateBindings = false;
        private bool UpdateTheme = false;

        public WeatherNow()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            // CommandBar
            CommandBarLabel = App.Current.ResLoader.GetString("label_nav_weathernow");
            PrimaryCommands = new List<ICommandBarElement>()
            {
                new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Pin),
                    Label = App.Current.ResLoader.GetString("Label_Pin/Text"),
                    Tag = "pin",
                    Visibility = Visibility.Collapsed
                },
                new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Refresh),
                    Label = App.Current.ResLoader.GetString("action_refresh"),
                    Tag = "refresh"
                }
            };

            if (GetRefreshBtn() is AppBarButton btn)
            {
                btn.SetBinding(AppBarButton.IsEnabledProperty, new Binding()
                {
                    Mode = BindingMode.OneWay,
                    Source = LoadingRing,
                    Path = new("IsActive"),
                    Converter = App.Current.Resources["inverseBoolConverter"] as IValueConverter,
                });
            }

            GetRefreshBtn().Tapped += RefreshButton_Click;
            GetPinBtn().Tapped += PinButton_Click;

            AnalyticsLogger.LogEvent("WeatherNow");

            Utils.FeatureSettings.OnFeatureSettingsChanged += FeatureSettings_OnFeatureSettingsChanged;
            SettingsManager.OnSettingsChanged += Settings_OnSettingsChanged;
            RadarProvider.RadarProviderChanged += RadarProvider_RadarProviderChanged;
            this.Loaded += WeatherNow_Loaded;
            this.Unloaded += WeatherNow_Unloaded;
        }

        public void ShowSnackbar(Snackbar snackbar)
        {
            Shell.Instance?.ShowSnackbar(snackbar);
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
            DispatcherQueue.EnqueueAsync(() =>
            {
                BannerMgr?.Show(banner);
            });
        }

        public void DismissBanner()
        {
            DispatcherQueue.EnqueueAsync(() =>
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
                        await DispatcherQueue.EnqueueAsync(() =>
                        {
                            var banner = Banner.MakeError(App.Current.ResLoader.GetString("prompt_location_not_set"));
                            banner.Icon = new muxc.SymbolIconSource()
                            {
                                Symbol = Symbol.Map
                            };
                            banner.SetAction(App.Current.ResLoader.GetString("label_fab_add_location"), () =>
                            {
                                Frame.Navigate(typeof(LocationsPage));
                            });
                            ShowBanner(banner);
                        }).ConfigureAwait(false);
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

                        Task.Run(async () =>
                        {
                            // Update home tile if it hasn't been already
                            bool isHome = Equals(locationData, await SettingsManager.GetHomeData());
                            if (isHome && (TimeSpan.FromTicks((long)(DateTime.Now.Ticks - SettingsManager.UpdateTime.Ticks)).TotalMinutes > SettingsManager.RefreshInterval))
                            {
                                await WeatherUpdateBackgroundTask.RequestAppTrigger();
                            }
                            else if (isHome)
                            {
                                await WeatherTileCreator.TileUpdater(locationData);
                                await WidgetUpdateHelper.RefreshWidgets(locationData.locationType == LocationType.GPS ? Constants.KEY_GPS : locationData.query);
                            }
                            else
                            {
                                if (SecondaryTileUtils.Exists(locationData?.query))
                                {
                                    await WeatherTileCreator.TileUpdater(locationData);
                                }
                                if (WidgetUtils.Exists(locationData?.query))
                                {
                                    await WidgetUpdateHelper.RefreshWidgets(locationData.query);
                                }
                            }
                        });
                    });
                    break;

                case nameof(WNowViewModel.Alerts):
                    {
                        var weatherAlerts = WNowViewModel.Alerts;
                        var locationData = WNowViewModel.UiState?.LocationData;

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

        private AppBarButton GetRefreshBtn()
        {
            return PrimaryCommands.LastOrDefault() as AppBarButton;
        }

        private AppBarButton GetPinBtn()
        {
            return PrimaryCommands.FirstOrDefault() as AppBarButton;
        }

        private void WeatherNow_Resuming(object sender, object e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: WeatherNow_Resuming");

            if (Shell.Instance.AppFrame.SourcePageType == this.GetType())
            {
                // Check pin tile status
                CheckTiles();
            }
        }

        private void ResizeAlertPanel()
        {
            double w = this.ActualWidth;

            if (w <= 0 || AlertButton == null)
                return;

            if (w <= 640)
                AlertButton.MaxWidth = w;
            else if (w <= 1080)
                AlertButton.MaxWidth = w * (0.75);
            else
                AlertButton.MaxWidth = w * (0.50);
        }

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: MainGrid_SizeChanged");
            AdjustViewLayout();
        }

        private void ConditionPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustViewLayout();
        }

        private void DeferedControl_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustViewLayout();
        }

        private void AdjustViewLayout()
        {
            if (MainWindow.Current == null) return;

            if (MainViewer == null) return;

            double w = MainViewer.ActualWidth - 16d - 16d; // Scrollbar padding
            double h = MainViewer.ActualHeight - MainViewer.Padding.Top - MainViewer.Padding.Bottom;

            if (w <= 0 || h <= 0) return;

            ResizeAlertPanel();

            if (SpacerRow != null)
            {
                if (Utils.FeatureSettings.BackgroundImage)
                {
                    SpacerRow.Height = new GridLength(
                        Math.Max(0, h - (ConditionPanelRow?.ActualHeight ?? 0) - (AlertButtonRow?.ActualHeight ?? 0) - (LocationRow?.ActualHeight ?? 0) - (UpdateDateRow?.ActualHeight ?? 0)),
                        GridUnitType.Pixel);
                }
                else
                {
                    SpacerRow.Height = new GridLength(
                        0,
                        GridUnitType.Pixel);
                }
            }

            if (SummaryText != null)
            {
                SummaryText.FontSize = w > 640 ? 14 : 12;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            radarViewProvider?.OnDestroyView();
            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            WNowViewModel.PropertyChanged -= WeatherView_PropertyChanged;
        }

        /// <summary>
        /// OnNavigatedTo
        /// </summary>
        /// <param name="e"></param>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            AnalyticsLogger.LogEvent("WeatherNow: OnNavigatedTo");

            WNowViewModel.PropertyChanged += WeatherView_PropertyChanged;

            // NOTE: ChangeView does not work here for some reason
            MainViewer?.ScrollToVerticalOffset(0);

            args = e?.Parameter as WeatherNowArgs;

            if (UpdateTheme)
            {
                UpdateControlTheme();
                UpdateTheme = false;
            }

            if (UpdateBindings)
            {
                this.Bindings.Update();
                UpdateBindings = false;
            }

            var locationData = args?.Location;

            DispatcherQueue.EnqueueAsync(async () =>
            {
                //WNowViewModel.Initialize(locationData);

                await InitializeState();
            });
        }

        private void OnErrorMessage(ErrorMessage error)
        {
            DispatcherQueue.EnqueueAsync(() =>
            {
                switch (error)
                {
                    case ErrorMessage.Resource err:
                        {
                            ShowSnackbar(Snackbar.MakeError(App.Current.ResLoader.GetString(err.ResourceId), SnackbarDuration.Short));
                        }
                        break;
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
                    {
                        // Show error message and prompt to refresh
                        Snackbar snackbar = Snackbar.MakeError(wEx.Message, SnackbarDuration.Long);
                        snackbar.SetAction(App.Current.ResLoader.GetString("action_retry"), () =>
                        {
                            WNowViewModel.RefreshWeather(false);
                        });
                        ShowSnackbar(snackbar);
                    }
                    break;

                case WeatherUtils.ErrorStatus.LocationNotSupported:
                    {
                        Snackbar snackbar = Snackbar.MakeError(wEx.Message, SnackbarDuration.Long);
                        snackbar.SetAction(App.Current.ResLoader.GetString("action_settings"), () =>
                        {
                            Shell.Instance.AppFrame.Navigate(typeof(SettingsPage));
                        });
                        ShowSnackbar(snackbar);
                    }
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

            // Check if we're loading from tile
            if (args?.TileId != null && SecondaryTileUtils.GetQueryFromId(args.TileId) is string locQuery)
            {
                locationData = await SettingsManager.GetLocation(locQuery).ConfigureAwait(true);
                locationChanged = true;
            }

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

            await (result.Data?.Let(async locationData =>
            {
                if (locationData.locationType == LocationType.GPS && SettingsManager.FollowGPS)
                {
                    var geoStatus = GeolocationAccessStatus.Unspecified;

                    try
                    {
                        geoStatus = await Geolocator.RequestAccessAsync();
                    }
                    catch (Exception)
                    {
                        // Access denied
                    }
                }
            }) ?? Task.CompletedTask);

            if (result is LocationResult.Changed || result is LocationResult.ChangedInvalid)
            {
                WNowViewModel.Initialize(result.Data);
            }
            else
            {
                WNowViewModel.RefreshWeather();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: RefreshButton_Click");
            WNowViewModel.RefreshWeather(true);
        }

        private void GotoAlertsPage()
        {
            AnalyticsLogger.LogEvent("WeatherNow: GotoAlertsPage");

            var transition = new SlideNavigationTransitionInfo();

            if (ApiInformation.IsPropertyPresent("Microsoft.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo", "Effect"))
            {
                transition.Effect = SlideNavigationTransitionEffect.FromRight;
            }

            Frame.Navigate(typeof(WeatherAlertPage), null, transition);
        }

        private void AlertButton_Click(object sender, RoutedEventArgs e)
        {
            GotoAlertsPage();
        }

        private void CheckTiles()
        {
            // Check if Tile service is available
            if (!DeviceTypeHelper.IsSecondaryTileSupported()) return;

            var pinBtn = GetPinBtn();

            if (pinBtn != null)
            {
                pinBtn.IsEnabled = false;

                // Check if your app is currently pinned
                var locationData = WNowViewModel.UiState?.LocationData;
                var query = locationData?.locationType == LocationType.GPS ? Constants.KEY_GPS : locationData?.query;
                if (query != null)
                {
                    bool isPinned = SecondaryTileUtils.Exists(query);

                    SetPinButton(isPinned);
                    pinBtn.Visibility = Visibility.Visible;
                    pinBtn.IsEnabled = true;
                }
                else
                {
                    pinBtn.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void SetPinButton(bool isPinned)
        {
            var pinBtn = GetPinBtn();

            if (pinBtn != null)
            {
                if (isPinned)
                {
                    pinBtn.Icon = new SymbolIcon(Symbol.UnPin);
                    pinBtn.Label = App.Current.ResLoader.GetString("Label_Unpin/Text");
                }
                else
                {
                    pinBtn.Icon = new SymbolIcon(Symbol.Pin);
                    pinBtn.Label = App.Current.ResLoader.GetString("Label_Pin/Text");
                }
            }
        }

        private async void PinButton_Click(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: PinButton_Click");

            var locationData = WNowViewModel.UiState?.LocationData;

            // Check if Tile service is available
            if (!DeviceTypeHelper.IsSecondaryTileSupported() || locationData?.IsValid() == false) return;

            if (sender is not AppBarButton pinBtn) return;

            pinBtn.IsEnabled = false;

            var query = locationData?.locationType == LocationType.GPS ? Constants.KEY_GPS : locationData?.query;

            try
            {
                if (SecondaryTileUtils.Exists(query))
                {
                    bool deleted = await new SecondaryTile(
                        SecondaryTileUtils.GetTileId(query)).RequestDeleteAsync().AsTask().ConfigureAwait(true);
                    if (deleted)
                    {
                        SecondaryTileUtils.RemoveTileId(query);
                    }

                    SetPinButton(!deleted);
                }
                else if (!string.IsNullOrWhiteSpace(query))
                {
                    if (!await BackgroundTaskHelper.IsBackgroundAccessEnabled().ConfigureAwait(true))
                    {
                        var snackbar = Snackbar.Make(App.Current.ResLoader.GetString("Msg_BGAccessDeniedSettings"), SnackbarDuration.Long, SnackbarInfoType.Error);
                        snackbar.SetAction(App.Current.ResLoader.GetString("action_settings"), async () =>
                        {
                            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
                        });
                        ShowSnackbar(snackbar);
                    }
                    else
                    {
                        // Initialize the tile with required arguments
                        var tileID = DateTime.Now.Ticks.ToInvariantString();
                        var tile = new SecondaryTile(
                            tileID,
                            "SimpleWeather",
                            "action=view-weather&query=" + query,
                            new Uri("ms-appx:///Assets/Square150x150Logo.png"),
                            TileSize.Default);

                        // Enable wide and large tile sizes
                        tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.png");
                        tile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/Square310x310Logo.png");

                        // Add a small size logo for better looking small tile
                        tile.VisualElements.Square71x71Logo = new Uri("ms-appx:///Assets/Square71x71Logo.png");

                        // Add a unique corner logo for the secondary tile
                        tile.VisualElements.Square44x44Logo = new Uri("ms-appx:///Assets/Square44x44Logo.png");

                        // Show the display name on all sizes
                        tile.VisualElements.ShowNameOnSquare150x150Logo = true;
                        tile.VisualElements.ShowNameOnWide310x150Logo = true;
                        tile.VisualElements.ShowNameOnSquare310x310Logo = true;

                        bool isPinned = await tile.RequestCreateAsync().AsTask().ConfigureAwait(true);
                        if (isPinned)
                        {
                            // Update tile with notifications
                            SecondaryTileUtils.AddTileId(query, tileID);
                            await Task.Run(async () =>
                            {
                                await WeatherTileCreator.TileUpdater(locationData);
                                await tile.UpdateAsync();
                            });
                        }

                        SetPinButton(isPinned);
                    }
                }
            }
            catch (Exception ex)
            {
                // Note: Exception - "Element not found."
                Logger.WriteLine(LoggerLevel.Error, ex, "Error (un)pinning tile...");
            }

            pinBtn.IsEnabled = true;
        }

        private void GotoDetailsPage(bool IsHourly, int Position)
        {
            Frame.Navigate(typeof(WeatherDetailsPage),
                new DetailsPageArgs()
                {
                    IsHourly = IsHourly,
                    ScrollToPosition = Position
                }, new DrillInNavigationTransitionInfo());
        }

        private void ForecastGraphPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: ForecastGraphPanel_Tapped");

            /*if (sender is IGraph control)
            {
                GotoDetailsPage(false,
                    control.GetItemPositionFromPoint((float)(e.GetPosition(control.Control).X + control.ScrollViewer.HorizontalOffset)));
                e.Handled = true;
            }
            else */if (sender is ForecastRangeBar rangeBar)
            {
                GotoDetailsPage(false, ForecastGraphPanel.GetItemPosition(rangeBar));
                e.Handled = true;
            }
            else
            {
                var point = e.GetPosition(ForecastGraphPanel);
                var rect = ForecastGraphPanel.ToRect();
                if (!rect.Contains(point))
                {
                    GotoDetailsPage(false, 0);
                    e.Handled = true;
                }
            }
        }

        private void HourlyForecastControl_ItemClick(object sender, ItemClickEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: GraphView_Tapped");
            GotoDetailsPage(true, HourlyForecastControl.GetItemPosition(e.ClickedItem));
        }

        private void HourlyForecastControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var point = e.GetPosition(HourlyForecastControl);
            var rect = HourlyForecastControl.ToRect();
            if (!rect.Contains(point))
            {
                GotoDetailsPage(true, 0);
                e.Handled = true;
            }
        }

        private void RadarWebView_Loaded(object sender, RoutedEventArgs e)
        {
            radarViewProvider?.OnDestroyView();
            radarViewProvider = RadarProvider.GetRadarViewProvider(RadarWebViewContainer);
            radarViewProvider.EnableInteractions(false);

            WNowViewModel.Weather?.Let(it =>
            {
                radarViewProvider?.UpdateCoordinates(it.LocationCoord, true);
            });
        }

        private void RadarWebView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: RadarWebView_Tapped");
            Frame.Navigate(typeof(WeatherRadarPage), null, new DrillInNavigationTransitionInfo());
            e.Handled = true;
        }

        private void RadarProvider_RadarProviderChanged(RadarProviderChangedEventArgs e)
        {
            if (Utils.FeatureSettings.WeatherRadar && RadarWebViewContainer != null)
            {
                radarViewProvider?.OnDestroyView();
                radarViewProvider = RadarProvider.GetRadarViewProvider(RadarWebViewContainer);
            }
        }

        private void BackgroundOverlay_ImageOpened(object sender, RoutedEventArgs e)
        {
            UpdateControlTheme(true);
        }

        private void BackgroundOverlay_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            UpdateControlTheme(false);
        }

        private void UpdateControlTheme()
        {
            UpdateControlTheme(Utils.FeatureSettings.BackgroundImage);
        }

        private void UpdateControlTheme(bool backgroundEnabled)
        {
            if (backgroundEnabled)
            {
                if (BackgroundOverlay != null)
                {
                    BackgroundOverlay.Visibility = Visibility.Visible;
                }
                if (GradientOverlay != null)
                {
                    GradientOverlay.Visibility = Visibility.Visible;
                }
                ControlTheme = ElementTheme.Dark;
                ControlShadowOpacity = 1;
                if (StackBackgroundBrush != null)
                {
                    StackBackgroundBrush.Opacity = 1.0;
                }
                if (CurTemp != null)
                {
                    CurTemp.Foreground = GetTempColorBrush();
                }
            }
            else
            {
                if (BackgroundOverlay != null)
                {
                    BackgroundOverlay.Visibility = Visibility.Collapsed;
                }
                if (GradientOverlay != null)
                {
                    GradientOverlay.Visibility = Visibility.Collapsed;
                }
                ControlTheme = Shell.Instance.AppFrame.ActualTheme;
                ControlShadowOpacity = 0;
                if (StackBackgroundBrush != null)
                {
                    StackBackgroundBrush.Opacity = 0.0;
                }
                if (CurTemp != null)
                {
                    CurTemp.Foreground = GetTempColorBrush();
                }
            }
        }

        private void GridLayout_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateViewOrder();
            UpdateControlTheme();
        }

        private SolidColorBrush GetTempColorBrush()
        {
            string temp = WNowViewModel?.Weather?.CurTemp;
            string temp_str = temp?.RemoveNonDigitChars();

            if (float.TryParse(temp_str, out float temp_f))
            {
                var tempUnit = SettingsManager.TemperatureUnit;

                if (Equals(tempUnit, Units.CELSIUS) || temp.EndsWith(Units.CELSIUS))
                {
                    temp_f = ConversionMethods.CtoF(temp_f);
                }

                var color = WeatherUtils.GetColorFromTempF(temp_f, Colors.Transparent);

                if (color != Colors.Transparent)
                {
                    return new SolidColorBrush(color);
                }
            }

            return Resources["ForegroundColorBrush"] as SolidColorBrush;
        }

        private void ForecastGraphPanel_GraphViewTapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is IGraph)
            {
                Frame.Navigate(typeof(WeatherChartsPage), null, new DrillInNavigationTransitionInfo());
                e.Handled = true;
            }
            else if (sender is HeaderedContentControl header && header.Content is FrameworkElement content)
            {
                var point = e.GetPosition(content);
                var rect = content.ToRect();
                if (!rect.Contains(point))
                {
                    Frame.Navigate(typeof(WeatherChartsPage), null, new DrillInNavigationTransitionInfo());
                    e.Handled = true;
                }
            }
            else
            {
                Frame.Navigate(typeof(WeatherChartsPage), null, new DrillInNavigationTransitionInfo());
                e.Handled = true;
            }
        }

        private void AQIndexControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(WeatherAQIPage), null, new DrillInNavigationTransitionInfo());
            e.Handled = true;
        }

        private void WeatherNow_Loaded(object sender, RoutedEventArgs e)
        {
            ForecastView.RequestedTheme = Shell.Instance.AppFrame.ActualTheme;
            Shell.Instance.AppFrame.ActualThemeChanged += WeatherNow_ActualThemeChanged;
        }

        private void WeatherNow_Unloaded(object sender, RoutedEventArgs e)
        {
            Shell.Instance.AppFrame.ActualThemeChanged -= WeatherNow_ActualThemeChanged;
        }

        private void WeatherNow_ActualThemeChanged(FrameworkElement sender, object args)
        {
            ForecastView.RequestedTheme = sender.ActualTheme;
        }

        private void OrderableView_Loaded(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            UpdateViewOrder(element);
        }

        private void UpdateViewOrder(FrameworkElement element = null)
        {
            if (GridLayout == null) return;

            var elements = GridLayout.Children.Cast<FrameworkElement>();
            if (element != null && !elements.Contains(element))
            {
                elements = elements.Append(element);
            }

            var orderableFeatures = FeatureSettings.GetFeatureOrder();

            var index = 0;

            if (orderableFeatures?.Any() == true)
            {
                var featureViewMap = orderableFeatures.ToDictionary(f => f, f =>
                {
                    return elements.FirstOrDefault(e => Equals(e.Tag, f));
                });

                orderableFeatures.ForEach(feature =>
                {
                    var v = featureViewMap[feature];

                    if (v != null)
                    {
                        Grid.SetRow(featureViewMap[feature], index++);
                    }
                });
            }
            else
            {
                elements.ForEach(v =>
                {
                    Grid.SetRow(v, index++);
                });
            }
        }
    }
}