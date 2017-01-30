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
                List<WeatherUnderground.AC_Location> results = await WeatherUnderground.AutoCompleteQuery.getLocations(sender.Text).ConfigureAwait(false);

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
                List<WeatherUnderground.AC_Location> results = await WeatherUnderground.AutoCompleteQuery.getLocations(args.QueryText).ConfigureAwait(false);

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
            if (String.IsNullOrWhiteSpace(Settings.API_KEY) && Settings.API == "WUnderground")
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

                await wu_Loader.loadWeatherData().ConfigureAwait(false);
            }
            else
            {
                // Save location query to List
                List<WeatherYahoo.Coordinate> locations = new List<WeatherYahoo.Coordinate>();

                wLoader = new WeatherYahoo.WeatherDataLoader(sender.Text, homeIdx);
                await wLoader.loadWeatherData().ConfigureAwait(false);
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
                    else
                        throw new Exception("Weather is null");
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
                    else
                        throw new Exception("Weather is null");
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
                KeyEntry.Text = Settings.API_KEY;

            if (Settings.WeatherLoaded)
            {
                // Weather was loaded before. Lets load it up...
                var localSettings = ApplicationData.Current.LocalSettings;
                if (Settings.API == "WUnderground")
                {
                    List<string> locations = await Settings.getLocations_WU();
                    string local = locations[homeIdx];

                    wu_Loader = new WeatherUnderground.WeatherDataLoader(local, homeIdx);

                    await wu_Loader.loadWeatherData().ConfigureAwait(false);

                    if (wu_Loader.getWeather() != null)
                    {
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
                        throw new Exception("Weather is null");
                }
                else
                {
                    List<WeatherYahoo.Coordinate> locations = await Settings.getLocations();
                    WeatherYahoo.Coordinate local = locations[homeIdx];

                    wLoader = new WeatherYahoo.WeatherDataLoader(local.ToString(), homeIdx);

                    await wLoader.loadWeatherData().ConfigureAwait(false);

                    if (wLoader.getWeather() != null)
                    {
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
                    else
                        throw new Exception("Weather is null");
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

            Geolocator geolocal = new Geolocator();
            Geoposition geoPos = await geolocal.GetGeopositionAsync();

            button.IsEnabled = false;

            WeatherUnderground.location gpsLocation = await WeatherUnderground.GeopositionQuery.getLocation(geoPos);
            LocationQueryView view = new LocationQueryView(gpsLocation);

            LocationQuerys.Clear();
            LocationQuerys.Add(view);

            // Refresh list
            Location.ItemsSource = null;
            Location.ItemsSource = LocationQuerys;

            Location.IsSuggestionListOpen = true;
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