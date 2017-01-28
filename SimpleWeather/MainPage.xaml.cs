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

            // Save location query to List
            List<string> locations = new List<string>();
            locations.Add(selected_query);
            Settings.saveLocations(locations);

            wu_Loader = new WeatherUnderground.WeatherDataLoader(selected_query, homeIdx);

            await wu_Loader.loadWeatherData().ConfigureAwait(false);

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (wu_Loader.getWeather() != null)
                {
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

                sender.IsSuggestionListOpen = false;
            });
        }

        private async void Restore()
        {
            // Hide UIElements
            SearchGrid.Visibility = Visibility.Collapsed;
            // Show Loading Ring
            LoadingRing.IsActive = true;

            if (Settings.WeatherLoaded)
            {
                // Weather was loaded before. Lets load it up...
                var localSettings = ApplicationData.Current.LocalSettings;
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

                /* Yahoo Code
                                // Weather was loaded before. Lets load it up...
                                var localSettings = ApplicationData.Current.LocalSettings;
                                List<Coordinate> locations = await Settings.getLocations();
                                Coordinate local = locations[homeIdx];

                                wLoader = new WeatherDataLoader(local.ToString(), homeIdx);

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
                */
            }
            else
            {
                LoadingRing.IsActive = false;
                SearchGrid.Visibility = Visibility.Visible;
            }
        }

        private async void GPS_Click(object sender, RoutedEventArgs e)
        {
            // Set window items
            //            LoadingRing.IsActive = true;
            //            GPSButton.IsEnabled = false;
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
            /* Yahoo Code
                        wLoader = new WeatherDataLoader(geoPos, homeIdx);
                        await wLoader.loadWeatherData(true).ConfigureAwait(false);

                        if (wLoader.getWeather() != null)
                        {
                            // Show location name
                            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                Coordinate location = new Coordinate(
                                    string.Join(",", wLoader.getWeather().location.lat, wLoader.getWeather().location._long));
                                Location.Text = location.ToString();

                                // Save coords to List
                                List<Coordinate> locations = new List<Coordinate>();
                                locations.Add(location);
                                Settings.saveLocations(locations);

                                // Save WeatherLoader
                                if (CoreApplication.Properties.ContainsKey("WeatherLoader"))
                                {
                                    CoreApplication.Properties.Remove("WeatherLoader");
                                }
                                CoreApplication.Properties.Add("WeatherLoader", wLoader);

                                Settings.WeatherLoaded = true;
                                this.Frame.Navigate(typeof(Shell));
                            });
                        }
                        else
                        {
                            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                LoadingRing.IsActive = false;
                                SearchGrid.Visibility = Visibility.Visible;
            //                        GPS.IsEnabled = true;

                                Location.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                                Location.BorderThickness = new Thickness(5);
                            });
                        }

            */
        }
    }
}
