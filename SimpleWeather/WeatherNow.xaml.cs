using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherNow : Page, WeatherLoadedListener
    {
        WeatherYahoo.WeatherDataLoader wLoader = null;
        WeatherUnderground.WeatherDataLoader wu_Loader = null;
        WeatherNowView weatherView = null;

        public void onWeatherLoaded(int locationIdx, object weather)
        {
            if (weather != null)
            {
                if (Settings.API == "WUnderground")
                    weatherView = new WeatherNowView(weather as WeatherUnderground.Weather);
                else
                    weatherView = new WeatherNowView(weather as WeatherYahoo.Weather);

                this.DataContext = weatherView;
                StackControl.ItemsSource = weatherView.Forecasts;
            }

            ShowLoadingGrid(false);
        }

        public WeatherNow()
        {
            this.InitializeComponent();

            MainGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                if (e.Parameter.GetType() == typeof(KeyValuePair<int, object>))
                {
                    KeyValuePair<int, object> pair = (KeyValuePair<int, object>)e.Parameter;

                    if (Settings.API == "WUnderground")
                        wu_Loader = new WeatherUnderground.WeatherDataLoader(this, pair.Value.ToString(), pair.Key);
                    else
                        wLoader = new WeatherYahoo.WeatherDataLoader(this, pair.Value.ToString(), pair.Key);

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
            if (wLoader == null && wu_Loader == null)
            {
                // Weather was loaded before. Lets load it up...
                List<string> locations = await Settings.getLocations();
                string local = locations[App.HomeIdx];

                if (Settings.API == "WUnderground")
                    wu_Loader = new WeatherUnderground.WeatherDataLoader(this, local, App.HomeIdx);
                else
                    wLoader = new WeatherYahoo.WeatherDataLoader(this, local, App.HomeIdx);

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
            ShowLoadingGrid(true);

            if (Settings.API == "WUnderground")
                await wu_Loader.loadWeatherData(forceRefresh);
            else
                await wLoader.loadWeatherData(forceRefresh);
        }

        private void ShowLoadingGrid(bool show)
        {
            LoadingRing.IsActive = show;
            LoadingGrid.Visibility = show ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
            MainGrid.Visibility = show ? Windows.UI.Xaml.Visibility.Collapsed : Windows.UI.Xaml.Visibility.Visible;
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
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 5);
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
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 5);
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
