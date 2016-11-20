using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        public WeatherNow()
        {
            this.InitializeComponent();

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
            // For UI Thread
            Windows.UI.Core.CoreDispatcher dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

            await wLoader.loadWeatherData(forceRefresh).ContinueWith(async (t) =>
            {
                Weather weather = wLoader.getWeather();

                if (weather != null)
                {
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                     {
                         weatherView = new WeatherNowView(weather);
                         this.DataContext = weatherView;
                         updateWindDirection(int.Parse(weather.wind.direction));
                         updatePressureState(int.Parse(weather.atmosphere.rising));
                         StackControl.ItemsSource = weatherView.Forecasts;
                     });
                }
                else
                {
                    // unable to load weather data; Refresh
                    RefreshWeather(true);
                }
            });
        }

        private void updateWindDirection(int angle)
        {
            RotateTransform rotation = new RotateTransform();
            rotation.Angle = angle;
            WindDirection.RenderTransformOrigin = new Point(0.5, 0.5);
            WindDirection.RenderTransform = rotation;
        }

        private void updatePressureState(int rising)
        {
            switch (rising)
            {
                // Steady
                case 0:
                default:
                    Rising.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
                // Rising
                case 1:
                    Rising.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    Rising.Text = "\uf058\uf058";
                    break;
                // Falling
                case 2:
                    Rising.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    Rising.Text = "\uf044\uf044";
                    break;
            }
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            ForecastViewer.ScrollToHorizontalOffset(ForecastViewer.HorizontalOffset - 150);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            ForecastViewer.ScrollToHorizontalOffset(ForecastViewer.HorizontalOffset + 150);
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
    }
}
