using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
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

        public WeatherNow()
        {
            this.InitializeComponent();

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(640, 480));

            // Try to get saved WeatherLoader
            object outValue;
            if (!CoreApplication.Properties.TryGetValue("WeatherLoader", out outValue)) { }
            wLoader = (WeatherDataLoader)outValue;

            updateUI(wLoader.getWeather());
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // For UI Thread
            Windows.UI.Core.CoreDispatcher dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

            await wLoader.loadWeatherData(true).ContinueWith(async (t) =>
            {
                if (wLoader.getWeather() != null)
                {
                    // Update Weather Data on UI
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        updateUI(wLoader.getWeather());
                    });
                }
                else
                {
                    // unable to load weather data
                }
            });
        }

        private void updateUI(Weather weather)
        {
            // Location
            Location.Text = weather.location.city + "," + weather.location.region;

            // Date Updated
            // ex. "2016-08-22T04:53:07Z"
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime updateTime = DateTime.ParseExact(weather.lastBuildDate,
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", provider).ToLocalTime();
            UpdateDate.Text = "Updated on " + updateTime.DayOfWeek.ToString() + " " + updateTime.ToString("t");

            // Update Current Condition
            CurTemp.Text = weather.condition.temp + "º";
            CurCondition.Text = weather.condition.text;
            updateWeatherIcon(WeatherIcon, int.Parse(weather.condition.code));

            // Clear panel before we begin
            ForecastPanel.Children.Clear();

            // Add UI elements
            foreach(Forecast forecast in weather.forecasts)
            {
                // Add border
                Border border = new Border();
                border.Style = this.Resources["ForecastBorder"] as Style;

                // Add inner grid
                Grid grid = new Grid();

                // Add weather elements
                TextBlock weatherIcon = new TextBlock();
                weatherIcon.Style = this.Resources["WeatherIcon"] as Style;
                updateWeatherIcon(weatherIcon, int.Parse(forecast.code));
                Viewbox box = new Viewbox();
                box.Style = this.Resources["ForecastIcon"] as Style;
                box.Child = weatherIcon;

                TextBlock date = new TextBlock();
                date.Style = this.Resources["ForecastDate"] as Style;
                date.Text = DateTime.Parse(forecast.date).ToString("dddd dd");

                TextBlock condition = new TextBlock();
                condition.Style = this.Resources["ForecastCondition"] as Style;
                condition.Text = forecast.text;

                RichTextBlock temp = new RichTextBlock();
                temp.Style = this.Resources["ForecastTemp"] as Style;

                Run HiTemp = new Run();
                Run LoTemp = new Run();
                LoTemp.FontSize = 18;
                HiTemp.Text = forecast.high + "º | ";
                LoTemp.Text = forecast.low + "º";
                Paragraph p = new Paragraph();
                p.Inlines.Add(HiTemp);
                p.Inlines.Add(LoTemp);
                temp.Blocks.Clear();
                temp.Blocks.Add(p);

                // Add all elements to grid
                grid.Children.Add(box);
                grid.Children.Add(date);
                grid.Children.Add(condition);
                grid.Children.Add(temp);

                // Add grid to border
                border.Child = grid;

                // Add border to panel
                ForecastPanel.Children.Add(border);
            }

            // Update background
            updateBg(weather);
        }

        private void updateWeatherIcon(TextBlock textBlock, int weatherCode)
        {
            switch (weatherCode)
            {
                case 0: // Tornado
                    textBlock.Text = "\uf056";
                    break;
                case 1: // Tropical Storm
                case 37:
                case 38: // Scattered Thunderstorms/showers
                case 39:
                case 45:
                case 47:
                    textBlock.Text = "\uf00e";
                    break;
                case 2: // Hurricane
                    textBlock.Text = "\uf073";
                    break;
                case 3:
                case 4: // Scattered Thunderstorms
                    textBlock.Text = "\uf01e";
                    break;
                case 5: // Mixed Rain/Snow
                case 6: // Mixed Rain/Sleet
                case 7: // Mixed Snow/Sleet
                case 18: // Sleet
                case 35: // Mixed Rain/Hail
                    textBlock.Text = "\uf017";
                    break;
                case 8: // Freezing Drizzle
                case 10: // Freezing Rain
                case 17: // Hail
                    textBlock.Text = "\uf015";
                    break;
                case 9: // Drizzle
                case 11: // Showers
                case 12:
                case 40: // Scattered Showers
                    textBlock.Text = "\uf01a";
                    break;
                case 13: // Snow Flurries
                case 16: // Snow
                case 42: // Scattered Snow Showers
                case 46: // Snow Showers
                    textBlock.Text = "\uf01b";
                    break;
                case 15: // Blowing Snow
                case 41: // Heavy Snow
                case 43:
                    textBlock.Text = "\uf064";
                    break;
                case 19: // Dust
                    textBlock.Text = "\uf063";
                    break;
                case 20: // Foggy
                    textBlock.Text = "\uf014";
                    break;
                case 21: // Haze
                    textBlock.Text = "\uf021";
                    break;
                case 22: // Smoky
                    textBlock.Text = "\uf062";
                    break;
                case 23: // Blustery
                case 24: // Windy
                    textBlock.Text = "\uf050";
                    break;
                case 25: // Cold
                    textBlock.Text = "\uf076";
                    break;
                case 26: // Cloudy
                    textBlock.Text = "\uf013";
                    break;
                case 27: // Mostly Cloudy (Night)
                case 29: // Partly Cloudy (Night)
                    textBlock.Text = "\uf031";
                    break;
                case 28: // Mostly Cloudy (Day)
                case 30: // Partly Cloudy (Day)
                    textBlock.Text = "\uf002";
                    break;
                case 31: // Clear (Night)
                    textBlock.Text = "\uf02e";
                    break;
                case 32: // Sunny
                    textBlock.Text = "\uf00d";
                    break;
                case 33: // Fair (Night)
                    textBlock.Text = "\uf083";
                    break;
                case 34: // Fair (Day)
                case 44: // Partly Cloudy
                    textBlock.Text = "\uf00c";
                    break;
                case 36: // HOT
                    textBlock.Text = "\uf072";
                    break;
                case 3200: // Not Available
                default:
                    textBlock.Text = "\uf077";
                    break;
            }
        }

        private void updateBg(Weather weather)
        {
            ImageBrush bg = new ImageBrush();
            switch (int.Parse(weather.condition.code))
            {
                // Apply Night Bkgrnd based on weather condition
                case 27:
                case 29:
                case 31:
                case 33:
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/NightSky.jpg"));
                    break;
            }

            // If NULL check time to see if its night
            if (bg.ImageSource == null)
            {
                // Set background based using sunset/rise times
                if (DateTime.Now < DateTime.Parse(weather.astronomy.sunrise)
                    || DateTime.Now > DateTime.Parse(weather.astronomy.sunset))
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/NightSky.jpg"));
                else
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/DaySky.jpg"));
            }

            MainViewer.Background = bg;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            HamBurgerMenu.IsPaneOpen = !HamBurgerMenu.IsPaneOpen;
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
