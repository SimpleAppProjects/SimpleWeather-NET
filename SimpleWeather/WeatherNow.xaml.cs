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

            // Update forecasts - manually b/c im dumb
            // TODO: simplify this...
            Date1.Text = DateTime.Parse(weather.forecasts[0].date).ToString("dddd dd");
            updateWeatherIcon(WeatherIcon1, int.Parse(weather.forecasts[0].code));
            Windows.UI.Xaml.Documents.Run Hi1 = new Windows.UI.Xaml.Documents.Run();
            Windows.UI.Xaml.Documents.Run Lo1 = new Windows.UI.Xaml.Documents.Run();
            Lo1.FontSize = 18;
            Hi1.Text = weather.forecasts[0].high + "º | ";
            Lo1.Text = weather.forecasts[0].low+ "º";
            Windows.UI.Xaml.Documents.Paragraph p1 = new Windows.UI.Xaml.Documents.Paragraph();
            p1.Inlines.Add(Hi1);
            p1.Inlines.Add(Lo1);
            Temp1.Blocks.Clear();
            Temp1.Blocks.Add(p1);
            Condition1.Text = weather.forecasts[0].text;

            Date2.Text = DateTime.Parse(weather.forecasts[1].date).ToString("dddd dd");
            updateWeatherIcon(WeatherIcon2, int.Parse(weather.forecasts[1].code));
            Windows.UI.Xaml.Documents.Run Hi2 = new Windows.UI.Xaml.Documents.Run();
            Windows.UI.Xaml.Documents.Run Lo2 = new Windows.UI.Xaml.Documents.Run();
            Lo2.FontSize = 18;
            Hi2.Text = weather.forecasts[1].high + "º | ";
            Lo2.Text = weather.forecasts[1].low+ "º";
            Windows.UI.Xaml.Documents.Paragraph p2 = new Windows.UI.Xaml.Documents.Paragraph();
            p2.Inlines.Add(Hi2);
            p2.Inlines.Add(Lo2);
            Temp2.Blocks.Clear();
            Temp2.Blocks.Add(p2);
            Condition2.Text = weather.forecasts[1].text;

            Date3.Text = DateTime.Parse(weather.forecasts[2].date).ToString("dddd dd");
            updateWeatherIcon(WeatherIcon3, int.Parse(weather.forecasts[2].code));
            Windows.UI.Xaml.Documents.Run Hi3 = new Windows.UI.Xaml.Documents.Run();
            Windows.UI.Xaml.Documents.Run Lo3 = new Windows.UI.Xaml.Documents.Run();
            Lo3.FontSize = 18;
            Hi3.Text = weather.forecasts[2].high + "º | ";
            Lo3.Text = weather.forecasts[2].low+ "º";
            Windows.UI.Xaml.Documents.Paragraph p3 = new Windows.UI.Xaml.Documents.Paragraph();
            p3.Inlines.Add(Hi3);
            p3.Inlines.Add(Lo3);
            Temp3.Blocks.Clear();
            Temp3.Blocks.Add(p3);
            Condition3.Text = weather.forecasts[2].text;

            Date4.Text = DateTime.Parse(weather.forecasts[3].date).ToString("dddd dd");
            updateWeatherIcon(WeatherIcon4, int.Parse(weather.forecasts[3].code));
            Windows.UI.Xaml.Documents.Run Hi4 = new Windows.UI.Xaml.Documents.Run();
            Windows.UI.Xaml.Documents.Run Lo4 = new Windows.UI.Xaml.Documents.Run();
            Lo4.FontSize = 18;
            Hi4.Text = weather.forecasts[3].high + "º | ";
            Lo4.Text = weather.forecasts[3].low+ "º";
            Windows.UI.Xaml.Documents.Paragraph p4 = new Windows.UI.Xaml.Documents.Paragraph();
            p4.Inlines.Add(Hi4);
            p4.Inlines.Add(Lo4);
            Temp4.Blocks.Clear();
            Temp4.Blocks.Add(p4);
            Condition4.Text = weather.forecasts[3].text;

            Date5.Text = DateTime.Parse(weather.forecasts[4].date).ToString("dddd dd");
            updateWeatherIcon(WeatherIcon5, int.Parse(weather.forecasts[4].code));
            Windows.UI.Xaml.Documents.Run Hi5 = new Windows.UI.Xaml.Documents.Run();
            Windows.UI.Xaml.Documents.Run Lo5 = new Windows.UI.Xaml.Documents.Run();
            Lo5.FontSize = 18;
            Hi5.Text = weather.forecasts[4].high + "º | ";
            Lo5.Text = weather.forecasts[4].low+ "º";
            Windows.UI.Xaml.Documents.Paragraph p5 = new Windows.UI.Xaml.Documents.Paragraph();
            p5.Inlines.Add(Hi5);
            p5.Inlines.Add(Lo5);
            Temp5.Blocks.Clear();
            Temp5.Blocks.Add(p5);
            Condition5.Text = weather.forecasts[4].text;

            Date6.Text = DateTime.Parse(weather.forecasts[5].date).ToString("dddd dd");
            updateWeatherIcon(WeatherIcon6, int.Parse(weather.forecasts[5].code));
            Windows.UI.Xaml.Documents.Run Hi6 = new Windows.UI.Xaml.Documents.Run();
            Windows.UI.Xaml.Documents.Run Lo6 = new Windows.UI.Xaml.Documents.Run();
            Lo6.FontSize = 18;
            Hi6.Text = weather.forecasts[5].high + "º | ";
            Lo6.Text = weather.forecasts[5].low+ "º";
            Windows.UI.Xaml.Documents.Paragraph p6 = new Windows.UI.Xaml.Documents.Paragraph();
            p6.Inlines.Add(Hi6);
            p6.Inlines.Add(Lo6);
            Temp6.Blocks.Clear();
            Temp6.Blocks.Add(p6);
            Condition6.Text = weather.forecasts[5].text;

            Date7.Text = DateTime.Parse(weather.forecasts[6].date).ToString("dddd dd");
            updateWeatherIcon(WeatherIcon7, int.Parse(weather.forecasts[6].code));
            Windows.UI.Xaml.Documents.Run Hi7 = new Windows.UI.Xaml.Documents.Run();
            Windows.UI.Xaml.Documents.Run Lo7 = new Windows.UI.Xaml.Documents.Run();
            Lo7.FontSize = 18;
            Hi7.Text = weather.forecasts[6].high + "º | ";
            Lo7.Text = weather.forecasts[6].low+ "º";
            Windows.UI.Xaml.Documents.Paragraph p7 = new Windows.UI.Xaml.Documents.Paragraph();
            p7.Inlines.Add(Hi7);
            p7.Inlines.Add(Lo7);
            Temp7.Blocks.Clear();
            Temp7.Blocks.Add(p7);
            Condition7.Text = weather.forecasts[6].text;

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

            // If NULL check time to see if its night time
            if (bg.ImageSource == null)
            {
                // If its past 7PM (19:00) its probably night out
                if (DateTime.Now.Hour >= 19)
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
    }
}
