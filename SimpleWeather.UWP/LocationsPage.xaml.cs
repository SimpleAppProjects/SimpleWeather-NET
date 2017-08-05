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
using Windows.UI.Core;
using Windows.UI.Input;
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
    public sealed partial class LocationsPage : Page, IWeatherLoadedListener, IDisposable
    {
        private CancellationTokenSource cts = new CancellationTokenSource();

        public ObservableCollection<LocationPanelViewModel> GPSPanelViewModel { get; set; }
        public ObservableCollection<LocationPanelViewModel> LocationPanels { get; set; }
        public ObservableCollection<LocationQueryViewModel> LocationQuerys { get; set; }
        private string selected_query = string.Empty;

        public bool EditMode { get; set; } = false;
        private bool DataChanged = false;

        public void OnWeatherLoaded(int locationIdx, Weather weather)
        {
            if (weather != null)
            {
                if (locationIdx == App.HomeIdx && Settings.FollowGPS)
                {
                    GPSPanelViewModel.First().SetWeather(weather);
                }
                else
                {
                    int index = Settings.FollowGPS ? locationIdx - 1 : locationIdx;
                    LocationPanelViewModel panelView = LocationPanels[index];
                    panelView.SetWeather(weather);
                }
            }
        }

        public LocationsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            GPSPanelViewModel = new ObservableCollection<LocationPanelViewModel>() { null };
            LocationPanels = new ObservableCollection<LocationPanelViewModel>();
            LocationPanels.CollectionChanged += LocationPanels_CollectionChanged;

            LocationQuerys = new ObservableCollection<LocationQueryViewModel>();
        }

        public void Dispose()
        {
            ((IDisposable)cts).Dispose();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (Settings.FollowGPS)
                GPSPanel.Visibility = Visibility.Visible;
            else
            {
                GPSPanelViewModel[0] = null;
                GPSPanel.Visibility = Visibility.Collapsed;
            }

            if (!Settings.FollowGPS && LocationPanels.Count == 0 ||
                Settings.FollowGPS && GPSPanelViewModel.First() == null)
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
            foreach (LocationPanelViewModel panelView in LocationPanels)
            {
                int index = LocationPanels.IndexOf(panelView);

                if (Settings.FollowGPS) index++;

                panelView.Pair = new KeyValuePair<int, string>(index, panelView.Pair.Value);
            }

            // Flag that data has changed
            if (EditMode && (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Move))
                DataChanged = true;

            // Cancel edit Mode
            // TODO: cleanup code; it looks ugly
            if (EditMode && (!Settings.FollowGPS && LocationPanels.Count == 1 || Settings.FollowGPS && LocationPanels.Count == 0))
                ToggleEditMode();

            // Disable EditMode if only single location
            // TODO: cleanup code; it looks ugly
            EditButton.Visibility = (!Settings.FollowGPS && LocationPanels.Count == 1 || Settings.FollowGPS && LocationPanels.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void LoadLocations()
        {
            // Lets load it up...
            List<string> locations = await Settings.GetLocations();
            LocationPanels.Clear();

            foreach (string location in locations)
            {
                int index = locations.IndexOf(location);

                LocationPanelViewModel panel = new LocationPanelViewModel()
                {
                    // Save index to tag (to easily retreive)
                    Pair = new KeyValuePair<int, string>(index, location)
                };

                // Set home
                if (index == App.HomeIdx && !Settings.FollowGPS)
                    panel.IsHome = true;
                else
                    panel.IsHome = false;

                if (index == App.HomeIdx && Settings.FollowGPS)
                {
                    panel.IsHome = true;
                    GPSPanelViewModel[0] = panel;
                }
                else
                    LocationPanels.Add(panel);
            }

            WeatherDataLoader wLoader = null;

            foreach (string location in locations)
            {
                int index = locations.IndexOf(location);

                wLoader = new WeatherDataLoader(this, location, index);
                await wLoader.LoadWeatherData(false);
            }
        }

        private async void RefreshLocations()
        {
            // Reload all panels if needed
            List<string> locations = await Settings.GetLocations();
            bool reload = !Settings.FollowGPS && locations.Count != LocationPanels.Count ||
                Settings.FollowGPS && (GPSPanelViewModel.First() == null || locations.Count - 1 != LocationPanels.Count);

            // Reload if weather source differs
            if ((GPSPanelViewModel.First() != null && GPSPanelViewModel.First().WeatherSource != Settings.API) ||
                (LocationPanels.Count >= 1 && LocationPanels[0].WeatherSource != Settings.API))
                reload = true;

            // Reload if panel queries dont match
            if (!reload && (GPSPanelViewModel.First() != null && locations[App.HomeIdx] != GPSPanelViewModel.First().Pair.Value))
                reload = true;

            if (reload)
            {
                LocationPanels.Clear();
                LoadLocations();
            }
            else
            {
                List<LocationPanelViewModel> dataset = LocationPanels.ToList();
                if (GPSPanelViewModel.First() != null)
                    dataset.Add(GPSPanelViewModel.First());

                foreach (LocationPanelViewModel view in dataset)
                {
                    WeatherDataLoader wLoader =
                        new WeatherDataLoader(this, view.Pair.Value, view.Pair.Key);
                    await wLoader.LoadWeatherData(false);
                }
            }
        }

        private void LocationsPanel_ItemClick(object sender, ItemClickEventArgs e)
        {
            LocationPanelViewModel panel = e.ClickedItem as LocationPanelViewModel;

            this.Frame.Navigate(typeof(WeatherNow), panel.Pair);
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

                    var results = await AutoCompleteQuery.GetLocations(query);

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
            if (args.SelectedItem is LocationQueryViewModel theChosenOne)
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
                LocationQueryViewModel theChosenOne = args.ChosenSuggestion as LocationQueryViewModel;

                if (!String.IsNullOrEmpty(theChosenOne.LocationQuery))
                    selected_query = theChosenOne.LocationQuery;
                else
                    selected_query = string.Empty;
            }
            else if (!String.IsNullOrEmpty(args.QueryText))
            {
                // Use args.QueryText to determine what to do.
                LocationQueryViewModel result = (await AutoCompleteQuery.GetLocations(args.QueryText)).First();

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

            // Show loading dialog
            await LoadingDialog.ShowAsync();

            OrderedDictionary weatherData = await Settings.GetWeatherData();
            int index = weatherData.Keys.Count;

            // Check if location already exists
            if (weatherData.Contains(selected_query))
            {
                // Hide dialog
                await LoadingDialog.HideAsync();
                ShowAddLocationsPanel(false);
                return;
            }

            Weather weather = await WeatherLoaderTask.GetWeather(selected_query);

            if (weather == null)
            {
                // Hide dialog
                await LoadingDialog.HideAsync();
                return;
            }

            // Save coords to List
            weatherData.Add(selected_query, weather);

            // Save data
            Settings.SaveWeatherData();

            LocationPanelViewModel panelView = new LocationPanelViewModel(weather)
            {
                // Save index to tag (to easily retreive)
                Pair = new KeyValuePair<int, string>(index, selected_query)
            };

            // Set properties if necessary
            if (EditMode)
            {
                panelView.EditMode = true;
                if (Settings.FollowGPS)
                    panelView.HomeBoxVisibility = Visibility.Collapsed;
            }

            // Add to collection
            LocationPanels.Add(panelView);

            // Hide add locations panel
            await LoadingDialog.HideAsync();
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
            EditButton.Label = EditMode ? App.ResLoader.GetString("Label_Done") : App.ResLoader.GetString("Label_Edit");
            LocationsPanel.IsItemClickEnabled = !EditMode;

            foreach (LocationPanelViewModel view in LocationPanels)
            {
                view.EditMode = EditMode;

                if (Settings.FollowGPS)
                    view.HomeBoxVisibility = Visibility.Collapsed;
            }

            if (!EditMode && DataChanged) Settings.SaveWeatherData();
            DataChanged = false;
        }

        private async void LocationPanel_DeleteClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = sender as FrameworkElement;
            if (button == null || (button != null && button.DataContext == null))
                return;

            LocationPanelViewModel view = button.DataContext as LocationPanelViewModel;
            KeyValuePair<int, string> pair = view.Pair;
            int idx = pair.Key;

            // Remove location from list
            OrderedDictionary weatherData = await Settings.GetWeatherData();
            weatherData.RemoveAt(idx);

            // Remove panel
            LocationPanels.RemoveAt(Settings.FollowGPS ? idx - 1 : idx);

            if (idx == App.HomeIdx && !Settings.FollowGPS)
                LocationPanels[App.HomeIdx].IsHome = true;
        }

        private void HomeBox_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement box = sender as FrameworkElement;
            if (box == null || (box != null && box.DataContext == null))
                return;

            LocationPanelViewModel view = box.DataContext as LocationPanelViewModel;
            int index = view.Pair.Key;

            if (index == App.HomeIdx)
                return;

            foreach (LocationPanelViewModel panelView in LocationPanels)
            {
                int panelIndex = LocationPanels.IndexOf(panelView);

                if (panelIndex == index)
                    panelView.IsHome = true;
                else
                    panelView.IsHome = false;
            }

            MoveData(view, index, App.HomeIdx);
        }

        private async void MoveData(LocationPanelViewModel view, int fromIdx, int toIdx)
        {
            // Move data in both weather dictionary and local dataset
            OrderedDictionary data = await Settings.GetWeatherData();

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
            if (!(args.Items.First() is LocationPanelViewModel panel))
                return;

            List<string> data = await Settings.GetLocations();
            int newIndex = panel.Pair.Key;
            int oldIndex = data.IndexOf(panel.Pair.Value);

            MoveData(panel, oldIndex, newIndex);

            // Reset home if necessary
            if ((oldIndex == App.HomeIdx || newIndex == App.HomeIdx) && !Settings.FollowGPS)
            {
                foreach (LocationPanelViewModel panelView in LocationPanels)
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
            if (e.HoldingState == HoldingState.Started)
            {
                if (!EditMode) ToggleEditMode();
                e.Handled = true;
            }
        }
    }
}