using SimpleWeather.AQICN;
using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace SimpleWeather.UWP.Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupLocationsPage : Page, IDisposable, IPageVerification, ISnackbarManager
    {
        private CancellationTokenSource cts = new CancellationTokenSource();

        private readonly WeatherManager wm;
        private Geoposition geoPos = null;

        public ObservableCollection<LocationQueryViewModel> LocationQuerys { get; set; }

        public SetupLocationsPage()
        {
            this.InitializeComponent();

            wm = WeatherManager.GetInstance();

            // Views
            LocationQuerys = new ObservableCollection<LocationQueryViewModel>();

            var LocationAPI = wm.LocationProvider.LocationAPI;
            var creditPrefix = App.ResLoader.GetString("credit_prefix");
            LocationSearchBox.Footer = String.Format("{0} {1}",
                creditPrefix, WeatherAPI.LocationAPIs.First(LApi => LocationAPI.Equals(LApi.Value)));
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

        public void Dispose()
        {
            cts.Dispose();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnalyticsLogger.LogEvent("SetupLocationsPage");
            InitSnackManager();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            UnloadSnackManager();
            cts?.Cancel();
        }

        private DispatcherTimer timer;

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
            // Cancel pending searches
            cts?.Cancel();

            if (!String.IsNullOrWhiteSpace(sender.Text) && args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                String query = sender.Text;

                cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var ctsToken = cts.Token;

                Task.Run(async () =>
                {
                    try
                    {
                        ctsToken.ThrowIfCancellationRequested();

                        var results = await wm.GetLocations(query);

                        ctsToken.ThrowIfCancellationRequested();

                        await Dispatcher.RunOnUIThread(() =>
                        {
                            // Refresh list
                            LocationQuerys = results;
                            RefreshSuggestionList(sender);
                            timer?.Stop();
                        });
                    }
                    catch (Exception ex)
                    {
                        await Dispatcher.RunOnUIThread(() =>
                        {
                            if (ex is WeatherException)
                            {
                                ShowSnackbar(Snackbar.MakeError(ex.Message, SnackbarDuration.Short));
                            }

                            LocationQuerys = new ObservableCollection<LocationQueryViewModel>() { new LocationQueryViewModel() };
                            RefreshSuggestionList(sender);
                            timer?.Stop();
                        });
                    }
                }, ctsToken);
            }
            else if (String.IsNullOrWhiteSpace(sender.Text))
            {
                // Cancel pending searches
                cts?.Cancel();
                cts = new CancellationTokenSource();
                // Hide flyout if query is empty or null
                LocationQuerys.Clear();
                sender.IsSuggestionListOpen = false;
                timer?.Stop();
            }
        }

        /// <summary>
        /// Raised before the text content of the editable control component is updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Location_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is LocationQueryViewModel theChosenOne)
            {
                if (!String.IsNullOrEmpty(theChosenOne.LocationQuery))
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
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        /// <exception cref="InvalidOperationException">Ignore.</exception>
        /// <exception cref="AggregateException">Ignore.</exception>
        /// <exception cref="TaskCanceledException">Ignore.</exception>
        /// <exception cref="WeatherException">Ignore.</exception>
        /// <exception cref="CustomException">Ignore.</exception>
        private void Location_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            EnableControls(false);

            // Cancel other tasks
            cts?.Cancel();
            cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
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
                    // Use args.QueryText to determine what to do.
                    query_vm = await Task.Run(async () =>
                    {
                        ObservableCollection<LocationQueryViewModel> results;
                        results = await Task.Run(async () => 
                        {
                            return await wm.GetLocations(queryText);
                        }).ConfigureAwait(false);

                        var result = results.FirstOrDefault();

                        if (result != null && !String.IsNullOrWhiteSpace(result.LocationQuery))
                        {
                            return result;
                        }
                        else
                        {
                            return new LocationQueryViewModel();
                        }
                    }, ctsToken).ConfigureAwait(false);
                }
                else if (String.IsNullOrWhiteSpace(queryText))
                {
                    // Stop since there is no valid query
                    throw new TaskCanceledException();
                }

                if (String.IsNullOrWhiteSpace(query_vm?.LocationQuery))
                {
                    // Stop since there is no valid query
                    throw new TaskCanceledException();
                }

                if (Settings.UsePersonalKey && String.IsNullOrWhiteSpace(Settings.API_KEY) && wm.KeyRequired)
                {
                    throw new CustomException(App.ResLoader.GetString("werror_invalidkey"));
                }

                ctsToken.ThrowIfCancellationRequested();

                // Need to get FULL location data for HERE API
                // Data provided is incomplete
                if (wm.LocationProvider.NeedsLocationFromID)
                {
                    query_vm = await Task.Run(async () =>
                    {
                        return await wm.LocationProvider.GetLocationFromID(query_vm).ConfigureAwait(false);
                    }, ctsToken).ConfigureAwait(false);
                }
                else if (wm.LocationProvider.NeedsLocationFromName)
                {
                    query_vm = await Task.Run(async () => 
                    {
                        return await wm.LocationProvider.GetLocationFromName(query_vm).ConfigureAwait(false);
                    }, ctsToken).ConfigureAwait(false);
                }
                else if (wm.LocationProvider.NeedsLocationFromGeocoder)
                {
                    query_vm = await Task.Run(async () => 
                    {
                        return await wm.LocationProvider.GetLocation(new WeatherUtils.Coordinate(query_vm.LocationLat, query_vm.LocationLong), query_vm.WeatherSource).ConfigureAwait(false);
                    }, ctsToken).ConfigureAwait(false);
                }

                if (String.IsNullOrWhiteSpace(query_vm?.LocationQuery))
                {
                    // Stop since there is no valid query
                    throw new CustomException(App.ResLoader.GetString("error_retrieve_location"));
                }
                else if (String.IsNullOrWhiteSpace(query_vm.LocationTZLong) && query_vm.LocationLat != 0 && query_vm.LocationLong != 0)
                {
                    String tzId = await Task.Run(async () => 
                    {
                        return await TZDB.TZDBCache.GetTimeZone(query_vm.LocationLat, query_vm.LocationLong).ConfigureAwait(false);
                    }, ctsToken).ConfigureAwait(false);
                    if (!String.IsNullOrWhiteSpace(tzId))
                        query_vm.LocationTZLong = tzId;
                }

                // Set default provider based on location
                var provider = RemoteConfig.RemoteConfig.GetDefaultWeatherProvider(query_vm.LocationCountry);
                Settings.API = provider;
                query_vm.UpdateWeatherSource(provider);
                wm.UpdateAPI();

                if (!wm.IsRegionSupported(query_vm.LocationCountry))
                {
                    throw new CustomException(App.ResLoader.GetString("error_message_weather_region_unsupported"));
                }

                ctsToken.ThrowIfCancellationRequested();

                // Weather Data
                var location = new LocationData(query_vm);
                if (!location.IsValid())
                {
                    throw new CustomException(App.ResLoader.GetString("werror_noweather"));
                }

                Weather weather = await Settings.GetWeatherData(location.query);
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
                    weather.weather_alerts = await wm.GetAlerts(location);
                }

                // Save weather data
                await Settings.DeleteLocations().ConfigureAwait(false);
                await Settings.AddLocation(location).ConfigureAwait(false);
                if (wm.SupportsAlerts && weather.weather_alerts != null)
                    await Settings.SaveWeatherAlerts(location, weather.weather_alerts).ConfigureAwait(false);
                await Settings.SaveWeatherData(weather).ConfigureAwait(false);
                await Settings.SaveWeatherForecasts(new Forecasts(weather)).ConfigureAwait(false);
                await Settings.SaveWeatherForecasts(location, weather.hr_forecast == null ? null :
                    weather.hr_forecast.Select(f => new HourlyForecasts(weather.query, f))).ConfigureAwait(false);

                // If we're using search
                // make sure gps feature is off
                Settings.FollowGPS = false;
                Settings.WeatherLoaded = true;

                return location;
            }, ctsToken).ContinueWith(async (t) =>
            {
                if (t.IsFaulted)
                {
                    Settings.FollowGPS = false;
                    Settings.WeatherLoaded = false;

                    var ex = t.Exception.GetBaseException();

                    await Dispatcher.RunOnUIThread(() =>
                    {
                        if (ex is not TaskCanceledException)
                        {
                            if (ex is WeatherException || ex is CustomException)
                            {
                                ShowSnackbar(Snackbar.MakeError(ex.Message, SnackbarDuration.Short));
                            }
                            else
                            {
                                ShowSnackbar(Snackbar.MakeError(App.ResLoader.GetString("error_retrieve_location"), SnackbarDuration.Short));
                            }
                        }

                        // Restore controls
                        EnableControls(true);
                    });
                }
                else if (t.IsCompletedSuccessfully && t.Result != null)
                {
                    await Dispatcher.RunOnUIThread(() =>
                    {
                        SetupPage.Instance.Location = t.Result;
                        SetupPage.Instance.Next();
                    });
                }
                else
                {
                    await Dispatcher.RunOnUIThread(() =>
                    {
                        // Restore controls
                        EnableControls(true);
                    });
                }
            });
        }

        private void RefreshSuggestionList(AutoSuggestBox sender)
        {
            // Refresh list
            sender.ItemsSource = null;
            sender.ItemsSource = LocationQuerys;
            sender.IsSuggestionListOpen = true;
        }

        private void EnableControls(bool Enable)
        {
            LocationSearchBox.IsEnabled = Enable;
            GPSButton.IsEnabled = Enable;
            LoadingRing.IsActive = !Enable;
        }

        private void GPS_Click(object sender, RoutedEventArgs e)
        {
            FetchGeoLocation();
        }

        /// <summary>
        /// FetchGeoLocation
        /// </summary>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        /// <exception cref="InvalidOperationException">Ignore.</exception>
        /// <exception cref="AggregateException">Ignore.</exception>
        /// <exception cref="TaskCanceledException">Ignore.</exception>
        /// <exception cref="WeatherException">Ignore.</exception>
        /// <exception cref="CustomException">Ignore.</exception>
        private void FetchGeoLocation()
        {
            GPSButton.IsEnabled = false;
            LoadingRing.IsActive = true;

            // Cancel other tasks
            cts?.Cancel();
            cts = new CancellationTokenSource(TimeSpan.FromSeconds(45));
            var ctsToken = cts.Token;

            if (geoPos == null)
            {
                UpdateLocation(ctsToken);
            }
            else
            {
                Task.Run(async () =>
                {
                    LocationQueryViewModel view = null;

                    ctsToken.ThrowIfCancellationRequested();

                    view = await Task.Run(async () => 
                    {
                        return await wm.GetLocation(geoPos).ConfigureAwait(false);
                    }, ctsToken);

                    if (String.IsNullOrWhiteSpace(view.LocationQuery))
                    {
                        // Stop since there is no valid query
                        throw new CustomException(App.ResLoader.GetString("error_retrieve_location"));
                    }
                    else if (String.IsNullOrEmpty(view.LocationTZLong) && view.LocationLat != 0 && view.LocationLong != 0)
                    {
                        String tzId = await Task.Run(async () => 
                        {
                            return await TZDB.TZDBCache.GetTimeZone(view.LocationLat, view.LocationLong).ConfigureAwait(false);
                        }, ctsToken).ConfigureAwait(false);
                        if (!String.IsNullOrWhiteSpace(tzId))
                            view.LocationTZLong = tzId;
                    }

                    // Set default provider based on location
                    var provider = RemoteConfig.RemoteConfig.GetDefaultWeatherProvider(view.LocationCountry);
                    Settings.API = provider;
                    view.UpdateWeatherSource(provider);
                    wm.UpdateAPI();

                    if (Settings.UsePersonalKey && String.IsNullOrWhiteSpace(Settings.API_KEY) && wm.KeyRequired)
                    {
                        throw new CustomException(App.ResLoader.GetString("werror_invalidkey"));
                    }

                    ctsToken.ThrowIfCancellationRequested();

                    if (!wm.IsRegionSupported(view.LocationCountry))
                    {
                        throw new CustomException(App.ResLoader.GetString("error_message_weather_region_unsupported"));
                    }

                    // Weather Data
                    var location = new LocationData(view, geoPos);
                    if (!location.IsValid())
                    {
                        throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
                    }

                    ctsToken.ThrowIfCancellationRequested();

                    Weather weather = await Settings.GetWeatherData(location.query).ConfigureAwait(false);
                    if (weather == null)
                    {
                        weather = await wm.GetWeather(location).ConfigureAwait(false);
                    }

                    if (weather == null)
                    {
                        throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
                    }
                    else if (wm.SupportsAlerts && wm.NeedsExternalAlertData)
                    {
                        weather.weather_alerts = await wm.GetAlerts(location).ConfigureAwait(false);
                    }

                    ctsToken.ThrowIfCancellationRequested();

                    // Save weather data
                    Settings.SaveLastGPSLocData(location);
                    await Settings.DeleteLocations().ConfigureAwait(false);
                    await Settings.AddLocation(new LocationData(view)).ConfigureAwait(false);
                    if (wm.SupportsAlerts && weather.weather_alerts != null)
                        await Settings.SaveWeatherAlerts(location, weather.weather_alerts).ConfigureAwait(false);
                    await Settings.SaveWeatherData(weather).ConfigureAwait(false);
                    await Settings.SaveWeatherForecasts(new Forecasts(weather)).ConfigureAwait(false);
                    await Settings.SaveWeatherForecasts(location, weather.hr_forecast == null ? null :
                        weather.hr_forecast.Select(f => new HourlyForecasts(weather.query, f))).ConfigureAwait(false);

                    Settings.FollowGPS = true;
                    Settings.WeatherLoaded = true;

                    return location;
                }, ctsToken).ContinueWith(async (t) =>
                {
                    if (t.IsFaulted)
                    {
                        Settings.FollowGPS = false;
                        Settings.WeatherLoaded = false;

                        var ex = t.Exception.GetBaseException();

                        await Dispatcher.RunOnUIThread(() =>
                        {
                            if (ex is WeatherException || ex is CustomException)
                            {
                                ShowSnackbar(Snackbar.MakeError(ex.Message, SnackbarDuration.Short));
                            }
                            else
                            {
                                ShowSnackbar(Snackbar.MakeError(App.ResLoader.GetString("error_retrieve_location"), SnackbarDuration.Short));
                            }

                            // Restore controls
                            EnableControls(true);
                        });
                    }
                    else if (t.IsCompletedSuccessfully && t.Result != null)
                    {
                        await Dispatcher.RunOnUIThread(() =>
                        {
                            SetupPage.Instance.Location = t.Result;
                            SetupPage.Instance.Next();
                        });
                    }
                    else
                    {
                        await Dispatcher.RunOnUIThread(() =>
                        {
                            // Restore controls
                            EnableControls(true);
                        });
                    }
                });
            }
        }

        /// <summary>
        /// UpdateLocation
        /// </summary>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        private void UpdateLocation(CancellationToken token)
        {
            Task.Run(async () =>
            {
                var geoStatus = GeolocationAccessStatus.Unspecified;

                // Catch error in case dialog is dismissed
                geoStatus = await Geolocator.RequestAccessAsync();

                Geolocator geolocal = new Geolocator() { DesiredAccuracyInMeters = 5000, ReportInterval = 900000, MovementThreshold = 1600 };

                switch (geoStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        geoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10));
                        break;
                }

                return geoStatus;
            }, token).ContinueWith(async (t) =>
            {
                await Dispatcher.RunOnUIThread(() =>
                {
                    // Restore controls
                    EnableControls(true);

                    if (t.IsFaulted || !t.IsCompletedSuccessfully || t.Result == GeolocationAccessStatus.Unspecified)
                    {
                        Settings.FollowGPS = false;
                        var ex = t.Exception?.GetBaseException();

                        if (ex != null && Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
                            ShowSnackbar(Snackbar.MakeError(App.ResLoader.GetString("werror_networkerror"), SnackbarDuration.Short));
                        else
                            ShowSnackbar(Snackbar.MakeError(App.ResLoader.GetString("error_retrieve_location"), SnackbarDuration.Short));
                    }

                    if (t.Result == GeolocationAccessStatus.Denied)
                    {
                        Settings.FollowGPS = false;
                        var snackbar = Snackbar.MakeError(App.ResLoader.GetString("Msg_LocDeniedSettings"), SnackbarDuration.Long);
                        snackbar.SetAction(App.ResLoader.GetString("action_settings"), async () =>
                        {
                            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
                        });
                        ShowSnackbar(snackbar);
                    }
                    else if (t.Result == GeolocationAccessStatus.Allowed && geoPos != null)
                    {
                        FetchGeoLocation();
                    }
                });
            });
        }

        public bool CanContinue()
        {
            return SetupPage.Instance.Location != null && SetupPage.Instance.Location.IsValid();
        }
    }
}