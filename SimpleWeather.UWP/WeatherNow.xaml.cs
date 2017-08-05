using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherNow : Page, IWeatherLoadedListener
    {
        WeatherDataLoader wLoader = null;
        WeatherNowViewModel WeatherView { get; set; }

        KeyValuePair<int, string> pair;

        Geolocator geolocal = null;
        Geoposition geoPos = null;

        public void OnWeatherLoaded(int locationIdx, Weather weather)
        {
            // Save index before update
            int index = TextForecastControl.SelectedIndex;

            if (weather != null)
                WeatherView.UpdateView(weather);

            // Set saved index from before update
            // Note: needed since ItemSource is cleared and index is reset
            if (index == 0) // Note: UWP Mobile Bug
                TextForecastControl.SelectedIndex = index + 1;
            TextForecastControl.SelectedIndex = index;

            if (WeatherView.WUExtras.HourlyForecast.Count >= 1)
                HourlyForecastPanel.Visibility = Visibility.Visible;
            else
                HourlyForecastPanel.Visibility = Visibility.Collapsed;

            if (WeatherView.WUExtras.TextForecast.Count >= 1)
                ForecastSwitch.Visibility = Visibility.Visible;
            else
                ForecastSwitch.Visibility = Visibility.Collapsed;

            if (!String.IsNullOrWhiteSpace(WeatherView.WUExtras.Chance))
            {
                if (!DetailsWrapGrid.Children.Contains(PrecipitationPanel))
                {
                    DetailsWrapGrid.Children.Add(PrecipitationPanel);
                    DetailsPanel_SizeChanged(DetailsWrapGrid, null);
                }

                PrecipitationPanel.Visibility = Visibility.Visible;
            }
            else
            {
                DetailsWrapGrid.Children.Remove(PrecipitationPanel);
                DetailsPanel_SizeChanged(DetailsWrapGrid, null);
            }

            LoadingRing.IsActive = false;
        }

        public WeatherNow()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            WeatherView = new WeatherNowViewModel();
            DetailsPanel.SizeChanged += DetailsPanel_SizeChanged;

            HeaderLeft.Click += delegate { ScrollTxtPanel(false); };
            HeaderRight.Click += delegate { ScrollTxtPanel(true); };

            // Additional Details (WUExtras)
            ForecastSwitch.Visibility = Visibility.Collapsed;
            TextForecastPanel.Visibility = Visibility.Collapsed;
            HourlyForecastPanel.Visibility = Visibility.Collapsed;
            PrecipitationPanel.Visibility = Visibility.Collapsed;

            geolocal = new Geolocator() { DesiredAccuracyInMeters = 5000, ReportInterval = 900000, MovementThreshold = 2500 };
        }

        private void DetailsPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double w = this.ActualWidth;

            if (w <= 600)
            {
                // Keep as one column on smaller screens
                DetailsWrapGrid.MaximumRowsOrColumns = 1;
                // Increase card width based on screen size
                DetailsWrapGrid.ItemWidth = w - DetailsPanel.Padding.Right;
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

                DetailsWrapGrid.MaximumRowsOrColumns = maxColumns;
                DetailsWrapGrid.ItemWidth = itemWidth;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // Clear cache if nav'ing back
            if (e.NavigationMode == NavigationMode.Back)
                NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Did home change?
            CoreApplication.Properties.TryGetValue("HomeChanged", out object homeChanged);
            CoreApplication.Properties.Remove("HomeChanged");

            // Reset loader if source is different
            if (wLoader != null && WeatherView.WeatherSource != Settings.API)
            {
                wLoader = null;
            }

            // Update view on resume
            // ex. If temperature unit changed
            if ((wLoader != null) && e.NavigationMode != NavigationMode.New && (homeChanged == null || homeChanged != null && !(bool)homeChanged))
            {
                if (wLoader != null)
                {
                    // Save index before update
                    int index = TextForecastControl.SelectedIndex;

                    if (wLoader.GetWeather() != null)
                        WeatherView.UpdateView(wLoader.GetWeather());

                    // Set saved index from before update
                    // Note: needed since ItemSource is cleared and index is reset
                    if (index == 0) // Note: UWP Mobile Bug
                        TextForecastControl.SelectedIndex = index + 1;
                    TextForecastControl.SelectedIndex = index;
                }

                if (pair.Key == App.HomeIdx)
                {
                    // Clear backstack since we're home
                    Frame.BackStack.Clear();
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                }

                return;
            }

            // Reset loader if new page instance created
            if (e.NavigationMode == NavigationMode.New)
            {
                wLoader = null;
            }

            // New page instance created, so restore
            if (e.Parameter != null && (wLoader == null || e.NavigationMode == NavigationMode.New))
            {
                if (e.Parameter.GetType() == typeof(KeyValuePair<int, string>))
                {
                    pair = (KeyValuePair<int, string>)e.Parameter;

                    wLoader = new WeatherDataLoader(this, pair.Value, pair.Key);

                    if (pair.Key == App.HomeIdx)
                    {
                        // Clear backstack since we're home
                        Frame.BackStack.Clear();
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    }
                }
            }

            Restore();
        }

        private async void Restore()
        {
            bool forceRefresh = false;

            if (wLoader == null)
            {
                // Clear backstack since we're home
                Frame.BackStack.Clear();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }

            // GPS Follow location
            if (Settings.FollowGPS && pair.Key == App.HomeIdx)
            {
                LocationData locData = await Settings.GetLastGPSLocData();

                if (locData == null)
                {
                    // Update location if not setup
                    await UpdateLocation();
                    wLoader = new WeatherDataLoader(this, pair.Value, pair.Key);
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
                        wLoader = new WeatherDataLoader(this, pair.Value, pair.Key);
                        forceRefresh = true;
                    }
                    else
                    {
                        // Setup loader saved location data
                        wLoader = new WeatherDataLoader(this, locData.query, App.HomeIdx);
                    }
                }
            }
            // Regular mode
            else if (wLoader == null)
            {
                // Weather was loaded before. Lets load it up...
                List<string> locations = await Settings.GetLocations();
                string local = locations[App.HomeIdx];

                wLoader = new WeatherDataLoader(this, local, App.HomeIdx);
            }

            // Load up weather data
            RefreshWeather(forceRefresh);
        }

        private async Task<bool> UpdateLocation()
        {
            bool locationChanged = false;

            if (Settings.FollowGPS && pair.Key == App.HomeIdx)
            {
                Geoposition newGeoPos = null;

                try
                {
                    newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10));
                }
                catch (Exception)
                {
                    GeolocationAccessStatus geoStatus = await Geolocator.RequestAccessAsync();

                    if (geoStatus == GeolocationAccessStatus.Allowed)
                    {
                        try
                        {
                            newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10));
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                        }
                    }
                    else if (geoStatus == GeolocationAccessStatus.Denied)
                    {
                        // Disable gps feature
                        Settings.FollowGPS = false;
                        return false;
                    }
                }

                // Access to location granted
                if (newGeoPos != null)
                {
                    LocationData lastGPSLocData = await Settings.GetLastGPSLocData();

                    // Check previous location difference
                    if (lastGPSLocData.query != null &&
                        geoPos != null && CalculateGeopositionDistance(geoPos, newGeoPos) < geolocal.MovementThreshold)
                    {
                        return false;
                    }

                    if (lastGPSLocData.query != null && 
                        Math.Abs(ConversionMethods.CalculateHaversine(lastGPSLocData.latitude, lastGPSLocData.longitude,
                        newGeoPos.Coordinate.Point.Position.Latitude, newGeoPos.Coordinate.Point.Position.Longitude)) < geolocal.MovementThreshold)
                    {
                        return false;
                    }

                    string selected_query = string.Empty;

                    await Task.Run(async () =>
                    {
                        LocationQueryViewModel view = await GeopositionQuery.GetLocation(newGeoPos);

                        if (!String.IsNullOrEmpty(view.LocationQuery))
                            selected_query = view.LocationQuery;
                        else
                            selected_query = string.Empty;
                    });

                    if (String.IsNullOrWhiteSpace(selected_query))
                    {
                        // Stop since there is no valid query
                        return false;
                    }

                    // Save location as last known
                    lastGPSLocData.SetData(selected_query, newGeoPos);
                    Settings.SaveLastGPSLocData();

                    pair = new KeyValuePair<int, string>(App.HomeIdx, selected_query);
                    geoPos = newGeoPos;
                    locationChanged = true;
                }
            }

            return locationChanged;
        }

        private double CalculateGeopositionDistance(Geoposition position1, Geoposition position2)
        {
            double lat1 = position1.Coordinate.Point.Position.Latitude;
            double lon1 = position1.Coordinate.Point.Position.Longitude;
            double lat2 = position2.Coordinate.Point.Position.Latitude;
            double lon2 = position2.Coordinate.Point.Position.Longitude;

            /* Returns value in meters */
            return Math.Abs(ConversionMethods.CalculateHaversine(lat1, lon1, lat2, lon2));
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.FollowGPS && await UpdateLocation())
                // Setup loader from updated location
                wLoader = new WeatherDataLoader(this, pair.Value, pair.Key);

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
                        Header.Text = source[control.SelectedIndex].Title;
                        HeaderRight.Content = source[control.SelectedIndex + 1].Title;

                        if (!HeaderRight.IsEnabled)
                            HeaderRight.IsEnabled = true;
                    }
                    else if (control.SelectedIndex == source.Count - 1)
                    {
                        if (!HeaderLeft.IsEnabled)
                            HeaderLeft.IsEnabled = true;

                        HeaderLeft.Content = source[control.SelectedIndex - 1].Title;
                        Header.Text = source[control.SelectedIndex].Title;
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
                        Header.Text = source[control.SelectedIndex].Title;
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
    }
}
