using Microsoft.Toolkit.Uwp.Helpers;
using SimpleWeather.AQICN;
using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherNow : CustomPage, IDisposable, IWeatherErrorListener
    {
        private WeatherManager wm;
        private WeatherDataLoader wLoader = null;

        private WeatherNowArgs args;

        internal LocationData locationData = null;
        internal WeatherNowViewModel WeatherView { get; private set; }
        internal ForecastGraphViewModel ForecastView { get; private set; }
        internal WeatherAlertsViewModel AlertsView { get; private set; }

        private CancellationTokenSource cts;

        private Geolocator geolocal = null;
        private Geoposition geoPos = null;

        public void Dispose()
        {
            cts?.Dispose();
        }

        public void OnWeatherLoaded(LocationData location, Weather weather)
        {
            if (cts?.IsCancellationRequested == true)
                return;

            if (weather?.IsValid() == true)
            {
                WeatherView.UpdateView(weather);

                Task.Run(async () =>
                {
                    await WeatherView.UpdateBackground();
                }).ContinueWith((t) =>
                {
                    LoadingRing.IsActive = false;
                }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(true);

                ForecastView.UpdateForecasts(locationData);

                Task.Run(async () =>
                {
                    // Update home tile if it hasn't been already
                    bool isHome = Equals(location, await Settings.GetHomeData());
                    if (isHome && (TimeSpan.FromTicks(DateTime.Now.Ticks - Settings.UpdateTime.Ticks).TotalMinutes > Settings.RefreshInterval))
                    {
                        WeatherUpdateBackgroundTask.RequestAppTrigger();
                    }
                    else if (isHome || SecondaryTileUtils.Exists(location?.query))
                    {
                        WeatherTileCreator.TileUpdater(location);
                    }
                });
            }
        }

        public void OnWeatherError(WeatherException wEx)
        {
            if (cts?.IsCancellationRequested == true)
                return;

            Dispatcher.RunOnUIThread(() =>
            {
                switch (wEx.ErrorStatus)
                {
                    case WeatherUtils.ErrorStatus.NetworkError:
                    case WeatherUtils.ErrorStatus.NoWeather:
                        // Show error message and prompt to refresh
                        Snackbar snackbar = Snackbar.Make(wEx.Message, SnackbarDuration.Long);
                        snackbar.SetAction(App.ResLoader.GetString("Action_Retry"), () =>
                        {
                            RefreshWeather(false);
                        });
                        ShowSnackbar(snackbar);
                        break;

                    case WeatherUtils.ErrorStatus.QueryNotFound:
                        if (WeatherAPI.NWS.Equals(Settings.API))
                        {
                            ShowSnackbar(Snackbar.Make(App.ResLoader.GetString("Error_WeatherUSOnly"), SnackbarDuration.Long));
                        }
                        break;

                    default:
                        // Show error message
                        ShowSnackbar(Snackbar.Make(wEx.Message, SnackbarDuration.Long));
                        break;
                }
            });
        }

        public WeatherNow()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            Application.Current.Resuming += WeatherNow_Resuming;
            cts = new CancellationTokenSource();

            wm = WeatherManager.GetInstance();
            WeatherView = new WeatherNowViewModel();
            WeatherView.PropertyChanged += WeatherView_PropertyChanged;
            ForecastView = new ForecastGraphViewModel(Dispatcher);
            AlertsView = new WeatherAlertsViewModel(Dispatcher);

            geolocal = new Geolocator() { DesiredAccuracyInMeters = 5000, ReportInterval = 900000, MovementThreshold = 1600 };

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("Nav_WeatherNow/Content");
            PrimaryCommands = new List<muxc.NavigationViewItemBase>()
            {
                new muxc.NavigationViewItem()
                {
                    Icon = new SymbolIcon(Symbol.Pin),
                    Content = App.ResLoader.GetString("Label_Pin/Text"),
                    Tag = "pin",
                    Visibility = Visibility.Collapsed
                },
                new muxc.NavigationViewItem()
                {
                    Icon = new SymbolIcon(Symbol.Refresh),
                    Content = App.ResLoader.GetString("Button_Refresh/Label"),
                    Tag = "refresh"
                }
            };
            GetRefreshBtn().Tapped += RefreshButton_Click;
            GetPinBtn().Tapped += PinButton_Click;

            AnalyticsLogger.LogEvent("WeatherNow");
        }

        private async void WeatherView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Sunrise":
                case "Sunset":
                    if (!String.IsNullOrWhiteSpace(WeatherView.Sunrise) && !String.IsNullOrWhiteSpace(WeatherView.Sunset))
                    {
                        while (SunPhasePanel == null || (bool)!SunPhasePanel?.ReadyToDraw)
                        {
                            await Task.Delay(1).ConfigureAwait(true);
                        }

                        SunPhasePanel?.SetSunriseSetTimes(
                            DateTime.Parse(WeatherView.Sunrise, CultureInfo.InvariantCulture).TimeOfDay,
                            DateTime.Parse(WeatherView.Sunset, CultureInfo.InvariantCulture).TimeOfDay,
                            locationData?.tz_offset);
                    }
                    break;

                case "Alerts":
                    ResizeAlertPanel();
                    break;

                case "RadarURL":
                    NavigateToRadarURL();
                    break;
            }
        }

        private muxc.NavigationViewItem GetRefreshBtn()
        {
            return PrimaryCommands.LastOrDefault() as muxc.NavigationViewItem;
        }

        private muxc.NavigationViewItem GetPinBtn()
        {
            return PrimaryCommands.FirstOrDefault() as muxc.NavigationViewItem;
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

        private void MainViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sender is ScrollViewer viewer && BackgroundOverlay != null && GradientOverlay != null)
            {
                // Default adj = 1.25f
                float adj = 1.25f;
                double backAlpha = ConditionPanel.ActualHeight > 0 ?
                    1 - (1 * adj * viewer.VerticalOffset / ConditionPanel.ActualHeight) : 1;
                double gradAlpha = ConditionPanel.ActualHeight > 0 ?
                    1 - (1 * adj * viewer.VerticalOffset / ConditionPanel.ActualHeight) : 1;
                BackgroundOverlay.Opacity = Math.Max(backAlpha, (float)0x25 / 0xFF); // 0x25
                GradientOverlay.Opacity = Math.Max(gradAlpha, 0);
            }
        }

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: MainGrid_SizeChanged");
            AdjustViewLayout();
        }

        private void DeferedControl_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustViewLayout();
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

        private void AdjustViewLayout()
        {
            if (Window.Current == null) return;
            var Bounds = Window.Current.Bounds;

            if (MainViewer == null) return;

            double w = MainViewer.ActualWidth - 16d - 16d; // Scrollbar padding
            double h = MainViewer.ActualHeight - MainViewer.Padding.Top - MainViewer.Padding.Bottom;

            if (w <= 0 || h <= 0) return;

            ResizeAlertPanel();

            if (ConditionPanel != null)
            {
                if (!FeatureSettings.BackgroundImage)
                {
                    ConditionPanel.Height = double.NaN;
                }
                else
                {
                    ConditionPanel.Height = h;
                }

                ConditionPanel.MaxWidth = 1280;
            }

            if (Bounds.Height >= 691)
            {
                if (WeatherBox != null) WeatherBox.Height = WeatherBox.Width = 155;
                if (SunPhasePanel != null) SunPhasePanel.Height = 250;
            }
            else if (Bounds.Height >= 641)
            {
                if (WeatherBox != null) WeatherBox.Height = WeatherBox.Width = 130;
                if (SunPhasePanel != null) SunPhasePanel.Height = 250;
            }
            else if (Bounds.Height >= 481)
            {
                if (WeatherBox != null) WeatherBox.Height = WeatherBox.Width = 100;
                if (SunPhasePanel != null) SunPhasePanel.Height = 180;
            }
            else if (Bounds.Height >= 361)
            {
                if (WeatherBox != null) WeatherBox.Height = WeatherBox.Width = 75;
                if (SunPhasePanel != null) SunPhasePanel.Height = 180;
            }
            else
            {
                if (WeatherBox != null) WeatherBox.Height = WeatherBox.Width = 50;
                if (SunPhasePanel != null) SunPhasePanel.Height = 180;
            }

            if (Bounds.Width >= 1007)
            {
                if (Location != null) Location.FontSize = 32;
                if (CurTemp != null) CurTemp.FontSize = 32;
                if (CurCondition != null) CurCondition.FontSize = 32;
            }
            else if (Bounds.Width >= 641)
            {
                if (Location != null) Location.FontSize = 28;
                if (CurTemp != null) CurTemp.FontSize = 28;
                if (CurCondition != null) CurCondition.FontSize = 28;
            }
            else
            {
                if (Location != null) Location.FontSize = 24;
                if (CurTemp != null) CurTemp.FontSize = 24;
                if (CurCondition != null) CurCondition.FontSize = 24;
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

            if (FeatureSettings.WasUpdated)
            {
                // Force binding update for FeatureSettings
                this.Bindings.Update();

                FeatureSettings.WasUpdated = false;
            }

            args = e?.Parameter as WeatherNowArgs;

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
                return await VerifyLocationData();
            }).ContinueWith((t) =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    var locationChanged = t.Result;

                    // New page instance -> loaded = true
                    // Navigating back to existing page instance => loaded = false
                    // Weather location changed (ex. due to GPS setting) -> locationChanged = true
                    if (locationChanged || wLoader == null)
                    {
                        Restore();
                    }
                    else
                    {
                        var userlang = GlobalizationPreferences.Languages[0];
                        var culture = new CultureInfo(userlang);
                        var locale = wm.LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

                        if (!String.Equals(WeatherView.WeatherSource, Settings.API) ||
                            wm.SupportsWeatherLocale && !String.Equals(WeatherView.WeatherLocale, locale))
                        {
                            Restore();
                        }
                        else
                        {
                            // Check pin tile status
                            CheckTiles();

                            Task.Run(async () =>
                            {
                                // Update weather if needed on resume
                                if (Settings.FollowGPS && await UpdateLocation())
                                {
                                    // Setup loader from updated location
                                    wLoader = new WeatherDataLoader(locationData);
                                }
                            }).ContinueWith((t2) =>
                            {
                                RefreshWeather(false);
                            }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(true);
                        }
                    }
                }

                if (t.IsFaulted)
                {
                    var ex = t.Exception.GetBaseException();

                    Logger.WriteLine(LoggerLevel.Error, ex);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(true);
        }

        /// <summary>
        /// Restore
        /// </summary>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        /// <exception cref="InvalidOperationException">Ignore.</exception>
        private void Restore()
        {
            // Check pin tile status
            CheckTiles();

            Task.Run(async () =>
            {
                bool forceRefresh = false;

                // GPS Follow location
                if (Settings.FollowGPS && (locationData == null || locationData.locationType == LocationType.GPS))
                {
                    LocationData locData = await Settings.GetLastGPSLocData();

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

                cts?.Token.ThrowIfCancellationRequested();

                if (locationData != null)
                    wLoader = new WeatherDataLoader(locationData);

                return forceRefresh;
            }).ContinueWith((t) =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    // Load up weather data
                    RefreshWeather(t.Result);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(true);
        }

        private Task<bool> UpdateLocation()
        {
            return Task.Run(async () =>
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
                        if (lastGPSLocData.query != null
                            && geoPos != null && ConversionMethods.CalculateGeopositionDistance(geoPos, newGeoPos) < geolocal.MovementThreshold)
                        {
                            return false;
                        }

                        if (lastGPSLocData.query != null
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
                                view = await wm.GetLocation(newGeoPos);

                                if (String.IsNullOrEmpty(view.LocationQuery))
                                    view = new LocationQueryViewModel();
                            }
                            catch (WeatherException ex)
                            {
                                view = new LocationQueryViewModel();

                                await Dispatcher.RunOnUIThread(() =>
                                {
                                    ShowSnackbar(Snackbar.Make(ex.Message, SnackbarDuration.Short));
                                }).ConfigureAwait(false);
                            }
                        });

                        if (String.IsNullOrWhiteSpace(view.LocationQuery))
                        {
                            // Stop since there is no valid query
                            return false;
                        }

                        if (cts?.IsCancellationRequested == true)
                            return false;

                        // Save oldkey
                        string oldkey = lastGPSLocData.query;

                        // Save location as last known
                        lastGPSLocData.SetData(view, newGeoPos);
                        Settings.SaveLastGPSLocData(lastGPSLocData);

                        // Update tile id for location
                        if (oldkey != null && SecondaryTileUtils.Exists(oldkey))
                        {
                            await SecondaryTileUtils.UpdateTileId(oldkey, lastGPSLocData.query).ConfigureAwait(false);
                        }

                        locationData = lastGPSLocData;
                        geoPos = newGeoPos;
                        locationChanged = true;
                    }
                }

                return locationChanged;
            });
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: RefreshButton_Click");

            if (Settings.FollowGPS && await UpdateLocation().ConfigureAwait(true))
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
                LoadingRing.IsActive = true;

                if (wLoader == null && locationData != null)
                {
                    wLoader = new WeatherDataLoader(locationData);
                }

                wLoader?.LoadWeatherResult(new WeatherRequest.Builder()
                        .ForceRefresh(forceRefresh)
                        .SetErrorListener(this)
                        .Build())
                        .ContinueWith((t) =>
                        {
                            if (t.IsCompletedSuccessfully)
                            {
                                Dispatcher.RunOnUIThread(() =>
                                {
                                    OnWeatherLoaded(locationData, t.Result.Weather);
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
                            AlertsView.UpdateAlerts(locationData);

                            if (wm.SupportsAlerts)
                            {
                                if (t.Result?.Any() == true)
                                {
                                    // Alerts are posted to the user here. Set them as notified.
#if DEBUG
                                    WeatherAlertHandler.PostAlerts(locationData, t.Result)
                                    .ConfigureAwait(false);
#endif
                                    WeatherAlertHandler.SetasNotified(locationData, t.Result);
                                }
                            }

                        }, TaskScheduler.FromCurrentSynchronizationContext())
                        .ConfigureAwait(true);
            }
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            var controlName = (sender as FrameworkElement)?.Name;

            if ((bool)controlName?.Contains("Hourly"))
            {
                ScrollViewerHelper.ScrollLeft(HourlyGraphPanel?.ScrollViewer);
            }
            else
            {
                ScrollViewerHelper.ScrollLeft(ForecastGraphPanel?.ScrollViewer);
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            var controlName = (sender as FrameworkElement)?.Name;

            if ((bool)controlName?.Contains("Hourly"))
            {
                ScrollViewerHelper.ScrollRight(HourlyGraphPanel?.ScrollViewer);
            }
            else
            {
                ScrollViewerHelper.ScrollRight(ForecastGraphPanel?.ScrollViewer);
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
                Location = locationData,
                WeatherNowView = WeatherView
            }, transition);
        }

        private void AlertButton_Click(object sender, RoutedEventArgs e)
        {
            GotoAlertsPage();
        }

        private void CheckTiles()
        {
            // Check if Tile service is available
            if (!DeviceTypeHelper.IsTileSupported()) return;

            var pinBtn = GetPinBtn();

            if (pinBtn != null)
            {
                pinBtn.IsEnabled = false;

                // Check if your app is currently pinned
                bool isPinned = SecondaryTileUtils.Exists(locationData?.query);

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
                    pinBtn.Content = App.ResLoader.GetString("Label_Unpin/Text");
                }
                else
                {
                    pinBtn.Icon = new SymbolIcon(Symbol.Pin);
                    pinBtn.Content = App.ResLoader.GetString("Label_Pin/Text");
                }
            }
        }

        private async void PinButton_Click(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: PinButton_Click");

            // Check if Tile service is available
            if (!DeviceTypeHelper.IsTileSupported() || locationData?.query == null) return;

            var pinBtn = sender as muxc.NavigationViewItem;
            pinBtn.IsEnabled = false;

            if (SecondaryTileUtils.Exists(locationData?.query))
            {
                bool deleted = await new SecondaryTile(
                    SecondaryTileUtils.GetTileId(locationData.query)).RequestDeleteAsync().AsTask().ConfigureAwait(true);
                if (deleted)
                {
                    SecondaryTileUtils.RemoveTileId(locationData.query);
                }

                SetPinButton(!deleted);

                pinBtn.IsEnabled = true;
            }
            else
            {
                // Initialize the tile with required arguments
                var tileID = DateTime.Now.Ticks.ToString();
                var tile = new SecondaryTile(
                    tileID,
                    "SimpleWeather",
                    "action=view-weather&query=" + locationData.query,
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
                    SecondaryTileUtils.AddTileId(locationData.query, tileID);
                    await WeatherTileCreator.TileUpdater(locationData).ConfigureAwait(true);
                    await tile.UpdateAsync().AsTask().ConfigureAwait(true);
                }

                SetPinButton(isPinned);

                pinBtn.IsEnabled = true;
            }
        }

        private void GotoDetailsPage(bool IsHourly, int Position)
        {
            Frame.Navigate(typeof(WeatherDetailsPage),
                new DetailsPageArgs()
                {
                    Location = locationData,
                    WeatherNowView = WeatherView,
                    IsHourly = IsHourly,
                    ScrollToPosition = Position
                }, new DrillInNavigationTransitionInfo());
        }

        private void LineView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: LineView_Tapped");

            var control = sender as LineView;
            FrameworkElement controlParent = null;

            try
            {
                controlParent = VisualTreeHelperExtensions.GetParent<ForecastGraphPanel>(control);
            }
            catch (Exception) { }

            GotoDetailsPage((bool)controlParent?.Name.StartsWith("Hourly"),
                control.GetItemPositionFromPoint((float)(e.GetPosition(control).X + control?.ScrollViewer?.HorizontalOffset)));
        }

        private void NavigateToRadarURL()
        {
            if (WeatherView?.RadarURL == null) return;

            var webview = RadarWebViewContainer?.Child as WebView;

            if (webview == null && RadarWebViewContainer == null) return;
            else if (RadarWebViewContainer != null && webview == null)
            {
                webview = CreateWebView();
                RadarWebViewContainer.Child = webview;
            }

            webview.NavigationStarting -= RadarWebView_NavigationStarting;
            webview.Navigate(WeatherView.RadarURL);
            webview.NavigationStarting += RadarWebView_NavigationStarting;
        }

        private WebView CreateWebView()
        {
            WebView webview = null;

            // Windows 1803+
            if (Windows.Foundation.Metadata.ApiInformation.IsEnumNamedValuePresent("Windows.UI.Xaml.Controls.WebViewExecutionMode", "SeparateProcess"))
            {
                try
                {
                    // NOTE: Potential managed code exception; don't know why
                    webview = new WebView(WebViewExecutionMode.SeparateProcess);
                }
                catch (Exception e)
                {
                    webview = null;
                }
            }

            if (webview == null)
                webview = new WebView(WebViewExecutionMode.SeparateThread);

            webview.NavigationCompleted += async (s, args) =>
            {
                var disableInteractions = new string[] { "var style = document.createElement('style'); style.innerHTML = '* { pointer-events: none !important; overscroll-behavior: none !important; overflow: hidden !important; } body { pointer-events: all !important; overscroll-behavior: none !important; overflow: hidden !important; }'; document.head.appendChild(style);" };
                try
                {
                    await s.InvokeScriptAsync("eval", disableInteractions);
                }
                catch (Exception e)
                {
                    //Logger.WriteLine(LoggerLevel.Error, e);
                }
            };

            webview.IsHitTestVisible = false;
            webview.IsDoubleTapEnabled = false;
            webview.IsHoldingEnabled = false;
            webview.IsRightTapEnabled = false;
            webview.IsTapEnabled = false;

            if (Windows.Foundation.Metadata.ApiInformation.IsEventPresent("Windows.UI.Xaml.Controls.WebView", "SeparateProcessLost"))
            {
                webview.SeparateProcessLost += (sender, e) =>
                {
                    if (RadarWebViewContainer == null) return;
                    var newWebView = CreateWebView();
                    RadarWebViewContainer.Child = newWebView;
                    NavigateToRadarURL();
                };
            }

            return webview;
        }

        private void RadarWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            // Cancel all navigation
            args.Cancel = true;
        }

        private void RadarWebView_Loaded(object sender, RoutedEventArgs e)
        {
            AsyncTask.Run(async () =>
            {
                await Dispatcher.AwaitableRunAsync(() =>
                {
                    NavigateToRadarURL();
                }, CoreDispatcherPriority.Low).ConfigureAwait(true);
            }, 1000);
        }

        private void RadarWebView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (WeatherView?.RadarURL != null)
            {
                AnalyticsLogger.LogEvent("WeatherNow: RadarWebView_Tapped");
                Frame.Navigate(typeof(WeatherRadarPage), WeatherView?.RadarURL, new DrillInNavigationTransitionInfo());
            }
        }

        private void BackgroundOverlay_ImageExOpened(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExOpenedEventArgs e)
        {
            var image = VisualTreeHelperExtensions.FindChild<Image>(sender as FrameworkElement, "Image");
            if (image != null)
            {
                if (image.ActualHeight / 3 < 500)
                {
                    ParllxView.VerticalShift = Math.Min(image.ActualHeight / 3, 100);
                }
                else
                {
                    ParllxView.VerticalShift = Math.Min(image.ActualHeight / 3, 500);
                }
            }
        }
    }
}