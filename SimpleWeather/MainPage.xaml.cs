using SimpleWeather.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimpleWeather
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        WeatherYahoo.WeatherDataLoader wLoader = null;
        WeatherUnderground.WeatherDataLoader wu_Loader = null;
        int homeIdx = 0;

        // For UI Thread
        CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

        public ObservableCollection<LocationQueryView> LocationQuerys { get; set; }
        private string selected_query = string.Empty;
        private string key = string.Empty;

        public MainPage()
        {
            this.InitializeComponent();

            // Views
            LocationQuerys = new ObservableCollection<LocationQueryView>();

            // TitleBar
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                // Mobile
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
            else
            {
                // Desktop
                var titlebar = ApplicationView.GetForCurrentView().TitleBar;
                titlebar.BackgroundColor = App.AppColor;
                titlebar.ButtonBackgroundColor = titlebar.BackgroundColor;
            }

            // Restore Weather if Location already set
            Restore();
        }

        private async void Location_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (!String.IsNullOrWhiteSpace(sender.Text) && args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                List<WeatherUnderground.AC_Location> results = new List<WeatherUnderground.AC_Location>(0);
                try
                {
                    results = await WeatherUnderground.AutoCompleteQuery.getLocations(sender.Text).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
                    {
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            MessageDialog error = new MessageDialog("Network Connection Error!!", "Error");
                            await error.ShowAsync();
                        });
                    }
                }

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    LocationQuerys.Clear();

                    // Show message if no results are found
                    if (results.Count == 0)
                    {
                        LocationQueryView noresults = new LocationQueryView();
                        noresults.LocationName = "No results found";
                        LocationQuerys.Add(noresults);
                    }
                    else
                    {
                        // Limit amount of results shown
                        int maxResults = 10;

                        foreach (WeatherUnderground.AC_Location location in results)
                        {
                            LocationQueryView view = new LocationQueryView(location);
                            LocationQuerys.Add(view);

                            // Limit amount of results
                            maxResults--;
                            if (maxResults <= 0)
                                break;
                        }
                    }

                    // Refresh list
                    Location.ItemsSource = null;
                    Location.ItemsSource = LocationQuerys;

                    sender.IsSuggestionListOpen = true;
                });
            }
            else if (String.IsNullOrWhiteSpace(sender.Text))
            {
                // Hide flyout if query is empty or null
                sender.IsSuggestionListOpen = false;
            }
        }

        private void Location_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            LocationQueryView theChosenOne = args.SelectedItem as LocationQueryView;

            if (theChosenOne != null)
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
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                LocationQueryView theChosenOne = args.ChosenSuggestion as LocationQueryView;

                if (!String.IsNullOrEmpty(theChosenOne.LocationQuery))
                    selected_query = theChosenOne.LocationQuery;
            }
            else if (!String.IsNullOrEmpty(args.QueryText))
            {
                // Use args.QueryText to determine what to do.
                List<WeatherUnderground.AC_Location> results = new List<WeatherUnderground.AC_Location>(0);
                try
                {
                    results = await WeatherUnderground.AutoCompleteQuery.getLocations(args.QueryText).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
                    {
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            MessageDialog error =
                            new MessageDialog("Network Connection Error!!", "Error");
                            await error.ShowAsync();
                        });
                    }
                    return;
                }

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (results.Count > 0)
                    {
                        sender.Text = results.First().name;
                        selected_query = results.First().l;
                    }
                });
            }

            // Stop if using WeatherUnderground and API Key is empty
            if (String.IsNullOrWhiteSpace(key) && Settings.API == "WUnderground")
            {
                TextBlock header = KeyEntry.Header as TextBlock;
                header.Visibility = Visibility.Visible;
                KeyEntry.BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
                KeyEntry.BorderThickness = new Thickness(2);
                return;
            }

            if (Settings.API == "WUnderground")
            {
                // Save location query to List
                List<string> locations = new List<string>();
                locations.Add(selected_query);
                Settings.saveLocations(locations);

                wu_Loader = new WeatherUnderground.WeatherDataLoader(selected_query, homeIdx);
                WeatherUtils.ErrorStatus ret = await wu_Loader.loadWeatherData().ConfigureAwait(false);

                // Error?
                if(wu_Loader.getWeather() == null)
                {
                    MessageDialog error = 
                        new MessageDialog("Unable to load weather data!!", "Error");
                    switch (ret)
                    {
                        case WeatherUtils.ErrorStatus.NETWORKERROR:
                            error.Content = "Network Connection Error!!";
                            break;
                        case WeatherUtils.ErrorStatus.QUERYNOTFOUND:
                            error.Content = "No cities match your search query";
                            break;
                        case WeatherUtils.ErrorStatus.INVALIDAPIKEY:
                            error.Content = "Invalid API Key";
                            break;
                        default:
                            break;
                    }
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await error.ShowAsync());
                    return;
                }
            }
            else
            {
                // Save location query to List
                List<WeatherYahoo.Coordinate> locations = new List<WeatherYahoo.Coordinate>();

                wLoader = new WeatherYahoo.WeatherDataLoader(sender.Text, homeIdx);
                WeatherUtils.ErrorStatus ret = await wLoader.loadWeatherData().ConfigureAwait(false);

                // Error?
                if (wLoader.getWeather() == null)
                {
                    MessageDialog error =
                        new MessageDialog("Unable to load weather data!!", "Error");
                    switch (ret)
                    {
                        case WeatherUtils.ErrorStatus.NETWORKERROR:
                            error.Content = "Network Connection Error!!";
                            break;
                        case WeatherUtils.ErrorStatus.QUERYNOTFOUND:
                            error.Content = "No cities match your search query";
                            break;
                        case WeatherUtils.ErrorStatus.INVALIDAPIKEY:
                            error.Content = "Invalid API Key";
                            break;
                        default:
                            break;
                    }
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await error.ShowAsync());
                    return;
                }

                WeatherYahoo.Coordinate local = new WeatherYahoo.Coordinate(
                    String.Format("{0}, {1}", wLoader.getWeather().location.lat, wLoader.getWeather().location._long));

                locations.Add(local);
                Settings.saveLocations(locations);
            }

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (Settings.API == "WUnderground")
                {
                    if (wu_Loader.getWeather() != null)
                    {
                        // Save API_KEY
                        Settings.API_KEY = key;

                        // Save WeatherLoader
                        if (CoreApplication.Properties.ContainsKey("WeatherLoader"))
                        {
                            CoreApplication.Properties.Remove("WeatherLoader");
                        }
                        CoreApplication.Properties.Add("WeatherLoader", wu_Loader);

                        Settings.WeatherLoaded = true;
                        this.Frame.Navigate(typeof(Shell));
                    }
                }
                else
                {
                    if (wLoader.getWeather() != null)
                    {
                        // Save WeatherLoader
                        if (CoreApplication.Properties.ContainsKey("WeatherLoader"))
                        {
                            CoreApplication.Properties.Remove("WeatherLoader");
                        }
                        CoreApplication.Properties.Add("WeatherLoader", wLoader);

                        Settings.WeatherLoaded = true;
                        this.Frame.Navigate(typeof(Shell));
                    }
                }

                sender.IsSuggestionListOpen = false;
            });
        }

        private async void Restore()
        {
            // Hide UIElements
            SearchGrid.Visibility = Visibility.Collapsed;
            // Show Loading Ring
            LoadingRing.IsActive = true;

            // Check for key
            if (!String.IsNullOrEmpty(Settings.API_KEY))
                KeyEntry.Text = key = Settings.API_KEY;

            if (Settings.WeatherLoaded)
            {
                // Weather was loaded before. Lets load it up...
                var localSettings = ApplicationData.Current.LocalSettings;
                if (Settings.API == "WUnderground")
                {
                    List<string> locations = await Settings.getLocations_WU();
                    string local = locations[homeIdx];

                    wu_Loader = new WeatherUnderground.WeatherDataLoader(local, homeIdx);

                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (CoreApplication.Properties.ContainsKey("WeatherLoader"))
                        {
                            CoreApplication.Properties.Remove("WeatherLoader");
                        }
                        CoreApplication.Properties.Add("WeatherLoader", wu_Loader);

                        this.Frame.Navigate(typeof(Shell));
                    });
                }
                else
                {
                    List<WeatherYahoo.Coordinate> locations = await Settings.getLocations();
                    WeatherYahoo.Coordinate local = locations[homeIdx];

                    wLoader = new WeatherYahoo.WeatherDataLoader(local.ToString(), homeIdx);

                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (CoreApplication.Properties.ContainsKey("WeatherLoader"))
                        {
                            CoreApplication.Properties.Remove("WeatherLoader");
                        }
                        CoreApplication.Properties.Add("WeatherLoader", wLoader);

                        this.Frame.Navigate(typeof(Shell));
                    });
                }
            }
            else
            {
                LoadingRing.IsActive = false;
                SearchGrid.Visibility = Visibility.Visible;
            }
        }

        private async void GPS_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;

            GeolocationAccessStatus geoStatus = await Geolocator.RequestAccessAsync();
            Geolocator geolocal = new Geolocator();
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
                            error = new MessageDialog("Network Connection Error!!", "Error");
                        else
                            error = new MessageDialog("Unable to retrieve location status", "Location access error");
                        await error.ShowAsync();
                    }
                    break;
                case GeolocationAccessStatus.Denied:
                    error = new MessageDialog("Access to location was denied. Please enable in Settings.", "Location access denied");
                    error.Commands.Add(new UICommand("Settings", async (command) =>
                    {
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
                    }, 0));
                    error.Commands.Add(new UICommand("Cancel", null, 1));
                    error.DefaultCommandIndex = 0;
                    error.CancelCommandIndex = 1;
                    await error.ShowAsync();
                    break;
                case GeolocationAccessStatus.Unspecified:
                    error = new MessageDialog("Unable to retrieve location status", "Location access error");
                    await error.ShowAsync();
                    break;
            }
            
            // Access to location granted
            if (geoPos != null)
            {
                button.IsEnabled = false;

                WeatherUnderground.location gpsLocation = null;
                try
                {
                    gpsLocation = await WeatherUnderground.GeopositionQuery.getLocation(geoPos);
                }
                catch (Exception ex)
                {
                    if (Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
                    {
                        error = new MessageDialog("Network Connection Error!!", "Error");
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await error.ShowAsync());
                    }
                }

                if (gpsLocation != null)
                {
                    LocationQueryView view = new LocationQueryView(gpsLocation);

                    LocationQuerys.Clear();
                    LocationQuerys.Add(view);

                    // Refresh list
                    Location.ItemsSource = null;
                    Location.ItemsSource = LocationQuerys;

                    Location.IsSuggestionListOpen = true;
                }
            }

            button.IsEnabled = true;
        }

        private void APIComboBox_DropDownClosed(object sender, object e)
        {
            ComboBox box = sender as ComboBox;
            int index = box.SelectedIndex;

            if (index == 0)
            {
                // WeatherUnderground
                if (KeyEntry != null)
                    KeyEntry.Visibility = Visibility.Visible;
                Settings.API = "WUnderground";
            }
            else if (index == 1)
            {
                // Yahoo Weather
                if (KeyEntry != null)
                    KeyEntry.Visibility = Visibility.Collapsed;
                Settings.API = "Yahoo";
            }
        }

        private void KeyEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(KeyEntry.Text) && KeyEntry.IsEnabled)
                key = KeyEntry.Text;
        }

        private void KeyEntry_GotFocus(object sender, RoutedEventArgs e)
        {
            KeyEntry.BorderThickness = new Thickness(0);
        }
    }
}