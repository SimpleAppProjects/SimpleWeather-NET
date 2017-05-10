using SimpleWeather.Controls;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        // For UI Thread
        CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

        public ObservableCollection<LocationQueryView> LocationQuerys { get; set; }
        private string selected_query = string.Empty;

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
                    results = await WeatherUnderground.AutoCompleteQuery.getLocations(sender.Text);
                }
                catch (Exception ex)
                {
                    if (Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
                    {
                        MessageDialog error = new MessageDialog("Network Connection Error!!", "Error");
                        await error.ShowAsync();
                    }
                }

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
                    results = await WeatherUnderground.AutoCompleteQuery.getLocations(args.QueryText);
                }
                catch (Exception ex)
                {
                    if (Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
                    {
                        await new MessageDialog("Network Connection Error!!", "Error").ShowAsync();
                    }
                    return;
                }

                if (results.Count > 0)
                {
                    sender.Text = results.First().name;
                    selected_query = results.First().l;
                }
            }

            // Stop if using WeatherUnderground and API Key is empty
            if (String.IsNullOrWhiteSpace(Settings.API_KEY) && Settings.API == "WUnderground")
            {
                TextBlock header = KeyEntry.Header as TextBlock;
                header.Visibility = Visibility.Visible;
                KeyEntry.BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
                KeyEntry.BorderThickness = new Thickness(2);
                return;
            }

            KeyValuePair<int, object> pair;

            if (Settings.API == "WUnderground")
            {
                // Weather Data
                OrderedDictionary weatherData = await Settings.getWeatherData();

                WeatherUnderground.Weather weather = await WeatherUnderground.WeatherLoaderTask.getWeather(selected_query);

                if (weather == null)
                    return;

                // Save weather data
                if (weatherData.Contains(selected_query))
                    weatherData[selected_query] = weather;
                else
                    weatherData.Add(selected_query, weather);
                Settings.saveWeatherData(weatherData);

                pair = new KeyValuePair<int, object>(App.HomeIdx, selected_query);
            }
            else
            {
                // Weather Data
                OrderedDictionary weatherData = await Settings.getWeatherData();

                WeatherYahoo.Weather weather = await WeatherYahoo.WeatherLoaderTask.getWeather(sender.Text);

                if (weather == null)
                    return;

                WeatherUtils.Coordinate local = new WeatherUtils.Coordinate(
                    String.Format("{0}, {1}", weather.location.lat, weather.location._long));

                // Save weather data
                if (weatherData.Contains(local.ToString()))
                    weatherData[local.ToString()] = weather;
                else
                    weatherData.Add(local.ToString(), weather);
                Settings.saveWeatherData(weatherData);

                pair = new KeyValuePair<int, object>(App.HomeIdx, local.ToString());
            }

            Settings.WeatherLoaded = true;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.Frame.Navigate(typeof(Shell), pair));
            sender.IsSuggestionListOpen = false;
        }

        private async void Restore()
        {
            // Hide UIElements
            SearchGrid.Visibility = Visibility.Collapsed;
            // Show Loading Ring
            LoadingRing.IsActive = true;

            // Check for key
            if (!String.IsNullOrEmpty(Settings.API_KEY))
                KeyEntry.Text = Settings.API_KEY;

            if (Settings.WeatherLoaded)
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.Frame.Navigate(typeof(Shell)));
            }
            else
            {
                LoadingRing.IsActive = false;
                SearchGrid.Visibility = Visibility.Visible;
                // Set WUnderground as default API
                APIComboBox.SelectedIndex = 0;
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
                        await error.ShowAsync();
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

        private void APIComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                Settings.API_KEY = KeyEntry.Text;
        }

        private void KeyEntry_GotFocus(object sender, RoutedEventArgs e)
        {
            KeyEntry.BorderThickness = new Thickness(0);
        }
    }
}