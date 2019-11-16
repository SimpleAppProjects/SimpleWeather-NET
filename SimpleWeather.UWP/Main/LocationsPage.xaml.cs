using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Tiles;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationsPage : CustomPage, IWeatherLoadedListener, IWeatherErrorListener
    {
        private WeatherManager wm;

        public ObservableCollection<LocationPanelViewModel> GPSPanelViewModel { get; set; }
        public ObservableCollection<LocationPanelViewModel> LocationPanels { get; set; }
        private Geolocator geolocal = null;

        public bool EditMode { get; set; } = false;
        private bool DataChanged = false;
        private bool HomeChanged = false;
        private bool[] ErrorCounter;

        private AppBarButton EditButton;

        public async void OnWeatherLoaded(LocationData location, Weather weather)
        {
            await AsyncTask.RunOnUIThread(() =>
            {
                if (weather?.IsValid() == true)
                {
                    if (Settings.FollowGPS && location.locationType == LocationType.GPS)
                    {
                        GPSPanelViewModel.First()?.SetWeather(weather);
                    }
                    else
                    {
                        var panelView = LocationPanels.FirstOrDefault(panelVM =>
                            (bool)!panelVM.LocationData?.locationType.Equals(LocationType.GPS)
                                && (bool)panelVM.LocationData?.query?.Equals(location.query));

                        // Just in case
                        if (panelView == null)
                        {
                            panelView = LocationPanels.FirstOrDefault(panelVM => panelVM.LocationData.name.Equals(location.name) &&
                                                            panelVM.LocationData.latitude.Equals(location.latitude) &&
                                                            panelVM.LocationData.longitude.Equals(location.longitude) &&
                                                            panelVM.LocationData.tz_long.Equals(location.tz_long));
                        }
                        panelView?.SetWeather(weather);
                    }
                }
            });
        }

        public async void OnWeatherError(WeatherException wEx)
        {
            await AsyncTask.RunOnUIThread(() =>
            {
                switch (wEx.ErrorStatus)
                {
                    case WeatherUtils.ErrorStatus.NetworkError:
                    case WeatherUtils.ErrorStatus.NoWeather:
                        // Show error message and prompt to refresh
                        // Only warn once
                        if (!ErrorCounter[(int)wEx.ErrorStatus])
                        {
                            Snackbar snackbar = Snackbar.Make(wEx.Message, SnackbarDuration.Short);
                            snackbar.SetAction(App.ResLoader.GetString("Action_Retry"), () =>
                            {
                                // Reset counter to allow retry
                                ErrorCounter[(int)wEx.ErrorStatus] = false;
                                RefreshLocations();
                            });
                            ShowSnackbar(snackbar);
                            ErrorCounter[(int)wEx.ErrorStatus] = true;
                        }
                        break;

                    case WeatherUtils.ErrorStatus.QueryNotFound:
                        if (!ErrorCounter[(int)wEx.ErrorStatus] && WeatherAPI.NWS.Equals(Settings.API))
                        {
                            ShowSnackbar(Snackbar.Make(App.ResLoader.GetString("Error_WeatherUSOnly"), SnackbarDuration.Short));
                            ErrorCounter[(int)wEx.ErrorStatus] = true;
                        }
                        break;

                    default:
                        // Show error message
                        // Only warn once
                        if (!ErrorCounter[(int)wEx.ErrorStatus])
                        {
                            ShowSnackbar(Snackbar.Make(wEx.Message, SnackbarDuration.Short));
                            ErrorCounter[(int)wEx.ErrorStatus] = true;
                        }
                        break;
                }
            });
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

            ErrorCounter = new bool[Enum.GetValues(typeof(WeatherUtils.ErrorStatus)).Length];

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

        private void LocationsPage_Resuming(object sender, object e)
        {
            RefreshLocations();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
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
                        RefreshLocations();
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

        private async void LocationPanels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool dataMoved = (e.Action == NotifyCollectionChangedAction.Remove) || (e.Action == NotifyCollectionChangedAction.Move);
            bool onlyHomeIsLeft = await AsyncTask.RunOnUIThread(() => (LocationPanels.Count == 1));
            bool limitReached = await AsyncTask.RunOnUIThread(() => (LocationPanels.Count >= Settings.MAX_LOCATIONS));

            // Flag that data has changed
            if (EditMode && dataMoved)
                DataChanged = true;

            if (EditMode && e.NewStartingIndex == App.HomeIdx)
                HomeChanged = true;

            // Cancel edit Mode
            if (EditMode && onlyHomeIsLeft)
                ToggleEditMode();

            // Disable EditMode if only single location
            await AsyncTask.RunOnUIThread(() =>
            {
                EditButton.Visibility = onlyHomeIsLeft ? Visibility.Collapsed : Visibility.Visible;
                AddLocationsButton.Visibility = limitReached ? Visibility.Collapsed : Visibility.Visible;
            });
        }

        private void LoadLocations()
        {
            AsyncTask.Run(async () =>
            {
                AsyncTask.RunOnUIThread(() =>
                {
                    // Disable EditMode button
                    EditButton.IsEnabled = false;
                });

                // Lets load it up...
                var locations = await AsyncTask.RunAsync(async () => await Settings.GetFavorites());
                await AsyncTask.RunOnUIThread(() =>
                {
                    LocationPanels.Clear();
                });

                // Setup saved favorite locations
                AsyncTask.Run(async () => await LoadGPSPanel());
                foreach (LocationData location in locations)
                {
                    var panel = new LocationPanelViewModel()
                    {
                        // Save index to tag (to easily retreive)
                        LocationData = location
                    };
                    AsyncTask.RunOnUIThread(() => LocationPanels.Add(panel));
                }

                foreach (LocationData location in locations)
                {
                    AsyncTask.Run(() =>
                    {
                        var wLoader = new WeatherDataLoader(location, this, this);
                        wLoader.LoadWeatherData(false);
                    });
                }

                AsyncTask.RunOnUIThread(() =>
                {
                    // Enable EditMode button
                    EditButton.IsEnabled = true;
                });
            });
        }

        private async Task LoadGPSPanel()
        {
            await AsyncTask.RunAsync(async () =>
            {
                if (Settings.FollowGPS)
                {
                    AsyncTask.RunOnUIThread(() =>
                    {
                        GPSLocationsPanel.Visibility = Visibility.Visible;
                    });
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
                        AsyncTask.RunOnUIThread(() =>
                        {
                            GPSPanelViewModel[0] = panel;

                            AsyncTask.Run(() =>
                            {
                                var wLoader = new WeatherDataLoader(locData, this, this);
                                wLoader.LoadWeatherData(false);
                            });
                        });
                    }
                }
            });
        }

        private void RefreshLocations()
        {
            AsyncTask.Run(async () =>
            {
                AsyncTask.RunOnUIThread(() =>
                {
                    // Disable EditMode button
                    EditButton.IsEnabled = false;
                });

                // Reload all panels if needed
                var locations = await Settings.GetLocationData();
                var homeData = await Settings.GetLastGPSLocData();
                var gpsPanelViewModel = GPSPanelViewModel.FirstOrDefault();
                bool reload = (locations.Count != LocationPanels.Count ||
                    (Settings.FollowGPS && (gpsPanelViewModel == null) || (!Settings.FollowGPS && gpsPanelViewModel != null)));

                // Reload if weather source differs
                if ((gpsPanelViewModel != null && !Settings.API.Equals(gpsPanelViewModel?.WeatherSource)) ||
                    (LocationPanels.Count >= 1 && !Settings.API.Equals(LocationPanels[0].WeatherSource)))
                {
                    reload = true;
                }

                // Reload if panel queries dont match
                if (!reload && (gpsPanelViewModel != null && !homeData.query.Equals(gpsPanelViewModel?.LocationData.query)))
                    reload = true;

                if (reload)
                {
                    await AsyncTask.RunOnUIThread(() =>
                    {
                        LocationPanels.Clear();
                    });
                    LoadLocations();
                }
                else
                {
                    var dataset = LocationPanels.ToList();
                    if (gpsPanelViewModel != null)
                        dataset.Add(gpsPanelViewModel);

                    foreach (var view in dataset)
                    {
                        var locData = view.LocationData;

                        AsyncTask.Run(() =>
                        {
                            var wLoader = new WeatherDataLoader(locData, this, this);
                            wLoader.LoadWeatherData(false);
                        });
                    }
                }

                AsyncTask.RunOnUIThread(() =>
                {
                    // Enable EditMode button
                    EditButton.IsEnabled = true;
                });
            });
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
                    LocationQueryViewModel view = await Task.Run(async () =>
                    {
                        LocationQueryViewModel locView = null;

                        try
                        {
                            locView = await wm.GetLocation(newGeoPos);

                            if (String.IsNullOrEmpty(locView.LocationQuery))
                                locView = new LocationQueryViewModel();
                        }
                        catch (WeatherException)
                        {
                            locView = new LocationQueryViewModel();
                        }

                        return locView;
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

        private async void SwipeItem_Invoked(Microsoft.UI.Xaml.Controls.SwipeItem sender, Microsoft.UI.Xaml.Controls.SwipeItemInvokedEventArgs args)
        {
            if (args.SwipeControl is FrameworkElement button && button.DataContext is LocationPanelViewModel view)
            {
                await RemovePanel(view);
            }
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            await BatchRemovePanels(LocationsPanel.SelectedItems.Cast<LocationPanelViewModel>());
        }

        private void DeletePanel(LocationPanelViewModel view)
        {
            LocationData data = view.LocationData;

            Task.Run(async () =>
            {
                if (view != null)
                {
                    // Remove location from list
                    await Settings.DeleteLocation(data.query);

                    // Remove secondary tile if it exists
                    if (SecondaryTileUtils.Exists(data.query))
                    {
                        await new SecondaryTile(
                            SecondaryTileUtils.GetTileId(data.query)).RequestDeleteAsync();
                    }
                }
            });
        }

        private async Task RemovePanel(LocationPanelViewModel panel)
        {
            await AsyncTask.RunOnUIThread(() =>
            {
                int dataPosition = LocationPanels.IndexOf(panel);

                // Create actions
                Action UndoAction = delegate
                {
                    if (!LocationPanels.Contains(panel))
                    {
                        if (dataPosition >= LocationPanels.Count)
                        {
                            LocationPanels.Add(panel);
                        }
                        else
                        {
                            LocationPanels.Insert(dataPosition, panel);
                        }
                    }
                };

                LocationPanels.Remove(panel);

                if (LocationPanels.Count <= 0)
                {
                    UndoAction.Invoke();
                    ShowSnackbar(Snackbar.Make(App.ResLoader.GetString("Message_NeedFavorite"), SnackbarDuration.Short));
                    return;
                }

                // Show undo snackbar
                Snackbar snackbar = Snackbar.Make(App.ResLoader.GetString("Message_LocationRemoved"), SnackbarDuration.Short);
                snackbar.SetAction(App.ResLoader.GetString("Action_Undo"), () =>
                {
                    //panel = null;
                    UndoAction.Invoke();
                });
                snackbar.Dismissed = (sender, @event) =>
                {
                    if (@event != SnackbarDismissEvent.Action)
                    {
                        DeletePanel(panel);
                    }
                };
                ShowSnackbar(snackbar);
            });
        }

        private async Task BatchRemovePanels(IEnumerable<LocationPanelViewModel> panelsToDelete)
        {
            await AsyncTask.RunOnUIThread(() =>
            {
                var panelPairs = new List<KeyValuePair<int, LocationPanelViewModel>>();
                foreach (LocationPanelViewModel panel in panelsToDelete)
                {
                    int dataPosition = LocationPanels.IndexOf(panel);
                    panelPairs.Add(new KeyValuePair<int, LocationPanelViewModel>(dataPosition, panel));
                }

                // Create actions
                Action UndoAction = delegate
                {
                    foreach (var panelPair in panelPairs)
                    {
                        if (!LocationPanels.Contains(panelPair.Value))
                        {
                            if (panelPair.Key >= LocationPanels.Count)
                            {
                                LocationPanels.Add(panelPair.Value);
                            }
                            else
                            {
                                LocationPanels.Insert(panelPair.Key, panelPair.Value);
                            }
                        }
                    }
                };

                foreach (var panelPair in panelPairs)
                {
                    LocationPanels.Remove(panelPair.Value);
                }

                if (LocationPanels.Count <= 0)
                {
                    UndoAction.Invoke();
                    ShowSnackbar(Snackbar.Make(App.ResLoader.GetString("Message_NeedFavorite"), SnackbarDuration.Short));
                    return;
                }

                // Show undo snackbar
                Snackbar snackbar = Snackbar.Make(App.ResLoader.GetString("Message_LocationRemoved"), SnackbarDuration.Short);
                snackbar.SetAction(App.ResLoader.GetString("Action_Undo"), () =>
                {
                    //panel = null;
                    UndoAction.Invoke();
                });
                snackbar.Dismissed = (sender, @event) =>
                {
                    if (@event != SnackbarDismissEvent.Action)
                    {
                        foreach (var panelPair in panelPairs)
                        {
                            DeletePanel(panelPair.Value);
                        }
                    }
                };
                ShowSnackbar(snackbar);
            });
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
            if (!(args.Items[0] is LocationPanelViewModel panel))
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