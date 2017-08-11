using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace SimpleWeather.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IDisposable
    {
        private CancellationTokenSource cts = new CancellationTokenSource();

        public ObservableCollection<LocationQueryViewModel> LocationQuerys { get; set; }
        private string selected_query = string.Empty;

        public MainPage()
        {
            this.InitializeComponent();

            // Views
            LocationQuerys = new ObservableCollection<LocationQueryViewModel>();

            // TitleBar
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                // Mobile
                StatusBar.GetForCurrentView().BackgroundOpacity = 1;
                StatusBar.GetForCurrentView().BackgroundColor = App.AppColor;
                StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
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

        public void Dispose()
        {
            ((IDisposable)cts).Dispose();
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

                    var results = await AutoCompleteQuery.GetLocations(query);

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
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                LocationQueryViewModel theChosenOne = args.ChosenSuggestion as LocationQueryViewModel;

                if (!String.IsNullOrEmpty(theChosenOne.LocationQuery))
                    selected_query = theChosenOne.LocationQuery;
                else
                    selected_query = string.Empty;
            }
            else if (!String.IsNullOrEmpty(args.QueryText))
            {
                // Use args.QueryText to determine what to do.
                LocationQueryViewModel result = (await AutoCompleteQuery.GetLocations(args.QueryText)).First();

                if (result != null && String.IsNullOrWhiteSpace(result.LocationQuery))
                {
                    sender.Text = result.LocationName;
                    selected_query = result.LocationQuery;
                }
            }
            else if (String.IsNullOrWhiteSpace(args.QueryText))
            {
                // Stop since there is no valid query
                return;
            }

            if (String.IsNullOrWhiteSpace(selected_query))
            {
                // Stop since there is no valid query
                return;
            }

            // Stop if using WeatherUnderground and API Key is empty
            if (String.IsNullOrWhiteSpace(Settings.API_KEY) && Settings.API == Settings.API_WUnderground)
            {
                TextBlock header = KeyEntry.Header as TextBlock;
                header.Visibility = Visibility.Visible;
                KeyEntry.BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
                KeyEntry.BorderThickness = new Thickness(2);
                return;
            }

            // Show loading dialog
            await LoadingDialog.ShowAsync();

            KeyValuePair<int, string> pair;

            // Weather Data
            OrderedDictionary weatherData = await Settings.GetWeatherData();

            Weather weather = await WeatherLoaderTask.GetWeather(selected_query);

            if (weather == null)
            {
                // Hide dialog
                await LoadingDialog.HideAsync();
                return;
            }

            // Save weather data
            if (weatherData.Contains(selected_query))
                weatherData[selected_query] = weather;
            else
                weatherData.Add(selected_query, weather);
            Settings.SaveWeatherData();

            pair = new KeyValuePair<int, string>(App.HomeIdx, selected_query);

            Settings.WeatherLoaded = true;
            // Hide dialog
            await LoadingDialog.HideAsync();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                sender.IsSuggestionListOpen = false;
                this.Frame.Navigate(typeof(Shell), pair);
            });
        }

        private async void Restore()
        {
            // Hide UIElements
            SearchGrid.Visibility = Visibility.Collapsed;

            // Check for key
            if (!String.IsNullOrEmpty(Settings.API_KEY))
                KeyEntry.Text = Settings.API_KEY;

            if (Settings.WeatherLoaded)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.Frame.Navigate(typeof(Shell)));
            }
            else
            {
                SearchGrid.Visibility = Visibility.Visible;
                // Set WUnderground as default API
                APIComboBox.SelectedIndex = 0;
            }
        }

        private async void GPS_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;

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

            Geolocator geolocal = new Geolocator() { DesiredAccuracyInMeters = 5000, ReportInterval = 900000, MovementThreshold = 2500 };
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
                button.IsEnabled = false;
                await LoadingDialog.ShowAsync();

                await Task.Run(async () =>
                {
                    if (cts.IsCancellationRequested) return;

                    LocationQueryViewModel view = await GeopositionQuery.GetLocation(geoPos);

                    if (!String.IsNullOrEmpty(view.LocationQuery))
                        selected_query = view.LocationQuery;
                    else
                        selected_query = string.Empty;
                });

                if (String.IsNullOrWhiteSpace(selected_query))
                {
                    // Stop since there is no valid query
                    goto exit;
                }

                // Stop if using WeatherUnderground and API Key is empty
                if (String.IsNullOrWhiteSpace(Settings.API_KEY) && Settings.API == Settings.API_WUnderground)
                {
                    TextBlock header = KeyEntry.Header as TextBlock;
                    header.Visibility = Visibility.Visible;
                    KeyEntry.BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
                    KeyEntry.BorderThickness = new Thickness(2);

                    goto exit;
                }

                KeyValuePair<int, string> pair;

                // Weather Data
                OrderedDictionary weatherData = await Settings.GetWeatherData();

                Weather weather = await WeatherLoaderTask.GetWeather(selected_query);

                if (weather == null)
                {
                    goto exit;
                }

                // Save weather data
                if (weatherData.Contains(selected_query))
                    weatherData[selected_query] = weather;
                else
                    weatherData.Add(selected_query, weather);
                Settings.SaveWeatherData();

                pair = new KeyValuePair<int, string>(App.HomeIdx, selected_query);

                Settings.FollowGPS = true;
                Settings.WeatherLoaded = true;
                // Hide dialog
                await LoadingDialog.HideAsync();
                this.Frame.Navigate(typeof(Shell), pair);
            }

            exit:
            button.IsEnabled = true;
            await LoadingDialog.HideAsync();
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
                Settings.API = Settings.API_WUnderground;
            }
            else if (index == 1)
            {
                // Yahoo Weather
                if (KeyEntry != null)
                    KeyEntry.Visibility = Visibility.Collapsed;
                Settings.API = Settings.API_Yahoo;
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