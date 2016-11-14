using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationsPage : Page
    {
        WeatherDataLoader wLoader = null;

        public LocationsPage()
        {
            this.InitializeComponent();

            LoadLocations();
        }

        private async void LoadLocations()
        {
            /* 
             * TODO:
             * Load locations from list
             * Get Home Location (first one)
             * Update Location card
             * Find other locations
             * Add cards for each (stack panel)
             * Update layout for card
            */
            // For UI Thread
            Windows.UI.Core.CoreDispatcher dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

            // Lets load it up...
            List<Coordinate> locations = await Settings.getLocations();
            Coordinate favorite = locations[0];

            // Clear Panel
            OtherLocationsPanel.Children.Clear();

            foreach (Coordinate location in locations)
            {
                int index = locations.IndexOf(location);

                wLoader = new WeatherDataLoader(location.ToString(), index);
                await wLoader.loadWeatherData().ContinueWith(async (t) =>
                {
                    Weather weather = wLoader.getWeather();

                    if (weather != null)
                    {
                        await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            if (location.Equals(favorite))
                            {
                                // Home Location
                                updatePanel(HomeLocation, weather);

                                // Register event handlers
                                //HomeLocation.PointerReleased += OtherLocationButton_PointerReleased;
                                HomeLocation.Holding += LocationButton_Holding;

                                // Save index to tag (to easily retreive)
                                KeyValuePair<int, Coordinate> pair = new KeyValuePair<int, Coordinate>(index, location);
                                HomeLocation.Tag = pair;
                                HomeLocation.Click += LocationButton_Click;
                            }
                            else
                            {
                                // Other Locations
                                Button otherLocal = new Button();
                                updatePanel(otherLocal, weather);

                                // Register event handlers
                                otherLocal.PointerReleased += OtherLocationButton_PointerReleased;
                                otherLocal.Holding += LocationButton_Holding;

                                // Save index to tag (to easily retreive)
                                KeyValuePair<int, Coordinate> pair = new KeyValuePair<int, Coordinate>(index, location);
                                otherLocal.Tag = pair;
                                otherLocal.Click += LocationButton_Click;

                                // Add to panel
                                OtherLocationsPanel.Children.Add(otherLocal);
                            }
                        });
                    }
                });
            }
        }

        private void LocationButton_Click(object sender, RoutedEventArgs e)
        {
            Button panel = sender as Button;
            KeyValuePair<int, Coordinate> pair = (KeyValuePair<int, Coordinate>)panel.Tag;
            wLoader = new WeatherDataLoader(pair.Value.ToString(), pair.Key);

            // Save WeatherLoader
            if (CoreApplication.Properties.ContainsKey("WeatherLoader"))
            {
                CoreApplication.Properties.Remove("WeatherLoader");
            }
            CoreApplication.Properties.Add("WeatherLoader", wLoader);

            this.Frame.Navigate(typeof(WeatherNow));
        }

        private async void OtherLocationButton_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Button panel = sender as Button;

            Windows.UI.Popups.PopupMenu menu = new Windows.UI.Popups.PopupMenu();
            menu.Commands.Add(new Windows.UI.Popups.UICommand("Delete Location", async (command) =>
            {
                // Get panel index
                int idx = OtherLocationsPanel.Children.IndexOf(panel) + 1;

                // Remove location from list
                List<Coordinate> locations = await Settings.getLocations();
                locations.RemoveAt(idx);
                Settings.saveLocations(locations);

                // Remove panel
                OtherLocationsPanel.Children.Remove(panel);
            }));

            Windows.UI.Popups.IUICommand chosenCommand = await menu.ShowForSelectionAsync(GetElementRect(panel));
            if (chosenCommand == null) // The command is null if no command was invoked. 
            {
                //Context menu dismissed
            }

            e.Handled = true;
        }

        private void LocationButton_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                Button panel = sender as Button;
                panel.ReleasePointerCaptures();
                e.Handled = true;
            }
        }

        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        private void updatePanel(Button weatherPanel, Weather weather)
        {
            // Update background
            updateBg(weatherPanel, weather);

            weatherPanel.Style = this.Resources["LocationButtonStyle"] as Style;

            Grid outergrid = new Grid();
            outergrid.Height = weatherPanel.Height;

            // Columns
            ColumnDefinition gridCol1 = new ColumnDefinition();
            gridCol1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition gridCol2 = new ColumnDefinition();
            gridCol2.Width = GridLength.Auto;
            outergrid.ColumnDefinitions.Add(gridCol1);
            outergrid.ColumnDefinitions.Add(gridCol2);

            TextBlock locationName = new TextBlock();
            locationName.Text = weather.location.description;
            locationName.FontSize = 24;
            locationName.VerticalAlignment = VerticalAlignment.Center;
            locationName.Padding = new Thickness(10);
            Grid.SetColumn(locationName, 0);

            Grid innergrid = new Grid();
            ColumnDefinition innerGridCol1 = new ColumnDefinition();
            innerGridCol1.Width = new GridLength(100);
            ColumnDefinition innerGridCol2 = new ColumnDefinition();
            innerGridCol2.Width = new GridLength(100);
            innergrid.ColumnDefinitions.Add(innerGridCol1);
            innergrid.ColumnDefinitions.Add(innerGridCol2);
            Grid.SetColumn(innergrid, 1);

            TextBlock currTemp = new TextBlock();
            currTemp.Text = weather.condition.temp + "º";
            currTemp.FontSize = 24;
            currTemp.Padding = new Thickness(10);

            TextBlock weatherIcon = new TextBlock();
            weatherIcon.Style = this.Resources["WeatherIcon"] as Style;
            updateWeatherIcon(weatherIcon, int.Parse(weather.condition.code));

            Viewbox temp = new Viewbox();
            temp.VerticalAlignment = VerticalAlignment.Stretch;
            temp.Width = 60;
            temp.Height = temp.Width;

            Viewbox icon = new Viewbox();
            icon.VerticalAlignment = VerticalAlignment.Stretch;
            icon.Width = 60;
            icon.Height = icon.Width;

            // Add to viewboxes
            temp.Child = currTemp;
            icon.Child = weatherIcon;

            // Set Columns for boxes
            Grid.SetColumn(temp, 0);
            Grid.SetColumn(icon, 1);

            // Add to innergrid
            innergrid.Children.Add(temp);
            innergrid.Children.Add(icon);

            // Add to outergrids
            outergrid.Children.Add(locationName);
            outergrid.Children.Add(innergrid);

            // Add to panel
            weatherPanel.Content = outergrid;
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


        private void updateBg(Button weatherPanel, Weather weather)
        {
            ImageBrush bg = new ImageBrush();
            bg.Stretch = Stretch.UniformToFill;
            bg.AlignmentX = AlignmentX.Right;
            bg.AlignmentY = AlignmentY.Center;

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

            weatherPanel.Background = bg;
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

        private void AddLocationsButton_Click(object sender, RoutedEventArgs e)
        {
            AddLocationsButton.Visibility = Visibility.Collapsed;
            AddLocationPanel.Visibility = Visibility.Visible;
        }

        private async void Location_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            Location.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);

            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // For UI Thread
                Windows.UI.Core.CoreDispatcher dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
                List<Coordinate> locations = await Settings.getLocations();
                int index = locations.Count;

                if (!String.IsNullOrWhiteSpace(Location.Text))
                {
                    wLoader = new WeatherDataLoader(Location.Text, index);
                    await wLoader.loadWeatherData(true).ContinueWith(async (t) =>
                    {
                        Weather weather = wLoader.getWeather();

                        if (weather != null)
                        {
                            // Show location name
                            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                Coordinate location = new Coordinate(
                                    string.Join(",", wLoader.getWeather().location.lat, wLoader.getWeather().location._long));

                                // Save coords to List
                                locations.Add(location);
                                Settings.saveLocations(locations);

                                // TODO: Just Re-load locations ^^??
                                Button otherLocal = new Button();
                                updatePanel(otherLocal, weather);

                                // Register event handlers
                                otherLocal.PointerReleased += OtherLocationButton_PointerReleased;
                                otherLocal.Holding += LocationButton_Holding;

                                // Save index to tag (to easily retreive)
                                KeyValuePair<int, Coordinate> pair = new KeyValuePair<int, Coordinate>(index, location);
                                otherLocal.Tag = pair;
                                otherLocal.Click += LocationButton_Click;

                                // Add to panel
                                OtherLocationsPanel.Children.Add(otherLocal);

                                // Hide add locations panel
                                Cancel_Click(sender, e);
                            });
                        }
                        else
                        {
                            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                /*
                                String errorMSG = "Unable to get weather data! Try again or enter a different location.";
                                Windows.UI.Popups.MessageDialog error = new Windows.UI.Popups.MessageDialog(errorMSG);
                                await error.ShowAsync();
                                */
                                Location.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                            });
                        }
                    });
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Put this in a function?
            AddLocationsButton.Visibility = Visibility.Visible;
            AddLocationPanel.Visibility = Visibility.Collapsed;
            Location.Text = string.Empty;
        }
    }
}
