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
    private bool IsItemClickEnabled { get; set; }

    public LocationsPage()
	{
		InitializeComponent();

        BindingContext = LocationsViewModel;

        PanelAdapter = new LocationPanelAdapter(LocationsPanel);
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
        LocationsPanel.ItemsSource = PanelAdapter.ItemCount > 0 ? PanelAdapter.LocationPanelGroups : null;

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

        LocationsViewModel.PropertyChanged += LocationsViewModel_PropertyChanged;
        LocationsViewModel.WeatherUpdated += LocationsViewModel_WeatherUpdated;
        LocationsViewModel.RefreshLocations();
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
        LocationsPanel.ItemsSource = PanelAdapter.ItemCount > 0 ? PanelAdapter.LocationPanelGroups : null;

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
        }
        else if (!ToolbarItems.Contains(EditButton))
        {
            ToolbarItems.Add(EditButton);
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
                    var newPage = new WeatherNow(new WeatherNowArgs()
                    {
                        IsHome = Equals(panel.LocationData, await SettingsManager.GetHomeData()),
                        Location = panel.LocationData
                    });
                    await Navigation.PushAsync(newPage);
                    // Remove all from backstack except home
                    Navigation.NavigationStack.Where(p => p != null && p != newPage).ToList().ForEach(p =>
                    {
                        Navigation.RemovePage(p);
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

    private void LocationsPanel_SizeChanged(object sender, EventArgs e)
    {
        // Resize StackControl items
        double StackWidth = LocationsPanel.Width;

        if (StackWidth <= 0)
            return;

        try
        {
            if (StackWidth >= 1280)
            {
                LocationsPanel.WidthRequest = 1280;
            }

            if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                LocationsPanelLayout.Span = 1;
            }
            else
            {
                var isLandscape = DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Landscape;
                var minColumns = isLandscape ? 2 : 1;

                // Min width for ea. card
                var minWidth = 480;
                // Available columns based on min card width
                var availColumns = (int)(StackWidth / minWidth) <= 1 ? minColumns : (int)(StackWidth / minWidth);

                LocationsPanelLayout.Span = availColumns;
            }
        }
        catch { }
    }
}