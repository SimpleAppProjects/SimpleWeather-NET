using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
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

        public ObservableCollection<LocationPanelView> LocationPanels { get; set; }
        public ObservableCollection<LocationQueryView> LocationQuerys { get; set; }
        private string selected_query = string.Empty;

        public bool EditMode { get; set; } = false;
        private bool DataChanged = false;

        public void onWeatherLoaded(int locationIdx, Weather weather)
        {
            if (weather != null)
            {
                LocationPanelView panelView = LocationPanels[locationIdx];
                panelView.setWeather(weather);
            }
        }

        public LocationsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            LocationPanels = new ObservableCollection<LocationPanelView>();
            LocationPanels.CollectionChanged += LocationPanels_CollectionChanged;

            LocationQuerys = new ObservableCollection<LocationQueryView>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (LocationPanels.Count == 0)
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

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // Cancel edit mode if moving away
            if (EditMode)
                ToggleEditMode();
        }

        private void LocationPanels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach(LocationPanelView panelView in LocationPanels)
            {
                int index = LocationPanels.IndexOf(panelView);
                panelView.Pair = new KeyValuePair<int, string>(index, panelView.Pair.Value);
            }

            // Cancel edit Mode
            if (EditMode && LocationPanels.Count == 1)
                ToggleEditMode();

            // Disable EditMode if only single location
            EditButton.Visibility = LocationPanels.Count == 1 ? Visibility.Collapsed : Visibility.Visible;

            // Flag that data has changed
            if (EditMode && (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Move))
                DataChanged = true;
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

                // Set home
                if (index == App.HomeIdx)
                    panel.IsHome = true;
                else
                    panel.IsHome = false;

                LocationPanels.Add(panel);
            }

            WeatherDataLoader wLoader = null;

            foreach (string location in locations)
            {
                int index = locations.IndexOf(location);

                wLoader = new WeatherDataLoader(this, location, index);
                await wLoader.loadWeatherData(false);
            }
        }

        private async void RefreshLocations()
        {
            foreach (LocationPanelView view in LocationPanels)
            {
                WeatherDataLoader wLoader =
                    new WeatherDataLoader(this, view.Pair.Value, view.Pair.Key);
                await wLoader.loadWeatherData(false);
            }
        }

        private void LocationsPanel_ItemClick(object sender, ItemClickEventArgs e)
        {
            LocationPanelView panel = e.ClickedItem as LocationPanelView;

            this.Frame.Navigate(typeof(WeatherNow), panel.Pair);
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

            OrderedDictionary weatherData = await Settings.getWeatherData();
            int index = weatherData.Keys.Count;

            // Check if location already exists
            if (weatherData.Contains(selected_query))
            {
                ShowAddLocationsPanel(false);
                return;
            }

            Weather weather = await WeatherLoaderTask.getWeather(selected_query);

            if (weather == null)
                return;

            // Save coords to List
            weatherData.Add(selected_query, weather);

            // Save data
            Settings.saveWeatherData();

            LocationPanelView panelView = new LocationPanelView(weather);
            // Save index to tag (to easily retreive)
            panelView.Pair = new KeyValuePair<int, string>(index, selected_query);

            // Set properties if necessary
            if (EditMode)
                panelView.EditMode = true;

            // Add to collection
            LocationPanels.Add(panelView);

            // Hide add locations panel
            ShowAddLocationsPanel(false);

            sender.IsSuggestionListOpen = false;
        }

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

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleEditMode();
        }

        private void ToggleEditMode()
        {
            // Toggle EditMode
            EditMode = !EditMode;

            EditButton.Icon = new SymbolIcon(EditMode ? Symbol.Accept : Symbol.Edit);
            EditButton.Label = EditMode ? "Done" : "Edit";
            LocationsPanel.IsItemClickEnabled = !EditMode;

            foreach (LocationPanelView view in LocationPanels)
            {
                view.EditMode = EditMode;
            }

            if (!EditMode && DataChanged) Settings.saveWeatherData();
            DataChanged = false;
        }

        private async void LocationPanel_DeleteClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = sender as FrameworkElement;
            if (button == null || (button != null && button.DataContext == null))
                return;

            LocationPanelView view = button.DataContext as LocationPanelView;
            KeyValuePair<int, string> pair = view.Pair;
            int idx = pair.Key;

            // Remove location from list
            OrderedDictionary weatherData = await Settings.getWeatherData();
            weatherData.RemoveAt(idx);

            // Remove panel
            LocationPanels.RemoveAt(idx);

            if (idx == App.HomeIdx)
                LocationPanels[App.HomeIdx].IsHome = true;
        }

        private void HomeBox_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement box = sender as FrameworkElement;
            if (box == null || (box != null && box.DataContext == null))
                return;

            LocationPanelView view = box.DataContext as LocationPanelView;
            int index = view.Pair.Key;

            if (index == App.HomeIdx)
                return;

            foreach(LocationPanelView panelView in LocationPanels)
            {
                int panelIndex = LocationPanels.IndexOf(panelView);

                if (panelIndex == index)
                    panelView.IsHome = true;
                else
                    panelView.IsHome = false;
            }

            MoveData(view, index, App.HomeIdx);
        }

        private async void MoveData(LocationPanelView view, int fromIdx, int toIdx)
        {
            OrderedDictionary data = await Settings.getWeatherData();

            Weather weather = data[fromIdx] as Weather;
            data.RemoveAt(fromIdx);
            data.Insert(toIdx, view.Pair.Value, weather);
            
            // Only move panels if we haven't already
            if (view.Pair.Key != toIdx)
                LocationPanels.Move(fromIdx, toIdx);

            // Flag that home location has changed
            if (fromIdx == App.HomeIdx || toIdx == App.HomeIdx)
            {
                if (CoreApplication.Properties.ContainsKey("HomeChanged"))
                    CoreApplication.Properties["HomeChanged"] = true;
                else
                    CoreApplication.Properties.Add("HomeChanged", true);
            }
        }

        private void LocationsPanel_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            if (!EditMode) ToggleEditMode();
        }

        private async void LocationsPanel_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            LocationPanelView panel = args.Items.First() as LocationPanelView;

            if (panel == null)
                return;

            List<string> data = await Settings.getLocations();
            int newIndex = panel.Pair.Key;
            int oldIndex = data.IndexOf(panel.Pair.Value);

            MoveData(panel, oldIndex, newIndex);

            // Reset home if necessary
            if (oldIndex == App.HomeIdx || newIndex == App.HomeIdx)
            {
                foreach (LocationPanelView panelView in LocationPanels)
                {
                    int panelIndex = LocationPanels.IndexOf(panelView);

                    if (panelIndex == App.HomeIdx)
                        panelView.IsHome = true;
                    else
                        panelView.IsHome = false;
                }
            }
        }

        private void LocationPanel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                if (!EditMode) ToggleEditMode();
                e.Handled = true;
            }
        }
    }
}