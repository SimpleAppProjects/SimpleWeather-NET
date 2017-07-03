using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
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

        public void OnWeatherLoaded(int locationIdx, Weather weather)
        {
            if (weather != null)
                WeatherView.UpdateView(weather);

            LoadingRing.IsActive = false;
        }

        public WeatherNow()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            WeatherView = new WeatherNowViewModel();
            DetailsPanel.SizeChanged += DetailsPanel_SizeChanged;
        }

        private void DetailsPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Width <= 600)
            {
                // Keep as one column on smaller screens
                DetailsWrapGrid.MaximumRowsOrColumns = 1;
                // Increase card width based on screen size
                DetailsWrapGrid.ItemWidth = e.NewSize.Width - DetailsPanel.Padding.Right;
            }
            else
            {
                // Minimum width for ea. card
                int minWidth = 250;
                // Size of the view
                int viewWidth = (int)(e.NewSize.Width - DetailsPanel.Padding.Right - DetailsPanel.Padding.Left);
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

            // Update view on resume
            // ex. If temperature unit changed
            if ((wLoader != null) && e.NavigationMode != NavigationMode.New && (homeChanged == null || homeChanged != null && !(bool)homeChanged))
            {
                if (wLoader != null)
                {
                    if (wLoader.GetWeather() != null)
                        WeatherView.UpdateView(wLoader.GetWeather());
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
            if (wLoader == null)
            {
                // Weather was loaded before. Lets load it up...
                List<string> locations = await Settings.GetLocations();
                string local = locations[App.HomeIdx];

                wLoader = new WeatherDataLoader(this, local, App.HomeIdx);

                // Clear backstack since we're home
                Frame.BackStack.Clear();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }

            // Load up weather data
            RefreshWeather(false);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshWeather(true);
        }

        private async void RefreshWeather(bool forceRefresh)
        {
            LoadingRing.IsActive = true;
            await wLoader.LoadWeatherData(forceRefresh);
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollLeft();
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollRight();
        }

        private void ForecastViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (ForecastViewer.HorizontalOffset == 0)
            {
                LeftButton.IsEnabled = false;
            }
            else if (ForecastViewer.HorizontalOffset == ForecastViewer.ScrollableWidth)
            {
                RightButton.IsEnabled = false;
            }
            else
            {
                if (!LeftButton.IsEnabled)
                    LeftButton.IsEnabled = true;
                if (!RightButton.IsEnabled)
                    RightButton.IsEnabled = true;
            }
        }

        private void ScrollLeft()
        {
            int counter = 0; // 128, 64, 32, 16, 8, 4, 2, 1
            int max_count = (int)ForecastViewer.HorizontalOffset / 64;
            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 5)
            };
            timer.Tick += ((sender, e) =>
            {
                counter++;
                ForecastViewer.ChangeView(
                    ForecastViewer.HorizontalOffset - 128 / counter, null, null);
                if (ForecastViewer.HorizontalOffset == 0) // can't scroll any more
                    ((DispatcherTimer)sender).Stop();
                if (counter >= max_count)
                    ((DispatcherTimer)sender).Stop();
            });
            timer.Start();
        }

        private void ScrollRight()
        {
            int counter = 0; // 128, 64, 32, 16, 8, 4, 2, 1
            int max_count = (int)(ForecastViewer.ScrollableWidth - ForecastViewer.HorizontalOffset) / 64;
            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 5)
            };
            timer.Tick += ((sender, e) =>
            {
                counter++;
                ForecastViewer.ChangeView(
                    ForecastViewer.HorizontalOffset + 128 / counter, null, null);
                if (ForecastViewer.HorizontalOffset >= ForecastViewer.ScrollableWidth) // can't scroll any more
                    ((DispatcherTimer)sender).Stop();
                if (counter >= max_count)
                    ((DispatcherTimer)sender).Stop();
            });
            timer.Start();
        }
    }
}
