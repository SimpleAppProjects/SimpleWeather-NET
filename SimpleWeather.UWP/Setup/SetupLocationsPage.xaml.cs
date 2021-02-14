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

            this.SizeChanged += SetupPage_SizeChanged;

            wm = WeatherManager.GetInstance();

            // Views
            LocationQuerys = new ObservableCollection<LocationQueryViewModel>();
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

        private void SetupPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeControls();
        }

        private void ResizeControls()
        {
            if (this.ActualWidth <= 640)
                Location.MaxWidth = this.ActualWidth;
            else if (this.ActualWidth <= 1080)
                Location.MaxWidth = this.ActualWidth * (0.75);
            else
                Location.MaxWidth = this.ActualWidth * (0.50);

            if (this.ActualHeight > 480)
                AppLogo.Height = 150;
            else
                AppLogo.Height = 100;
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
            Restore();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            UnloadSnackManager();
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
            // user is typing: reset already started timer (if existing)
            if (timer != null && timer.IsEnabled)
                timer.Stop();

            if (String.IsNullOrEmpty(sender.Text))
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
                    // Use args.QueryText to determine what to do.
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

                if (String.IsNullOrWhiteSpace(query_vm?.LocationQuery))
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
                        Settings.API = WeatherAPI.WeatherUnlocked;
                        query_vm.UpdateWeatherSource(WeatherAPI.WeatherUnlocked);
                    }
                    wm.UpdateAPI();
                }

                if (WeatherAPI.NWS.Equals(Settings.API) && !isUS)
                {
                    throw new CustomException(App.ResLoader.GetString("Error_WeatherUSOnly"));
                }

                ctsToken.ThrowIfCancellationRequested();

                // Weather Data
                var location = new LocationData(query_vm);
                if (!location.IsValid())
                {
                    throw new CustomException(App.ResLoader.GetString("WError_NoWeather"));
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
                await Settings.DeleteLocations();
                await Settings.AddLocation(location);
                if (wm.SupportsAlerts && weather.weather_alerts != null)
                    await Settings.SaveWeatherAlerts(location, weather.weather_alerts);
                await Settings.SaveWeatherData(weather);
                await Settings.SaveWeatherForecasts(new Forecasts(weather.query, weather.forecast, weather.txt_forecast));
                await Settings.SaveWeatherForecasts(location, weather.hr_forecast == null ? null :
                    weather.hr_forecast.Select(f => new HourlyForecasts(weather.query, f)));

                // If we're using search
                // make sure gps feature is off
                Settings.FollowGPS = false;
                Settings.WeatherLoaded = true;

                return location;
            }).ContinueWith((t) =>
            {
                // Restore controls
                EnableControls(true);

                if (t.IsFaulted)
                {
                    Settings.FollowGPS = false;
                    Settings.WeatherLoaded = false;

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
                else if (t.IsCompletedSuccessfully && t.Result != null)
                {
                    SetupPage.Instance.Location = t.Result;
                    SetupPage.Instance.Next();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(true);
        }

        private void EnableControls(bool Enable)
        {
            Location.IsEnabled = Enable;
            GPSButton.IsEnabled = Enable;
            LoadingRing.IsActive = !Enable;
        }

        private void Restore()
        {
            // Sizing
            ResizeControls();
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
            cts = new CancellationTokenSource();
            var ctsToken = cts.Token;

            if (geoPos == null)
            {
                UpdateLocation();
            }
            else
            {
                Task.Run(async () =>
                {
                    LocationQueryViewModel view = null;

                    ctsToken.ThrowIfCancellationRequested();

                    view = await wm.GetLocation(geoPos);

                    if (String.IsNullOrWhiteSpace(view.LocationQuery))
                    {
                        // Stop since there is no valid query
                        throw new CustomException(App.ResLoader.GetString("Error_Location"));
                    }
                    else if (String.IsNullOrEmpty(view.LocationTZLong) && view.LocationLat != 0 && view.LocationLong != 0)
                    {
                        String tzId = await TZDB.TZDBCache.GetTimeZone(view.LocationLat, view.LocationLong);
                        if (!String.IsNullOrWhiteSpace(tzId))
                            view.LocationTZLong = tzId;
                    }

                    bool isUS = LocationUtils.IsUS(view.LocationCountry);

                    if (!Settings.WeatherLoaded)
                    {
                        // Default US location to NWS
                        if (isUS)
                        {
                            Settings.API = WeatherAPI.NWS;
                            view.UpdateWeatherSource(WeatherAPI.NWS);
                        }
                        else
                        {
                            Settings.API = WeatherAPI.WeatherUnlocked;
                            view.UpdateWeatherSource(WeatherAPI.WeatherUnlocked);
                        }
                        wm.UpdateAPI();
                    }

                    if (Settings.UsePersonalKey && String.IsNullOrWhiteSpace(Settings.API_KEY) && wm.KeyRequired)
                    {
                        throw new CustomException(App.ResLoader.GetString("WError_InvalidKey"));
                    }

                    ctsToken.ThrowIfCancellationRequested();

                    if (WeatherAPI.NWS.Equals(Settings.API) && !isUS)
                    {
                        throw new CustomException(App.ResLoader.GetString("Error_WeatherUSOnly"));
                    }

                    // Weather Data
                    var location = new LocationData(view, geoPos);
                    if (!location.IsValid())
                    {
                        throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
                    }

                    ctsToken.ThrowIfCancellationRequested();

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

                    ctsToken.ThrowIfCancellationRequested();

                    // Save weather data
                    Settings.SaveLastGPSLocData(location);
                    await Settings.DeleteLocations();
                    await Settings.AddLocation(new LocationData(view));
                    if (wm.SupportsAlerts && weather.weather_alerts != null)
                        await Settings.SaveWeatherAlerts(location, weather.weather_alerts);
                    await Settings.SaveWeatherData(weather);
                    await Settings.SaveWeatherForecasts(new Forecasts(weather.query, weather.forecast, weather.txt_forecast));
                    await Settings.SaveWeatherForecasts(location, weather.hr_forecast == null ? null :
                        weather.hr_forecast.Select(f => new HourlyForecasts(weather.query, f)));

                    Settings.FollowGPS = true;
                    Settings.WeatherLoaded = true;

                    return location;
                }).ContinueWith((t) =>
                {
                    // Restore controls
                    EnableControls(true);

                    if (t.IsFaulted)
                    {
                        Settings.FollowGPS = false;
                        Settings.WeatherLoaded = false;

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
                    else if (t.IsCompletedSuccessfully && t.Result != null)
                    {
                        SetupPage.Instance.Location = t.Result;
                        SetupPage.Instance.Next();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(true);
            }
        }

        /// <summary>
        /// UpdateLocation
        /// </summary>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        private void UpdateLocation()
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
            }).ContinueWith((t) =>
            {
                // Restore controls
                EnableControls(true);

                if (t.IsFaulted || !t.IsCompletedSuccessfully || t.Result == GeolocationAccessStatus.Unspecified)
                {
                    Settings.FollowGPS = false;
                    var ex = t.Exception?.GetBaseException();

                    if (ex != null && Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
                        ShowSnackbar(Snackbar.Make(App.ResLoader.GetString("WError_NetworkError"), SnackbarDuration.Short));
                    else
                        ShowSnackbar(Snackbar.Make(App.ResLoader.GetString("Error_Location"), SnackbarDuration.Short));
                }

                if (t.Result == GeolocationAccessStatus.Denied)
                {
                    Settings.FollowGPS = false;
                    var snackbar = Snackbar.Make(App.ResLoader.GetString("Msg_LocDeniedSettings"), SnackbarDuration.Long);
                    snackbar.SetAction(App.ResLoader.GetString("Label_Settings"), async () =>
                    {
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
                    });
                    ShowSnackbar(snackbar);
                }
                else if (t.Result == GeolocationAccessStatus.Allowed && geoPos != null)
                {
                    FetchGeoLocation();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(true);
        }

        public bool CanContinue()
        {
            return SetupPage.Instance.Location != null && SetupPage.Instance.Location.IsValid();
        }
    }
}