using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationSearchPage : CustomPage, IBackRequestedPage, IDisposable
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        private WeatherManager wm;
        public ObservableCollection<LocationQueryViewModel> LocationQuerys { get; private set; }
        private ProgressRing LoadingRing { get { return Location?.ProgressRing; } }

        public LocationSearchPage()
        {
            this.InitializeComponent();

            wm = WeatherManager.GetInstance();

            LocationQuerys = new ObservableCollection<LocationQueryViewModel>();

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("Nav_Locations/Content");
            AnalyticsLogger.LogEvent("LocationSearchPage");
        }

        public Task<bool> OnBackRequested()
        {
            if (Frame.CanGoBack) Frame.GoBack(); else Frame.Navigate(typeof(LocationsPage));

            return Task.FromResult(true);
        }

        public void Dispose()
        {
            cts?.Dispose();
        }

        private DispatcherTimer timer;

        private void EnableControls(bool enable)
        {
            if (LoadingRing != null)
                LoadingRing.IsActive = !enable;
        }

        /// <summary>
        /// Location_TextChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        private void Location_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // user is typing: reset already started timer (if existing)
            if (timer != null && timer.IsEnabled)
                timer.Stop();

            if (String.IsNullOrWhiteSpace(sender.Text))
            {
                // Cancel pending searches
                cts?.Cancel();
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

                        // Cancel pending searches
                        cts?.Cancel();
                        cts = new CancellationTokenSource();
                        var ctsToken = cts.Token;

                        Task.Run(async () =>
                        {
                            ctsToken.ThrowIfCancellationRequested();

                            ObservableCollection<LocationQueryViewModel> results = await wm.GetLocations(query);

                            ctsToken.ThrowIfCancellationRequested();

                            return results;
                        }).ContinueWith((task) =>
                        {
                            ObservableCollection<LocationQueryViewModel> results;

                            if (task.IsFaulted)
                            {
                                var ex = task.Exception.GetBaseException();

                                if (ex is WeatherException)
                                {
                                    ShowSnackbar(Snackbar.Make(ex.Message, SnackbarDuration.Short));
                                }
                            }

                            if (task.Result != null)
                            {
                                results = task.Result;
                            }
                            else
                            {
                                results = new ObservableCollection<LocationQueryViewModel>() { new LocationQueryViewModel() };
                            }

                            // Refresh list
                            LocationQuerys = results;
                            sender.ItemsSource = null;
                            sender.ItemsSource = LocationQuerys;
                            sender.IsSuggestionListOpen = true;
                            timer.Stop();
                        }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(true);
                    }
                    else if (String.IsNullOrWhiteSpace(sender.Text))
                    {
                        // Cancel pending searches
                        cts?.Cancel();
                        cts = new CancellationTokenSource();
                        // Hide flyout if query is empty or null
                        LocationQuerys.Clear();
                        sender.IsSuggestionListOpen = false;
                        timer.Stop();
                    }
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

        /// <summary>
        /// Location_QuerySubmitted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        private void Location_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            EnableControls(false);

            // Cancel other tasks
            cts?.Cancel();
            cts = new CancellationTokenSource();
            var ctsToken = cts.Token;

            var theChosenOne = args.ChosenSuggestion as LocationQueryViewModel;
            // Use args.QueryText to determine what to do.
            var queryText = args.QueryText;

            Task.Run(async () =>
            {
                LocationQueryViewModel query_vm = null;

                if (theChosenOne != null)
                {
                    if (!String.IsNullOrEmpty(theChosenOne.LocationQuery))
                        query_vm = theChosenOne;
                    else
                        query_vm = new LocationQueryViewModel();
                }
                else if (!String.IsNullOrEmpty(queryText))
                {
                    query_vm = await Task.Run(async () =>
                    {
                        ObservableCollection<LocationQueryViewModel> results;
                        results = await AsyncTask.RunAsync(wm.GetLocations(queryText));

                        var result = results.FirstOrDefault();

                        if (result != null && !String.IsNullOrWhiteSpace(result.LocationQuery))
                        {
                            return result;
                        }
                        else
                        {
                            return new LocationQueryViewModel();
                        }
                    }, ctsToken).ConfigureAwait(true);
                }
                else if (String.IsNullOrWhiteSpace(queryText))
                {
                    // Stop since there is no valid query
                    throw new CustomException(App.ResLoader.GetString("Error_Location"));
                }

                if (String.IsNullOrWhiteSpace(query_vm.LocationQuery))
                {
                    // Stop since there is no valid query
                    throw new CustomException(App.ResLoader.GetString("Error_Location"));
                }

                if (Settings.UsePersonalKey && String.IsNullOrWhiteSpace(Settings.API_KEY) && wm.KeyRequired)
                {
                    throw new CustomException(App.ResLoader.GetString("WError_InvalidKey"));
                }

                ctsToken.ThrowIfCancellationRequested();

                // Need to get FULL location data for HERE API
                // Data provided is incomplete
                if (query_vm.LocationLat == -1 && query_vm.LocationLong == -1
                        && query_vm.LocationTZLong == null
                        && wm.LocationProvider.NeedsLocationFromID)
                {
                    query_vm = await AsyncTask.RunAsync(
                        wm.LocationProvider.GetLocationFromID(query_vm));
                }
                else if (wm.LocationProvider.NeedsLocationFromName)
                {
                    query_vm = await AsyncTask.RunAsync(
                        wm.LocationProvider.GetLocationFromName(query_vm));
                }
                else if (wm.LocationProvider.NeedsLocationFromGeocoder)
                {
                    query_vm = await AsyncTask.RunAsync(
                        wm.LocationProvider.GetLocation(new WeatherUtils.Coordinate(query_vm.LocationLat, query_vm.LocationLong), query_vm.WeatherSource));
                }

                if (query_vm == null)
                {
                    throw new OperationCanceledException();
                }
                else if (String.IsNullOrEmpty(query_vm.LocationTZLong) && query_vm.LocationLat != 0 && query_vm.LocationLong != 0)
                {
                    String tzId = await TZDB.TZDBCache.GetTimeZone(query_vm.LocationLat, query_vm.LocationLong);
                    if (!String.IsNullOrWhiteSpace(tzId))
                        query_vm.LocationTZLong = tzId;
                }

                bool isUS = LocationUtils.IsUS(query_vm.LocationCountry);

                if (!Settings.WeatherLoaded)
                {
                    // Default US location to NWS
                    if (isUS)
                    {
                        Settings.API = WeatherAPI.NWS;
                        query_vm.UpdateWeatherSource(WeatherAPI.NWS);
                    }
                    else
                    {
                        Settings.API = WeatherAPI.Yahoo;
                        query_vm.UpdateWeatherSource(WeatherAPI.Yahoo);
                    }
                    wm.UpdateAPI();
                }

                if (WeatherAPI.NWS.Equals(Settings.API) && !isUS)
                {
                    throw new CustomException(App.ResLoader.GetString("Error_WeatherUSOnly"));
                }

                // Check if location already exists
                var locData = await Settings.GetLocationData();
                if (locData.Any(l => l.query == query_vm.LocationQuery))
                {
                    return true;
                }

                ctsToken.ThrowIfCancellationRequested();

                var location = new LocationData(query_vm);
                if (!location.IsValid())
                {
                    throw new CustomException(App.ResLoader.GetString("WError_NoWeather"));
                }

                var weather = await Settings.GetWeatherData(location.query);
                if (weather == null)
                {
                    weather = await wm.GetWeather(location);
                }

                if (weather == null)
                {
                    throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
                }
                else if (wm.SupportsAlerts && wm.NeedsExternalAlertData)
                {
                    weather.weather_alerts = await AsyncTask.RunAsync(wm.GetAlerts(location));
                }

                // Save data
                await Settings.AddLocation(location);
                if (wm.SupportsAlerts && weather.weather_alerts != null)
                    await Settings.SaveWeatherAlerts(location, weather.weather_alerts);
                await Settings.SaveWeatherData(weather);
                await Settings.SaveWeatherForecasts(new Forecasts(weather.query, weather.forecast, weather.txt_forecast));
                await Settings.SaveWeatherForecasts(location, weather.hr_forecast == null ? null :
                    weather.hr_forecast.Select(f => new HourlyForecasts(weather.query, f)));

                return true;
            }).ContinueWith((t) =>
            {
                // Restore controls
                EnableControls(true);

                if (t.IsFaulted)
                {
                    var ex = t.Exception.GetBaseException();

                    if (ex is WeatherException || ex is CustomException)
                    {
                        ShowSnackbar(Snackbar.Make(ex.Message, SnackbarDuration.Short));
                    }
                    else
                    {
                        ShowSnackbar(Snackbar.Make(App.ResLoader.GetString("Error_Location"), SnackbarDuration.Short));
                    }
                }
                else if (t.IsCompletedSuccessfully && t.Result)
                {
                    if (Frame.CanGoBack)
                        Frame.GoBack();
                    else
                        Frame.Navigate(typeof(LocationsPage));
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(true);
        }
    }
}