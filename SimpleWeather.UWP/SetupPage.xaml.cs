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
    public sealed partial class SetupPage : Page, IDisposable
    {
        private CancellationTokenSource cts = new CancellationTokenSource();

        private WeatherManager wm;

        public ObservableCollection<LocationQueryViewModel> LocationQuerys { get; set; }

        public SetupPage()
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
                if (Math.Abs(MainPanel.ActualHeight - this.ActualHeight) <= 100)
                {
                    MainPanel.Margin = new Thickness(0);
                    MainPanel.VerticalAlignment = VerticalAlignment.Center;
                }
                else
                {
                    MainPanel.Margin = new Thickness(0, (this.ActualHeight / 8), 0, 0);
                    MainPanel.VerticalAlignment = VerticalAlignment.Top;
                }

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
            ((IDisposable)cts).Dispose();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Restore();
        }

        private void Location_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Cancel pending searches
            cts.Cancel();
            cts = new CancellationTokenSource();

            if (!String.IsNullOrWhiteSpace(sender.Text) && args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                String query = sender.Text;

                Task.Run(async () =>
                {
                    if (cts.IsCancellationRequested) return;

                    var results = await wm.GetLocations(query);

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
                LocationQueryViewModel theChosenOne = args.ChosenSuggestion as LocationQueryViewModel;

                if (!String.IsNullOrEmpty(theChosenOne.LocationQuery))
                    query_vm = theChosenOne;
                else
                    query_vm = new LocationQueryViewModel();
            }
            else if (!String.IsNullOrEmpty(args.QueryText))
            {
                // Use args.QueryText to determine what to do.
                LocationQueryViewModel result = (await wm.GetLocations(args.QueryText)).First();

                if (result != null && !String.IsNullOrWhiteSpace(result.LocationQuery))
                {
                    sender.Text = result.LocationName;
                    query_vm = result;
                }
                else
                    query_vm = new LocationQueryViewModel();
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

            // Stop if using provider that req's a key and is empty
            if (String.IsNullOrWhiteSpace(Settings.API_KEY) && wm.KeyRequired)
            {
                TextBlock header = KeyEntry.Header as TextBlock;
                header.Visibility = Visibility.Visible;
                KeyEntry.BorderBrush = new SolidColorBrush(Colors.Red);
                KeyEntry.BorderThickness = new Thickness(2);
                return;
            }

            // Cancel other tasks
            cts.Cancel();
            cts = new CancellationTokenSource();

            LoadingRing.IsActive = true;

            if (cts.IsCancellationRequested)
            {
                EnableControls(true);
                return;
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
                this.Frame.Navigate(typeof(Shell), location);
            });
        }

        private void EnableControls(bool Enable)
        {
            Location.IsEnabled = Enable;
            GPSButton.IsEnabled = Enable;
            APIComboBox.IsEnabled = Enable;
            KeyEntry.IsEnabled = Enable;
            LoadingRing.IsActive = !Enable;
        }

        private void Restore()
        {
            var mainPanel = FindName("MainPanel") as FrameworkElement;
            mainPanel.Visibility = Visibility.Visible;

            // Sizing
            ResizeControls();

            APIComboBox.ItemsSource = WeatherAPI.APIs;
            APIComboBox.DisplayMemberPath = "Display";
            APIComboBox.SelectedValuePath = "Value";

            // Check for key
            if (!String.IsNullOrEmpty(Settings.API_KEY))
                KeyEntry.Text = Settings.API_KEY;
            else
                KeyEntry.Text = String.Empty;

            SearchGrid.Visibility = Visibility.Visible;
            // Set Yahoo as default API
            APIComboBox.SelectedValue = WeatherAPI.Yahoo;
        }

        private async void GPS_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            LoadingRing.IsActive = true;

            // Cancel other tasks
            cts.Cancel();
            cts = new CancellationTokenSource();

            if (cts.IsCancellationRequested)
            {
                EnableControls(true);
                return;
            }

            GeolocationAccessStatus geoStatus = GeolocationAccessStatus.Unspecified;

            try
            {
                // Catch error in case dialog is dismissed
                geoStatus = await Geolocator.RequestAccessAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            if (cts.IsCancellationRequested)
            {
                EnableControls(true);
                return;
            }

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

                        System.Diagnostics.Debug.WriteLine(ex.StackTrace);
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
                    error = new MessageDialog(App.ResLoader.GetString("Error_Location"), App.ResLoader.GetString("Label_ErrorLocation"));
                    await error.ShowAsync();
                    break;
            }

            // Access to location granted
            if (geoPos != null)
            {
                LocationQueryViewModel view = null;

                if (cts.IsCancellationRequested)
                {
                    EnableControls(true);
                    return;
                }

                button.IsEnabled = false;

                await Task.Run(async () =>
                {
                    if (cts.IsCancellationRequested)
                    {
                        EnableControls(true);
                        return;
                    }

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

                // Stop if using provider that req's a key and is empty
                if (String.IsNullOrWhiteSpace(Settings.API_KEY) && wm.KeyRequired)
                {
                    TextBlock header = KeyEntry.Header as TextBlock;
                    header.Visibility = Visibility.Visible;
                    KeyEntry.BorderBrush = new SolidColorBrush(Colors.Red);
                    KeyEntry.BorderThickness = new Thickness(2);

                    EnableControls(true);
                    return;
                }

                if (cts.IsCancellationRequested)
                {
                    EnableControls(true);
                    return;
                }

                // Weather Data
                var location = new LocationData(view, geoPos);
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

                this.Frame.Navigate(typeof(Shell), location);
            }
            else
                EnableControls(true);
        }

        private void APIComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;

            Settings.API = box.SelectedValue.ToString();
            wm.UpdateAPI();

            if (wm.KeyRequired)
            {
                if (KeyEntry != null)
                    KeyEntry.Visibility = Visibility.Visible;
                if (RegisterKeyButton != null)
                    RegisterKeyButton.Visibility = Visibility.Visible;
            }
            else
            {
                if (KeyEntry != null)
                    KeyEntry.Visibility = Visibility.Collapsed;
                if (RegisterKeyButton != null)
                    RegisterKeyButton.Visibility = Visibility.Collapsed;
                Settings.API_KEY = KeyEntry.Text = String.Empty;
            }

            UpdateRegisterLink();
        }

        private void KeyEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(KeyEntry.Text) && KeyEntry.IsEnabled)
                Settings.API_KEY = KeyEntry.Text;
        }

        private void KeyEntry_GotFocus(object sender, RoutedEventArgs e)
        {
            KeyEntry.BorderThickness = new Thickness(0);
        }

        private void UpdateRegisterLink()
        {
            string API = APIComboBox?.SelectedValue?.ToString();

            switch (API)
            {
                case WeatherAPI.WeatherUnderground:
                case WeatherAPI.OpenWeatherMap:
                    RegisterKeyButton.NavigateUri =
                        new Uri(WeatherAPI.APIs.First(prov => prov.Value == API).APIRegisterURL);
                    break;
                default:
                    RegisterKeyButton.NavigateUri =
                        new Uri(WeatherAPI.APIs.First(prov => prov.Value == API).MainURL);
                    break;
            }
        }
    }
}