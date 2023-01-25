using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using SimpleWeather.Common.Utils;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.ComponentModel;
using SimpleWeather.LocationData;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Maui.Location;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using System.ComponentModel;

namespace SimpleWeather.Maui.Setup;

public partial class SetupLocationsPage : BaseSetupPage, IPageVerification, ISnackbarManager, IRecipient<LocationSelectedMessage>
{
    public LocationSearchViewModel LocationSearchViewModel { get; } = Ioc.Default.GetViewModel<LocationSearchViewModel>();

    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    private IDispatcherTimer timer;

    public SetupLocationsPage(SetupViewModel viewModel) : base(viewModel)
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

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        AnalyticsLogger.LogEvent("SetupLocationsPage: OnNavigatedTo");
        InitSnackManager();

        LocationSearchViewModel.PropertyChanged += LocationSearchViewModel_PropertyChanged;
        LocationSearchViewModel.Initialize();

        WeakReferenceMessenger.Default.Register<LocationSelectedMessage>(this);
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        AnalyticsLogger.LogEvent("SetupLocationsPage: OnNavigatedFrom");
        UnloadSnackManager();

        LocationSearchViewModel.PropertyChanged -= LocationSearchViewModel_PropertyChanged;
    }

    private async void LocationSearchViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(LocationSearchViewModel.CurrentLocation):
                {
                    if (LocationSearchViewModel.CurrentLocation?.IsValid() == true)
                    {
                        await OnLocationReceived(LocationSearchViewModel.CurrentLocation);
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

    public void Receive(LocationSelectedMessage message)
    {
        Dispatcher.DispatchAsync(async () => await OnLocationReceived(message.Value));
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
        ViewModel.LocationData = location;
        ViewModel.Next();
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

    private async void GPS_Click(object sender, EventArgs e)
    {
        await FetchGeoLocation();
    }

    private async Task FetchGeoLocation()
    {
        await this.LocationPermissionEnabled();

        LocationSearchViewModel.FetchGeoLocation();
    }

    public bool CanContinue()
    {
        return ViewModel.LocationData?.IsValid() == true;
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

    private void LocationSearchBox_QuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is LocationQuery theChosenOne && theChosenOne != LocationQuery.Empty)
        {
            LocationSearchViewModel.OnLocationSelected(theChosenOne);
        }
        else if (!String.IsNullOrEmpty(args.QueryText))
        {
            FetchLocations(args.QueryText);
        }
    }

    private void LocationSearchBox_SuggestionChosen(object sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is LocationQuery theChosenOne && sender is ProgressAutoSuggestBox suggestBox)
        {
            if (!String.IsNullOrEmpty(theChosenOne.Location_Query))
            {
                suggestBox.Text = theChosenOne.LocationName;
                suggestBox.IsSuggestionListOpen = false;
            }
        }
    }

    private void LocationSearchBox_TextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        // user is typing: reset already started timer (if existing)
        if (timer?.IsRunning == true)
            timer.Stop();

        if (String.IsNullOrEmpty(args.NewText))
        {
            FetchLocations(args.NewText);
        }
        else
        {
            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += (t, _) =>
            {
                timer?.Stop();
                FetchLocations(args.NewText);
            };
            timer.Start();
        }
    }

    private async void SearchBar_Tapped(object sender, TappedEventArgs e)
    {
        await App.Current.Navigation.PushModalAsync(new LocationSearchPage());
    }
}