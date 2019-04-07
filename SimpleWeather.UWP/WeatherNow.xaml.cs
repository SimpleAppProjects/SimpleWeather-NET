using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
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
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherNow : Page, ICommandBarPage, IWeatherLoadedListener, IWeatherErrorListener
    {
        private WeatherManager wm;
        private WeatherDataLoader wLoader = null;
        private WeatherNowViewModel WeatherView { get; set; }
        public string CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        private ToggleButton SelectedButton;

        private LocationData location = null;
        private double BGAlpha = 1.0;

        private Geolocator geolocal = null;
        private Geoposition geoPos = null;

        public void OnWeatherLoaded(LocationData location, Weather weather)
        {
            if (weather?.IsValid() == true)
            {
                wm.UpdateWeather(weather);
                WeatherView.UpdateView(weather);

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

                    // Show/Hide Alert panel
                    if (WeatherView.Extras.Alerts.Count > 0)
                    {
                        FrameworkElement alertButton = AlertButton;
                        if (alertButton == null)
                            alertButton = FindName(nameof(AlertButton)) as FrameworkElement;

                        ResizeAlertPanel();
                        alertButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        FrameworkElement alertButton = AlertButton;
                        if (alertButton != null)
                            alertButton.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    FrameworkElement alertButton = AlertButton;
                    if (alertButton != null)
                        alertButton.Visibility = Visibility.Collapsed;
                }

                // Update home tile if it hasn't been already
                if (Settings.HomeData.Equals(location)
                    && (TimeSpan.FromTicks(DateTime.Now.Ticks - Settings.UpdateTime.Ticks).TotalMinutes > Settings.RefreshInterval)
                    || !WeatherTileCreator.TileUpdated)
                {
                    Task.Run(async () => await WeatherUpdateBackgroundTask.RequestAppTrigger());
                }
                else if (SecondaryTileUtils.Exists(location.query))
                {
                    WeatherTileCreator.TileUpdater(location, weather);
                }

                // Shell
                Shell.Instance.AppBarColor = WeatherView.PendingBackgroundColor;
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    // Mobile
                    StatusBar.GetForCurrentView().BackgroundColor = WeatherView.PendingBackgroundColor;
                }
                else
                {
                    // Desktop
                    var titlebar = ApplicationView.GetForCurrentView().TitleBar;
                    titlebar.BackgroundColor = WeatherView.PendingBackgroundColor;
                    titlebar.ButtonBackgroundColor = titlebar.BackgroundColor;
                }
            }

            if (WeatherView.Extras.HourlyForecast.Count >= 1)
                HourlyForecastPanel.Visibility = Visibility.Visible;
            else
                HourlyForecastPanel.Visibility = Visibility.Collapsed;

            if ((Settings.API.Equals(WeatherAPI.OpenWeatherMap) || Settings.API.Equals(WeatherAPI.MetNo)) &&
                RainToggleButton.Visibility == Visibility.Visible)
            {
                RainToggleButton.Visibility = Visibility.Collapsed;
                if ((bool)RainToggleButton.IsChecked)
                {
                    RainToggleButton.IsChecked = false;
                    TempToggleButton.IsChecked = true;
                }
            }

            LoadingRing.IsActive = false;
        }

        public void OnWeatherError(WeatherException wEx)
        {
            switch (wEx.ErrorStatus)
            {
                case WeatherUtils.ErrorStatus.NetworkError:
                case WeatherUtils.ErrorStatus.NoWeather:
                    // Show error message and prompt to refresh
                    Snackbar snackBar = Snackbar.Make(Content as Grid, wEx.Message, SnackbarDuration.Long);
                    snackBar.SetAction(App.ResLoader.GetString("Action_Retry"), () =>
                    {
                        Task.Run(async () => await RefreshWeather(false));
                    });
                    snackBar.Show();
                    break;
                default:
                    // Show error message
                    Snackbar.Make(Content as Grid, wEx.Message, SnackbarDuration.Long).Show();
                    break;
            }
        }

        public WeatherNow()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            Application.Current.Resuming += WeatherNow_Resuming;

            wm = WeatherManager.GetInstance();
            WeatherView = new WeatherNowViewModel();
            WeatherView.PropertyChanged += WeatherView_PropertyChanged;
            WeatherView.Extras.PropertyChanged += WeatherView_PropertyChanged;
            StackControl.SizeChanged += StackControl_SizeChanged;
            MainViewer.SizeChanged += MainViewer_SizeChanged;
            MainViewer.ViewChanged += MainViewer_ViewChanged;

            // Additional Details (Extras)
            HourlyForecastPanel.Visibility = Visibility.Collapsed;
            HourlySwitch.IsOn = true;

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
        }

        private async void WeatherView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Sunrise":
                case "Sunset":
                    if (!String.IsNullOrWhiteSpace(WeatherView.Sunrise) && !String.IsNullOrWhiteSpace(WeatherView.Sunset))
                    {
                        while (!SunPhasePanel.ReadyToDraw)
                        {
                            await Task.Delay(1);
                        }

                        SunPhasePanel.SetSunriseSetTimes(
                            DateTime.Parse(WeatherView.Sunrise).TimeOfDay, DateTime.Parse(WeatherView.Sunset).TimeOfDay,
                            location?.tz_offset);
                    }
                    break;
                case "HourlyForecast":
                    await UpdateLineView();
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
            if (sender is ScrollViewer viewer && viewer.Background != null)
            {
                // Default adj = 1
                float adj = 2.5f;
                double alpha = 1 - (adj * viewer.VerticalOffset / viewer.ScrollableHeight);
                MainViewer.Background.Opacity = (alpha >= 0) ? BGAlpha = alpha : BGAlpha = 0;
            }
        }

        private void StackControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeForecastItems();
        }

        private void ResizeForecastItems()
        {
            // Resize StackControl items
            double StackWidth = ForecastViewer.ActualWidth;

            if (StackWidth <= 0)
                return;

            // For first launch resize forecast panels when panel is filled
            if (StackControl.ItemsPanelRoot != null && StackControl.Items.Count > 0 &&
                StackControl.Items.Count == WeatherView.Forecasts.Count)
            {
                var StackCollection = StackControl.ItemsPanelRoot.Children.Cast<FrameworkElement>();
                var firstElement = StackCollection.First();
                double itemsWidth = firstElement.ActualWidth * StackControl.Items.Count;

                if (itemsWidth < StackWidth)
                {
                    double freeSpace = StackWidth - itemsWidth;
                    double itemWidth = (itemsWidth / StackControl.Items.Count) + (freeSpace / StackControl.Items.Count);

                    foreach (FrameworkElement element in StackCollection)
                    {
                        element.Width = itemWidth;
                    }
                }
                // Only change width we did before (default is NaN)
                else if (itemsWidth > StackWidth + 100 && !Double.IsNaN(firstElement.Width))
                {
                    foreach (FrameworkElement element in StackCollection)
                    {
                        element.Width = Double.NaN;
                    }
                }
            }

            if (DetailsPanel.ItemsPanelRoot is ItemsWrapGrid WrapsGrid)
            {
                if (StackWidth <= 640)
                {
                    double freeSpace = StackWidth - WrapsGrid.Margin.Left - WrapsGrid.Margin.Right;
                    int columns = 3;
                    WrapsGrid.ItemWidth =  freeSpace / columns;
                }
                else
                {
                    WrapsGrid.ItemWidth = Double.NaN;
                }
            }
        }

        private void MainViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeAlertPanel();

            double w = this.ActualWidth;
            double h = this.ActualHeight;

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

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // Clear cache if nav'ing back
            if (e.NavigationMode == NavigationMode.Back)
                NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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
                MainViewer.ChangeView(null, 0, null);

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

            MainViewer.ChangeView(null, 0, null);
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
                        ttl = Settings.DefaultInterval;
                    TimeSpan span = DateTimeOffset.Now - weather.update_time;
                    if (span.TotalMinutes > ttl)
                    {
                        await RefreshWeather(false);
                    }
                    else
                    {
                        WeatherView.UpdateView(wLoader.GetWeather());
                        Shell.Instance.AppBarColor = WeatherView.PendingBackgroundColor;
                        if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                        {
                            // Mobile
                            StatusBar.GetForCurrentView().BackgroundColor = WeatherView.PendingBackgroundColor;
                        }
                        else
                        {
                            // Desktop
                            var titlebar = ApplicationView.GetForCurrentView().TitleBar;
                            titlebar.BackgroundColor = WeatherView.PendingBackgroundColor;
                            titlebar.ButtonBackgroundColor = titlebar.BackgroundColor;
                        }
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
                    if (locData.source != Settings.API)
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
                        view = await wm.GetLocation(newGeoPos);

                        if (String.IsNullOrEmpty(view.LocationQuery))
                            view = new LocationQueryViewModel();
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
            LoadingRing.IsActive = true;
            await wLoader.LoadWeatherData(forceRefresh);
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            var controlName = (sender as FrameworkElement)?.Name;

            if ((bool)controlName?.Contains("Hourly"))
            {
                if (HourlyForecastViewer.Visibility == Visibility.Visible)
                    ScrollLeft(HourlyForecastViewer);
                else
                    ScrollLeft(HourlyLineView.ScrollViewer);
            }
            else
            {
                ScrollLeft(ForecastViewer);
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            var controlName = (sender as FrameworkElement)?.Name;

            if ((bool)controlName?.Contains("Hourly"))
            {
                if (HourlyForecastViewer.Visibility == Visibility.Visible)
                    ScrollRight(HourlyForecastViewer);
                else
                    ScrollRight(HourlyLineView.ScrollViewer);
            }
            else
            {
                ScrollRight(ForecastViewer);
            }
        }

        private void ForecastViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer viewer = sender as ScrollViewer;
            var controlName = (sender as FrameworkElement).Name;
            var controlParent = viewer?.Parent as FrameworkElement;

            if (viewer.HorizontalOffset == 0)
            {
                if (controlName.Contains("Hourly") || (bool)controlParent?.Name?.Contains("Hourly"))
                    HourlyLeftButton.IsEnabled = false;
                else
                    LeftButton.IsEnabled = false;
            }
            else if (viewer.HorizontalOffset == viewer.ScrollableWidth)
            {
                if (controlName.Contains("Hourly") || (bool)controlParent?.Name?.Contains("Hourly"))
                    HourlyRightButton.IsEnabled = false;
                else
                    RightButton.IsEnabled = false;
            }
            else
            {
                if (controlName.Contains("Hourly") || (bool)controlParent?.Name?.Contains("Hourly"))
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
            viewer.ChangeView(viewer.HorizontalOffset - viewer.ActualWidth, null, null);
        }

        private static void ScrollRight(ScrollViewer viewer)
        {
            viewer.ChangeView(viewer.HorizontalOffset + viewer.ActualWidth, null, null);
        }

        private void HourlySwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch sw = sender as ToggleSwitch;

            HourlyGraphPanel.Visibility = sw.IsOn ?
                Visibility.Collapsed : Visibility.Visible;
            HourlyForecastViewer.Visibility = sw.IsOn ? 
                Visibility.Visible : Visibility.Collapsed;

            ForecastViewer_ViewChanged(sw.IsOn ? HourlyForecastViewer : HourlyLineView.ScrollViewer, null);
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

        private async void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            SelectedButton = btn;

            if (btn.Parent is FrameworkElement parent && VisualTreeHelper.GetChildrenCount(parent) > 1)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    if (VisualTreeHelper.GetChild(parent, i) is ToggleButton other && !other.Tag.Equals(btn.Tag))
                    {
                        other.IsChecked = false;
                    }
                }
            }

            // Update line view
            await UpdateLineView();
        }

        private async Task UpdateLineView()
        {
            switch (SelectedButton?.Tag)
            {
                case "Temp":
                default:
                    if (WeatherView?.Extras?.HourlyForecast?.Count > 0)
                    {
                        HourlyLineView.DrawGridLines = false;
                        HourlyLineView.DrawDotLine = false;
                        HourlyLineView.DrawDataLabels = true;
                        HourlyLineView.DrawIconsLabels = true;
                        HourlyLineView.DrawGraphBackground = true;
                        HourlyLineView.DrawDotPoints = false;

                        List<KeyValuePair<String, String>> tempLabels = new List<KeyValuePair<String, String>>();
                        List<List<float>> tempDataList = new List<List<float>>();
                        List<KeyValuePair<String, int>> iconLabels = new List<KeyValuePair<String, int>>();
                        List<float> tempData = new List<float>();

                        foreach (HourlyForecastItemViewModel forecastItemViewModel in WeatherView.Extras.HourlyForecast)
                        {
                            try
                            {
                                float temp = float.Parse(forecastItemViewModel.HiTemp.RemoveNonDigitChars());

                                tempLabels.Add(new KeyValuePair<String, String>(forecastItemViewModel.Date, forecastItemViewModel.HiTemp.Trim()));
                                iconLabels.Add(new KeyValuePair<String, int>(forecastItemViewModel.WeatherIcon, 0));
                                tempData.Add(temp);

                            }
                            catch (FormatException ex)
                            {
                                Logger.WriteLine(LoggerLevel.Debug, ex, "WeatherNow: error!!");
                            }
                        }

                        tempDataList.Add(tempData);

                        while (!HourlyLineView.ReadyToDraw)
                        {
                            await Task.Delay(1);
                        }

                        HourlyLineView.SetDataLabels(iconLabels, tempLabels);
                        HourlyLineView.SetDataList(tempDataList);
                    }
                    break;
                case "Wind":
                    if (WeatherView?.Extras?.HourlyForecast?.Count > 0)
                    {
                        HourlyLineView.DrawGridLines = false;
                        HourlyLineView.DrawDotLine = false;
                        HourlyLineView.DrawDataLabels = true;
                        HourlyLineView.DrawIconsLabels = false;
                        HourlyLineView.DrawGraphBackground = true;
                        HourlyLineView.DrawDotPoints = false;

                        List<KeyValuePair<String, String>> windLabels = new List<KeyValuePair<String, String>>();
                        List<List<float>> windDataList = new List<List<float>>();
                        List<float> windData = new List<float>();
                        List<KeyValuePair<String, int>> iconLabels = new List<KeyValuePair<String, int>>();

                        foreach (HourlyForecastItemViewModel forecastItemViewModel in WeatherView.Extras.HourlyForecast)
                        {
                            try
                            {
                                float wind = float.Parse(forecastItemViewModel.WindSpeed.RemoveNonDigitChars());

                                windLabels.Add(new KeyValuePair<String, String>(forecastItemViewModel.Date, forecastItemViewModel.WindSpeed));
                                iconLabels.Add(new KeyValuePair<String, int>(WeatherIcons.WIND_DIRECTION, (int)forecastItemViewModel.WindDirection.Angle));
                                windData.Add(wind);

                            }
                            catch (FormatException ex)
                            {
                                Logger.WriteLine(LoggerLevel.Debug, ex, "WeatherNow: error!!");
                            }
                        }

                        windDataList.Add(windData);

                        while (!HourlyLineView.ReadyToDraw)
                        {
                            await Task.Delay(1);
                        }

                        HourlyLineView.SetDataLabels(iconLabels, windLabels);
                        HourlyLineView.SetDataList(windDataList);
                    }
                    break;
                case "Rain":
                    if (WeatherView?.Extras?.HourlyForecast?.Count > 0)
                    {
                        HourlyLineView.DrawGridLines = false;
                        HourlyLineView.DrawDotLine = false;
                        HourlyLineView.DrawDataLabels = true;
                        HourlyLineView.DrawIconsLabels = false;
                        HourlyLineView.DrawGraphBackground = true;
                        HourlyLineView.DrawDotPoints = false;

                        List<KeyValuePair<String, String>> popLabels = new List<KeyValuePair<String, String>>();
                        List<List<float>> popDataList = new List<List<float>>();
                        List<float> popData = new List<float>();
                        List<KeyValuePair<String, int>> iconLabels = new List<KeyValuePair<String, int>>();

                        foreach (HourlyForecastItemViewModel forecastItemViewModel in WeatherView.Extras.HourlyForecast)
                        {
                            try
                            {
                                float pop = float.Parse(forecastItemViewModel.PoP.RemoveNonDigitChars());

                                popLabels.Add(new KeyValuePair<String, String>(forecastItemViewModel.Date, forecastItemViewModel.PoP.Trim()));
                                popData.Add(pop);

                            }
                            catch (FormatException ex)
                            {
                                Logger.WriteLine(LoggerLevel.Debug, ex, "WeatherNow: error!!");
                            }
                        }

                        iconLabels.Add(new KeyValuePair<string, int>(WeatherIcons.RAINDROP, 0));
                        popDataList.Add(popData);

                        while (!HourlyLineView.ReadyToDraw)
                        {
                            await Task.Delay(1);
                        }

                        HourlyLineView.SetDataLabels(iconLabels, popLabels);
                        HourlyLineView.SetDataList(popDataList);
                    }
                    break;
            }

            HourlyLineView?.ScrollViewer?.ChangeView(0, null, null);
        }
    }
}
