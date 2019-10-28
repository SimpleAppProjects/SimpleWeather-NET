using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
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
using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationsPage : Page, ICommandBarPage, IWeatherLoadedListener, IWeatherErrorListener
    {
        private WeatherManager wm;

        public ObservableCollection<LocationPanelViewModel> GPSPanelViewModel { get; set; }
        public ObservableCollection<LocationPanelViewModel> LocationPanels { get; set; }
        Geolocator geolocal = null;

        public bool EditMode { get; set; } = false;
        private bool DataChanged = false;
        private bool HomeChanged = false;
        private bool[] ErrorCounter;

        public string CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }
        private AppBarButton EditButton;

        public void OnWeatherLoaded(LocationData location, Weather weather)
        {
            if (weather?.IsValid() == true)
            {
                if (Settings.FollowGPS && location.locationType == LocationType.GPS)
                {
                    GPSPanelViewModel.First()?.SetWeather(weather);
                }
                else
                {
                    var panelView = LocationPanels.First(panelVM =>
                        (bool)!panelVM.LocationData?.locationType.Equals(LocationType.GPS)
                            && (bool)panelVM.LocationData?.query?.Equals(location.query));
                    // Just in case
                    if (panelView == null)
                    {
                        panelView = LocationPanels.First(panelVM => panelVM.LocationData.name.Equals(location.name) &&
                                                        panelVM.LocationData.latitude.Equals(location.latitude) &&
                                                        panelVM.LocationData.longitude.Equals(location.longitude) &&
                                                        panelVM.LocationData.tz_long.Equals(location.tz_long));
                    }
                    panelView?.SetWeather(weather);
                }
            }
        }

        public void OnWeatherError(WeatherException wEx)
        {
            switch (wEx.ErrorStatus)
            {
                case WeatherUtils.ErrorStatus.NetworkError:
                case WeatherUtils.ErrorStatus.NoWeather:
                    // Show error message and prompt to refresh
                    // Only warn once
                    if (!ErrorCounter[(int)wEx.ErrorStatus])
                    {
                        Snackbar snackBar = Snackbar.Make(Content as Grid, wEx.Message, SnackbarDuration.Short);
                        snackBar.SetAction(App.ResLoader.GetString("Action_Retry"), async () =>
                        {
                            // Reset counter to allow retry
                            ErrorCounter[(int)wEx.ErrorStatus] = false;
                            await RefreshLocations();
                        });
                        snackBar.Show();
                        ErrorCounter[(int)wEx.ErrorStatus] = true;
                    }
                    break;
                default:
                    // Show error message
                    // Only warn once
                    if (!ErrorCounter[(int)wEx.ErrorStatus])
                    {
                        Snackbar.Make(Content as Grid, wEx.Message, SnackbarDuration.Short).Show();
                        ErrorCounter[(int)wEx.ErrorStatus] = true;
                    }
                    break;
            }
        }

        public LocationsPage()
        {
            this.InitializeComponent();

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                NavigationCacheMode = NavigationCacheMode.Disabled;
            else
                NavigationCacheMode = NavigationCacheMode.Required;

            Application.Current.Resuming += LocationsPage_Resuming;
            LocationsPanel.SizeChanged += StackControl_SizeChanged;

            wm = WeatherManager.GetInstance();

            GPSPanelViewModel = new ObservableCollection<LocationPanelViewModel>() { null };
            LocationPanels = new ObservableCollection<LocationPanelViewModel>();
            LocationPanels.CollectionChanged += LocationPanels_CollectionChanged;

            int max = Enum.GetValues(typeof(WeatherUtils.ErrorStatus)).Cast<int>().Max();
            ErrorCounter = new bool[max];

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("Nav_Locations/Label");
            PrimaryCommands = new List<ICommandBarElement>()
            {
                new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Edit),
                    Label = App.ResLoader.GetString("Label_Edit"),
                }
            };
            EditButton = PrimaryCommands.First() as AppBarButton;
            EditButton.Click += AppBarButton_Click;
        }

        private async void LocationsPage_Resuming(object sender, object e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await RefreshLocations());
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Back || e.NavigationMode == NavigationMode.New)
            {
                // Remove all from backstack except home
                if (this.Frame.BackStackDepth >= 1)
                {
                    var home = this.Frame.BackStack.ElementAt(0);
                    this.Frame.BackStack.Clear();
                    this.Frame.BackStack.Add(home);
                }
            }

            if (e.Parameter != null)
            {
                string arg = e.Parameter.ToString();

                switch (arg)
                {
                    case "toast-refresh":
                        await RefreshLocations();
                        return;
                    default:
                        break;
                }
            }

            if (Settings.FollowGPS)
            {
                GPSLocationsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                GPSPanelViewModel[0] = null;
                GPSLocationsPanel.Visibility = Visibility.Collapsed;
            }

            bool reload = (!Settings.FollowGPS && LocationPanels.Count == 0)
                || (Settings.FollowGPS && GPSPanelViewModel.First() == null);

            if (reload)
            {
                // New instance; Get locations and load up weather data
                await LoadLocations();
            }
            else
            {
                // Refresh view
                await RefreshLocations();
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // Cancel edit mode if moving away
            if (EditMode)
                ToggleEditMode();

            // Reset error counter
            Array.Clear(ErrorCounter, 0, ErrorCounter.Length);
        }

        private void StackControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Resize StackControl items
            double StackWidth = LocationsPanel.ActualWidth;

            if (StackWidth <= 0)
                return;

            if (LocationsPanel.ItemsPanelRoot is ItemsWrapGrid WrapsGrid)
            {
                if (StackWidth >= 1007)
                {
                    WrapsGrid.ItemWidth = (StackWidth - WrapsGrid.Margin.Left - WrapsGrid.Margin.Right) / 2;
                }
                else
                {
                    WrapsGrid.ItemWidth = Double.NaN;
                }
            }
            if (GPSLocationsPanel.ItemsPanelRoot is ItemsWrapGrid GPSWrapsGrid)
            {
                GPSWrapsGrid.ItemWidth = StackWidth - GPSWrapsGrid.Margin.Left - GPSWrapsGrid.Margin.Right;
            }
        }

        private void LocationPanels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool dataMoved = (e.Action == NotifyCollectionChangedAction.Remove) || (e.Action == NotifyCollectionChangedAction.Move);
            bool onlyHomeIsLeft = (LocationPanels.Count == 1);
            bool limitReached = (LocationPanels.Count >= Settings.MAX_LOCATIONS);

            // Flag that data has changed
            if (EditMode && dataMoved)
                DataChanged = true;

            if (EditMode && e.NewStartingIndex == App.HomeIdx)
                HomeChanged = true;

            // Cancel edit Mode
            if (EditMode && onlyHomeIsLeft)
                ToggleEditMode();

            // Disable EditMode if only single location
            EditButton.Visibility = onlyHomeIsLeft ? Visibility.Collapsed : Visibility.Visible;
            AddLocationsButton.Visibility = limitReached ? Visibility.Collapsed : Visibility.Visible;
        }

        private async Task LoadLocations()
        {
            // Disable EditMode button
            EditButton.IsEnabled = false;

            // Lets load it up...
            var locations = await Settings.GetFavorites();
            LocationPanels.Clear();

            // Setup saved favorite locations
            await LoadGPSPanel();
            foreach (LocationData location in locations)
            {
                var panel = new LocationPanelViewModel()
                {
                    // Save index to tag (to easily retreive)
                    LocationData = location
                };

                LocationPanels.Add(panel);
            }

            foreach (LocationData location in locations)
            {
                var wLoader = new WeatherDataLoader(location, this, this);
                await wLoader.LoadWeatherData(false);
            }

            // Enable EditMode button
            EditButton.IsEnabled = true;
        }

        private async Task LoadGPSPanel()
        {
            if (Settings.FollowGPS)
            {
                GPSLocationsPanel.Visibility = Visibility.Visible;
                var locData = await Settings.GetLastGPSLocData();

                if (locData == null || locData.query == null)
                {
                    locData = await UpdateLocation();
                }

                if (locData != null && locData.query != null)
                {
                    var panel = new LocationPanelViewModel()
                    {
                        LocationData = locData
                    };
                    GPSPanelViewModel[0] = panel;

                    var wLoader = new WeatherDataLoader(locData, this, this);
                    await wLoader.LoadWeatherData(false);
                }
            }
        }

        private async Task RefreshLocations()
        {
            // Disable EditMode button
            EditButton.IsEnabled = false;

            // Reload all panels if needed
            var locations = await Settings.GetLocationData();
            var homeData = await Settings.GetLastGPSLocData();
            bool reload = (locations.Count != LocationPanels.Count ||
                (Settings.FollowGPS && (GPSPanelViewModel.First() == null) || (!Settings.FollowGPS && GPSPanelViewModel.First() != null)));

            // Reload if weather source differs
            if ((GPSPanelViewModel.First() != null && !Settings.API.Equals(GPSPanelViewModel.First().WeatherSource)) ||
                (LocationPanels.Count >= 1 && !Settings.API.Equals(LocationPanels[0].WeatherSource)))
            {
                reload = true;
            }

            // Reload if panel queries dont match
            if (!reload && (GPSPanelViewModel.First() != null && !homeData.query.Equals(GPSPanelViewModel.First().LocationData.query)))
                reload = true;

            if (reload)
            {
                LocationPanels.Clear();
                await LoadLocations();
            }
            else
            {
                var dataset = LocationPanels.ToList();
                if (GPSPanelViewModel.First() != null)
                    dataset.Add(GPSPanelViewModel.First());

                foreach (LocationPanelViewModel view in dataset)
                {
                    var wLoader = new WeatherDataLoader(view.LocationData, this, this);
                    await wLoader.LoadWeatherData(false);
                }
            }

            // Enable EditMode button
            EditButton.IsEnabled = true;
        }

        private async Task<LocationData> UpdateLocation()
        {
            LocationData locationData = null;

            if (Settings.FollowGPS)
            {
                Geoposition newGeoPos = null;

                try
                {
                    newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10));
                }
                catch (Exception)
                {
                    var geoStatus = GeolocationAccessStatus.Unspecified;

                    try
                    {
                        geoStatus = await Geolocator.RequestAccessAsync();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(LoggerLevel.Error, ex, "LocationsPage: error getting location permission");
                    }
                    finally
                    {
                        if (geoStatus == GeolocationAccessStatus.Allowed)
                        {
                            try
                            {
                                newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10));
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLine(LoggerLevel.Error, ex, "LocationsPage: error getting location");
                            }
                        }
                        else if (geoStatus == GeolocationAccessStatus.Denied)
                        {
                            // Disable gps feature
                            Settings.FollowGPS = false;
                            GPSPanelViewModel[0] = null;
                            GPSLocationsPanel.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            GPSPanelViewModel[0] = null;
                            GPSLocationsPanel.Visibility = Visibility.Collapsed;
                        }
                    }

                    if (!Settings.FollowGPS)
                        return null;
                }

                // Access to location granted
                if (newGeoPos != null)
                {
                    LocationQueryViewModel view = null;

                    await Task.Run(async () =>
                    {
                        view = await wm.GetLocation(newGeoPos);

                        if (String.IsNullOrEmpty(view.LocationQuery))
                            view = new LocationQueryViewModel();
                    });

                    if (String.IsNullOrWhiteSpace(view.LocationQuery))
                    {
                        // Stop since there is no valid query
                        GPSPanelViewModel[0] = null;
                        GPSLocationsPanel.Visibility = Visibility.Collapsed;
                    }

                    // Save location as last known
                    locationData = new LocationData(view, newGeoPos);
                }
            }

            return locationData;
        }

        private void LocationsPanel_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is LocationPanelViewModel panel)
            {
                if (panel.LocationData.Equals(Settings.HomeData))
                {
                    // Remove all from backstack except home
                    var home = this.Frame.BackStack.ElementAt(0);
                    this.Frame.BackStack.Clear();
                    this.Frame.BackStack.Add(home);
                    this.Frame.GoBack();
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                }
                else
                    this.Frame.Navigate(typeof(WeatherNow), panel.LocationData);
            }
        }

        private void AddLocationsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LocationSearchPage));
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
            // Enable selection mode for non-Mobile (non-Touch devices)
            if (!ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                LocationsPanel.IsMultiSelectCheckBoxEnabled = EditMode;
                LocationsPanel.SelectionMode = EditMode ? ListViewSelectionMode.Multiple : ListViewSelectionMode.None;
                if (EditMode && PrimaryCommands.Count == 1)
                {
                    PrimaryCommands.Insert(0,
                        new AppBarButton()
                        {
                            Icon = new SymbolIcon(Symbol.Delete),
                            Label = App.ResLoader.GetString("Label_Delete"),
                        }
                    );
                    var deleteBtn = PrimaryCommands[0] as AppBarButton;
                    deleteBtn.Click += DeleteBtn_Click;
                }
                else if (PrimaryCommands.Count > 1)
                {
                    PrimaryCommands.Remove(PrimaryCommands[0]);
                }
                Shell.Instance.RequestCommandBarUpdate();
            }

            foreach (LocationPanelViewModel view in LocationPanels)
            {
                view.EditMode = EditMode;

                if (!EditMode && DataChanged)
                {
                    string query = view.LocationData.query;
                    int pos = LocationPanels.IndexOf(view);
                    Task.Run(() => Settings.MoveLocation(query, pos));
                }
            }

            if (!EditMode && HomeChanged)
            {
                Task.Run(WeatherUpdateBackgroundTask.RequestAppTrigger);
            }

            DataChanged = false;
            HomeChanged = false;
        }

        private void SwipeItem_Invoked(Microsoft.UI.Xaml.Controls.SwipeItem sender, Microsoft.UI.Xaml.Controls.SwipeItemInvokedEventArgs args)
        {
            if (args.SwipeControl is FrameworkElement button && button.DataContext is LocationPanelViewModel view)
            {
                DeleteItem(view);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (LocationPanelViewModel view in LocationsPanel.SelectedItems)
            {
                DeleteItem(view);
            }
        }

        private async void DeleteItem(LocationPanelViewModel view)
        {
            if (view != null)
            {
                LocationData data = view.LocationData;

                // Remove location from list
                await Settings.DeleteLocation(data.query);

                // Remove panel
                LocationPanels.Remove(view);

                // Remove secondary tile if it exists
                if (SecondaryTileUtils.Exists(data.query))
                {
                    await new SecondaryTile(
                        SecondaryTileUtils.GetTileId(data.query)).RequestDeleteAsync();
                }
            }
        }

        private void MoveData(LocationPanelViewModel view, int fromIdx, int toIdx)
        {
            // Only move panels if we haven't already
            if (LocationPanels.IndexOf(view) != toIdx)
                LocationPanels.Move(fromIdx, toIdx);
        }

        private void LocationsPanel_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            if (!EditMode) ToggleEditMode();
        }

        private async void LocationsPanel_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            if (!(args.Items.First() is LocationPanelViewModel panel))
                return;

            var data = await Settings.GetFavorites();
            int newIndex = LocationPanels.IndexOf(panel);
            int oldIndex = data.FindIndex(location => location.query == panel.LocationData.query);

            if (oldIndex != newIndex)
                MoveData(panel, oldIndex, newIndex);

            // Make sure we're still in EditMode after
            if (!EditMode) ToggleEditMode();

            if (oldIndex != newIndex)
                DataChanged = true;
            if (newIndex == App.HomeIdx)
                HomeChanged = true;
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