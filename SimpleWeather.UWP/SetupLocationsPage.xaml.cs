using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.WeatherData;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace SimpleWeather.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupLocationsPage : Page, IDisposable, IPageVerification
    {
        private CancellationTokenSource cts = new CancellationTokenSource();

        private WeatherManager wm;

        public ObservableCollection<LocationQueryViewModel> LocationQuerys { get; set; }

        public SetupLocationsPage()
        {
            this.InitializeComponent();

            this.SizeChanged += SetupPage_SizeChanged;

            wm = WeatherManager.GetInstance();

            // Views
            LocationQuerys = new ObservableCollection<LocationQueryViewModel>();
        }

        private void SetupPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeControls();
        }

        private void ResizeControls()
        {
            if (MainPanel != null)
            {
                if (this.ActualWidth > 640)
                    Location.Width = ActualWidth / 2;
                else
                    Location.Width = double.NaN;

                if (this.ActualHeight > 480)
                    AppLogo.Height = 150;
                else
                    AppLogo.Height = 100;
            }
        }

        public void Dispose()
        {
            cts.Dispose();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Restore();
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

            if (String.IsNullOrEmpty(sender.Text))
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
                if (cts.Token.IsCancellationRequested)
                {
                    EnableControls(true);
                    return;
                }

                // Use args.QueryText to determine what to do.
                var result = (await wm.GetLocations(args.QueryText)).First();

                if (cts.Token.IsCancellationRequested)
                {
                    EnableControls(true);
                    return;
                }

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
                EnableControls(true);
                return;
            }

            // Need to get FULL location data for HERE API
            // Data provided is incomplete
            if (WeatherAPI.Here.Equals(Settings.API)
                    && query_vm.LocationLat == -1 && query_vm.LocationLong == -1
                    && query_vm.LocationTZ_Long == null)
            {
                query_vm = await new HERE.HEREWeatherProvider().GetLocationFromLocID(query_vm.LocationQuery);
            }

            // Weather Data
            var location = new LocationData(query_vm);
            if (!location.IsValid())
            {
                await Toast.ShowToastAsync(App.ResLoader.GetString("WError_NoWeather"), ToastDuration.Short);
                EnableControls(true);
                return;
            }
            Weather weather = await Settings.GetWeatherData(location.query);
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
                EnableControls(true);
                return;
            }

            // We got our data so disable controls just in case
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                EnableControls(false);
                sender.IsSuggestionListOpen = false;
            });

            // Save weather data
            await Settings.DeleteLocations();
            await Settings.AddLocation(location);
            if (wm.SupportsAlerts && weather.weather_alerts != null)
                await Settings.SaveWeatherAlerts(location, weather.weather_alerts);
            await Settings.SaveWeatherData(weather);

            // If we're using search
            // make sure gps feature is off
            Settings.FollowGPS = false;
            Settings.WeatherLoaded = true;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SetupPage.Instance.Location = location;
                SetupPage.Instance.Next();
            });
        }

        private void EnableControls(bool Enable)
        {
            Location.IsEnabled = Enable;
            GPSButton.IsEnabled = Enable;
            LoadingRing.IsActive = !Enable;
        }

        private void Restore()
        {
            var mainPanel = FindName(nameof(MainPanel)) as FrameworkElement;
            mainPanel.Visibility = Visibility.Visible;

            // Sizing
            ResizeControls();

            // Set HERE as default API
            Settings.API = WeatherAPI.Here;
            wm.UpdateAPI();

            if (String.IsNullOrWhiteSpace(wm.GetAPIKey()))
            {
                // If (internal) key doesn't exist, fallback to Met.no
                Settings.API = WeatherAPI.MetNo;
                wm.UpdateAPI();
                Settings.UsePersonalKey = true;
                Settings.KeyVerified = false;
            }
            else
            {
                // If key exists, go ahead
                Settings.UsePersonalKey = false;
                Settings.KeyVerified = true;
            }
        }

        private async void GPS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                button.IsEnabled = false;
                LoadingRing.IsActive = true;

                // Cancel other tasks
                cts.Cancel();
                cts = new CancellationTokenSource();
                var ctsToken = cts.Token;

                ctsToken.ThrowIfCancellationRequested();

                var geoStatus = GeolocationAccessStatus.Unspecified;

                try
                {
                    // Catch error in case dialog is dismissed
                    geoStatus = await Geolocator.RequestAccessAsync();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "SetupPage: error requesting location permission");
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }

                ctsToken.ThrowIfCancellationRequested();

                Geolocator geolocal = new Geolocator() { DesiredAccuracyInMeters = 5000, ReportInterval = 900000, MovementThreshold = 1600 };
                Geoposition geoPos = null;

                // Setup error just in case
                MessageDialog error = null;

                switch (geoStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        try
                        {
                            geoPos = await geolocal.GetGeopositionAsync();
                        }
                        catch (Exception ex)
                        {
                            if (Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
                                error = new MessageDialog(App.ResLoader.GetString("WError_NetworkError"), App.ResLoader.GetString("Label_Error"));
                            else
                                error = new MessageDialog(App.ResLoader.GetString("Error_Location"), App.ResLoader.GetString("Label_ErrorLocation"));
                            await error.ShowAsync();

                            Logger.WriteLine(LoggerLevel.Error, ex, "SetupPage: error getting geolocation");
                        }
                        break;
                    case GeolocationAccessStatus.Denied:
                        error = new MessageDialog(App.ResLoader.GetString("Msg_LocDeniedSettings"), App.ResLoader.GetString("Label_ErrLocationDenied"));
                        error.Commands.Add(new UICommand(App.ResLoader.GetString("Label_Settings"), async (command) =>
                        {
                            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
                        }, 0));
                        error.Commands.Add(new UICommand(App.ResLoader.GetString("Label_Cancel"), null, 1));
                        error.DefaultCommandIndex = 0;
                        error.CancelCommandIndex = 1;
                        await error.ShowAsync();
                        break;
                    case GeolocationAccessStatus.Unspecified:
                    default:
                        error = new MessageDialog(App.ResLoader.GetString("Error_Location"), App.ResLoader.GetString("Label_ErrorLocation"));
                        await error.ShowAsync();
                        break;
                }

                // Access to location granted
                if (geoPos != null)
                {
                    LocationQueryViewModel view = null;

                    ctsToken.ThrowIfCancellationRequested();

                    button.IsEnabled = false;

                    await Task.Run(async () =>
                    {
                        ctsToken.ThrowIfCancellationRequested();

                        view = await wm.GetLocation(geoPos);

                        if (String.IsNullOrEmpty(view.LocationQuery))
                            view = new LocationQueryViewModel();
                    });

                    if (String.IsNullOrWhiteSpace(view.LocationQuery))
                    {
                        // Stop since there is no valid query
                        EnableControls(true);
                        return;
                    }

                    ctsToken.ThrowIfCancellationRequested();

                    // Weather Data
                    var location = new LocationData(view, geoPos);
                    if (!location.IsValid())
                    {
                        await Toast.ShowToastAsync(App.ResLoader.GetString("WError_NoWeather"), ToastDuration.Short);
                        EnableControls(true);
                        return;
                    }

                    ctsToken.ThrowIfCancellationRequested();

                    Weather weather = await Settings.GetWeatherData(location.query);
                    if (weather == null)
                    {
                        ctsToken.ThrowIfCancellationRequested();

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
                        EnableControls(true);
                        return;
                    }

                    ctsToken.ThrowIfCancellationRequested();

                    // We got our data so disable controls just in case
                    EnableControls(false);

                    // Save weather data
                    Settings.SaveLastGPSLocData(location);
                    await Settings.DeleteLocations();
                    await Settings.AddLocation(new LocationData(view));
                    if (wm.SupportsAlerts && weather.weather_alerts != null)
                        await Settings.SaveWeatherAlerts(location, weather.weather_alerts);
                    await Settings.SaveWeatherData(weather);

                    Settings.FollowGPS = true;
                    Settings.WeatherLoaded = true;

                    SetupPage.Instance.Location = location;
                    SetupPage.Instance.Next();
                }
                else
                {
                    EnableControls(true);
                }
            }
            catch (OperationCanceledException)
            {
                // Restore controls
                EnableControls(true);
                Settings.FollowGPS = false;
                Settings.WeatherLoaded = false;
            }
        }

        public bool CanContinue()
        {
            return SetupPage.Instance.Location != null && SetupPage.Instance.Location.IsValid();
        }
    }
}