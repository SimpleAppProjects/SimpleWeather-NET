using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SimpleWeather.Common.Utils;
using SimpleWeather.LocationData;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Helpers;
using SimpleWeather.NET.ViewModels;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using System.Collections.Immutable;
using System.Collections.Specialized;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.NET.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationsPage : ViewModelPage, ICommandBarPage, ISnackbarPage
    {
        public String CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        private LocationsViewModel LocationsViewModel { get; set; }
        private LocationPanelAdapter PanelAdapter { get; set; }

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        private readonly DispatcherTimer HoldingTimer = new()
        {
            Interval = TimeSpan.FromSeconds(1.5)
        };
        private readonly List<Action> HoldingListeners = [];
        private bool IsHolding = false;

        private readonly List<object> ItemStack = [];
        private readonly DispatcherTimer DeleteTimer = new() { Interval = TimeSpan.FromMilliseconds(500) };
        private readonly List<Action> RemoveListeners = [];

        public bool EditMode { get; set; } = false;
        private bool DataChanged = false;
        private bool HomeChanged = false;

        private AppBarButton EditButton;

        public void ShowSnackbar(Snackbar snackbar)
        {
            Shell.Instance?.ShowSnackbar(snackbar);
        }

        public LocationsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            PanelAdapter = new LocationPanelAdapter(LocationsPanel, SnackbarContainer);
            PanelAdapter.ListChanged += LocationPanels_CollectionChanged;

            // CommandBar
            CommandBarLabel = App.Current.ResLoader.GetString("label_nav_locations");
            PrimaryCommands = new List<ICommandBarElement>()
            {
                new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Edit),
                    Label = App.Current.ResLoader.GetString("action_editmode"),
                }
            };
            EditButton = PrimaryCommands[0] as AppBarButton;
            EditButton.Tapped += AppBarButton_Click;
            EditButton.Visibility = PanelAdapter.FavoritesCount > 1 ? Visibility.Visible : Visibility.Collapsed;

            HoldingTimer.Tick += (s, e) =>
            {
                HoldingTimer?.Stop();

                if (!EditMode)
                {
                    ToggleEditMode();

                    IsHolding = true;

                    HoldingListeners?.ForEach(l =>
                    {
                        l.Invoke();
                    });
                }
            };

            DeleteTimer.Tick += (s, e) =>
            {
                DeleteTimer?.Stop();

                RemoveListeners?.ForEach(l =>
                {
                    l.Invoke();
                });
            };

            AnalyticsLogger.LogEvent("LocationsPage");
        }

        private void LocationsPage_Resuming(object sender, object e)
        {
            AnalyticsLogger.LogEvent("LocationsPage: LocationsPage_Resuming");
            LocationsViewModel.RefreshLocations();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnalyticsLogger.LogEvent("LocationsPage: OnNavigatedTo");

            LocationsViewModel = this.GetViewModel<LocationsViewModel>();

            LocationsViewModel.PropertyChanged += LocationsViewModel_PropertyChanged;
            LocationsViewModel.WeatherUpdated += LocationsViewModel_WeatherUpdated;
            LocationsViewModel.RefreshLocations();
        }

        private void LocationsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(LocationsViewModel.GPSLocation):
                    PanelAdapter.RemoveGPSPanel();
                    if (LocationsViewModel.GPSLocation != null)
                    {
                        PanelAdapter.Add(LocationsViewModel.GPSLocation);
                    }
                    break;
                case nameof(LocationsViewModel.Locations):
                    {
                        var gpsLocation = LocationsViewModel.Locations?.FirstOrDefault(it => it.LocationType == (int)LocationType.GPS);
                        var locations = gpsLocation == null ? LocationsViewModel.Locations : LocationsViewModel.Locations.Except(Enumerable.Repeat(gpsLocation, 1));

                        PanelAdapter.ReplaceAll(LocationType.Search, locations);
                    }
                    break;
                case nameof(LocationsViewModel.ErrorMessages):
                    {
                        var errorMessages = LocationsViewModel.ErrorMessages;

                        var error = errorMessages.FirstOrDefault();

                        if (error != null)
                        {
                            OnErrorMessage(error);
                        }
                    }
                    break;
            }
        }

        private async void LocationsViewModel_WeatherUpdated(object sender, WeatherUpdatedEventArgs e)
        {
            if (e.Location != null)
            {
                var cToken = GetCancellationToken();
                await Task.Run(e.Location.UpdateBackground, cToken);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            LocationsViewModel.WeatherUpdated -= LocationsViewModel_WeatherUpdated;
            LocationsViewModel.PropertyChanged -= LocationsViewModel_PropertyChanged;

            // Cancel edit mode if moving away
            if (EditMode)
                ToggleEditMode();
        }

        private void OnErrorMessage(ErrorMessage error)
        {
            DispatcherQueue.EnqueueAsync(() =>
            {
                switch (error)
                {
                    case ErrorMessage.Resource err:
                        {
                            ShowSnackbar(Snackbar.MakeError(App.Current.ResLoader.GetString(err.ResourceId), SnackbarDuration.Short));
                        }
                        break;
                    case ErrorMessage.String err:
                        {
                            ShowSnackbar(Snackbar.MakeError(err.Message, SnackbarDuration.Short));
                        }
                        break;
                    case ErrorMessage.WeatherError err:
                        {
                            OnWeatherError(err.Exception);
                        }
                        break;
                }
            });

            LocationsViewModel.SetErrorMessageShown(error);
        }

        private void OnWeatherError(WeatherException wEx)
        {
            switch (wEx.ErrorStatus)
            {
                case WeatherUtils.ErrorStatus.NetworkError:
                case WeatherUtils.ErrorStatus.NoWeather:
                    // Show error message and prompt to refresh
                    Snackbar snackbar = Snackbar.MakeError(wEx.Message, SnackbarDuration.Long);
                    snackbar.SetAction(App.Current.ResLoader.GetString("action_retry"), () =>
                    {
                        LocationsViewModel.RefreshLocations();
                    });
                    ShowSnackbar(snackbar);
                    break;

                case WeatherUtils.ErrorStatus.QueryNotFound:
                    ShowSnackbar(Snackbar.MakeError(wEx.Message, SnackbarDuration.Long));
                    break;

                default:
                    // Show error message
                    ShowSnackbar(Snackbar.MakeError(wEx.Message, SnackbarDuration.Long));
                    break;
            }
        }

        private void StackControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Resize StackControl items
            double StackWidth = (this.Content as FrameworkElement)?.ActualWidth ?? this.ActualWidth;
            //LocationsPanel.Height = this.ActualHeight;

            if (StackWidth <= 0)
                return;

            var panels = ImmutableList.Create(GPSLocationsPanel, LocationsPanel);

            panels.ForEach(p =>
            {
                UpdateItemsGridSize(p, StackWidth);
            });
        }

        private void LocationsPanel_Loaded(object sender, RoutedEventArgs e)
        {
            // Resize StackControl items
            double StackWidth = (this.Content as FrameworkElement)?.ActualWidth ?? this.ActualWidth;
            //LocationsPanel.Height = this.ActualHeight;

            if (StackWidth <= 0)
                return;

            UpdateItemsGridSize(sender as ItemsControl, StackWidth);
        }

        private void UpdateItemsGridSize(ItemsControl ItemsPanel, double StackWidth)
        {
            if (ItemsPanel?.ItemsPanelRoot is ItemsWrapGrid WrapsGrid)
            {
                if (StackWidth >= 1280)
                {
                    WrapsGrid.ItemWidth = (StackWidth - WrapsGrid.Margin.Left - WrapsGrid.Margin.Right) / 4;
                    WrapsGrid.MaximumRowsOrColumns = 4;
                }
                else if (StackWidth >= 1007)
                {
                    WrapsGrid.ItemWidth = (StackWidth - WrapsGrid.Margin.Left - WrapsGrid.Margin.Right) / 3;
                    WrapsGrid.MaximumRowsOrColumns = 3;
                }
                else if (StackWidth >= 640)
                {
                    WrapsGrid.ItemWidth = (StackWidth - WrapsGrid.Margin.Left - WrapsGrid.Margin.Right) / 2;
                    WrapsGrid.MaximumRowsOrColumns = 2;
                }
                else
                {
                    WrapsGrid.ItemWidth = Double.NaN;
                    WrapsGrid.MaximumRowsOrColumns = 1;
                }
            }
        }

        private void LocationPanels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DeleteTimer.Stop();
            RemoveListeners.Clear();

            bool dataMoved = false;

            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems is not null)
            {
                if (ItemStack.Count > 0) ItemStack.Clear();
                ItemStack.AddRange(e.OldItems.Cast<object>());
            }
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
            {
                dataMoved = ItemStack.Any(e.NewItems.Contains);

                if (dataMoved)
                {
                    ItemStack.Clear();
                }
            }

            dataMoved = dataMoved || e.Action == NotifyCollectionChangedAction.Move;

            RemoveListeners.Add(() =>
            {
                bool onlyHomeIsLeft = PanelAdapter.FavoritesCount <= 1;
                bool limitReached = PanelAdapter.ItemCount >= SettingsManager.MAX_LOCATIONS;

                // Flag that data has changed
                if (EditMode && dataMoved)
                    DataChanged = true;

                if (EditMode && e.NewStartingIndex == 0)
                    HomeChanged = true;

                // Cancel edit Mode
                if (EditMode && onlyHomeIsLeft)
                    ToggleEditMode();

                // Disable EditMode if only single location
                EditButton.Visibility = onlyHomeIsLeft ? Visibility.Collapsed : Visibility.Visible;
                AddLocationsButton.Visibility = limitReached ? Visibility.Collapsed : Visibility.Visible;

                if (dataMoved && !EditMode)
                {
                    var dataSet = PanelAdapter.GetDataset(LocationType.Search);

                    dataSet?.ForEach(view =>
                    {
                        if (view.LocationType != (int)LocationType.GPS)
                        {
                            UpdateFavoritesPosition(view);
                        }
                    });
                }
            });

            DeleteTimer.Start();
        }

        private async void LocationsPanel_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (EditMode) return;

            AnalyticsLogger.LogEvent("LocationsPage: LocationsPanel_ItemClick");
            if (e.ClickedItem is LocationPanelUiModel panel)
            {
                try
                {
                    this.Frame.Navigate(typeof(WeatherNow), new WeatherNowArgs()
                    {
                        IsHome = Equals(panel.LocationData, await SettingsManager.GetHomeData()),
                        Location = panel.LocationData
                    });
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "Exception!!");
                }
            }
        }

        private void AddLocationsButton_Click(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("LocationsPage: AddLocationsButton_Click");
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
            EditButton.Label = EditMode ? App.Current.ResLoader.GetString("Label_Done") : App.Current.ResLoader.GetString("action_editmode");
            LocationsPanel.IsItemClickEnabled = !EditMode;

            // Enable selection mode for non-Mobile (non-Touch devices)
            LocationsPanel.IsMultiSelectCheckBoxEnabled = EditMode;
            LocationsPanel.SelectionMode = EditMode ? ListViewSelectionMode.Multiple : ListViewSelectionMode.None;
            if (EditMode && PrimaryCommands.Count == 1)
            {
                PrimaryCommands.Insert(0,
                    new AppBarButton()
                    {
                        Icon = new SymbolIcon(Symbol.Delete),
                        Label = App.Current.ResLoader.GetString("delete"),
                    }
                );
                var deleteBtn = PrimaryCommands[0] as AppBarButton;
                deleteBtn.Tapped += DeleteBtn_Click;
            }
            else if (PrimaryCommands.Count > 1)
            {
                PrimaryCommands.Remove(PrimaryCommands[0]);
            }
            Shell.Instance.RequestCommandBarUpdate();

            // Enable reordering
            LocationsPanel.CanReorderItems = LocationsPanel.CanDragItems = LocationsPanel.AllowDrop = EditMode;

            foreach (LocationPanelUiModel view in PanelAdapter.GetDataset())
            {
                view.EditMode = EditMode;

                var itemContainer = LocationsPanel.ContainerFromItem(view);
                if (itemContainer is SelectorItem container)
                {
                    var presenter = VisualTreeHelperExtensions.FindChild<ListViewItemPresenter>(container);

                    if (presenter != null)
                    {
                        presenter.DisabledOpacity = 1;
                        if (EditMode && view.LocationType == (int)LocationType.GPS)
                        {
                            presenter.CheckBoxBrush = null;
                        }
                        else if (presenter.CheckBoxBrush == null &&
                            App.Current.Resources.TryGetValue("GridViewItemCheckBoxBrush", out object brush))
                        {
                            presenter.CheckBoxBrush = brush as Brush;
                        }
                    }

                    container.IsEnabled = view.LocationType != (int)LocationType.GPS || !EditMode;
                    container.IsHitTestVisible = view.LocationType != (int)LocationType.GPS || !EditMode;
                }

                if (view.LocationType != (int)LocationType.GPS && !EditMode && (DataChanged || HomeChanged))
                {
                    UpdateFavoritesPosition(view);
                }
            }

            if (!EditMode && HomeChanged)
            {
                SharedModule.Instance.RequestAction(CommonActions.ACTION_WEATHER_SENDLOCATIONUPDATE,
                    new Dictionary<string, object>
                    {
                        { CommonActions.EXTRA_FORCEUPDATE, false }
                    });
            }

            DataChanged = false;
            HomeChanged = false;
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("LocationsPage: DeleteBtn_Click");
            await PanelAdapter.BatchRemovePanels(LocationsPanel.SelectedItems.Cast<LocationPanelUiModel>()).ConfigureAwait(true);
        }

        private void LocationPanel_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            HoldingTimer.Stop();
            HoldingListeners.Clear();
            if (!EditMode)
            {
                HoldingListeners.Add(() =>
                {
                    if (sender is FrameworkElement element && element.DataContext is LocationPanelUiModel model && model.LocationType == (int)LocationType.Search)
                    {
                        LocationsPanel.SelectedItem = element.DataContext;
                    }
                });
                HoldingTimer.Start();
            }
        }

        private void LocationPanel_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            HoldingListeners.Clear();
            HoldingTimer.Stop();
            IsHolding = false;
            e.Handled = false;
        }

        private void LocationPanel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == HoldingState.Started)
            {
                if (!EditMode) ToggleEditMode();
                IsHolding = e.Handled = true;
            }
            else
            {
                IsHolding = false;
            }
        }

        private void LocationPanel_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            if (sender is FrameworkElement element && element.DataContext is LocationPanelUiModel model && model.LocationType == (int)LocationType.GPS)
            {
                args.Handled = true;
            }
        }

        private async void DeleteFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is LocationPanelUiModel model && model.LocationType == (int)LocationType.Search)
            {
                await PanelAdapter.RemovePanel(model);
            }
        }

        private void LocationsPanel_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Item is LocationPanelUiModel view)
            {
                var container = args.ItemContainer;
                var presenter = VisualTreeHelperExtensions.FindChild<ListViewItemPresenter>(container);

                if (presenter != null)
                {
                    presenter.DisabledOpacity = 1;
                    if (EditMode && view.LocationType == (int)LocationType.GPS)
                    {
                        presenter.CheckBoxBrush = null;
                    }
                    else if (presenter.CheckBoxBrush == null &&
                        App.Current.Resources.TryGetValue("GridViewItemCheckBoxBrush", out object brush))
                    {
                        presenter.CheckBoxBrush = brush as Brush;
                    }

                    presenter.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                }

                if (container != null)
                {
                    container.IsEnabled = view.LocationType != (int)LocationType.GPS || !EditMode;
                    container.IsHitTestVisible = view.LocationType != (int)LocationType.GPS || !EditMode;
                }
            }
        }

        private void UpdateFavoritesPosition(LocationPanelUiModel view)
        {
            var query = view?.LocationData?.query;
            var dataPosition = PanelAdapter?.GetDataset(LocationType.Search)?.IndexOf(view);
            Task.Run(async () =>
            {
                if (query != null && dataPosition.HasValue)
                {
                    await SettingsManager.MoveLocation(query, dataPosition.Value);
                }
            });
        }
    }
}