using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.ComponentModel;
using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.ViewModels;
using SimpleWeather.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationsPage : Page, ICommandBarPage, ISnackbarPage, IDisposable
    {
        public String CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        private LocationsViewModel LocationsViewModel { get; } = Ioc.Default.GetViewModel<LocationsViewModel>();
        private LocationPanelAdapter PanelAdapter { get; set; }

        public bool EditMode { get; set; } = false;
        private bool HomeChanged = false;
        private CancellationTokenSource cts;

        private AppBarButton EditButton;

        public void ShowSnackbar(Snackbar snackbar)
        {
            Shell.Instance?.ShowSnackbar(snackbar);
        }

        public void Dispose()
        {
            cts?.Dispose();
        }

        public LocationsPage()
        {
            this.InitializeComponent();

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                NavigationCacheMode = NavigationCacheMode.Disabled;
            else
                NavigationCacheMode = NavigationCacheMode.Required;

            Application.Current.Resuming += LocationsPage_Resuming;

            PanelAdapter = new LocationPanelAdapter(LocationsPanel);
            PanelAdapter.ListChanged += LocationPanels_CollectionChanged;

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("label_nav_locations");
            PrimaryCommands = new List<ICommandBarElement>()
            {
                new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Edit),
                    Label = App.ResLoader.GetString("action_editmode"),
                }
            };
            EditButton = PrimaryCommands[0] as AppBarButton;
            EditButton.Tapped += AppBarButton_Click;
            EditButton.Visibility = PanelAdapter.FavoritesCount > 1 ? Visibility.Visible : Visibility.Collapsed;

            cts = new CancellationTokenSource();

            AnalyticsLogger.LogEvent("LocationsPage");
        }

        private void LocationsPage_Resuming(object sender, object e)
        {
            AnalyticsLogger.LogEvent("LocationsPage: LocationsPage_Resuming");
            //LocationsViewModel.RefreshLocations();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnalyticsLogger.LogEvent("LocationsPage: OnNavigatedTo");

            cts = new CancellationTokenSource();

            if (e.NavigationMode == NavigationMode.Back || e.NavigationMode == NavigationMode.New)
            {
                // Remove all from backstack except home
                if (this.Frame.BackStackDepth >= 1)
                {
                    try
                    {
                        var home = this.Frame.BackStack.ElementAt(0);
                        this.Frame.BackStack.Clear();
                        this.Frame.BackStack.Add(home);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(LoggerLevel.Error, ex, "Exception!!");
                    }
                }
            }

            LocationsViewModel.PropertyChanged += LocationsViewModel_PropertyChanged;
            LocationsViewModel.WeatherUpdated += LocationsViewModel_WeatherUpdated;
            LocationsViewModel.LoadLocations();
        }

        private void LocationsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(LocationsViewModel.Locations):
                    PanelAdapter.ReplaceAll(LocationsViewModel.Locations);
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
                await Task.Run(e.Location.UpdateBackground, cts.Token);
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

            cts?.Cancel();
        }

        private void OnErrorMessage(ErrorMessage error)
        {
            Dispatcher.RunOnUIThread(() =>
            {
                switch (error)
                {
                    case ErrorMessage.Resource err:
                        {
                            ShowSnackbar(Snackbar.MakeError(App.ResLoader.GetString(err.ResourceId), SnackbarDuration.Short));
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
                    snackbar.SetAction(App.ResLoader.GetString("action_retry"), () =>
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
            double StackWidth = LocationsPanel.ActualWidth;
            LocationsPanel.Height = this.ActualHeight;

            if (StackWidth <= 0)
                return;

            if (LocationsPanel.ItemsPanelRoot is ItemsWrapGrid WrapsGrid)
            {
                if (StackWidth >= 1280)
                {
                    WrapsGrid.ItemWidth = (StackWidth - WrapsGrid.Margin.Left - WrapsGrid.Margin.Right) / 4;
                }
                else if (StackWidth >= 1007)
                {
                    WrapsGrid.ItemWidth = (StackWidth - WrapsGrid.Margin.Left - WrapsGrid.Margin.Right) / 3;
                }
                else if (StackWidth >= 640)
                {
                    WrapsGrid.ItemWidth = (StackWidth - WrapsGrid.Margin.Left - WrapsGrid.Margin.Right) / 2;
                }
                else
                {
                    WrapsGrid.ItemWidth = Double.NaN;
                }
            }
        }

        private void LocationPanels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool dataMoved = (e.Action == NotifyCollectionChangedAction.Remove) || (e.Action == NotifyCollectionChangedAction.Move);
            bool onlyHomeIsLeft = PanelAdapter.FavoritesCount <= 1;
            bool limitReached = PanelAdapter.ItemCount >= Settings.MAX_LOCATIONS;

            if (EditMode && e.NewStartingIndex == App.HomeIdx)
                HomeChanged = true;

            // Cancel edit Mode
            if (EditMode && onlyHomeIsLeft)
                ToggleEditMode();

            // Disable EditMode if only single location
            EditButton.Visibility = onlyHomeIsLeft ? Visibility.Collapsed : Visibility.Visible;
            AddLocationsButton.Visibility = limitReached ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void LocationsPanel_ItemClick(object sender, ItemClickEventArgs e)
        {
            AnalyticsLogger.LogEvent("LocationsPage: LocationsPanel_ItemClick");
            if (e.ClickedItem is LocationPanelUiModel panel)
            {
                try
                {
                    this.Frame.Navigate(typeof(WeatherNow), new WeatherNowArgs()
                    {
                        IsHome = Equals(panel.LocationData, await Settings.GetHomeData()),
                        Location = panel.LocationData
                    });
                    // Remove all from backstack except home
                    this.Frame.BackStack.Clear();
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
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
            EditButton.Label = EditMode ? App.ResLoader.GetString("Label_Done") : App.ResLoader.GetString("action_editmode");
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
                            Label = App.ResLoader.GetString("delete"),
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
            }

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
            }

            if (!EditMode && HomeChanged)
            {
                SharedModule.Instance.RequestAction(CommonActions.ACTION_WEATHER_SENDLOCATIONUPDATE,
                    new Dictionary<string, object>
                    {
                        { CommonActions.EXTRA_FORCEUPDATE, false }
                    });
            }

            HomeChanged = false;
        }

        private async void SwipeItem_Invoked(Microsoft.UI.Xaml.Controls.SwipeItem sender, Microsoft.UI.Xaml.Controls.SwipeItemInvokedEventArgs args)
        {
            if (args.SwipeControl is FrameworkElement button && button.DataContext is LocationPanelUiModel view)
            {
                AnalyticsLogger.LogEvent("LocationsPage: SwipeItem_Invoked");
                await PanelAdapter.RemovePanel(view).ConfigureAwait(true);
            }
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("LocationsPage: DeleteBtn_Click");
            await PanelAdapter.BatchRemovePanels(LocationsPanel.SelectedItems.Cast<LocationPanelUiModel>()).ConfigureAwait(true);
        }

        private void LocationPanel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == HoldingState.Started)
            {
                if (!EditMode) ToggleEditMode();
                e.Handled = true;
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
                }

                if (container != null)
                {
                    container.IsEnabled = view.LocationType != (int)LocationType.GPS || !EditMode;
                    container.IsHitTestVisible = view.LocationType != (int)LocationType.GPS || !EditMode;
                }
            }
        }
    }
}