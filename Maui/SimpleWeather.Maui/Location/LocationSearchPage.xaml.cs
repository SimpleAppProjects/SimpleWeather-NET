using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using SimpleWeather.Common.Utils;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.ComponentModel;
using SimpleWeather.LocationData;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using System.ComponentModel;

namespace SimpleWeather.Maui.Location;

public partial class LocationSearchPage : ScopePage, ISnackbarManager
{
    public LocationSearchViewModel LocationSearchViewModel { get; } = Ioc.Default.GetViewModel<LocationSearchViewModel>();

    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    public LocationSearchPage()
    {
        InitializeComponent();
    }

    private SnackbarManager SnackMgr;

    public void InitSnackManager()
    {
        if (SnackMgr == null)
        {
            SnackMgr = new SnackbarManager(Content as Layout);
        }
    }

    public void ShowSnackbar(Snackbar snackbar)
    {
        SnackMgr?.Show(snackbar);
    }

    public void DismissAllSnackbars()
    {
        SnackMgr?.DismissAll();
    }

    public void UnloadSnackManager()
    {
        DismissAllSnackbars();
        SnackMgr = null;
    }

    private IDispatcherTimer timer;

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        AnalyticsLogger.LogEvent("LocationSearchPage: OnNavigatedTo");
        InitSnackManager();

        LocationSearchViewModel.PropertyChanged += LocationSearchViewModel_PropertyChanged;
        LocationSearchViewModel.Initialize();
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        AnalyticsLogger.LogEvent("LocationSearchPage: OnNavigatedFrom");
        UnloadSnackManager();

        LocationSearchViewModel.PropertyChanged -= LocationSearchViewModel_PropertyChanged;
    }

    private async void LocationSearchViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(LocationSearchViewModel.SelectedSearchLocation):
                {
                    switch (LocationSearchViewModel.SelectedSearchLocation)
                    {
                        case LocationSearchResult.AlreadyExists result:
                            {
                                if (result.Data?.IsValid() == true)
                                {
                                    await OnLocationReceived(result.Data);
                                }
                            }
                            break;
                        case LocationSearchResult.Success result:
                            {
                                if (result.Data?.IsValid() == true)
                                {
                                    await OnLocationReceived(result.Data);
                                }
                            }
                            break;
                        case LocationSearchResult.Failed:
                        default:
                            break;
                    }
                }
                break;
            case nameof(LocationSearchViewModel.ErrorMessages):
                {
                    var errorMessages = LocationSearchViewModel.ErrorMessages;

                    var error = errorMessages.FirstOrDefault();

                    if (error != null)
                    {
                        OnErrorMessage(error);
                    }
                }
                break;
        }
    }

    private async Task OnLocationReceived(LocationData.LocationData location)
    {
        await SettingsManager.DeleteLocations();

        if (location.locationType == LocationType.GPS)
        {
            await SettingsManager.SaveLastGPSLocData(location);
            await SettingsManager.AddLocation(new LocationQuery(location).ToLocationData());
            SettingsManager.FollowGPS = true;
        }
        else
        {
            await SettingsManager.SaveLastGPSLocData(null);
            await SettingsManager.AddLocation(location);
            SettingsManager.FollowGPS = false;
        }

        SettingsManager.WeatherLoaded = true;

        // Setup complete
        App.Current.SendBackButtonPressed();
        WeakReferenceMessenger.Default.Send(new LocationSelectedMessage(location));
    }

    private void OnErrorMessage(ErrorMessage error)
    {
        Dispatcher.DispatchAsync(() =>
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
                        ShowSnackbar(Snackbar.MakeError(err.Exception.Message, SnackbarDuration.Short));
                    }
                    break;
            }
        });

        LocationSearchViewModel.SetErrorMessageShown(error);
    }

    private void LocationSearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // user is typing: reset already started timer (if existing)
        if (timer?.IsRunning == true)
            timer.Stop();

        if (String.IsNullOrEmpty(e.NewTextValue))
        {
            FetchLocations(e.NewTextValue);
        }
        else
        {
            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += (t, _) =>
            {
                timer?.Stop();
                FetchLocations(e.NewTextValue);
            };
            timer.Start();
        }
    }

    private void FetchLocations(string queryString)
    {
        if (!String.IsNullOrWhiteSpace(queryString))
        {
            LocationSearchViewModel.FetchLocations(queryString);
        }
        else if (String.IsNullOrWhiteSpace(queryString))
        {
            timer?.Stop();
            LocationSearchViewModel.FetchLocations(queryString);
        }
    }

    /// <summary>
    /// Event is triggered when user selects an item from suggestion list or when query icon is pressed
    /// </summary>
    /// <param name="sender">AutoSuggestBox instance where event was triggered</param>
    /// <param name="args">Event args</param>
    private void LocationSearchBox_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is LocationQuery theChosenOne && theChosenOne != LocationQuery.Empty)
        {
            LocationSearchViewModel.OnLocationSelected(theChosenOne);
        }
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await App.Current.Navigation.PopModalAsync();
    }

    private async void BackButton_Tapped(object sender, TappedEventArgs e)
    {
        await App.Current.Navigation.PopModalAsync();
    }
}