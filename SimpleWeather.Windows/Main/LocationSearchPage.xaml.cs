using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SimpleWeather.Common.Utils;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Helpers;
using SimpleWeather.NET.ViewModels;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.NET.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationSearchPage : ViewModelPage, ICommandBarPage, ISnackbarPage, IBackRequestedPage
    {
        public String CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        private LocationSearchViewModel LocationSearchViewModel { get; set; }

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public LocationSearchPage()
        {
            this.InitializeComponent();

            // CommandBar
            CommandBarLabel = App.Current.ResLoader.GetString("label_nav_locations");
            AnalyticsLogger.LogEvent("LocationSearchPage");
        }

        public void ShowSnackbar(Snackbar snackbar)
        {
            Shell.Instance?.ShowSnackbar(snackbar);
        }

        public Task<bool> OnBackRequested()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(LocationsPage));
            }

            return Task.FromResult(true);
        }

        private DispatcherTimer timer;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnalyticsLogger.LogEvent("LocationSearchPage: OnNavigatedTo");

            LocationSearchViewModel = this.GetViewModel<LocationSearchViewModel>();

            LocationSearchViewModel.PropertyChanged += LocationSearchViewModel_PropertyChanged;
            LocationSearchViewModel.Initialize();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            AnalyticsLogger.LogEvent("LocationSearchPage: OnNavigatedFrom");

            LocationSearchViewModel.PropertyChanged -= LocationSearchViewModel_PropertyChanged;
        }

        private void LocationSearchViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
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
                case nameof(LocationSearchViewModel.SelectedSearchLocation):
                    {
                        switch (LocationSearchViewModel.SelectedSearchLocation)
                        {
                            case LocationSearchResult.AlreadyExists result:
                                {
                                    if (Frame.CanGoBack)
                                        Frame.GoBack();
                                    else
                                        Frame.Navigate(typeof(LocationsPage));
                                }
                                break;
                            case LocationSearchResult.Success result:
                                {
                                    DispatcherQueue.EnqueueAsync(async () =>
                                    {
                                        if (result.Data?.IsValid() == true)
                                        {
                                            await SettingsManager.AddLocation(result.Data);
                                        }

                                        if (Frame.CanGoBack)
                                            Frame.GoBack();
                                        else
                                            Frame.Navigate(typeof(LocationsPage));
                                    });
                                }
                                break;
                            case LocationSearchResult.Failed:
                            default:
                                break;
                        }
                    }
                    break;
            }
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
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // user is typing: reset already started timer (if existing)
                if (timer?.IsEnabled == true)
                    timer.Stop();

                if (String.IsNullOrWhiteSpace(sender.Text))
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
            if (!String.IsNullOrWhiteSpace(sender.Text) && args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                LocationSearchViewModel.FetchLocations(sender.Text);
            }
            else if (String.IsNullOrWhiteSpace(sender.Text))
            {
                timer?.Stop();
                LocationSearchViewModel.FetchLocations(sender.Text);
            }
        }

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
            var theChosenOne = args.ChosenSuggestion as LocationQuery;
            if (theChosenOne != LocationQuery.Empty)
            {
                LocationSearchViewModel.OnLocationSelected(theChosenOne);
            }
        }
    }
}