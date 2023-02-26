using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using SimpleWeather.Common.Utils;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.ComponentModel;
using SimpleWeather.LocationData;
using SimpleWeather.Maui.Controls;
using SimpleWeather.NET.Controls;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using System.ComponentModel;

namespace SimpleWeather.Maui.Location;

public partial class LocationSearchPage : ScopePage, ISnackbarManager
{
    public LocationSearchViewModel LocationSearchViewModel { get; } = Ioc.Default.GetViewModel<LocationSearchViewModel>();
    private LocationSelectedMessage PendingMessage = null;

    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    public LocationSearchPage()
    {
        InitializeComponent();
#if ANDROID
        this.SearchView.HandlerChanged += SearchView_HandlerChanged;
#endif
    }

    private void SearchView_HandlerChanged(object sender, EventArgs e)
    {
#if ANDROID
        if (this.SearchView.Handler?.PlatformView is Android.Widget.EditText view)
        {
            view.Background = null;
            view.Gravity = Android.Views.GravityFlags.CenterVertical;
            view.SetCursorVisible(true);
            view.Focusable = true;
            view.FocusableInTouchMode = true;
            view.SetSingleLine();
        }
#endif
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

        this.SearchView.Focus();
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
                                PendingMessage = new LocationSelectedMessage(result);
                                await App.Current.Navigation.PopAsync();
                            }
                            break;
                        case LocationSearchResult.Success result:
                            {
                                PendingMessage = new LocationSelectedMessage(result);
                                await App.Current.Navigation.PopAsync();
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
        await App.Current.Navigation.PopAsync();
    }

    private async void BackButton_Tapped(object sender, TappedEventArgs e)
    {
        await App.Current.Navigation.PopAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (PendingMessage != null)
        {
            Dispatcher.Dispatch(() =>
            {
                WeakReferenceMessenger.Default.Send(PendingMessage);
            });
        }
    }
}