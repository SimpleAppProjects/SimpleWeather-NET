using SimpleWeather.AQICN;
using SimpleWeather.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Controls.Graphs;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Radar;
using SimpleWeather.UWP.Shared.Helpers;
using SimpleWeather.UWP.Tiles;
using SimpleWeather.UWP.Utils;
using SimpleWeather.UWP.WeatherAlerts;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.Foundation.Metadata;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;
using mtuh = Microsoft.Toolkit.Uwp.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherNow : Page, ICommandBarPage, ISnackbarPage, IDisposable, IWeatherErrorListener
    {
        public String CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        private readonly WeatherManager wm = WeatherManager.GetInstance();
        private readonly WeatherIconsManager wim = WeatherIconsManager.GetInstance();

        private WeatherDataLoader wLoader = null;
        private RadarViewProvider radarViewProvider;

        private WeatherNowArgs args;

        internal LocationData locationData = null;
        private WeatherNowViewModel WeatherView { get; set; }
        private ForecastsNowViewModel ForecastView { get; set; }
        private WeatherAlertsViewModel AlertsView { get; set; }

        private CancellationTokenSource cts;

        private Geolocator geolocal = null;
        private Geoposition geoPos = null;

        public double ControlShadowOpacity
        {
            get { return (double)GetValue(ControlShadowOpacityProperty); }
            set { SetValue(ControlShadowOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlShadowOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlShadowOpacityProperty =
            DependencyProperty.Register("ControlShadowOpacity", typeof(double), typeof(WeatherNow), new PropertyMetadata(0d));

        public ElementTheme ControlTheme
        {
            get { return (ElementTheme)GetValue(ControlThemeProperty); }
            set { SetValue(ControlThemeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlTheme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlThemeProperty =
            DependencyProperty.Register("ControlTheme", typeof(ElementTheme), typeof(WeatherNow), new PropertyMetadata(ElementTheme.Default));

        private bool UpdateBindings = false;
        private bool UpdateTheme = false;
        private bool ClearGraphIconCache = false;

        public void Dispose()
        {
            cts?.Dispose();
        }

        public void OnWeatherLoaded(Weather weather)
        {
            if (cts?.IsCancellationRequested == true)
                return;

            if (weather?.IsValid() == true)
            {
                WeatherView.UpdateView(weather);

                Dispatcher.LaunchOnUIThread(() =>
                {
                    if (locationData?.locationType == LocationType.GPS)
                    {
                        GPSIcon.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        GPSIcon.Visibility = Visibility.Collapsed;
                    }
                });

                Task.Run(async () =>
                {
                    await WeatherView.UpdateBackground();

                    Dispatcher.LaunchOnUIThread(() => 
                    {
                        ShowProgressRing(false);
                        radarViewProvider?.UpdateCoordinates(WeatherView.LocationCoord, true);
                    });
                });

                if (locationData != null)
                {
                    ForecastView.UpdateForecasts(locationData);

                    Task.Run(async () =>
                    {
                        // Update home tile if it hasn't been already
                        bool isHome = Equals(locationData, await Settings.GetHomeData());
                        if (isHome && (TimeSpan.FromTicks(DateTime.Now.Ticks - Settings.UpdateTime.Ticks).TotalMinutes > Settings.RefreshInterval))
                        {
                            await WeatherUpdateBackgroundTask.RequestAppTrigger();
                        }
                        else if (isHome || SecondaryTileUtils.Exists(locationData?.query))
                        {
                            await WeatherTileCreator.TileUpdater(locationData);
                        }
                    });
                }
            }
        }

        public void OnWeatherError(WeatherException wEx)
        {
            if (cts?.IsCancellationRequested == true)
                return;

            Dispatcher.LaunchOnUIThread(() =>
            {
                switch (wEx.ErrorStatus)
                {
                    case WeatherUtils.ErrorStatus.NetworkError:
                    case WeatherUtils.ErrorStatus.NoWeather:
                        // Show error message and prompt to refresh
                        Snackbar snackbar = Snackbar.MakeError(wEx.Message, SnackbarDuration.Long);
                        snackbar.SetAction(App.ResLoader.GetString("action_retry"), () =>
                        {
                            RefreshWeather(false);
                        });
                        ShowSnackbar(snackbar);
                        break;

                    case WeatherUtils.ErrorStatus.QueryNotFound:
                        if (!wm.IsRegionSupported(locationData.country_code))
                        {
                            ShowSnackbar(Snackbar.MakeError(App.ResLoader.GetString("error_message_weather_region_unsupported"), SnackbarDuration.Long));
                        }
                        else
                        {
                            ShowSnackbar(Snackbar.MakeError(wEx.Message, SnackbarDuration.Long));
                        }
                        break;

                    default:
                        // Show error message
                        ShowSnackbar(Snackbar.MakeError(wEx.Message, SnackbarDuration.Long));
                        break;
                }

                ShowProgressRing(false);
            });
        }

        public WeatherNow()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            Application.Current.Resuming += WeatherNow_Resuming;
            cts = new CancellationTokenSource();

            WeatherView = Shell.Instance.GetViewModel<WeatherNowViewModel>();
            WeatherView.PropertyChanged += WeatherView_PropertyChanged;
            ForecastView = Shell.Instance.GetViewModel<ForecastsNowViewModel>();
            AlertsView = Shell.Instance.GetViewModel<WeatherAlertsViewModel>();

            geolocal = new Geolocator() { DesiredAccuracyInMeters = 5000, ReportInterval = 900000, MovementThreshold = 1600 };

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("label_nav_weathernow");
            PrimaryCommands = new List<ICommandBarElement>()
            {
                new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Pin),
                    Label = App.ResLoader.GetString("Label_Pin/Text"),
                    Tag = "pin",
                    Visibility = Visibility.Collapsed
                },
                new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Refresh),
                    Label = App.ResLoader.GetString("Button_Refresh/Label"),
                    Tag = "refresh"
                }
            };
            GetRefreshBtn().Tapped += RefreshButton_Click;
            GetPinBtn().Tapped += PinButton_Click;

            AnalyticsLogger.LogEvent("WeatherNow");

            Utils.FeatureSettings.OnFeatureSettingsChanged += FeatureSettings_OnFeatureSettingsChanged;
            Settings.OnSettingsChanged += Settings_OnSettingsChanged;
        }

        public void ShowSnackbar(Snackbar snackbar)
        {
            Shell.Instance?.ShowSnackbar(snackbar);
        }

        private void Settings_OnSettingsChanged(SettingsChangedEventArgs e)
        {
            if (e.Key == Settings.KEY_ICONSSOURCE)
            {
                // When page is loaded again from cache, clear icon cache
                ClearGraphIconCache = true;
                UpdateBindings = true;
            }
            else if (e.Key == Settings.KEY_USERTHEME)
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
                case "SunPhase":
                    if (WeatherView.SunPhase?.SunriseTime != null && WeatherView.SunPhase?.SunsetTime != null)
                    {
                        while (SunPhasePanel == null || !SunPhasePanel.ReadyToDraw)
                        {
                            await Task.Delay(1).ConfigureAwait(true);
                        }

                        SunPhasePanel.SetSunriseSetTimes(
                            WeatherView.SunPhase.SunriseTime, WeatherView.SunPhase.SunsetTime,
                            locationData?.tz_offset ?? TimeSpan.Zero
                        );
                    }
                    break;

                case "Alerts":
                    ResizeAlertPanel();
                    break;

                case "LocationCoord":
                    radarViewProvider?.UpdateCoordinates(WeatherView.LocationCoord, true);
                    break;

                case "WeatherSummary":
                    AdjustViewLayout();
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
                Resume();
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
            if (Window.Current == null) return;

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
                        h - (ConditionPanelRow?.ActualHeight ?? 0) - (AlertButtonRow?.ActualHeight ?? 0) - (LocationRow?.ActualHeight ?? 0) - (UpdateDateRow?.ActualHeight ?? 0)
                    );
                }
                else
                {
                    SpacerRow.Height = new GridLength(0);
                }
            }

            if (SummaryText != null)
            {
                SummaryText.FontSize = w > 640 ? 14 : 12;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            cts?.Cancel();
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

            cts = new CancellationTokenSource();

            // NOTE: ChangeView does not work here for some reason
            MainViewer?.ScrollToVerticalOffset(0);

            args = e?.Parameter as WeatherNowArgs;

            if (UpdateTheme)
            {
                UpdateControlTheme();
                UpdateTheme = false;
            }

            if (ClearGraphIconCache)
            {
                WeatherBox?.UpdateWeatherIcon();
                ClearGraphIconCache = false;
            }

            if (UpdateBindings)
            {
                this.Bindings.Update();
                UpdateBindings = false;
            }

            Resume();
        }

        private async Task<bool> VerifyLocationData()
        {
            bool locationChanged = false;

            // Check if we're loading from tile
            if (args?.TileId != null && SecondaryTileUtils.GetQueryFromId(args.TileId) is string locQuery)
            {
                locationData = await Settings.GetLocation(locQuery).ConfigureAwait(true);
                locationChanged = true;
            }

            // Check if current location still exists (is valid)
            if (locationData?.locationType == LocationType.Search)
            {
                if (await Settings.GetLocation(locationData?.query).ConfigureAwait(true) == null)
                {
                    locationData = null;
                    wLoader = null;
                    locationChanged = true;
                }
            }
            // Load new favorite location if argument data is present
            if (args == null || args?.IsHome == true)
            {
                // Check if home location changed
                // For ex. due to GPS setting change
                LocationData homeData = await Settings.GetHomeData().ConfigureAwait(true);
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

            if (locationData == null)
                locationData = args?.Location;

            return locationChanged;
        }

        /// <summary>
        /// Resume
        /// </summary>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        /// <exception cref="InvalidOperationException">Ignore.</exception>
        private void Resume()
        {
            Task.Run(async () =>
            {
                try
                {
                    var locationChanged = await VerifyLocationData();

                    // New page instance -> loaded = true
                    // Navigating back to existing page instance => loaded = false
                    // Weather location changed (ex. due to GPS setting) -> locationChanged = true
                    if (locationChanged || wLoader == null)
                    {
                        Restore();
                    }
                    else
                    {
                        var culture = CultureUtils.UserCulture;
                        var locale = wm.LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

                        if (!String.Equals(WeatherView.WeatherSource, Settings.API)
                                || wm.SupportsWeatherLocale && !String.Equals(WeatherView.WeatherLocale, locale))
                        {
                            Restore();
                        }
                        else
                        {
                            // Update weather if needed on resume
                            if (Settings.FollowGPS && await UpdateLocation())
                            {
                                // Setup loader from updated location
                                wLoader = new WeatherDataLoader(locationData);
                            }

                            RefreshWeather(false);

                            await Dispatcher.RunOnUIThread(() =>
                            {
                                // Check pin tile status
                                CheckTiles();
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex);
                }
            });
        }

        /// <summary>
        /// Restore
        /// </summary>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        /// <exception cref="InvalidOperationException">Ignore.</exception>
        private void Restore()
        {
            Task.Run(async () =>
            {
                bool forceRefresh = false;

                // GPS Follow location
                if (Settings.FollowGPS && (locationData == null || locationData.locationType == LocationType.GPS))
                {
                    var locData = await Settings.GetLastGPSLocData();

                    if (locData == null)
                    {
                        // Update location if not setup
                        await UpdateLocation();
                        forceRefresh = true;
                    }
                    else
                    {
                        // Reset locdata if source is different
                        if (locData.weatherSource != Settings.API)
                            Settings.SaveLastGPSLocData(new LocationData());

                        if (await UpdateLocation())
                        {
                            // Setup loader from updated location
                            forceRefresh = true;
                        }
                        else
                        {
                            // Setup loader saved location data
                            locationData = locData;
                        }
                    }
                }
                // Regular mode
                else if (locationData == null && wLoader == null)
                {
                    // Weather was loaded before. Lets load it up...
                    locationData = await Settings.GetHomeData();
                }

                if (locationData != null)
                    wLoader = new WeatherDataLoader(locationData);

                return forceRefresh;
            }).ContinueWith((t) =>
            {
                Dispatcher.LaunchOnUIThread(() =>
                {
                    // Check pin tile status
                    CheckTiles();
                });

                if (t.IsCompletedSuccessfully)
                {
                    // Load up weather data
                    RefreshWeather(t.Result);
                }
            });
        }

        private async Task<bool> UpdateLocation()
        {
            bool locationChanged = false;

            if (Settings.FollowGPS && (locationData == null || locationData.locationType == LocationType.GPS))
            {
                Geoposition newGeoPos = null;
                var geoStatus = GeolocationAccessStatus.Unspecified;

                try
                {
                    geoStatus = await Geolocator.RequestAccessAsync();
                }
                catch (Exception)
                {
                    // Access denied
                }

                try
                {
                    // Fallback to coarse (less accurate) location
                    geolocal.AllowFallbackToConsentlessPositions();
                    newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10));
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "Error retrieving location");
                }

                if (cts?.IsCancellationRequested == true)
                    return false;

                // Access to location granted
                if (newGeoPos != null)
                {
                    LocationData lastGPSLocData = await Settings.GetLastGPSLocData().ConfigureAwait(false);

                    // Check previous location difference
                    if (lastGPSLocData?.query != null
                        && geoPos != null && ConversionMethods.CalculateGeopositionDistance(geoPos, newGeoPos) < geolocal.MovementThreshold)
                    {
                        return false;
                    }

                    if (lastGPSLocData?.query != null
                        && Math.Abs(ConversionMethods.CalculateHaversine(lastGPSLocData.latitude, lastGPSLocData.longitude,
                        newGeoPos.Coordinate.Point.Position.Latitude, newGeoPos.Coordinate.Point.Position.Longitude)) < geolocal.MovementThreshold)
                    {
                        return false;
                    }

                    LocationQueryViewModel view = null;

                    if (cts?.IsCancellationRequested == true)
                        return false;

                    await Task.Run(async () =>
                    {
                        try
                        {
                            view = await wm.GetLocation(newGeoPos).ConfigureAwait(false);

                            if (String.IsNullOrEmpty(view.LocationQuery))
                            {
                                view = new LocationQueryViewModel();
                            }
                            else if (String.IsNullOrEmpty(view.LocationTZLong) && view.LocationLat != 0 && view.LocationLong != 0)
                            {
                                String tzId = await TZDB.TZDBCache.GetTimeZone(view.LocationLat, view.LocationLong);
                                if (!String.IsNullOrWhiteSpace(tzId))
                                    view.LocationTZLong = tzId;
                            }
                        }
                        catch (WeatherException ex)
                        {
                            view = new LocationQueryViewModel();

                            await Dispatcher.RunOnUIThread(() =>
                            {
                                ShowSnackbar(Snackbar.MakeError(ex.Message, SnackbarDuration.Short));
                            }).ConfigureAwait(false);
                        }
                    }).ConfigureAwait(false);

                    if (String.IsNullOrWhiteSpace(view.LocationQuery))
                    {
                        // Stop since there is no valid query
                        return false;
                    }

                    if (cts?.IsCancellationRequested == true)
                        return false;

                    // Save location as last known
                    lastGPSLocData = new LocationData(view, newGeoPos);
                    Settings.SaveLastGPSLocData(lastGPSLocData);

                    SimpleLibrary.GetInstance().RequestAction(CommonActions.ACTION_WEATHER_SENDLOCATIONUPDATE);

                    locationData = lastGPSLocData;
                    geoPos = newGeoPos;
                    locationChanged = true;
                }
            }

            return locationChanged;
        }

        private void ShowProgressRing(bool show)
        {
            LoadingRing.IsActive = show;
            if (GetRefreshBtn() is AppBarButton btn)
            {
                btn.IsEnabled = !show;
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: RefreshButton_Click");

            if (Settings.FollowGPS && await UpdateLocation())
                // Setup loader from updated location
                wLoader = new WeatherDataLoader(locationData);

            RefreshWeather(true);
        }

        /// <summary>
        /// RefreshWeather
        /// </summary>
        /// <param name="forceRefresh"></param>
        /// <exception cref="InvalidOperationException">Ignore.</exception>
        private void RefreshWeather(bool forceRefresh)
        {
            if (cts?.IsCancellationRequested == false)
            {
                Dispatcher.LaunchOnUIThread(() =>
                {
                    ShowProgressRing(true);

                    if (wLoader == null && locationData != null)
                    {
                        wLoader = new WeatherDataLoader(locationData);
                    }

                    Task.Run(() => wLoader?.LoadWeatherResult(new WeatherRequest.Builder()
                            .ForceRefresh(forceRefresh)
                            .SetErrorListener(this)
                            .Build())
                            .ContinueWith((t) =>
                            {
                                if (t.IsCompletedSuccessfully)
                                {
                                    Dispatcher.RunOnUIThread(() =>
                                    {
                                        OnWeatherLoaded(t.Result.Weather);
                                        if (AlertButton != null)
                                            AlertButton.Visibility = Visibility.Collapsed;
                                    });
                                    return wLoader.LoadWeatherAlerts(t.Result.IsSavedData);
                                }
                                else
                                {
                                    return Task.FromCanceled<ICollection<WeatherAlert>>(new CancellationToken(true));
                                }
                            })
                            .Unwrap()
                            .ContinueWith((t) =>
                            {
                                Dispatcher.LaunchOnUIThread(() =>
                                {
                                    if (locationData != null)
                                    {
                                        AlertsView.UpdateAlerts(locationData);
                                    }

                                    if (t.IsCompletedSuccessfully)
                                    {
                                        if (wm.SupportsAlerts && locationData != null)
                                        {
                                            if (t.Result?.Any() == true)
                                            {
                                                if (AlertButton != null)
                                                    AlertButton.Visibility = Visibility.Visible;

                                                _ = Task.Run(async () =>
                                                {
                                                    // Alerts are posted to the user here. Set them as notified.
#if DEBUG
                                                    await WeatherAlertHandler.PostAlerts(locationData, t.Result)
                                                            .ConfigureAwait(false);
#endif
                                                    await WeatherAlertHandler.SetasNotified(locationData, t.Result)
                                                            .ConfigureAwait(false);

                                                });
                                            }
                                        }
                                    }
                                });
                            }));
                });
            }
        }

        private void GotoAlertsPage()
        {
            AnalyticsLogger.LogEvent("WeatherNow: GotoAlertsPage");

            var transition = new SlideNavigationTransitionInfo();

            if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo", "Effect"))
            {
                transition.Effect = SlideNavigationTransitionEffect.FromRight;
            }

            Frame.Navigate(typeof(WeatherAlertPage), new WeatherPageArgs()
            {
                Location = locationData
            }, transition);
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
                var query = locationData?.locationType == LocationType.GPS ? Constants.KEY_GPS : locationData?.query;
                bool isPinned = SecondaryTileUtils.Exists(query);

                SetPinButton(isPinned);
                pinBtn.Visibility = Visibility.Visible;
                pinBtn.IsEnabled = true;
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
                    pinBtn.Label = App.ResLoader.GetString("Label_Unpin/Text");
                }
                else
                {
                    pinBtn.Icon = new SymbolIcon(Symbol.Pin);
                    pinBtn.Label = App.ResLoader.GetString("Label_Pin/Text");
                }
            }
        }

        private async void PinButton_Click(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: PinButton_Click");

            // Check if Tile service is available
            if (!DeviceTypeHelper.IsSecondaryTileSupported() || locationData?.IsValid() == false) return;

            if (!(sender is AppBarButton pinBtn)) return;

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
                        var snackbar = Snackbar.Make(App.ResLoader.GetString("Msg_BGAccessDeniedSettings"), SnackbarDuration.Long, SnackbarInfoType.Error);
                        snackbar.SetAction(App.ResLoader.GetString("action_settings"), async () =>
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
                    Location = locationData,
                    IsHourly = IsHourly,
                    ScrollToPosition = Position
                }, new DrillInNavigationTransitionInfo());
        }

        private void ForecastGraphPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: ForecastGraphPanel_Tapped");

            if (sender is IGraph control)
            {
                GotoDetailsPage(false,
                    control.GetItemPositionFromPoint((float)(e.GetPosition(control.GetControl()).X + control.GetScrollViewer().HorizontalOffset)));
            }
        }

        private void HourlyForecastControl_ItemClick(object sender, ItemClickEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: GraphView_Tapped");
            GotoDetailsPage(true, HourlyForecastControl.GetItemPosition(e.ClickedItem));
        }

        private void RadarWebView_Loaded(object sender, RoutedEventArgs e)
        {
            AsyncTask.Run(async () =>
            {
                await Dispatcher.RunOnUIThread(() =>
                {
                    radarViewProvider = RadarProvider.GetRadarViewProvider(RadarWebViewContainer);
                    radarViewProvider.EnableInteractions(false);
                    radarViewProvider.UpdateCoordinates(WeatherView.LocationCoord, true);
                }, CoreDispatcherPriority.Low);
            }, 1000, cts.Token);
        }

        private void RadarWebView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: RadarWebView_Tapped");
            Frame.Navigate(typeof(WeatherRadarPage), WeatherView?.LocationCoord, new DrillInNavigationTransitionInfo());
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
                if (GradientOverlay != null)
                {
                    GradientOverlay.Visibility = Visibility.Visible;
                }
                ControlTheme = ElementTheme.Dark;
                ControlShadowOpacity = 1;
                if (StackBackgroundBrush != null)
                {
                    StackBackgroundBrush.Opacity = 0.85;
                }
                if (this.Resources["tempToColorTempConverter"] is TempToColorTempConverter tempConv)
                {
                    tempConv.UseFallback = false;
                }
            }
            else
            {
                if (GradientOverlay != null)
                {
                    GradientOverlay.Visibility = Visibility.Collapsed;
                }
                switch (Settings.UserTheme)
                {
                    case UserThemeMode.System:
                        ControlTheme = App.IsSystemDarkTheme ? ElementTheme.Dark : ElementTheme.Light;
                        break;
                    case UserThemeMode.Light:
                        ControlTheme = ElementTheme.Light;
                        break;
                    case UserThemeMode.Dark:
                        ControlTheme = ElementTheme.Dark;
                        break;
                }
                ControlShadowOpacity = 0;
                if (StackBackgroundBrush != null)
                {
                    StackBackgroundBrush.Opacity = 0.0;
                }
                if (this.Resources["tempToColorTempConverter"] is TempToColorTempConverter tempConv)
                {
                    tempConv.UseFallback = true;
                }
            }
        }

        private void ForecastGraphPanel_GraphViewTapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(WeatherChartsPage),
                new WeatherPageArgs()
                {
                    Location = locationData
                }, new DrillInNavigationTransitionInfo());
        }
    }
}