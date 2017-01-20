using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherNow : Page
    {
        WeatherDataLoader wLoader = null;
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
            wLoader = (WeatherDataLoader)outValue;

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

            // Loop until we get weather data
            do
            {
                await wLoader.loadWeatherData(forceRefresh).ContinueWith(async (t) =>
                {
                    Weather weather = wLoader.getWeather();

                    if (weather != null)
                    {
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            weatherView = new WeatherNowView(weather);
                            this.DataContext = weatherView;
                            StackControl.ItemsSource = weatherView.Forecasts;
                        });
                    }
                }).ConfigureAwait(false);

                if (wLoader.getWeather() == null)
                    await Task.Delay(1000);

            } while (wLoader.getWeather() == null);

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
