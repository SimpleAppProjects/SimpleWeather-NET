using SimpleWeather.Controls;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationsPage : Page, WeatherLoadedListener
    {
        /* Panel Animation Workaround */
        public List<LocationPanelView> HomePanel { get; set; }
        public ObservableCollection<LocationPanelView> LocationPanels { get; set; }

        public ObservableCollection<LocationQueryView> LocationQuerys { get; set; }
        private string selected_query = string.Empty;

        public void onWeatherLoaded(int locationIdx, object weather)
        {
            if (weather != null)
            {
                if (locationIdx == 0)
                {
                    if (weather.GetType() == typeof(WeatherUnderground.Weather))
                        HomePanel.First().setWeather(weather as WeatherUnderground.Weather);
                    else if (weather.GetType() == typeof(WeatherYahoo.Weather))
                        HomePanel.First().setWeather(weather as WeatherYahoo.Weather);
                }
                else
                {
                    LocationPanelView panelView = LocationPanels[locationIdx - 1];

                    if (weather.GetType() == typeof(WeatherUnderground.Weather))
                        panelView.setWeather(weather as WeatherUnderground.Weather);
                    else if (weather.GetType() == typeof(WeatherYahoo.Weather))
                        panelView.setWeather(weather as WeatherYahoo.Weather);
                }
            }
        }

        public LocationsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            /* Panel Animation Workaround */
            HomePanel = new List<LocationPanelView>(1);
            LocationPanels = new ObservableCollection<LocationPanelView>();
            LocationPanels.CollectionChanged += LocationPanels_CollectionChanged;

            LocationQuerys = new ObservableCollection<LocationQueryView>();

            // Get locations and load up weather data
            LoadLocations();
        }

        private void LocationPanels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(LocationPanelView panelView in LocationPanels)
            {
                int index = LocationPanels.IndexOf(panelView) + 1;
                panelView.Pair = new KeyValuePair<int, object>(index, panelView.Pair.Value);
            }

            // Refresh ItemsSource
            OtherLocationsPanel.ItemsSource = null;
            OtherLocationsPanel.ItemsSource = LocationPanels;
        }

        private async void LoadLocations()
        {
            // Lets load it up...
            List<string> locations = await Settings.getLocations();

            foreach (string location in locations)
            {
                int index = locations.IndexOf(location);

                LocationPanelView panel = new LocationPanelView();
                panel.Background = new SolidColorBrush(App.AppColor);
                // Save index to tag (to easily retreive)
                panel.Pair = new KeyValuePair<int, object>(index, location);

                if (index == 0) // Home
                    HomePanel.Add(panel);
                else
                    LocationPanels.Add(panel);
            }

            // Refresh
            RefreshPanels();

            if (Settings.API == "WUnderground")
            {
                WeatherUnderground.WeatherDataLoader wu_Loader = null;

                foreach (string location in locations)
                {
                    int index = locations.IndexOf(location);

                    wu_Loader = new WeatherUnderground.WeatherDataLoader(this, location, index);
                    await wu_Loader.loadWeatherData(false);
                }
            }
            else
            {
                WeatherYahoo.WeatherDataLoader wLoader = null;

                foreach (string location in locations)
                {
                    int index = locations.IndexOf(location);

                    wLoader = new WeatherYahoo.WeatherDataLoader(this, location, index);
                    await wLoader.loadWeatherData(false);
                }
            }

            // Refresh
            RefreshPanels();
        }

        private void RefreshPanels()
        {
            // Refresh
            HomeLocation.ItemsSource = null;
            OtherLocationsPanel.ItemsSource = null;
            HomeLocation.ItemsSource = HomePanel;
            OtherLocationsPanel.ItemsSource = LocationPanels;
        }

        private void LocationButton_Click(object sender, RoutedEventArgs e)
        {
            LocationPanel panel = sender as LocationPanel;
            KeyValuePair<int, object> pair = (KeyValuePair<int, object>)panel.Tag;

            this.Frame.Navigate(typeof(WeatherNow), pair);
        }

        private async void GPS_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;

            GeolocationAccessStatus geoStatus = await Geolocator.RequestAccessAsync();
            Geolocator geolocal = new Geolocator();
            Geoposition geoPos = null;

            // Setup error just in case
            MessageDialog error = null;

            switch (geoStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    try
                    {
                        geoPos = await geolocal.GetGeopositionAsync();
                    }
                    catch (Exception)
                    {
                        error = new MessageDialog("Unable to retrieve location status", "Location access error");
                        await error.ShowAsync();
                    }
                    break;
                case GeolocationAccessStatus.Denied:
                    error = new MessageDialog("Access to location was denied. Please enable in Settings.", "Location access denied");
                    error.Commands.Add(new UICommand("Settings", async (command) =>
                    {
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
                    }, 0));
                    error.Commands.Add(new UICommand("Cancel", null, 1));
                    error.DefaultCommandIndex = 0;
                    error.CancelCommandIndex = 1;
                    await error.ShowAsync();
                    break;
                case GeolocationAccessStatus.Unspecified:
                    error = new MessageDialog("Unable to retrieve location status", "Location access error");
                    await error.ShowAsync();
                    break;
            }

            // Access to location granted
            if (geoPos != null)
            {
                button.IsEnabled = false;

                WeatherUnderground.location gpsLocation = await WeatherUnderground.GeopositionQuery.getLocation(geoPos);
                LocationQueryView view = new LocationQueryView(gpsLocation);

                LocationQuerys.Clear();
                LocationQuerys.Add(view);

                // Refresh list
                NewHomeLocation.ItemsSource = null;
                Location.ItemsSource = null;
                NewHomeLocation.ItemsSource = LocationQuerys;
                Location.ItemsSource = LocationQuerys;

                NewHomeLocation.IsSuggestionListOpen = true;
                Location.IsSuggestionListOpen = true;
            }

            button.IsEnabled = true;
        }

        private async void Location_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            LocationPanel panel = sender as LocationPanel;
            KeyValuePair<int, object> pair = (KeyValuePair<int, object>)panel.Tag;
            int idx = pair.Key;

            PopupMenu menu = new PopupMenu();

            if (idx == 0)
            {
                menu.Commands.Add(new UICommand("Change Favorite Location", (command) =>
                {
                    ShowChangeHomePanel(true);
                }));
            }
            else
            {
                menu.Commands.Add(new UICommand("Delete Location", async (command) =>
                {
                    // Remove location from list
                    OrderedDictionary weatherData = await Settings.getWeatherData();
                    weatherData.RemoveAt(idx);
                    Settings.saveWeatherData(weatherData);

                    // Remove panel
                    LocationPanels.RemoveAt(idx - 1);
                }));
            }

            IUICommand chosenCommand = await menu.ShowForSelectionAsync(GetElementRect(panel));
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

        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        private async void Location_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (!String.IsNullOrWhiteSpace(sender.Text) && args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                List<WeatherUnderground.AC_Location> results = await WeatherUnderground.AutoCompleteQuery.getLocations(sender.Text);

                LocationQuerys.Clear();

                // Show message if no results are found
                if (results.Count == 0)
                {
                    LocationQueryView noresults = new LocationQueryView();
                    noresults.LocationName = "No results found";
                    LocationQuerys.Add(noresults);
                }
                else
                {
                    // Limit amount of results shown
                    int maxResults = 10;

                    foreach (WeatherUnderground.AC_Location location in results)
                    {
                        LocationQueryView view = new LocationQueryView(location);
                        LocationQuerys.Add(view);

                        // Limit amount of results
                        maxResults--;
                        if (maxResults <= 0)
                            break;
                    }
                }

                // Refresh list
                sender.ItemsSource = null;
                sender.ItemsSource = LocationQuerys;

                sender.IsSuggestionListOpen = true;
            }
            else if (String.IsNullOrWhiteSpace(sender.Text))
            {
                // Hide flyout if query is empty or null
                sender.IsSuggestionListOpen = false;
            }
        }

        private void Location_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            LocationQueryView theChosenOne = args.SelectedItem as LocationQueryView;

            if (theChosenOne != null)
            {
                if (String.IsNullOrEmpty(theChosenOne.LocationQuery))
                    sender.Text = String.Empty;
                else
                    sender.Text = theChosenOne.LocationName;
            }

            sender.IsSuggestionListOpen = false;
        }

        private async void Location_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                LocationQueryView theChosenOne = args.ChosenSuggestion as LocationQueryView;

                if (!String.IsNullOrEmpty(theChosenOne.LocationQuery))
                    selected_query = theChosenOne.LocationQuery;
            }
            else if (!String.IsNullOrEmpty(args.QueryText))
            {
                // Use args.QueryText to determine what to do.
                List<WeatherUnderground.AC_Location> results = await WeatherUnderground.AutoCompleteQuery.getLocations(args.QueryText);

                if (results.Count > 0)
                {
                    sender.Text = results.First().name;
                    selected_query = results.First().l;
                }
            }

            int index = 0;
            if (Settings.API == "WUnderground")
            {
                OrderedDictionary weatherData = await Settings.getWeatherData();

                if (sender.Name == "NewHomeLocation")
                    index = 0;
                else
                    index = weatherData.Keys.Count;

                // Check if location already exists
                if (index == 0)
                {
                    if (weatherData.Keys.Cast<string>().First().Equals(selected_query))
                    {
                        ShowChangeHomePanel(false);
                        return;
                    }
                }
                else if (weatherData.Contains(selected_query))
                {
                    ShowAddLocationsPanel(false);
                    return;
                }

                WeatherUnderground.Weather weather = await WeatherUnderground.WeatherLoaderTask.getWeather(selected_query);

                if (weather == null)
                    return;

                if (weather != null)
                {
                    // Save coords to List
                    if (sender.Name == "NewHomeLocation")
                    {
                        weatherData.RemoveAt(0);
                        weatherData.Insert(0, selected_query, weather);
                    }
                    else
                    {
                        weatherData.Add(selected_query, weather);
                    }
                    // Save data
                    Settings.saveWeatherData(weatherData);

                    if (index == 0)
                    {
                        HomePanel.First().setWeather(weather);
                        // Save index to tag (to easily retreive)
                        HomePanel.First().Pair = new KeyValuePair<int, object>(index, selected_query);

                        // Hide change location panel
                        ShowChangeHomePanel(false);

                        // Refresh
                        HomeLocation.ItemsSource = null;
                        HomeLocation.ItemsSource = HomePanel;
                    }
                    else
                    {
                        LocationPanelView panelView = new LocationPanelView(weather);
                        // Save index to tag (to easily retreive)
                        panelView.Pair = new KeyValuePair<int, object>(index, selected_query);

                        // Add to collection
                        LocationPanels.Add(panelView);

                        // Hide add locations panel
                        ShowAddLocationsPanel(false);
                    }
                }
            }
            else
            {
                OrderedDictionary weatherData = await Settings.getWeatherData();

                if (sender.Name == "NewHomeLocation")
                    index = 0;
                else
                    index = weatherData.Keys.Count;

                WeatherYahoo.Weather weather = await WeatherYahoo.WeatherLoaderTask.getWeather(sender.Text);

                if (weather == null)
                    return;

                if (weather != null)
                {
                    // Show location name
                    WeatherUtils.Coordinate location = new WeatherUtils.Coordinate(
                        string.Join(",", weather.location.lat, weather.location._long));

                    // Check if location already exists
                    if (index == 0)
                    {
                        if (weatherData.Keys.Cast<string>().First().Equals(location.ToString()))
                        {
                            ShowChangeHomePanel(false);
                            return;
                        }
                    }
                    else if (weatherData.Contains(location.ToString()))
                    {
                        ShowAddLocationsPanel(false);
                        return;
                    }

                    // Save coords to List
                    if (sender.Name == "NewHomeLocation")
                    {
                        weatherData.RemoveAt(0);
                        weatherData.Insert(0, location.ToString(), weather);
                    }
                    else
                    {
                        weatherData.Add(location.ToString(), weather);
                    }
                    // Save data
                    Settings.saveWeatherData(weatherData);

                    if (index == 0)
                    {
                        HomePanel.First().setWeather(weather);
                        // Save index to tag (to easily retreive)
                        HomePanel.First().Pair = new KeyValuePair<int, object>(index, /*location*/selected_query);

                        // Hide change location panel
                        ShowChangeHomePanel(false);

                        // Refresh
                        HomeLocation.ItemsSource = null;
                        HomeLocation.ItemsSource = HomePanel;
                    }
                    else
                    {
                        LocationPanelView panelView = new LocationPanelView(weather);
                        // Save index to tag (to easily retreive)
                        panelView.Pair = new KeyValuePair<int, object>(index, /*location*/selected_query);

                        // Add to collection
                        LocationPanels.Add(panelView);

                        // Hide add locations panel
                        ShowAddLocationsPanel(false);
                    }
                }
            }

            sender.IsSuggestionListOpen = false;
        }

        #region LocationsPage HomePanelFunctions
        private void ShowChangeHomePanel(bool show)
        {
            // Hide Textbox
            ChangeHomePanel.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            // Show HomeLocation Panel
            HomeLocation.Visibility = show ? Visibility.Collapsed : Visibility.Visible;

            if (!show)
            {
                NewHomeLocation.Text = string.Empty;
                NewHomeLocation.IsSuggestionListOpen = false;
            }
        }

        private void NewHome_Cancel_Click(object sender, RoutedEventArgs e)
        {
            ShowChangeHomePanel(false);
        }
        #endregion

        #region LocationsPage OtherLocationPanelFunctions
        private void AddLocationsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAddLocationsPanel(true);
        }

        private void ShowAddLocationsPanel(bool show)
        {
            AddLocationsButton.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
            AddLocationPanel.Visibility = show ? Visibility.Visible : Visibility.Collapsed;

            if (!show)
            {
                Location.Text = string.Empty;
                Location.IsSuggestionListOpen = false;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ShowAddLocationsPanel(false);
        }
        #endregion
    }
}