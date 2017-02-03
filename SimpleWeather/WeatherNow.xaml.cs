using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherNow : Page
    {
        WeatherYahoo.WeatherDataLoader wLoader = null;
        WeatherUnderground.WeatherDataLoader wu_Loader = null;
        WeatherNowView weatherView = null;

        // For UI Thread
        CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

        public WeatherNow()
        {
            this.InitializeComponent();
            MainGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            Restore();
        }

        private void Restore()
        {
            // Restore weather loader
            object outValue;
            if (!CoreApplication.Properties.TryGetValue("WeatherLoader", out outValue)) { }

            if (Settings.API == "WUnderground")
                wu_Loader = (WeatherUnderground.WeatherDataLoader)outValue;
            else
                wLoader = (WeatherYahoo.WeatherDataLoader)outValue;

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

            object weather;
            WeatherUtils.ErrorStatus ret;

            if (Settings.API == "WUnderground")
            {
                ret = await wu_Loader.loadWeatherData(forceRefresh).ConfigureAwait(false);
                weather = wu_Loader.getWeather();
            }
            else
            {
                ret = await wLoader.loadWeatherData(forceRefresh).ConfigureAwait(false);
                weather = wLoader.getWeather();
            }

            if (weather != null)
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (Settings.API == "WUnderground")
                        weatherView = new WeatherNowView(weather as WeatherUnderground.Weather);
                    else
                        weatherView = new WeatherNowView(weather as WeatherYahoo.Weather);

                    this.DataContext = weatherView;
                    StackControl.ItemsSource = weatherView.Forecasts;
                });
            }
            else
            {
                MessageDialog error = new MessageDialog("Unable to load weather data!!", "Error");

                if (ret == WeatherUtils.ErrorStatus.NETWORKERROR)
                    error.Content = "Network Connection Error!!";

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await error.ShowAsync());
            }

            ShowLoadingGrid(false);
        }

        private async void ShowLoadingGrid(bool show)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                LoadingRing.IsActive = show;
                LoadingGrid.Visibility = show ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
                MainGrid.Visibility = show ? Windows.UI.Xaml.Visibility.Collapsed : Windows.UI.Xaml.Visibility.Visible;
            });
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
                ForecastViewer.ScrollToHorizontalOffset(
                    ForecastViewer.HorizontalOffset - 128 / counter);
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
                ForecastViewer.ScrollToHorizontalOffset(
                    ForecastViewer.HorizontalOffset + 128 / counter);
                if (ForecastViewer.HorizontalOffset >= ForecastViewer.ScrollableWidth) // can't scroll any more
                    ((DispatcherTimer)sender).Stop();
                if (counter >= max_count)
                    ((DispatcherTimer)sender).Stop();
            });
            timer.Start();
        }
    }
}
