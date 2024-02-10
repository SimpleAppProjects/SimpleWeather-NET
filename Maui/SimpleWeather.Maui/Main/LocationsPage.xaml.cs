using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common.Utils;
using SimpleWeather.LocationData;
using SimpleWeather.NET.Controls;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Maui.ViewModels;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using SimpleWeather.NET.ViewModels;
using ResStrings = SimpleWeather.Resources.Strings.Resources;
using SimpleWeather.Maui.MaterialIcons;
using CommunityToolkit.Maui.Markup;
using SimpleWeather.Maui.Location;
using SimpleWeather.Maui.Controls;
using CommunityToolkit.Mvvm.Messaging;
using SimpleWeather.Common.ViewModels;

namespace SimpleWeather.Maui.Main;

public partial class LocationsPage : ViewModelPage, IRecipient<LocationSelectedMessage>, ISnackbarManager, ISnackbarPage
{
    private SnackbarManager SnackMgr { get; set; }

    private LocationsViewModel LocationsViewModel { get; set; }
    private LocationPanelAdapter PanelAdapter { get; set; }

    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    public bool EditMode { get; set; } = false;
    private bool HomeChanged = false;

    private readonly ToolbarItem EditButton;
    private bool IsItemClickEnabled { get; set; } = true;

    private readonly HashSet<VisualElement> ResizeElements = new HashSet<VisualElement>();

    public LocationsPage()
	{
		InitializeComponent();

        BindingContext = LocationsViewModel;

        PanelAdapter = new LocationPanelAdapter(LocationsPanel, SnackbarContainer);
        PanelAdapter.ListChanged += LocationPanels_CollectionChanged;

        // Toolbar
        EditButton = CreateEditButton();

        if (PanelAdapter.FavoritesCount > 1)
        {
            ToolbarItems.Add(EditButton);
        }
        else
        {
            ToolbarItems.Clear();
        }

        BindableLayout.SetItemsSource(GPSPanelLayout, PanelAdapter?.GetDataset(LocationType.GPS));
        LocationsPanel.ItemsSource = PanelAdapter?.GetDataset(LocationType.Search);

        GPSPanelLayout.IsVisible = !(LocationsViewModel?.UiState?.IsLoading ?? true);
        LocationsPanel.IsVisible = !(LocationsViewModel?.UiState?.IsLoading ?? true);
        ContentIndicator.IsRunning = LocationsViewModel?.UiState?.IsLoading ?? true;
        AddLocationsButton.IsVisible = !(LocationsViewModel?.UiState?.IsLoading ?? true);
        NoLocationsView.IsVisible = !(LocationsViewModel?.UiState?.IsLoading ?? true) && (LocationsViewModel?.Locations?.Any() == false);

        MainGrid.Bind<Grid, double, Thickness>(Grid.MarginProperty, nameof(AddLocationsButton.Height), BindingMode.OneWay,
            source: AddLocationsButton, convert: (height) => new Thickness(0, 0, 0, height));

        MainGrid.Children.Cast<VisualElement>().ForEach(v => ResizeElements.Add(v));

        AdjustViewsLayout(0);

        AnalyticsLogger.LogEvent("LocationsPage");
    }

    private ToolbarItem CreateEditButton()
    {
        App.Current.Resources.TryGetValue("LightOnSurface", out var LightOnSurface);
        App.Current.Resources.TryGetValue("DarkOnSurface", out var DarkOnSurface);

        return new ToolbarItem()
        {
            Text = ResStrings.action_editmode,
            IconImageSource =
                new MaterialIcon(MaterialSymbol.Edit)
                {
                    Size = 24
                }.AppThemeBinding(MaterialIcon.ColorProperty, LightOnSurface, DarkOnSurface)
        }.Apply(it =>
        {
            it.Clicked += EditButton_Clicked;
        });
    }

    public void InitSnackManager()
    {
        if (SnackMgr == null)
        {
            SnackMgr = new SnackbarManager(SnackbarContainer);
        }
    }

    public void ShowSnackbar(Snackbar snackbar)
    {
        Dispatcher.Dispatch(() =>
        {
            SnackMgr?.Show(snackbar);
        });
    }
    public void DismissAllSnackbars()
    {
        Dispatcher.Dispatch(() =>
        {
            SnackMgr?.DismissAll();
        });
    }

    public void UnloadSnackManager()
    {
        DismissAllSnackbars();
        SnackMgr = null;
    }

    public void Receive(LocationSelectedMessage message)
    {
        switch (message.Value)
        {
            case LocationSearchResult.AlreadyExists:
            case LocationSearchResult.Success:
                Dispatcher.DispatchAsync(async () =>
                {
                    if (message.Value.Data is LocationData.LocationData data && data.IsValid())
                    {
                        await SettingsManager.AddLocation(data);
                    }
                    LocationsViewModel.RefreshLocations();
                });
                break;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        WeakReferenceMessenger.Default.Register<LocationSelectedMessage>(this);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        WeakReferenceMessenger.Default.Unregister<LocationSelectedMessage>(this);
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs e)
    {
        base.OnNavigatedTo(e);
        AnalyticsLogger.LogEvent("LocationsPage: OnNavigatedTo");
        InitSnackManager();

        LocationsViewModel = this.GetViewModel<LocationsViewModel>();
        BindingContext = LocationsViewModel;

        LocationsViewModel.PropertyChanged += LocationsViewModel_PropertyChanged;
        LocationsViewModel.WeatherUpdated += LocationsViewModel_WeatherUpdated;
        LocationsViewModel.RefreshLocations();
    }

    private void LocationsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(LocationsViewModel.Locations):
                {
                    PanelAdapter.ReplaceAll(LocationsViewModel.Locations);
                    NoLocationsView.IsVisible = !(LocationsViewModel?.UiState?.IsLoading ?? true) && (LocationsViewModel?.Locations?.Any() == false);
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
            case nameof(LocationsViewModel.UiState):
                {
                    GPSPanelLayout.IsVisible = !(LocationsViewModel?.UiState?.IsLoading ?? true);
                    LocationsPanel.IsVisible = !(LocationsViewModel?.UiState?.IsLoading ?? true);
                    ContentIndicator.IsRunning = LocationsViewModel?.UiState?.IsLoading ?? true;
                    AddLocationsButton.IsVisible = !(LocationsViewModel?.UiState?.IsLoading ?? true);
                    NoLocationsView.IsVisible = !(LocationsViewModel?.UiState?.IsLoading ?? true) && (LocationsViewModel?.Locations?.Any() == false);
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

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);

        LocationsViewModel.WeatherUpdated -= LocationsViewModel_WeatherUpdated;
        LocationsViewModel.PropertyChanged -= LocationsViewModel_PropertyChanged;

        // Cancel edit mode if moving away
        if (EditMode)
            ToggleEditMode();
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs e)
    {
        base.OnNavigatedFrom(e);
        UnloadSnackManager();
    }

    private void OnErrorMessage(ErrorMessage error)
    {
        Dispatcher.Dispatch(() =>
        {
            switch (error)
            {
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
                snackbar.SetAction(ResStrings.action_retry, () =>
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

    private void LocationPanels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        BindableLayout.SetItemsSource(GPSPanelLayout, PanelAdapter?.GetDataset(LocationType.GPS));
        LocationsPanel.ItemsSource = PanelAdapter?.GetDataset(LocationType.Search);

        bool dataMoved = (e.Action == NotifyCollectionChangedAction.Remove) || (e.Action == NotifyCollectionChangedAction.Move);
        bool onlyHomeIsLeft = PanelAdapter.FavoritesCount <= 1;
        bool limitReached = PanelAdapter.ItemCount >= SettingsManager.MAX_LOCATIONS;

        if (EditMode && e.NewStartingIndex == 0)
            HomeChanged = true;

        // Cancel edit Mode
        if (EditMode && onlyHomeIsLeft)
            ToggleEditMode();

        // Disable EditMode if only single location
        if (onlyHomeIsLeft)
        {
            ToolbarItems.Remove(EditButton);
            this.OnPropertyChanged(nameof(ToolbarItems));
        }
        else if (!ToolbarItems.Contains(EditButton))
        {
            ToolbarItems.Add(EditButton);
            this.OnPropertyChanged(nameof(ToolbarItems));
        }
        AddLocationsButton.IsVisible = !limitReached;
    }

    private async void LocationsPanel_ItemTapped(object sender, TappedEventArgs e)
    {
        if (IsItemClickEnabled)
        {
            AnalyticsLogger.LogEvent("LocationsPage: LocationsPanel_ItemClick");
            if (sender is BindableObject obj && obj.BindingContext is LocationPanelUiModel panel)
            {
                try
                {
                    await AppShell.Current.GoToAsync("///root", new Dictionary<string, object>()
                    {
                        {
                            "args",
                            new WeatherNowArgs()
                            {
                                IsHome = Equals(panel.LocationData, await SettingsManager.GetHomeData()),
                                Location = panel.LocationData
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "Exception!!");
                }
            }
        }
        else
        {
            if (sender is BindableObject obj && obj.BindingContext is LocationPanelUiModel panel)
            {
                if (LocationsPanel.SelectedItems.Contains(panel))
                {
                    LocationsPanel.SelectedItems.Remove(panel);
                }
                else
                {
                    LocationsPanel.SelectedItems.Add(panel);
                }
            }
        }
    }

    private async void AddLocationButton_Clicked(object sender, EventArgs e)
    {
        AnalyticsLogger.LogEvent("LocationsPage: AddLocationsButton_Click");
        await Navigation.PushAsync(new LocationSearchPage());
    }

    private void EditButton_Clicked(object sender, EventArgs e)
    {
        ToggleEditMode();
    }

    private void ToggleEditMode()
    {
        // Toggle EditMode
        EditMode = !EditMode;

        if (EditButton.IconImageSource is MaterialIcon icon)
        {
            icon.Symbol = EditMode ? MaterialSymbol.Done : MaterialSymbol.Edit;
        }
        EditButton.Text = EditMode ? ResStrings.Label_Done : ResStrings.action_editmode;
        IsItemClickEnabled = !EditMode;
        // Enable selection mode for non-Mobile (non-Touch devices)
        LocationsPanel.SelectionMode = EditMode ? SelectionMode.Multiple : SelectionMode.None;
        LocationsPanel.SelectedItems?.Clear();
        if (EditMode && !ToolbarItems.Any(ti => ti.StyleId == "delete"))
        {
            var deleteBtn = CreateDeleteButton();
            ToolbarItems.Insert(0, deleteBtn);
        }
        else if (ToolbarItems.FirstOrDefault(ti => ti.StyleId == "delete") is ToolbarItem item)
        {
            ToolbarItems.Remove(item);
        }

        foreach (LocationPanelUiModel view in PanelAdapter.GetDataset())
        {
            view.EditMode = EditMode;
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

        this.OnPropertyChanged(nameof(ToolbarItems));
    }

    private ToolbarItem CreateDeleteButton()
    {
        App.Current.Resources.TryGetValue("LightOnSurface", out var LightOnSurface);
        App.Current.Resources.TryGetValue("DarkOnSurface", out var DarkOnSurface);

        return new ToolbarItem()
        {
            Text = ResStrings.delete,
            IconImageSource =
                new MaterialIcon(MaterialSymbol.Delete)
                {
                    Size = 24
                }.AppThemeBinding(MaterialIcon.ColorProperty, LightOnSurface, DarkOnSurface),
            StyleId = "delete"
        }.Apply(it =>
        {
            it.Clicked += DeleteBtn_Clicked;
        });
    }

    private async void SwipeItem_Invoked(object sender, EventArgs e)
    {
        if (sender is BindableObject obj && obj.BindingContext is LocationPanelUiModel view)
        {
            AnalyticsLogger.LogEvent("LocationsPage: SwipeItem_Invoked");
            await PanelAdapter.RemovePanel(view).ConfigureAwait(true);
        }
    }

    private async void DeleteBtn_Clicked(object sender, EventArgs e)
    {
        AnalyticsLogger.LogEvent("LocationsPage: DeleteBtn_Clicked");
        await PanelAdapter.BatchRemovePanels(LocationsPanel.SelectedItems.Cast<LocationPanelUiModel>()).ConfigureAwait(true);
    }

    private void LocationPanel_LongPress(object sender, EventArgs e)
    {
        if (sender is BindableObject obj && obj.BindingContext is LocationPanelUiModel model && model.LocationType == (int)LocationType.Search)
        {
            if (!EditMode && PanelAdapter.FavoritesCount > 1)
            {
                ToggleEditMode();

                LocationsPanel.SelectedItems?.Clear();
                LocationsPanel.SelectedItems?.Add(model);
            }
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        Dispatcher.Dispatch(() =>
        {
            AdjustViewsLayout(width);
        });
    }

    private void AdjustViewsLayout(double? width = null)
    {
        // Resize StackControl items
        double maxWidth = 1280;
        double requestedWidth = width ?? RootView.Width;
        double requestedPadding = 0;

        if (requestedWidth <= 0)
            return;

        if (requestedWidth > maxWidth)
        {
            requestedPadding = (requestedWidth - maxWidth) / 2;
        }

        try
        {
            foreach (var element in ResizeElements)
            {
                if (element is View v)
                {
                    v.Margins(
                        left: requestedPadding, right: requestedPadding,
                        top: v.Margin.Top, bottom: v.Margin.Bottom);
                }
            }

            if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                if (LocationsPanel.ItemsLayout is not GridItemsLayout layout || layout.Span != 1)
                {
                    LocationsPanel.ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical)
                    {
                        HorizontalItemSpacing = 4,
                        VerticalItemSpacing = 4
                    };
                }
            }
            else
            {
                var StackWidth = requestedWidth - LocationsPanel.Margin.HorizontalThickness;

                var minColumns = 1;

                // Min width for ea. card
                var minWidth = 480;
                // Available columns based on min card width
                var availColumns = (int)(StackWidth / minWidth) <= 1 ? minColumns : (int)(StackWidth / minWidth);

                if (LocationsPanel.ItemsLayout is not GridItemsLayout layout || layout.Span != availColumns)
                {
                    LocationsPanel.ItemsLayout = new GridItemsLayout(availColumns, ItemsLayoutOrientation.Vertical)
                    {
                        HorizontalItemSpacing = 4,
                        VerticalItemSpacing = 4
                    };
                }
            }
        }
        catch { }
    }
}