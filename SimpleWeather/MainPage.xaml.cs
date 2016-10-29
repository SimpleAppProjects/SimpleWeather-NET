using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
                titlebar.BackgroundColor = Windows.UI.Color.FromArgb(1, 0, 111, 191);
                titlebar.ButtonBackgroundColor = titlebar.BackgroundColor;
            }

            // Restore Weather if Location already set
            Restore();
        }

        private async void Restore()
        {
            // For UI Thread
            Windows.UI.Core.CoreDispatcher dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

            // Hide UIElements
            SearchGrid.Visibility = Visibility.Collapsed;
            // Show Loading Ring
            LoadingRing.IsActive = true;

            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values["weatherLoaded"] != null)
            {
                if (localSettings.Values["weatherLoaded"].Equals("true"))
                {
                    // Weather was loaded before. Lets load it up...
                    Coordinate local = new Coordinate(ApplicationData.Current.LocalSettings.Values["HomeLocation"].ToString());
                    wLoader = new WeatherDataLoader(local.ToString());

                    await wLoader.loadWeatherData(true).ContinueWith(async (t) =>
                    {
                        if (wLoader.getWeather() != null)
                        {
                            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                if (CoreApplication.Properties.ContainsKey("WeatherLoader"))
                                {
                                    CoreApplication.Properties.Remove("WeatherLoader");
                                }
                                CoreApplication.Properties.Add("WeatherLoader", wLoader);

                                this.Frame.Navigate(typeof(WeatherNow));
                            });
                        }
                    });
                }
            }
            else
            {
                LoadingRing.IsActive = false;
                SearchGrid.Visibility = Visibility.Visible;
            }
        }

        private async void Location_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // For UI Thread
                Windows.UI.Core.CoreDispatcher dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

                if (!String.IsNullOrWhiteSpace(Location.Text))
                {
                    // Set window items
                    LoadingRing.IsActive = true;
                    GPS.IsEnabled = false;

                    wLoader = new WeatherDataLoader(Location.Text);
                    await wLoader.loadWeatherData(false).ContinueWith(async (t) =>
                    {
                        if (wLoader.getWeather() != null)
                        {
                            // Show location name
                            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                ApplicationData.Current.LocalSettings.Values["HomeLocation"] =
                                        string.Join(",", wLoader.getWeather().location.lat, wLoader.getWeather().location._long);

                                Location.Text = wLoader.getWeather().location.city + ", " + wLoader.getWeather().location.region;

                                if (CoreApplication.Properties.ContainsKey("WeatherLoader"))
                                {
                                    CoreApplication.Properties.Remove("WeatherLoader");
                                }
                                CoreApplication.Properties.Add("WeatherLoader", wLoader);

                                ApplicationData.Current.LocalSettings.Values["weatherLoaded"] = "true";
                                this.Frame.Navigate(typeof(WeatherNow), Location.Tag);
                            });
                        }
                    });
                }
            }
        }

        private async void GPS_Click(object sender, RoutedEventArgs e)
        {
            // For UI Thread
            Windows.UI.Core.CoreDispatcher dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

            // Set window items
            LoadingRing.IsActive = true;
            GPS.IsEnabled = false;

            Windows.Devices.Geolocation.Geolocator geolocal = new Windows.Devices.Geolocation.Geolocator();
            Windows.Devices.Geolocation.Geoposition geoPos = await geolocal.GetGeopositionAsync();

            wLoader = new WeatherDataLoader(geoPos);
            await wLoader.loadWeatherData(false).ContinueWith(async (t) =>
            {
                if (wLoader.getWeather() != null)
                {
                    // Show location name
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        ApplicationData.Current.LocalSettings.Values["HomeLocation"] = 
                                string.Join(",", wLoader.getWeather().location.lat, wLoader.getWeather().location._long);

                        Location.Text = wLoader.getWeather().location.city + ", " + wLoader.getWeather().location.region;

                        if (CoreApplication.Properties.ContainsKey("WeatherLoader"))
                        {
                            CoreApplication.Properties.Remove("WeatherLoader");
                        }
                        CoreApplication.Properties.Add("WeatherLoader", wLoader);

                        ApplicationData.Current.LocalSettings.Values["weatherLoaded"] = "true";
                        this.Frame.Navigate(typeof(WeatherNow), GPS.Tag);
                    });
                }
            });
        }
    }
}
