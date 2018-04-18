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
        WeatherManager wm;
        WeatherDataLoader wLoader = null;
        WeatherNowViewModel WeatherView { get; set; }
        public string CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        LocationData location = null;
        double BGAlpha = 1.0;

        Geolocator geolocal = null;
        Geoposition geoPos = null;

        public async void OnWeatherLoaded(LocationData location, Weather weather)
        {
            // Save index before update
            int index = TextForecastControl.SelectedIndex;

            if (weather != null)
            {
                wm.UpdateWeather(weather);
                WeatherView.UpdateView(weather);

                if (wm.SupportsAlerts)
                {
                    if (weather.weather_alerts != null && weather.weather_alerts.Count > 0)
                    {
                        // Alerts are posted to the user here. Set them as notified.
                        await WeatherAlertHandler.SetasNotified(location, weather.weather_alerts);
                    }

                    // Show/Hide Alert panel
                    if (WeatherView.Extras.Alerts.Count > 0)
                    {
                        FrameworkElement alertButton = AlertButton;
                        if (alertButton == null)
                            alertButton = FindName("AlertButton") as FrameworkElement;

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
                if (Settings.HomeData.Equals(location) && 
                    (TimeSpan.FromTicks(DateTime.Now.Ticks - Settings.UpdateTime.Ticks).TotalMinutes > Settings.RefreshInterval) ||
                    !WeatherTileCreator.TileUpdated)
                {
                    await WeatherUpdateBackgroundTask.RequestAppTrigger();
                }
                else if (SecondaryTileUtils.Exists(location.query))
                {
                    WeatherTileCreator.TileUpdater(location, weather);
                }

                // Shell
                Shell.Instance.HamburgerButtonColor = WeatherView.PendingBackgroundColor;
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

            // Set saved index from before update
            // Note: needed since ItemSource is cleared and index is reset
            if (index == 0) // Note: UWP Mobile Bug
                TextForecastControl.SelectedIndex = index + 1;
            TextForecastControl.SelectedIndex = index;

            if (WeatherView.Extras.HourlyForecast.Count >= 1)
                HourlyForecastPanel.Visibility = Visibility.Visible;
            else
                HourlyForecastPanel.Visibility = Visibility.Collapsed;

            if (WeatherView.Extras.TextForecast.Count >= 1)
                ForecastSwitch.Visibility = Visibility.Visible;
            else
                ForecastSwitch.Visibility = Visibility.Collapsed;

            if (!String.IsNullOrWhiteSpace(WeatherView.Extras.Chance))
            {
                if (!Settings.API.Equals(WeatherAPI.MetNo))
                {
                    if (!DetailsWrapGrid.Children.Contains(PrecipitationPanel))
                    {
                        DetailsWrapGrid.Children.Insert(0, PrecipitationPanel);
                        ResizeDetailItems();
                    }

                    PrecipitationPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    DetailsWrapGrid.Children.Remove(PrecipitationPanel);
                    ResizeDetailItems();
                }

                int precipCount = PrecipitationPanel.Children.Count;
                int atmosCount = AtmospherePanel.Children.Count;

                if (Settings.API.Equals(WeatherAPI.OpenWeatherMap) || Settings.API.Equals(WeatherAPI.MetNo))
                {
                    if (ChanceItem != null)
                        PrecipitationPanel.Children.Remove(ChanceItem);

                    FrameworkElement cloudinessItem = CloudinessItem;
                    if (cloudinessItem == null)
                        cloudinessItem = FindName("CloudinessItem") as FrameworkElement;

                    if (cloudinessItem != null && !AtmospherePanel.Children.Contains(cloudinessItem))
                        AtmospherePanel.Children.Insert(2, cloudinessItem);
                }
                else
                {
                    FrameworkElement chanceItem = ChanceItem;
                    if (chanceItem == null)
                        chanceItem = FindName("ChanceItem") as FrameworkElement;

                    if (chanceItem != null && !PrecipitationPanel.Children.Contains(chanceItem))
                        PrecipitationPanel.Children.Insert(2, chanceItem);

                    if (CloudinessItem != null)
                        AtmospherePanel.Children.Remove(CloudinessItem);
                }

                if (precipCount != PrecipitationPanel.Children.Count || atmosCount != AtmospherePanel.Children.Count)
                    ResizeDetailItems();
            }
            else
            {
                DetailsWrapGrid.Children.Remove(PrecipitationPanel);
                if (CloudinessItem != null)
                    AtmospherePanel.Children.Remove(CloudinessItem);
                ResizeDetailItems();
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
                        RefreshWeather(false);
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
            StackControl.SizeChanged += StackControl_SizeChanged;
            DetailsPanel.SizeChanged += DetailsPanel_SizeChanged;
            MainViewer.SizeChanged += MainViewer_SizeChanged;
            MainViewer.ViewChanged += MainViewer_ViewChanged;

            HeaderLeft.Click += delegate { ScrollTxtPanel(false); };
            HeaderRight.Click += delegate { ScrollTxtPanel(true); };

            // Additional Details (Extras)
            ForecastSwitch.Visibility = Visibility.Collapsed;
            TextForecastPanel.Visibility = Visibility.Collapsed;
            HourlyForecastPanel.Visibility = Visibility.Collapsed;
            PrecipitationPanel.Visibility = Visibility.Collapsed;

            geolocal = new Geolocator() { DesiredAccuracyInMeters = 5000, ReportInterval = 900000, MovementThreshold = 1600 };

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("Nav_WeatherNow/Text");
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
                double alpha = 1 - (viewer.VerticalOffset / viewer.ScrollableHeight);
                MainViewer.Background.Opacity = (alpha >= 0) ? BGAlpha = alpha : BGAlpha = 0;
            }
        }

        private void StackControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // TextForecastPanel resizing
            TextForecastPanel.Height = e.NewSize.Height;
            TextForecastControl.Height = TextForecastPanel.Height - 50;

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
        }

        private void DetailsPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeDetailItems();
        }

        private void ResizeDetailItems()
        {
            double w = this.ActualWidth;

            if (w <= 0)
                return;

            if (w <= 600)
            {
                // Keep as one column on smaller screens
                foreach (FrameworkElement element in DetailsWrapGrid.Children)
                {
                    // Increase card width based on screen size
                    element.Width = w - DetailsPanel.Padding.Right;
                }
            }
            else
            {
                // Minimum width for ea. card
                int minWidth = 250;
                // Size of the view
                int viewWidth = (int)(w - DetailsPanel.Padding.Right - DetailsPanel.Padding.Left);
                // Available columns based on min card width
                int availColumns = (viewWidth / minWidth) == 0 ? 1 : viewWidth / minWidth;
                // Maximum columns to use
                int maxColumns = (availColumns > DetailsWrapGrid.Children.Count) ? DetailsWrapGrid.Children.Count : availColumns;

                int freeSpace = viewWidth - (minWidth * maxColumns);
                // Increase card width to fill available space
                int itemWidth = minWidth + (freeSpace / maxColumns);

                // Resizing needed if first details element is less than others
                foreach (FrameworkElement element in DetailsWrapGrid.Children)
                {
                    element.Width = itemWidth - 1;
                }
            }
        }

        private void MainViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeAlertPanel();
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
                    wLoader = new WeatherDataLoader(this, this, location);
                }

                Restore();
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
                    Restore();
            }
        }

        private async Task Resume()
        {
            // Save index before update
            int index = TextForecastControl.SelectedIndex;

            // Check pin tile status
            CheckTiles();

            if (wLoader.GetWeather() != null)
            {
                Weather weather = wLoader.GetWeather();

                // Update weather if needed on resume
                if (Settings.FollowGPS && await UpdateLocation())
                {
                    // Setup loader from updated location
                    wLoader = new WeatherDataLoader(this, this, this.location);
                    RefreshWeather(false);
                }
                else
                {
                    // Check weather data expiration
                    if (!int.TryParse(weather.ttl, out int ttl))
                        ttl = Settings.DefaultInterval;
                    TimeSpan span = DateTimeOffset.Now - weather.update_time;
                    if (span.TotalMinutes > ttl)
                        RefreshWeather(false);
                    else
                    {
                        WeatherView.UpdateView(wLoader.GetWeather());
                        Shell.Instance.HamburgerButtonColor = WeatherView.PendingBackgroundColor;
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

            // Set saved index from before update
            // Note: needed since ItemSource is cleared and index is reset
            if (index == 0) // Note: UWP Mobile Bug
                TextForecastControl.SelectedIndex = index + 1;
            TextForecastControl.SelectedIndex = index;
        }

        private async void Restore()
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
                    wLoader = new WeatherDataLoader(this, this, location);
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
                        wLoader = new WeatherDataLoader(this, this, location);
                        forceRefresh = true;
                    }
                    else
                    {
                        // Setup loader saved location data
                        location = locData;
                        wLoader = new WeatherDataLoader(this, this, location);
                    }
                }
            }
            // Regular mode
            else if (wLoader == null)
            {
                // Weather was loaded before. Lets load it up...
                location = Settings.HomeData;
                wLoader = new WeatherDataLoader(this, this, location);
            }

            // Check pin tile status
            CheckTiles();

            // Load up weather data
            RefreshWeather(forceRefresh);
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
                    GeolocationAccessStatus geoStatus = GeolocationAccessStatus.Unspecified;

                    try
                    {
                        geoStatus = await Geolocator.RequestAccessAsync();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                    }
                    finally
                    {
                        if (geoStatus == GeolocationAccessStatus.Allowed)
                        {
                            newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10));
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
                    if (lastGPSLocData.query != null &&
                        geoPos != null && ConversionMethods.CalculateGeopositionDistance(geoPos, newGeoPos) < geolocal.MovementThreshold)
                    {
                        return false;
                    }

                    if (lastGPSLocData.query != null && 
                        Math.Abs(ConversionMethods.CalculateHaversine(lastGPSLocData.latitude, lastGPSLocData.longitude,
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
                        SecondaryTileUtils.UpdateTileId(oldkey, lastGPSLocData.query);
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
                wLoader = new WeatherDataLoader(this, this, location);

            RefreshWeather(true);
        }

        private async void RefreshWeather(bool forceRefresh)
        {
            LoadingRing.IsActive = true;
            await wLoader.LoadWeatherData(forceRefresh);
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            var controlName = (sender as FrameworkElement).Name;

            if (controlName.Contains("Hourly"))
                ScrollLeft(HourlyForecastViewer);
            else
                ScrollLeft(ForecastViewer);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            var controlName = (sender as FrameworkElement).Name;

            if (controlName.Contains("Hourly"))
                ScrollRight(HourlyForecastViewer);
            else
                ScrollRight(ForecastViewer);
        }

        private void ForecastViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer viewer = sender as ScrollViewer;
            var controlName = (sender as FrameworkElement).Name;

            if (viewer.HorizontalOffset == 0)
            {
                if (controlName.Contains("Hourly"))
                    HourlyLeftButton.IsEnabled = false;
                else
                    LeftButton.IsEnabled = false;
            }
            else if (viewer.HorizontalOffset == viewer.ScrollableWidth)
            {
                if (controlName.Contains("Hourly"))
                    HourlyRightButton.IsEnabled = false;
                else
                    RightButton.IsEnabled = false;
            }
            else
            {
                if (controlName.Contains("Hourly"))
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

        private void ScrollTxtPanel(bool right)
        {
            if (right && TextForecastControl.SelectedIndex < TextForecastControl.Items.Count - 1)
            {
                TextForecastControl.SelectedIndex++;
            }
            else if (!right && TextForecastControl.SelectedIndex > 0)
            {
                TextForecastControl.SelectedIndex--;
            }
        }

        private void TextForecastControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is Pivot control && control.ItemsSource != null)
            {
                var source = (ObservableCollection<TextForecastItemViewModel>)control.ItemsSource;

                if (source != null && source.Count > 1)
                {
                    if (control.SelectedItem == null)
                    {
                        control.SelectedIndex = 0;
                    }
                    else if (control.SelectedIndex == 0)
                    {
                        HeaderLeft.IsEnabled = false;

                        HeaderLeft.Content = String.Empty;
                        Header.Text = source[control.SelectedIndex].Title ?? String.Empty; // set to empty if null
                        HeaderRight.Content = source[control.SelectedIndex + 1].Title;

                        if (!HeaderRight.IsEnabled)
                            HeaderRight.IsEnabled = true;
                    }
                    else if (control.SelectedIndex == source.Count - 1)
                    {
                        if (!HeaderLeft.IsEnabled)
                            HeaderLeft.IsEnabled = true;

                        HeaderLeft.Content = source[control.SelectedIndex - 1].Title;
                        Header.Text = source[control.SelectedIndex].Title ?? String.Empty; // set to empty if null
                        HeaderRight.Content = String.Empty;

                        HeaderRight.IsEnabled = false;
                    }
                    else
                    {
                        if (!HeaderLeft.IsEnabled)
                            HeaderLeft.IsEnabled = true;
                        if (!HeaderRight.IsEnabled)
                            HeaderRight.IsEnabled = true;

                        HeaderLeft.Content = source[control.SelectedIndex - 1].Title;
                        Header.Text = source[control.SelectedIndex].Title ?? String.Empty; // set to empty if null
                        HeaderRight.Content = source[control.SelectedIndex + 1].Title;
                    }
                }
            }
        }

        private void ScrollLeft(ScrollViewer viewer)
        {
            viewer.ChangeView(viewer.HorizontalOffset - viewer.ActualWidth, null, null);
        }

        private void ScrollRight(ScrollViewer viewer)
        {
            viewer.ChangeView(viewer.HorizontalOffset + viewer.ActualWidth, null, null);
        }

        private void ForecastSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ForecastGrid.Visibility = ForecastSwitch.IsOn ?
                Visibility.Collapsed : Visibility.Visible;
            TextForecastPanel.Visibility = ForecastSwitch.IsOn ? 
                Visibility.Visible : Visibility.Collapsed;

            if (ForecastSwitch.IsOn)
                TextForecastControl_SelectionChanged(TextForecastControl, null);
            else
                ForecastViewer_ViewChanged(ForecastViewer, null);
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
                SecondaryTile tile = new SecondaryTile(
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
                    WeatherTileCreator.TileUpdater(location);
                    await tile.UpdateAsync();
                }

                SetPinButton(isPinned);

                GetPinBtn().IsEnabled = true;
            }
        }
    }
}
