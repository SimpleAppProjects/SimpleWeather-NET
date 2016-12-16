using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
        /* Panel Animation Workaround */
        public List<LocationPanelView> HomePanel { get; set; }
        public ObservableCollection<LocationPanelView> LocationPanels { get; set; }

        // For UI Thread
        CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

        public LocationsPage()
        {
            this.InitializeComponent();

            /* Panel Animation Workaround */
            HomePanel = new List<LocationPanelView>(1);
            LocationPanels = new ObservableCollection<LocationPanelView>();
            LocationPanels.CollectionChanged += LocationPanels_CollectionChanged;

            // Get locations and load up weather data
            LoadLocations();
        }

        private void LocationPanels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(LocationPanelView panelView in LocationPanels)
            {
                int index = LocationPanels.IndexOf(panelView) + 1;
                panelView.Pair = new KeyValuePair<int, Coordinate>(index, panelView.Pair.Value);
            }

            // Refresh ItemsSource
            OtherLocationsPanel.ItemsSource = null;
            OtherLocationsPanel.ItemsSource = LocationPanels;
        }

        private async void LoadLocations()
        {
            // Lets load it up...
            List<Coordinate> locations = await Settings.getLocations();
            HomeLocation.ItemsSource = HomePanel;
            OtherLocationsPanel.ItemsSource = LocationPanels;

            foreach(Coordinate location in locations)
            {
                int index = locations.IndexOf(location);

                LocationPanelView panel = new LocationPanelView();
                panel.Background = new SolidColorBrush(App.AppColor);

                if (index == 0) // Home
                    HomePanel.Add(panel);
                else
                    LocationPanels.Add(panel);
            }

            foreach (Coordinate location in locations)
            {
                int index = locations.IndexOf(location);

                wLoader = new WeatherDataLoader(location.ToString(), index);
                await wLoader.loadWeatherData().ContinueWith(async (t) =>
                {
                    Weather weather = wLoader.getWeather();

                    if (weather != null)
                    {
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (index == 0)
                            {
                                HomePanel.First().setWeather(weather);
                                HomePanel.First().Pair = new KeyValuePair<int, Coordinate>(index, location);
                            }
                            else
                            {
                                LocationPanelView panelView = LocationPanels[index - 1];
                                panelView.setWeather(weather);

                                // Save index to tag (to easily retreive)
                                panelView.Pair = new KeyValuePair<int, Coordinate>(index, location);
                            }
                        });
                    }
                    else
                    {
                        throw new NullReferenceException();
                    }
                }).ConfigureAwait(false);
            }

            // Refresh
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                HomeLocation.ItemsSource = null;
                OtherLocationsPanel.ItemsSource = null;
                HomeLocation.ItemsSource = HomePanel;
                OtherLocationsPanel.ItemsSource = LocationPanels;
            });
        }

        private void LocationButton_Click(object sender, RoutedEventArgs e)
        {
            LocationPanel panel = sender as LocationPanel;
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

        private void ShowAddLocationsPanel(bool show)
        {
            AddLocationsButton.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
            AddLocationPanel.Visibility = show ? Visibility.Visible : Visibility.Collapsed;

            if (!show)
                Location.Text = string.Empty;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ShowAddLocationsPanel(false);
        }

        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        #region LocationsPage HomePanelFunctions
        private async void HomeGPS_Click(object sender, RoutedEventArgs e)
        {
            NewHomeLocation.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            NewHomeLocation.BorderThickness = new Thickness(2);

            // Set window items
            HomeGPS.IsEnabled = false;

            Geolocator geolocal = new Geolocator();
            Geoposition geoPos = await geolocal.GetGeopositionAsync();
            List<Coordinate> locations = await Settings.getLocations();
            int index = 0; // Home Location

            wLoader = new WeatherDataLoader(geoPos, index);
            wLoader.loadWeatherData(true).ContinueWith(async (t) =>
            {
                Weather weather = wLoader.getWeather();

                if (weather != null)
                {
                    // Show location name
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Coordinate location = new Coordinate(
                            string.Join(",", wLoader.getWeather().location.lat, wLoader.getWeather().location._long));

                        // Save coords to List
                        locations[0] = location;
                        Settings.saveLocations(locations);

                        HomePanel.First().setWeather(weather);
                        // Save index to tag (to easily retreive)
                        HomePanel.First().Pair = new KeyValuePair<int, Coordinate>(index, location);

                        // Hide add locations panel
                        ShowChangeHomePanel(false);
                    });
                }
                else
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        NewHomeLocation.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                        NewHomeLocation.BorderThickness = new Thickness(5);
                        HomeGPS.IsEnabled = true;
                    });
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(false).GetAwaiter();

            // Refresh
            HomeLocation.ItemsSource = null;
            HomeLocation.ItemsSource = HomePanel;

            // Re-enable Button
            HomeGPS.IsEnabled = true;
        }

        private async void NewHomeLocation_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            NewHomeLocation.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            NewHomeLocation.BorderThickness = new Thickness(2);

            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                List<Coordinate> locations = await Settings.getLocations();
                int index = 0; // Home Location

                if (!String.IsNullOrWhiteSpace(NewHomeLocation.Text))
                {
                    wLoader = new WeatherDataLoader(NewHomeLocation.Text, index);
                    wLoader.loadWeatherData(true).ContinueWith(async (t) =>
                    {
                        Weather weather = wLoader.getWeather();

                        if (weather != null)
                        {
                            // Show location name
                            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                Coordinate location = new Coordinate(
                                    string.Join(",", wLoader.getWeather().location.lat, wLoader.getWeather().location._long));

                                // Save coords to List
                                locations[0] = location;
                                Settings.saveLocations(locations);

                                HomePanel.First().setWeather(weather);
                                // Save index to tag (to easily retreive)
                                HomePanel.First().Pair = new KeyValuePair<int, Coordinate>(index, location);

                                // Hide change location panel
                                ShowChangeHomePanel(false);
                            });
                        }
                        else
                        {
                            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                NewHomeLocation.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                                NewHomeLocation.BorderThickness = new Thickness(5);
                            });
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(false).GetAwaiter();
                }

                // Refresh
                HomeLocation.ItemsSource = null;
                HomeLocation.ItemsSource = HomePanel;

                e.Handled = true;
            }
        }

        private void ShowChangeHomePanel(bool show)
        {
            // Hide Textbox
            ChangeHomePanel.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            // Show HomeLocation Panel
            HomeLocation.Visibility = show ? Visibility.Collapsed : Visibility.Visible;

            if (!show)
                NewHomeLocation.Text = string.Empty;
        }

        private void NewHome_Cancel_Click(object sender, RoutedEventArgs e)
        {
            ShowChangeHomePanel(false);
        }

        private async void HomeLocation_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Button panel = sender as Button;

            Windows.UI.Popups.PopupMenu menu = new Windows.UI.Popups.PopupMenu();
            menu.Commands.Add(new Windows.UI.Popups.UICommand("Change Favorite Location", (command) =>
            {
                ShowChangeHomePanel(true);
            }));

            Windows.UI.Popups.IUICommand chosenCommand = await menu.ShowForSelectionAsync(GetElementRect(panel));
            if (chosenCommand == null) { } // The command is null if no command was invoked. 

            e.Handled = true;
        }
        #endregion

        #region LocationsPage OtherLocationPanelFunctions
        private async void OtherLocationButton_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            LocationPanel panel = sender as LocationPanel;

            Windows.UI.Popups.PopupMenu menu = new Windows.UI.Popups.PopupMenu();
            menu.Commands.Add(new Windows.UI.Popups.UICommand("Delete Location", async (command) =>
            {
                // Get panel index
                KeyValuePair<int, Coordinate> pair = (KeyValuePair<int, Coordinate>)panel.Tag;
                int idx = pair.Key;

                // Remove location from list
                List<Coordinate> locations = await Settings.getLocations();
                locations.RemoveAt(idx);
                Settings.saveLocations(locations);

                // Remove panel
                LocationPanels.RemoveAt(idx - 1);
            }));

            Windows.UI.Popups.IUICommand chosenCommand = await menu.ShowForSelectionAsync(GetElementRect(panel));
            if (chosenCommand == null) { } // The command is null if no command was invoked. 

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

        private void AddLocationsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAddLocationsPanel(true);
        }

        private async void Location_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            Location.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            Location.BorderThickness = new Thickness(2);

            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                List<Coordinate> locations = await Settings.getLocations();
                int index = locations.Count;

                if (!String.IsNullOrWhiteSpace(Location.Text))
                {
                    wLoader = new WeatherDataLoader(Location.Text, index);
                    wLoader.loadWeatherData(true).ContinueWith(async (t) =>
                    {
                        Weather weather = wLoader.getWeather();

                        if (weather != null)
                        {
                            // Show location name
                            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                Coordinate location = new Coordinate(
                                    string.Join(",", wLoader.getWeather().location.lat, wLoader.getWeather().location._long));

                                // Save coords to List
                                locations.Add(location);
                                Settings.saveLocations(locations);

                                LocationPanelView panelView = new LocationPanelView(weather);
                                // Save index to tag (to easily retreive)
                                panelView.Pair = new KeyValuePair<int, Coordinate>(index, location);

                                // Add to collection
                                LocationPanels.Add(panelView);

                                // Hide add locations panel
                                ShowAddLocationsPanel(false);
                            });
                        }
                        else
                        {
                            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                Location.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                                Location.BorderThickness = new Thickness(5);
                            });
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(false).GetAwaiter();
                }

                e.Handled = true;
            }
        }

        private async void OtherGPS_Click(object sender, RoutedEventArgs e)
        {
            Location.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            Location.BorderThickness = new Thickness(2);

            List<Coordinate> locations = await Settings.getLocations();

            // Set window items
            OtherGPS.IsEnabled = false;

            Geolocator geolocal = new Geolocator();
            Geoposition geoPos = await geolocal.GetGeopositionAsync();
            int index = locations.Count;

            wLoader = new WeatherDataLoader(geoPos, index);
            wLoader.loadWeatherData(true).ContinueWith(async (t) =>
            {
                Weather weather = wLoader.getWeather();

                if (weather != null)
                {
                    // Show location name
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Coordinate location = new Coordinate(
                            string.Join(",", wLoader.getWeather().location.lat, wLoader.getWeather().location._long));

                        // Save coords to List
                        locations.Add(location);
                        Settings.saveLocations(locations);

                        LocationPanelView panelView = new LocationPanelView(weather);
                        // Save index to tag (to easily retreive)
                        panelView.Pair = new KeyValuePair<int, Coordinate>(index, location);

                        // Add to collection
                        LocationPanels.Add(panelView);

                        // Hide add locations panel
                        ShowAddLocationsPanel(false);
                    });
                }
                else
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Location.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                        Location.BorderThickness = new Thickness(5);
                    });
                }

            }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(false).GetAwaiter();

            // Re-enable Button
            OtherGPS.IsEnabled = true;
        }
        #endregion
    }
}