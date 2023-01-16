using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using SimpleWeather.Common.Utils;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.ComponentModel;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Uno.Controls;
using SimpleWeather.Uno.Location;
using SimpleWeather.Utils;
using System;
using System.ComponentModel;
using System.Linq;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace SimpleWeather.Uno.Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupLocationSearchPage : Page, ISnackbarManager
    {
        private LocationSearchViewModel LocationSearchViewModel { get; } = Ioc.Default.GetViewModel<LocationSearchViewModel>();

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public SetupLocationSearchPage()
        {
            this.InitializeComponent();
        }

        private SnackbarManager SnackMgr;

        public void InitSnackManager()
        {
            if (SnackMgr == null)
            {
                SnackMgr = new SnackbarManager(Content as Panel);
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

        private DispatcherTimer timer;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnalyticsLogger.LogEvent("SetupLocationSearchPage: OnNavigatedTo");
            InitSnackManager();

            LocationSearchViewModel.PropertyChanged += LocationSearchViewModel_PropertyChanged;
            LocationSearchViewModel.Initialize();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            AnalyticsLogger.LogEvent("SetupLocationSearchPage: OnNavigatedFrom");
            UnloadSnackManager();

            LocationSearchViewModel.PropertyChanged -= LocationSearchViewModel_PropertyChanged;
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            this.Frame.GoBack();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void LocationSearchViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
                                        OnLocationReceived(result.Data);
                                    }
                                }
                                break;
                            case LocationSearchResult.Success result:
                                {
                                    if (result.Data?.IsValid() == true)
                                    {
                                        OnLocationReceived(result.Data);
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

        private void OnLocationReceived(LocationData.LocationData location)
        {
            this.Frame.GoBack();
            WeakReferenceMessenger.Default.Send(new LocationSelectedMessage(location));
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
                            ShowSnackbar(Snackbar.MakeError(err.Exception.Message, SnackbarDuration.Short));
                        }
                        break;
                }
            });

            LocationSearchViewModel.SetErrorMessageShown(error);
        }

        /// <summary>
        /// Location_TextChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        private void Location_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
#if !ANDROID
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
#else
            if (args.Reason != AutoSuggestionBoxTextChangeReason.SuggestionChosen)
#endif
            {
                // user is typing: reset already started timer (if existing)
                if (timer?.IsEnabled == true)
                    timer.Stop();

                if (String.IsNullOrEmpty(sender.Text))
                {
                    FetchLocations(sender, args);
                }
                else
                {
                    timer = new DispatcherTimer()
                    {
                        Interval = TimeSpan.FromMilliseconds(1000)
                    };
                    timer.Tick += (t, e) =>
                    {
                        timer?.Stop();
                        FetchLocations(sender, args);
                    };
                    timer.Start();
                }
            }
        }

        private void FetchLocations(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (!String.IsNullOrWhiteSpace(sender.Text) &&
#if !ANDROID
                args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
#else
                args.Reason != AutoSuggestionBoxTextChangeReason.SuggestionChosen)
#endif
            {
                LocationSearchViewModel.FetchLocations(sender.Text);
            }
            else if (String.IsNullOrWhiteSpace(sender.Text))
            {
                timer?.Stop();
                LocationSearchViewModel.FetchLocations(sender.Text);
            }
        }

        /// <summary>
        /// Raised before the text content of the editable control component is updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Location_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is LocationQuery theChosenOne)
            {
                if (!String.IsNullOrEmpty(theChosenOne.Location_Query))
                {
                    sender.Text = theChosenOne.LocationName;
                    sender.IsSuggestionListOpen = false;
                }
            }
        }

        /// <summary>
        /// Event is triggered when user selects an item from suggestion list or when query icon is pressed
        /// </summary>
        /// <param name="sender">AutoSuggestBox instance where event was triggered</param>
        /// <param name="args">Event args</param>
        private void Location_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion is LocationQuery theChosenOne && theChosenOne != LocationQuery.Empty)
            {
                LocationSearchViewModel.OnLocationSelected(theChosenOne);
            }
        }

        private void SearchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems?.FirstOrDefault() is LocationQuery theChosenOne && theChosenOne != LocationQuery.Empty)
            {
                LocationSearchViewModel.OnLocationSelected(theChosenOne);
            }
        }
    }
}