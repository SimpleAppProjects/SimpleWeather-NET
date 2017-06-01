using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP

{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationsPage : Page, WeatherLoadedListener
    {
        private CancellationTokenSource cts = new CancellationTokenSource();

        /* Panel Animation Workaround */
        public List<LocationPanelView> HomePanel { get; set; }
        public ObservableCollection<LocationPanelView> LocationPanels { get; set; }

        public ObservableCollection<LocationQueryView> LocationQuerys { get; set; }
        private string selected_query = string.Empty;

        public void onWeatherLoaded(int locationIdx, Weather weather)
        {
            if (weather != null)
            {
                if (locationIdx == App.HomeIdx)
                {
                    HomePanel.First().setWeather(weather);
                }
                else
                {
                    LocationPanelView panelView = LocationPanels[locationIdx - 1];
                    panelView.setWeather(weather);
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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (HomePanel.Count < 1 && LocationPanels.Count == 0)
            {
                // New instance; Get locations and load up weather data
                LoadLocations();
            }
            else
            {
                // Refresh view
                RefreshLocations();
            }
        }

        private void LocationPanels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(LocationPanelView panelView in LocationPanels)
            {
                int index = LocationPanels.IndexOf(panelView) + 1;
                panelView.Pair = new KeyValuePair<int, string>(index, panelView.Pair.Value);
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
                // Save index to tag (to easily retreive)
                panel.Pair = new KeyValuePair<int, string>(index, location);

                if (index == App.HomeIdx) // Home
                    HomePanel.Add(panel);
                else
                    LocationPanels.Add(panel);
            }

            // Refresh
            RefreshPanels();

            WeatherDataLoader wLoader = null;

            foreach (string location in locations)
            {
                int index = locations.IndexOf(location);

                wLoader = new WeatherDataLoader(this, location, index);
                await wLoader.loadWeatherData(false);
            }

            // Refresh
            RefreshPanels();
        }

        private async void RefreshLocations()
        {
            foreach (LocationPanelView view in HomePanel.Concat(LocationPanels))
            {
                WeatherDataLoader wLoader =
                    new WeatherDataLoader(this, view.Pair.Value, view.Pair.Key);
                await wLoader.loadWeatherData(false);
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
            KeyValuePair<int, string> pair = (KeyValuePair<int, string>)panel.Tag;

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

                await Task.Run(async () =>
                {
                    if (cts.IsCancellationRequested) return;

                    LocationQueryView view = await GeopositionQuery.getLocation(geoPos);

                    // Refresh list
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        LocationQuerys.Clear();
                        LocationQuerys.Add(view);

                        AutoSuggestBox box = null;
                        DependencyObject parent = button.Parent;
                        while (!(parent is AutoSuggestBox))
                        {
                            parent = VisualTreeHelper.GetParent(parent);
                        }
                        box = parent as AutoSuggestBox;

                        box.ItemsSource = null;
                        box.ItemsSource = LocationQuerys;
                        box.IsSuggestionListOpen = true;
                    });
                });
            }

            button.IsEnabled = true;
        }

        private async void Location_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            LocationPanel panel = sender as LocationPanel;
            KeyValuePair<int, string> pair = (KeyValuePair<int, string>)panel.Tag;
            int idx = pair.Key;

            PopupMenu menu = new PopupMenu();

            if (idx == App.HomeIdx)
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
                    Settings.saveWeatherData();

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

        private void Location_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Cancel pending searches
            cts.Cancel();
            cts = new CancellationTokenSource();

            if (!String.IsNullOrWhiteSpace(sender.Text) && args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                String query = sender.Text;

                Task.Run(async () =>
                {
                    if (cts.IsCancellationRequested) return;

                    var results = await AutoCompleteQuery.getLocations(query);

                    if (cts.IsCancellationRequested) return;

                    // Refresh list
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        LocationQuerys = results;
                        sender.ItemsSource = null;
                        sender.ItemsSource = LocationQuerys;
                        sender.IsSuggestionListOpen = true;
                    });
                });
            }
            else if (String.IsNullOrWhiteSpace(sender.Text))
            {
                // Cancel pending searches
                cts.Cancel();
                // Hide flyout if query is empty or null
                LocationQuerys.Clear();
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
                else
                    selected_query = string.Empty;
            }
            else if (!String.IsNullOrEmpty(args.QueryText))
            {
                // Use args.QueryText to determine what to do.
                LocationQueryView result = (await AutoCompleteQuery.getLocations(args.QueryText)).First();

                if (result != null && String.IsNullOrWhiteSpace(result.LocationQuery))
                {
                    sender.Text = result.LocationName;
                    selected_query = result.LocationQuery;
                }
            }
            else if (String.IsNullOrWhiteSpace(args.QueryText))
            {
                // Stop since there is no valid query
                return;
            }

            if (String.IsNullOrWhiteSpace(selected_query))
            {
                // Stop since there is no valid query
                return;
            }

            int index = 0;
            OrderedDictionary weatherData = await Settings.getWeatherData();

            if (sender.Name == "NewHomeLocation")
                index = App.HomeIdx;
            else
                index = weatherData.Keys.Count;

            // Check if location already exists
            if (index == App.HomeIdx)
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

            Weather weather = await WeatherLoaderTask.getWeather(selected_query);

            if (weather == null)
                return;

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
            Settings.saveWeatherData();

            if (index == App.HomeIdx)
            {
                HomePanel.First().setWeather(weather);
                // Save index to tag (to easily retreive)
                HomePanel.First().Pair = new KeyValuePair<int, string>(index, selected_query);

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
                panelView.Pair = new KeyValuePair<int, string>(index, selected_query);

                // Add to collection
                LocationPanels.Add(panelView);

                // Hide add locations panel
                ShowAddLocationsPanel(false);
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