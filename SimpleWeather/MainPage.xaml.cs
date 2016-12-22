using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimpleWeather
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        WeatherDataLoader wLoader = null;
        int homeIdx = 0;

        // For UI Thread
        CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

        public MainPage()
        {
            this.InitializeComponent();

            // TitleBar
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                // Mobile
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
            else
            {
                // Desktop
                var titlebar = ApplicationView.GetForCurrentView().TitleBar;
                titlebar.BackgroundColor = App.AppColor;
                titlebar.ButtonBackgroundColor = titlebar.BackgroundColor;
            }

            // Restore Weather if Location already set
            Restore();
        }

        private async void Restore()
        {
            // Hide UIElements
            SearchGrid.Visibility = Visibility.Collapsed;
            // Show Loading Ring
            LoadingRing.IsActive = true;

            if (Settings.WeatherLoaded)
            {
                // Weather was loaded before. Lets load it up...
                var localSettings = ApplicationData.Current.LocalSettings;
                List<Coordinate> locations = await Settings.getLocations();
                Coordinate local = locations[homeIdx];

                wLoader = new WeatherDataLoader(local.ToString(), homeIdx);

                // Loop until we get weather data
                do
                {
                    await wLoader.loadWeatherData().ContinueWith(async (t) =>
                    {
                        if (wLoader.getWeather() != null)
                        {
                            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                if (CoreApplication.Properties.ContainsKey("WeatherLoader"))
                                {
                                    CoreApplication.Properties.Remove("WeatherLoader");
                                }
                                CoreApplication.Properties.Add("WeatherLoader", wLoader);

                                this.Frame.Navigate(typeof(Shell));
                            });
                        }
                    }).ConfigureAwait(false);

                    if (wLoader.getWeather() == null)
                        await Task.Delay(1000);

                } while (wLoader.getWeather() == null);
            }
            else
            {
                LoadingRing.IsActive = false;
                SearchGrid.Visibility = Visibility.Visible;
            }
        }

        private async void Location_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            Location.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            Location.BorderThickness = new Thickness(2);

            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (!String.IsNullOrWhiteSpace(Location.Text))
                {
                    // Set window items
                    LoadingRing.IsActive = true;
                    GPS.IsEnabled = false;

                    wLoader = new WeatherDataLoader(Location.Text, homeIdx);
                    await wLoader.loadWeatherData(true).ContinueWith(async (t) =>
                    {
                        if (wLoader.getWeather() != null)
                        {
                            // Show location name
                            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                Coordinate location = new Coordinate(
                                    string.Join(",", wLoader.getWeather().location.lat, wLoader.getWeather().location._long));
                                Location.Text = location.ToString();

                                // Save coords to List
                                List<Coordinate> locations = new List<Coordinate>();
                                locations.Add(location);
                                Settings.saveLocations(locations);

                                // Save WeatherLoader
                                if (CoreApplication.Properties.ContainsKey("WeatherLoader"))
                                {
                                    CoreApplication.Properties.Remove("WeatherLoader");
                                }
                                CoreApplication.Properties.Add("WeatherLoader", wLoader);

                                Settings.WeatherLoaded = true;
                                this.Frame.Navigate(typeof(Shell), Location.Tag);
                            });
                        }
                        else
                        {
                            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                LoadingRing.IsActive = false;
                                SearchGrid.Visibility = Visibility.Visible;
                                GPS.IsEnabled = true;

                                Location.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                                Location.BorderThickness = new Thickness(5);
                            });
                        }
                    }).ConfigureAwait(false);
                }

                e.Handled = true;
            }
        }

        private async void GPS_Click(object sender, RoutedEventArgs e)
        {
            Location.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);

            // Set window items
            LoadingRing.IsActive = true;
            GPS.IsEnabled = false;

            Geolocator geolocal = new Geolocator();
            Geoposition geoPos = await geolocal.GetGeopositionAsync();

            wLoader = new WeatherDataLoader(geoPos, homeIdx);
            await wLoader.loadWeatherData(true).ContinueWith(async (t) =>
            {
                if (wLoader.getWeather() != null)
                {
                    // Show location name
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Coordinate location = new Coordinate(
                            string.Join(",", wLoader.getWeather().location.lat, wLoader.getWeather().location._long));
                        Location.Text = location.ToString();

                        // Save coords to List
                        List<Coordinate> locations = new List<Coordinate>();
                        locations.Add(location);
                        Settings.saveLocations(locations);

                        // Save WeatherLoader
                        if (CoreApplication.Properties.ContainsKey("WeatherLoader"))
                        {
                            CoreApplication.Properties.Remove("WeatherLoader");
                        }
                        CoreApplication.Properties.Add("WeatherLoader", wLoader);

                        Settings.WeatherLoaded = true;
                        this.Frame.Navigate(typeof(Shell), GPS.Tag);
                    });
                }
                else
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        LoadingRing.IsActive = false;
                        SearchGrid.Visibility = Visibility.Visible;
                        GPS.IsEnabled = true;

                        Location.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                        Location.BorderThickness = new Thickness(5);
                    });
                }
            }).ConfigureAwait(false);
        }
    }
}
