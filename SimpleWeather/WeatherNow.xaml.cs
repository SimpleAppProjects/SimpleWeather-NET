using System;
using System.Collections.Generic;
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
                    // Update Weather Data on UI
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        updateUI(weather);
                    });
                }
                else
                {
                    // unable to load weather data; Refresh
                    RefreshWeather(true);
                }
            });
        }

        private void updateUI(Weather weather)
        {
            // Update background
            updateBg(weather);

            // Location
            Location.Text = weather.location.description;

            // Date Updated
            UpdateDate.Text = updateLastBuildDate(weather);

            // Update Current Condition
            CurTemp.Text = weather.condition.temp +
                (weather.units.temperature == "F" ? "\uf045" : "\uf03c");
            CurCondition.Text = weather.condition.text;
            updateWeatherIcon(WeatherIcon, int.Parse(weather.condition.code));

            // WeatherDetails
            // Astronomy
            Sunrise.Text = weather.astronomy.sunrise;
            Sunset.Text = weather.astronomy.sunset;
            // Wind
            Chill.Text = (weather.units.temperature == "F" ? weather.wind.chill : ConversionMethods.FtoC(weather.wind.chill)) + "º";
            updateWindDirection(int.Parse(weather.wind.direction));
            Speed.Text = weather.wind.speed;
            SpeedUnit.Text = weather.units.speed;

            // Atmosphere
            Humidity.Text = weather.atmosphere.humidity;
            Pressure.Text = (weather.units.temperature == "F" ?
                weather.atmosphere.pressure : Math.Round(double.Parse(weather.atmosphere.pressure)).ToString());
            PressureUnit.Text = weather.units.pressure;
            updatePressureState(int.Parse(weather.atmosphere.rising));
            Visibility.Text = weather.atmosphere.visibility;
            VisibilityUnit.Text = weather.units.distance;

            // Clear panel before we begin
            ForecastPanel.Children.Clear();

            // Add UI elements
            foreach (Forecast forecast in weather.forecasts)
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
                date.Text = forecast.date;

                TextBlock condition = new TextBlock();
                condition.Style = this.Resources["ForecastCondition"] as Style;
                condition.Text = forecast.text;

                RichTextBlock temp = new RichTextBlock();
                temp.Style = this.Resources["ForecastTemp"] as Style;

                Run HiTemp = new Run();
                Run LoTemp = new Run();
                LoTemp.FontSize = 16;
                LoTemp.Foreground = new SolidColorBrush(Windows.UI.Colors.LightGray);
                LoTemp.FontWeight = Windows.UI.Text.FontWeights.SemiBold;
                HiTemp.FontWeight = Windows.UI.Text.FontWeights.SemiBold;
                HiTemp.Text = forecast.high + "º ";
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

        private String updateLastBuildDate(Weather weather)
        {
            String date;

            // ex. "2016-08-22T04:53:07Z"
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime updateTime = DateTime.ParseExact(weather.lastBuildDate,
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", provider).ToLocalTime();

            if (updateTime.DayOfWeek == DateTime.Today.DayOfWeek)
            {
                date = "Updated at " + updateTime.ToString("t");
            }
            else
                date = "Updated on " + updateTime.ToString("ddd") + " " + updateTime.ToString("t");

            return date;
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

        private void updateBg(Weather weather)
        {
            ImageBrush bg = new ImageBrush();
            bg.Stretch = Stretch.UniformToFill;
            bg.AlignmentX = AlignmentX.Right;

            // Apply background based on weather condition
            switch (int.Parse(weather.condition.code))
            {
                // Night
                case 31:
                case 33:
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/NightSky.jpg"));
                    break;
                // Rain 
                case 9:
                case 11:
                case 12:
                case 40:
                // (Mixed) Rain/Snow/Sleet
                case 5:
                case 6:
                case 7:
                case 18:
                // Hail / Freezing Rain
                case 8:
                case 10:
                case 17:
                case 35:
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/RainySky.jpg"));
                    break;
                // Tornado / Hurricane / Thunderstorm / Tropical Storm
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 37:
                case 38:
                case 39:
                case 45:
                case 47:
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/StormySky.jpg"));
                    break;
                // Dust
                case 19:
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/Dust.jpg"));
                    break;
                // Foggy / Haze
                case 20:
                case 21:
                case 22:
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/FoggySky.jpg"));
                    break;
                // Snow / Snow Showers/Storm
                case 13:
                case 14:
                case 15:
                case 16:
                case 41:
                case 42:
                case 43:
                case 46:
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/Snow.jpg"));
                    break;
                /* Ambigious weather conditions */
                // (Mostly) Cloudy
                case 28:
                case 26:
                case 27:
                    if (isNight(weather))
                        bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/MostlyCloudy-Night.jpg"));
                    else
                        bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/MostlyCloudy-Day.jpg"));
                    break;
                // Partly Cloudy
                case 44:
                case 29:
                case 30:
                    if (isNight(weather))
                        bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/PartlyCloudy-Night.jpg"));
                    else
                        bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/PartlyCloudy-Day.jpg"));
                    break;
            }

            if (bg.ImageSource == null)
            {
                // Set background based using sunset/rise times
                if (isNight(weather))
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/NightSky.jpg"));
                else
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/DaySky.jpg"));
            }

            MainViewer.Background = bg;
        }

        private bool isNight(Weather weather)
        {
            // Determine whether its night using sunset/rise times
            if (DateTime.Now < DateTime.Parse(weather.astronomy.sunrise)
                    || DateTime.Now > DateTime.Parse(weather.astronomy.sunset))
                return true;
            else
                return false;
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
