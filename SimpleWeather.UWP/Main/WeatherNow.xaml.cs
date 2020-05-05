using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Shared.Helpers;
using SimpleWeather.UWP.Tiles;
using SimpleWeather.UWP.WeatherAlerts;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

        private LocationData location = null;
        private bool loaded = false;
        private WeatherNowViewModel WeatherView { get; set; }
        private ForecastGraphViewModel ForecastView { get; set; }
        private WeatherAlertsViewModel AlertsView { get; set; }

        private double BGAlpha = 1.0;
        private double GradAlpha = 1.0;
        private CancellationTokenSource cts;

        private Geolocator geolocal = null;
        private Geoposition geoPos = null;

        public void Dispose()
        {
            cts?.Dispose();
        }

        public void OnWeatherLoaded(LocationData location, Weather weather)
        {
            AsyncTask.Run(async () =>
            {
                if (cts?.IsCancellationRequested == true)
                    return;

                if (weather?.IsValid() == true)
                {
                    await WeatherView.UpdateViewAsync(weather);
                    await AlertsView.UpdateAlerts(weather);
                    await ForecastView.UpdateForecasts(weather);
                    await WeatherView.UpdateBackground();
                    await Dispatcher.RunOnUIThread(() =>
                    {
                        LoadingRing.IsActive = false;
                    }).ConfigureAwait(false);

                    if (wm.SupportsAlerts)
                    {
                        if (weather.weather_alerts != null && weather.weather_alerts.Any())
                        {
                            // Alerts are posted to the user here. Set them as notified.
                            AsyncTask.Run(async () =>
                            {
#if DEBUG
                                await WeatherAlertHandler.PostAlerts(location, weather.weather_alerts)
                                .ConfigureAwait(false);
#endif
                                await WeatherAlertHandler.SetasNotified(location, weather.weather_alerts)
                                .ConfigureAwait(false);
                            });
                        }
                    }

                    // Update home tile if it hasn't been already
                    bool isHome = Equals(location, await Settings.GetHomeData());
                    if (isHome && (TimeSpan.FromTicks(DateTime.Now.Ticks - Settings.UpdateTime.Ticks).TotalMinutes > Settings.RefreshInterval))
                    {
                        AsyncTask.Run(async () => await WeatherUpdateBackgroundTask.RequestAppTrigger()
                        .ConfigureAwait(false));
                    }
                    else if (isHome || SecondaryTileUtils.Exists(location?.query))
                    {
                        AsyncTask.Run(() =>
                        {
                            WeatherTileCreator.TileUpdater(location);
                        });
                    }
                }
            });
        }

        public void OnWeatherError(WeatherException wEx)
        {
            Dispatcher.RunOnUIThread(() =>
            {
                if (cts?.IsCancellationRequested == true)
                    return;

                switch (wEx.ErrorStatus)
                {
                    case WeatherUtils.ErrorStatus.NetworkError:
                    case WeatherUtils.ErrorStatus.NoWeather:
                        // Show error message and prompt to refresh
                        Snackbar snackbar = Snackbar.Make(wEx.Message, SnackbarDuration.Long);
                        snackbar.SetAction(App.ResLoader.GetString("Action_Retry"), () =>
                        {
                            AsyncTask.Run(() => RefreshWeather(false));
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
            ForecastView = new ForecastGraphViewModel();
            AlertsView = new WeatherAlertsViewModel();

            geolocal = new Geolocator() { DesiredAccuracyInMeters = 5000, ReportInterval = 900000, MovementThreshold = 1600 };

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("Nav_WeatherNow/Label");
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
            GetRefreshBtn().Click += RefreshButton_Click;
            GetPinBtn().Click += PinButton_Click;

            MainGrid.Loaded += (s, e) =>
            {
                UpdateWindowColors();
            };

            loaded = true;

            AnalyticsLogger.LogEvent("WeatherNow");
        }

        private void UpdateWindowColors()
        {
            if (cts?.IsCancellationRequested == true)
                return;

            if ((Settings.UserTheme == UserThemeMode.Dark || (Settings.UserTheme == UserThemeMode.System && App.IsSystemDarkTheme)) && WeatherView?.PendingBackgroundColor != App.AppColor)
            {
                var color = ColorUtils.BlendColor(WeatherView.PendingBackgroundColor, Colors.Black, 0.75f);
                MainGrid.Background = new SolidColorBrush(color);
                Shell.Instance.AppBarColor = color;
            }
            else
            {
                var color = WeatherView.PendingBackgroundColor;
                MainGrid.Background = new SolidColorBrush(color);
                Shell.Instance.AppBarColor = color;
            }
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
                            location?.tz_offset);
                    }
                    break;

                case "Alerts":
                    ResizeAlertPanel();
                    break;

                case "PendingBackgroundColor":
                    UpdateWindowColors();
                    break;

                case "RadarURL":
                    NavigateToRadarURL();
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

        private async void WeatherNow_Resuming(object sender, object e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: WeatherNow_Resuming");

            if (Shell.Instance.AppFrame.SourcePageType == this.GetType())
            {
                // Check pin tile status
                await CheckTiles();

                await Resume();
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
                BGAlpha = Math.Max(backAlpha, (float)0x25 / 0xFF); // 0x25
                GradAlpha = Math.Max(gradAlpha, 0);
                BackgroundOverlay.Opacity = BGAlpha;
                GradientOverlay.Opacity = GradAlpha;
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
                AlertButton.Width = w;
            else if (w <= 1080)
                AlertButton.Width = w * (0.75);
            else
                AlertButton.Width = w * (0.50);
        }

        private void AdjustViewLayout()
        {
            if (Window.Current == null) return;
            var Bounds = Window.Current.Bounds;

            if (MainViewer == null) return;

            double w = MainViewer.ActualWidth - MainViewer.Padding.Left - MainViewer.Padding.Right;
            double h = MainViewer.ActualHeight - MainViewer.Padding.Top - MainViewer.Padding.Bottom;

            if (w <= 0 || h <= 0) return;

            ResizeAlertPanel();

            if (ConditionPanel != null)
            {
                ConditionPanel.Height = h;
                if (w >= 1280)
                    ConditionPanel.Width = 1280;
                else
                    ConditionPanel.Width = w;
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
            loaded = false;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            AnalyticsLogger.LogEvent("WeatherNow: OnNavigatedTo");

            cts = new CancellationTokenSource();

            // NOTE: ChangeView does not work here for some reason
            MainViewer?.ScrollToVerticalOffset(0);
            BGAlpha = GradAlpha = 1.0f;

            WeatherNowArgs args = e.Parameter as WeatherNowArgs;

            bool locationChanged = false;

            // Check if we're loading from tile
            if (args?.TileId != null && SecondaryTileUtils.GetQueryFromId(args.TileId) is string locQuery)
            {
                location = await Settings.GetLocation(locQuery);
                locationChanged = loaded = true;
            }

            if (!loaded)
            {
                // Check if current location still exists (is valid)
                if (location?.locationType == LocationType.Search)
                {
                    if (await Settings.GetLocation(location?.query) == null)
                    {
                        location = null;
                        wLoader = null;
                        locationChanged = true;
                    }
                }
                // Load new favorite location if argument data is present
                if (args?.Location != null && !Object.Equals(location, args?.Location))
                {
                    location = args?.Location;
                    locationChanged = true;
                }
                else if (args?.IsHome == true)
                {
                    // Check if home location changed
                    // For ex. due to GPS setting change
                    LocationData homeData = await Settings.GetHomeData();
                    if (!location.Equals(homeData))
                    {
                        location = homeData;
                        locationChanged = true;
                    }
                }
            }
            else
            {
                if (location == null)
                    location = args?.Location;
            }

            // New page instance -> loaded = true
            // Navigating back to existing page instance => loaded = false
            // Weather location changed (ex. due to GPS setting) -> locationChanged = true
            if (loaded || locationChanged || wLoader == null)
            {
                await Restore();
            }
            else
            {
                var userlang = GlobalizationPreferences.Languages[0];
                var culture = new CultureInfo(userlang);
                var locale = wm.LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

                if (!String.Equals(WeatherView.WeatherSource, Settings.API) ||
                    wm.SupportsWeatherLocale && !String.Equals(WeatherView.WeatherLocale, locale))
                {
                    await Restore();
                }
                else
                {
                    await Resume();
                }
            }

            loaded = true;
        }

        private Task Resume()
        {
            return Task.Run(async () =>
            {
                // Check pin tile status
                await CheckTiles().ConfigureAwait(false);

                // Update weather if needed on resume
                if (Settings.FollowGPS && await UpdateLocation())
                {
                    // Setup loader from updated location
                    wLoader = new WeatherDataLoader(location);
                }

                if (cts?.IsCancellationRequested == true)
                    return;

                RefreshWeather(false);

                loaded = true;
            });
        }

        private Task Restore()
        {
            return Task.Run(async () =>
            {
                bool forceRefresh = false;

                // GPS Follow location
                if (Settings.FollowGPS && (location == null || location.locationType == LocationType.GPS))
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
                            location = locData;
                        }
                    }
                }
                // Regular mode
                else if (location == null && wLoader == null)
                {
                    // Weather was loaded before. Lets load it up...
                    location = await Settings.GetHomeData();
                }

                if (cts?.IsCancellationRequested == true)
                    return;

                // Check pin tile status
                await CheckTiles().ConfigureAwait(false);

                if (location != null)
                    wLoader = new WeatherDataLoader(location);

                // Load up weather data
                RefreshWeather(forceRefresh);
            });
        }

        private Task<bool> UpdateLocation()
        {
            return Task.Run(async () =>
            {
                bool locationChanged = false;

                if (Settings.FollowGPS && (location == null || location.locationType == LocationType.GPS))
                {
                    Geoposition newGeoPos = null;

                    try
                    {
                        newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10));
                    }
                    catch (Exception)
                    {
                        var geoStatus = GeolocationAccessStatus.Unspecified;

                        try
                        {
                            geoStatus = await Geolocator.RequestAccessAsync();
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLine(LoggerLevel.Error, ex, "WeatherNow: GetWeather error");
                        }
                        finally
                        {
                            if (geoStatus == GeolocationAccessStatus.Allowed)
                            {
                                try
                                {
                                    newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10));
                                }
                                catch (Exception ex)
                                {
                                    Logger.WriteLine(LoggerLevel.Error, ex, "WeatherNow: GetWeather error");
                                }
                            }
                            else if (geoStatus == GeolocationAccessStatus.Denied)
                            {
                                // Disable gps feature
                                Settings.FollowGPS = false;
                            }
                        }

                        if (!Settings.FollowGPS)
                            return false;
                    }

                    if (cts?.IsCancellationRequested == true)
                        return false;

                    // Access to location granted
                    if (newGeoPos != null)
                    {
                        LocationData lastGPSLocData = await Settings.GetLastGPSLocData();

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
                            await AsyncTask.RunAsync(SecondaryTileUtils.UpdateTileId(oldkey, lastGPSLocData.query));
                        }

                        location = lastGPSLocData;
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

            if (Settings.FollowGPS && await UpdateLocation())
                // Setup loader from updated location
                wLoader = new WeatherDataLoader(location);

            RefreshWeather(true);
        }

        private void RefreshWeather(bool forceRefresh)
        {
            Dispatcher.RunOnUIThread(() => LoadingRing.IsActive = true);
            AsyncTask.Run(() =>
            {
                if (cts?.IsCancellationRequested == false)
                    wLoader?.LoadWeatherData(new WeatherRequest.Builder()
                            .ForceRefresh(forceRefresh)
                            .SetErrorListener(this)
                            .Build())
                            .ContinueWith((t) => 
                            {
                                if (t.IsCompletedSuccessfully)
                                    OnWeatherLoaded(location, t.Result);
                            });
            });
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
            Frame.Navigate(typeof(WeatherAlertPage), new WeatherPageArgs() 
            {
                Location = location,
                WeatherNowView = WeatherView
            });
        }

        private void AlertButton_Click(object sender, RoutedEventArgs e)
        {
            GotoAlertsPage();
        }

        private Task CheckTiles()
        {
            // Check if Tile service is available
            if (!DeviceTypeHelper.IsTileSupported())
                return Task.CompletedTask;

            return Dispatcher.RunOnUIThread(async () =>
            {
                var pinBtn = GetPinBtn();

                if (pinBtn != null)
                {
                    pinBtn.IsEnabled = false;

                    // Check if your app is currently pinned
                    bool isPinned = SecondaryTileUtils.Exists(location?.query);

                    await SetPinButton(isPinned).ConfigureAwait(true);
                    pinBtn.Visibility = Visibility.Visible;
                    pinBtn.IsEnabled = true;
                }
            });
        }

        private Task SetPinButton(bool isPinned)
        {
            return Dispatcher.RunOnUIThread(() =>
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
            });
        }

        private async void PinButton_Click(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("WeatherNow: PinButton_Click");

            // Check if Tile service is available
            if (!DeviceTypeHelper.IsTileSupported()) return;

            var pinBtn = sender as AppBarButton;
            pinBtn.IsEnabled = false;

            if (SecondaryTileUtils.Exists(location?.query))
            {
                bool deleted = await new SecondaryTile(
                    SecondaryTileUtils.GetTileId(location.query)).RequestDeleteAsync();
                if (deleted)
                {
                    SecondaryTileUtils.RemoveTileId(location.query);
                }

                await SetPinButton(!deleted).ConfigureAwait(true);

                pinBtn.IsEnabled = true;
            }
            else
            {
                // Initialize the tile with required arguments
                var tileID = DateTime.Now.Ticks.ToString();
                var tile = new SecondaryTile(
                    tileID,
                    "SimpleWeather",
                    "action=view-weather&query=" + location.query,
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

                bool isPinned = await tile.RequestCreateAsync();
                if (isPinned)
                {
                    // Update tile with notifications
                    SecondaryTileUtils.AddTileId(location.query, tileID);
                    await WeatherTileCreator.TileUpdater(location).ConfigureAwait(true);
                    await tile.UpdateAsync();
                }

                await SetPinButton(isPinned).ConfigureAwait(true);

                pinBtn.IsEnabled = true;
            }
        }

        private void GotoDetailsPage(bool IsHourly, int Position)
        {
            Frame.Navigate(typeof(WeatherDetailsPage),
                new DetailsPageArgs()
                {
                    Location = location,
                    WeatherNowView = WeatherView,
                    IsHourly = IsHourly,
                    ScrollToPosition = Position
                });
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
            WebView webview = new WebView(WebViewExecutionMode.SeparateThread);

            webview.NavigationCompleted += async (s, args) =>
            {
                var disableInteractions = new string[] { "var style = document.createElement('style'); style.innerHTML = '* { pointer-events: none !important; overscroll-behavior: none !important; overflow: hidden !important; } body { pointer-events: all !important; overscroll-behavior: none !important; overflow: hidden !important; }'; document.head.appendChild(style);" };
                try
                {
                    await s.InvokeScriptAsync("eval", disableInteractions);
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e);
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
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    NavigateToRadarURL();
                });
            }, 1000);
        }

        private void RadarWebView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (WeatherView?.RadarURL != null)
            {
                AnalyticsLogger.LogEvent("WeatherNow: RadarWebView_Tapped");
                Frame.Navigate(typeof(WeatherRadarPage), WeatherView?.RadarURL);
            }
        }
    }
}