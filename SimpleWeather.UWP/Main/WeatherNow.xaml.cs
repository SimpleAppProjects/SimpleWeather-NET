using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Tiles;
using SimpleWeather.UWP.WeatherAlerts;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.Foundation.Metadata;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherNow : CustomPage, IWeatherLoadedListener, IWeatherErrorListener
    {
        private WeatherManager wm;
        private WeatherDataLoader wLoader = null;
        private WeatherNowViewModel WeatherView { get; set; }

        private LocationData location = null;
        private double BGAlpha = 1.0;
        private double GradAlpha = 1.0;

        private Geolocator geolocal = null;
        private Geoposition geoPos = null;

        public async void OnWeatherLoaded(LocationData location, Weather weather)
        {
            await Task.Run(async () => 
            {
                if (weather?.IsValid() == true)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        WeatherView.UpdateView(weather);
                        LoadingRing.IsActive = false;
                    });

                    if (wm.SupportsAlerts)
                    {
                        if (weather.weather_alerts != null && weather.weather_alerts.Count > 0)
                        {
                            // Alerts are posted to the user here. Set them as notified.
                            Task.Run(async () =>
                            {
                                await WeatherAlertHandler.SetasNotified(location, weather.weather_alerts);
                            });
                        }
                    }

                    // Update home tile if it hasn't been already
                    if (Settings.HomeData.Equals(location)
                        && (TimeSpan.FromTicks(DateTime.Now.Ticks - Settings.UpdateTime.Ticks).TotalMinutes > Settings.RefreshInterval)
                        || !WeatherTileCreator.TileUpdated)
                    {
                        Task.Run(WeatherUpdateBackgroundTask.RequestAppTrigger);
                    }
                    else if (SecondaryTileUtils.Exists(location.query))
                    {
                        WeatherTileCreator.TileUpdater(location, WeatherView);
                    }
                }
            });
        }

        public async void OnWeatherError(WeatherException wEx)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
            {
                switch (wEx.ErrorStatus)
                {
                    case WeatherUtils.ErrorStatus.NetworkError:
                    case WeatherUtils.ErrorStatus.NoWeather:
                        // Show error message and prompt to refresh
                        Snackbar snackbar = Snackbar.Make(wEx.Message, SnackbarDuration.Long);
                        snackbar.SetAction(App.ResLoader.GetString("Action_Retry"), () =>
                        {
                            Task.Run(() => RefreshWeather(false));
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

            wm = WeatherManager.GetInstance();
            WeatherView = new WeatherNowViewModel();
            WeatherView.PropertyChanged += WeatherView_PropertyChanged;
            WeatherView.Extras.PropertyChanged += WeatherView_PropertyChanged;

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
        }

        private void UpdateWindowColors()
        {
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
                            await Task.Delay(1);
                        }

                        var userlang = GlobalizationPreferences.Languages[0];
                        var culture = new CultureInfo(userlang);

                        SunPhasePanel?.SetSunriseSetTimes(
                            DateTime.Parse(WeatherView.Sunrise, culture).TimeOfDay, DateTime.Parse(WeatherView.Sunset, culture).TimeOfDay,
                            location?.tz_offset);
                    }
                    break;
                case "Alerts":
                    ResizeAlertPanel();
                    break;
                case "PendingBackgroundColor":
                    UpdateWindowColors();
                    break;
            }
        }

        private AppBarButton GetRefreshBtn()
        {
            return PrimaryCommands.Last() as AppBarButton;
        }

        private AppBarButton GetPinBtn()
        {
            return PrimaryCommands.First() as AppBarButton;
        }

        private async void WeatherNow_Resuming(object sender, object e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => 
            {
                if (Shell.Instance.AppFrame.SourcePageType == this.GetType())
                {
                    // Check pin tile status
                    CheckTiles();

                    await Resume();
                }
            });
        }

        private void MainViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sender is ScrollViewer viewer)
            {
                // Default adj = 1.25f
                float adj = 1.25f;
                double backAlpha = 1 - (1 * adj * viewer.VerticalOffset / viewer.ScrollableHeight);
                double gradAlpha = 1 - (1 * adj * viewer.VerticalOffset / ConditionPanel.ActualHeight);
                BGAlpha = Math.Max(backAlpha, (float)0x25 / 0xFF); // 0x25
                GradAlpha = Math.Max(gradAlpha, 0);
                BackgroundOverlay.Opacity = BGAlpha;
                GradientOverlay.Opacity = GradAlpha;
            }
        }

        private void MainViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var scroller = sender as ScrollViewer;
            ResizeAlertPanel();

            double w = scroller.ActualWidth - scroller.Padding.Left - scroller.Padding.Right;
            double h = scroller.ActualHeight - scroller.Padding.Top - scroller.Padding.Bottom;

            ConditionPanel.Height = h;
            ConditionPanel.Width = w;
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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            MainViewer?.ChangeView(null, 0, null, true);
            BGAlpha = GradAlpha = 1.0f;

            if (e.Parameter != null)
            {
                string arg = e.Parameter.ToString();

                switch (arg)
                {
                    case "view-alerts":
                        GotoAlertsPage();
                        break;
                    default:
                        break;
                }
            }

            LocationData LocParameter = e.Parameter as LocationData;

            if (e.NavigationMode == NavigationMode.New)
            {
                // Reset loader if new page instance created
                location = null;
                wLoader = null;
                MainViewer?.ChangeView(null, 0, null, true);

                // New page instance created, so restore
                if (LocParameter != null)
                {
                    location = LocParameter;
                    wLoader = new WeatherDataLoader(location, this, this);
                }

                await Restore();
            }
            else
            {
                LocationData homeData = Settings.HomeData;

                // Did home change?
                bool homeChanged = false;
                if (location != null && Frame.BackStack.Count == 0)
                {
                    if (!location.Equals(homeData))
                    {
                        location = homeData;
                        homeChanged = true;
                    }
                }

                if (wLoader != null)
                {
                    var userlang = GlobalizationPreferences.Languages.First();
                    var culture = new CultureInfo(userlang);
                    var locale = wm.LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

                    // Reset loader if source, query or locale is different
                    bool resetLoader = WeatherView.WeatherSource != Settings.API || homeChanged;
                    if (wm.SupportsWeatherLocale && !resetLoader)
                        resetLoader = WeatherView.WeatherLocale != locale;

                    if (resetLoader) wLoader = null;
                }

                // Update view on resume
                // ex. If temperature unit changed
                if ((wLoader != null) && !homeChanged)
                {
                    await Resume();

                    if (location.query == homeData.query)
                    {
                        // Clear backstack since we're home
                        Frame.BackStack.Clear();
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    }
                }
                else
                {
                    await Restore();
                }
            }
        }

        private async Task Resume()
        {
            // Check pin tile status
            CheckTiles();

            if (wLoader.GetWeather()?.IsValid() == true)
            {
                Weather weather = wLoader.GetWeather();

                // Update weather if needed on resume
                if (Settings.FollowGPS && await UpdateLocation())
                {
                    // Setup loader from updated location
                    wLoader = new WeatherDataLoader(location, this, this);
                    await RefreshWeather(false);
                }
                else
                {
                    // Check weather data expiration
                    if (!int.TryParse(weather.ttl, out int ttl))
                    {
                        ttl = Settings.DefaultInterval;
                    }
                    ttl = Math.Max(ttl, Settings.RefreshInterval);
                    TimeSpan span = DateTimeOffset.Now - weather.update_time;
                    if (span.TotalMinutes > ttl)
                    {
                        await RefreshWeather(false);
                    }
                    else
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
                        {
                            WeatherView.UpdateView(wLoader.GetWeather());
                            UpdateWindowColors();
                        });
                    }
                }
            }
        }

        private async Task Restore()
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
                    wLoader = new WeatherDataLoader(location, this, this);
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
                        wLoader = new WeatherDataLoader(location, this, this);
                        forceRefresh = true;
                    }
                    else
                    {
                        // Setup loader saved location data
                        location = locData;
                        wLoader = new WeatherDataLoader(location, this, this);
                    }
                }
            }
            // Regular mode
            else if (wLoader == null)
            {
                // Weather was loaded before. Lets load it up...
                location = Settings.HomeData;
                wLoader = new WeatherDataLoader(location, this, this);
            }

            // Check pin tile status
            CheckTiles();

            // Load up weather data
            await RefreshWeather(forceRefresh);
        }

        private async Task<bool> UpdateLocation()
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

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
                            {
                                ShowSnackbar(Snackbar.Make(ex.Message, SnackbarDuration.Short));
                            });
                        }
                    });

                    if (String.IsNullOrWhiteSpace(view.LocationQuery))
                    {
                        // Stop since there is no valid query
                        return false;
                    }

                    // Save oldkey
                    string oldkey = lastGPSLocData.query;

                    // Save location as last known
                    lastGPSLocData.SetData(view, newGeoPos);
                    Settings.SaveLastGPSLocData(lastGPSLocData);

                    // Update tile id for location
                    if (oldkey != null && SecondaryTileUtils.Exists(oldkey))
                    {
                        await SecondaryTileUtils.UpdateTileId(oldkey, lastGPSLocData.query);
                    }

                    location = lastGPSLocData;
                    geoPos = newGeoPos;
                    locationChanged = true;
                }
            }

            return locationChanged;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.FollowGPS && await UpdateLocation())
                // Setup loader from updated location
                wLoader = new WeatherDataLoader(location, this, this);

            await RefreshWeather(true);
        }

        private async Task RefreshWeather(bool forceRefresh)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => LoadingRing.IsActive = true);
            Task.Run(() => wLoader.LoadWeatherData(forceRefresh));
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            var controlName = (sender as FrameworkElement)?.Name;

            if ((bool)controlName?.Contains("Hourly"))
            {
                ScrollLeft(HourlyGraphPanel?.ScrollViewer);
            }
            else
            {
                ScrollLeft(ForecastGraphPanel?.ScrollViewer);
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            var controlName = (sender as FrameworkElement)?.Name;

            if ((bool)controlName?.Contains("Hourly"))
            {
                ScrollRight(HourlyGraphPanel?.ScrollViewer);
            }
            else
            {
                ScrollRight(ForecastGraphPanel?.ScrollViewer);
            }
        }

        private void ForecastViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer viewer = sender as ScrollViewer;
            FrameworkElement controlParent = null;

            if ("InternalScrollViewer".Equals(viewer?.Name))
            {
                try
                {
                    controlParent = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(
                        VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(viewer?.Parent)))) as FrameworkElement;
                } catch (Exception) { }
            }

            if (viewer?.HorizontalOffset == 0)
            {
                if (controlParent?.Name?.Contains("Hourly") ?? false)
                    HourlyLeftButton.IsEnabled = false;
                else
                    LeftButton.IsEnabled = false;
            }
            else if (viewer?.HorizontalOffset == viewer?.ScrollableWidth)
            {
                if (controlParent?.Name?.Contains("Hourly") ?? false)
                    HourlyRightButton.IsEnabled = false;
                else
                    RightButton.IsEnabled = false;
            }
            else
            {
                if (controlParent?.Name?.Contains("Hourly") ?? false)
                {
                    if (!HourlyLeftButton.IsEnabled)
                        HourlyLeftButton.IsEnabled = true;
                    if (!HourlyRightButton.IsEnabled)
                        HourlyRightButton.IsEnabled = true;
                }
                else
                {
                    if (!LeftButton.IsEnabled)
                        LeftButton.IsEnabled = true;
                    if (!RightButton.IsEnabled)
                        RightButton.IsEnabled = true;
                }
            }
        }

        private static void ScrollLeft(ScrollViewer viewer)
        {
            viewer?.ChangeView(viewer?.HorizontalOffset - viewer?.ActualWidth, null, null);
        }

        private static void ScrollRight(ScrollViewer viewer)
        {
            viewer?.ChangeView(viewer?.HorizontalOffset + viewer?.ActualWidth, null, null);
        }

        private void GotoAlertsPage()
        {
            Frame.Navigate(typeof(WeatherAlertPage), WeatherView);
        }

        private void AlertButton_Click(object sender, RoutedEventArgs e)
        {
            GotoAlertsPage();
        }

        private void CheckTiles()
        {
            var pinBtn = GetPinBtn();
            pinBtn.IsEnabled = false;

            // Check if your app is currently pinned
            bool isPinned = SecondaryTileUtils.Exists(location.query);

            SetPinButton(isPinned);
            pinBtn.Visibility = Visibility.Visible;
            pinBtn.IsEnabled = true;
        }

        private void SetPinButton(bool isPinned)
        {
            var pinBtn = GetPinBtn();

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

        private async void PinButton_Click(object sender, RoutedEventArgs e)
        {
            var pinBtn = sender as AppBarButton;
            pinBtn.IsEnabled = false;

            if (SecondaryTileUtils.Exists(location.query))
            {
                bool deleted = await new SecondaryTile(
                    SecondaryTileUtils.GetTileId(location.query)).RequestDeleteAsync();
                if (deleted)
                {
                    SecondaryTileUtils.RemoveTileId(location.query);
                }

                SetPinButton(!deleted);

                GetPinBtn().IsEnabled = true;
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
                    await WeatherTileCreator.TileUpdater(location);
                    await tile.UpdateAsync();
                }

                SetPinButton(isPinned);

                GetPinBtn().IsEnabled = true;
            }
        }

        private void GotoDetailsPage(bool IsHourly, int Position)
        {
            Frame.Navigate(typeof(WeatherDetailsPage),
                new WeatherDetailsPage.DetailsPageArgs()
                {
                    WeatherNowView = WeatherView,
                    IsHourly = IsHourly,
                    ScrollToPosition = Position
                });
        }

        private void LineView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var control = sender as LineView;
            FrameworkElement controlParent = null;

            try
            {
                controlParent = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(
                    VisualTreeHelper.GetParent(control?.Parent))) as FrameworkElement;
            }
            catch (Exception) { }

            GotoDetailsPage((bool)controlParent?.Name.StartsWith("Hourly"),
                control.GetItemPositionFromPoint((float)e.GetPosition(null).X));
        }
    }
}
