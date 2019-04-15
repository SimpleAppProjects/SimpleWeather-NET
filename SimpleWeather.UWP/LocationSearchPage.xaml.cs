using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationSearchPage : Page, ICommandBarPage, IDisposable
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        private WeatherManager wm;
        public ObservableCollection<LocationQueryViewModel> LocationQuerys { get; set; }
        private ProgressRing LoadingRing;

        public string CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        public LocationSearchPage()
        {
            this.InitializeComponent();

            wm = WeatherManager.GetInstance();

            LocationQuerys = new ObservableCollection<LocationQueryViewModel>();
            LoadingRing = Location.ProgressRing;

            // Event Listeners
            SystemNavigationManager.GetForCurrentView().BackRequested += LocationSearchPage_BackRequested;

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("Nav_Locations/Label");
            PrimaryCommands = new List<ICommandBarElement>(0);
        }

        private void LocationSearchPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            if (Frame.CanGoBack) Frame.GoBack(); else Frame.Navigate(typeof(LocationsPage));
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.SourcePageType == typeof(WeatherNow)) e.Cancel = true;
            // Unsubscribe from event
            SystemNavigationManager.GetForCurrentView().BackRequested -= LocationSearchPage_BackRequested;

            base.OnNavigatingFrom(e);
        }

        public void Dispose()
        {
            cts.Dispose();
        }

        private DispatcherTimer timer;

        private void Location_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Cancel pending searches
            cts.Cancel();
            cts = new CancellationTokenSource();
            var ctsToken = cts.Token;
            // user is typing: reset already started timer (if existing)
            if (timer != null && timer.IsEnabled)
                timer.Stop();

            if (String.IsNullOrWhiteSpace(sender.Text))
            {
                // Cancel pending searches
                cts.Cancel();
                cts = new CancellationTokenSource();
                // Hide flyout if query is empty or null
                LocationQuerys.Clear();
                sender.IsSuggestionListOpen = false;
            }
            else
            {
                timer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds(1000)
                };
                timer.Tick += (t, e) =>
                {
                    if (!String.IsNullOrWhiteSpace(sender.Text) && args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                    {
                        String query = sender.Text;

                        Task.Run(async () =>
                        {
                            if (ctsToken.IsCancellationRequested) return;

                            var results = await wm.GetLocations(query);

                            if (ctsToken.IsCancellationRequested) return;

                            // Refresh list
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                LocationQuerys = results;
                                sender.ItemsSource = null;
                                sender.ItemsSource = LocationQuerys;
                                sender.IsSuggestionListOpen = true;
                            });
                        });
                    }
                    else if (String.IsNullOrWhiteSpace(sender.Text))
                    {
                        // Cancel pending searches
                        cts.Cancel();
                        cts = new CancellationTokenSource();
                        // Hide flyout if query is empty or null
                        LocationQuerys.Clear();
                        sender.IsSuggestionListOpen = false;
                    }

                    timer.Stop();
                };
                timer.Start();
            }
        }

        private void Location_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is LocationQueryViewModel theChosenOne)
            {
                if (String.IsNullOrEmpty(theChosenOne.LocationQuery))
                    sender.Text = String.Empty;
                else
                    sender.Text = theChosenOne.LocationName;
            }

            sender.IsSuggestionListOpen = false;
        }

        private async void Location_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            LocationQueryViewModel query_vm = null;

            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                var theChosenOne = args.ChosenSuggestion as LocationQueryViewModel;

                if (!String.IsNullOrEmpty(theChosenOne.LocationQuery))
                    query_vm = theChosenOne;
                else
                    query_vm = new LocationQueryViewModel();
            }
            else if (!String.IsNullOrEmpty(args.QueryText))
            {
                // Use args.QueryText to determine what to do.
                var result = (await wm.GetLocations(args.QueryText)).First();

                if (result != null && !String.IsNullOrWhiteSpace(result.LocationQuery))
                {
                    sender.Text = result.LocationName;
                    query_vm = result;
                }
                else
                {
                    query_vm = new LocationQueryViewModel();
                }
            }
            else if (String.IsNullOrWhiteSpace(args.QueryText))
            {
                // Stop since there is no valid query
                return;
            }

            if (String.IsNullOrWhiteSpace(query_vm.LocationQuery))
            {
                // Stop since there is no valid query
                return;
            }

            // Cancel other tasks
            cts.Cancel();
            cts = new CancellationTokenSource();
            var ctsToken = cts.Token;

            LoadingRing.IsActive = true;

            if (ctsToken.IsCancellationRequested)
            {
                LoadingRing.IsActive = false;
                return;
            }

            // Check if location already exists
            var locData = await Settings.GetLocationData();
            if (locData.Exists(l => l.query == query_vm.LocationQuery))
            {
                Frame.Navigate(typeof(LocationsPage));
                return;
            }

            if (ctsToken.IsCancellationRequested)
            {
                LoadingRing.IsActive = false;
                return;
            }

            // Need to get FULL location data for HERE API
            // Data provided is incomplete
            if (WeatherAPI.Here.Equals(Settings.API)
                    && query_vm.LocationLat == -1 && query_vm.LocationLong == -1
                    && query_vm.LocationTZ_Long == null)
            {
                query_vm = await new HERE.HERELocationProvider().GetLocationFromLocID(query_vm.LocationQuery);
            }
            else if (WeatherAPI.MetNo.Equals(Settings.API)
                    && query_vm.LocationLat == -1 && query_vm.LocationLong == -1
                    && query_vm.LocationTZ_Long == null)
            {
                query_vm = await new Bing.BingMapsLocationProvider().GetLocationFromAddress(query_vm.LocationQuery);
            }

            var location = new LocationData(query_vm);
            if (!location.IsValid())
            {
                await Toast.ShowToastAsync(App.ResLoader.GetString("WError_NoWeather"), ToastDuration.Short);
                LoadingRing.IsActive = false;
                return;
            }
            var weather = await Settings.GetWeatherData(location.query);
            if (weather == null)
            {
                try
                {
                    weather = await wm.GetWeather(location);
                }
                catch (WeatherException wEx)
                {
                    weather = null;
                    await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                }
            }

            if (weather == null)
            {
                LoadingRing.IsActive = false;
                return;
            }

            // We got our data so disable controls just in case
            sender.IsSuggestionListOpen = false;

            // Save data
            await Settings.AddLocation(location);
            if (wm.SupportsAlerts && weather.weather_alerts != null)
                await Settings.SaveWeatherAlerts(location, weather.weather_alerts);
            await Settings.SaveWeatherData(weather);

            var panelView = new LocationPanelViewModel(weather)
            {
                LocationData = location
            };

            // Hide add locations panel
            LoadingRing.IsActive = false;
            if (Frame.CanGoBack) Frame.GoBack(); else Frame.Navigate(typeof(LocationsPage));
        }
    }
}
