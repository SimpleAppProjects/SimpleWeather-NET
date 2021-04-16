using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherAlertPage : CustomPage, IBackRequestedPage, IWeatherErrorListener
    {
        private LocationData location { get; set; }
        public WeatherNowViewModel WeatherView { get; set; }
        public WeatherAlertsViewModel AlertsView { get; set; }

        public WeatherAlertPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("Label_WeatherAlerts/Text");
            AnalyticsLogger.LogEvent("WeatherAlertPage");
        }

        public void OnWeatherError(WeatherException wEx)
        {
            Dispatcher.LaunchOnUIThread(() =>
            {
                switch (wEx.ErrorStatus)
                {
                    case WeatherUtils.ErrorStatus.NetworkError:
                    case WeatherUtils.ErrorStatus.NoWeather:
                        // Show error message and prompt to refresh
                        ShowSnackbar(Snackbar.Make(wEx.Message, SnackbarDuration.Long));
                        break;

                    case WeatherUtils.ErrorStatus.QueryNotFound:
                        if (WeatherAPI.NWS.Equals(Settings.API))
                        {
                            ShowSnackbar(Snackbar.Make(App.ResLoader.GetString("Error_WeatherUSOnly"), SnackbarDuration.Long));
                        }
                        break;

                    default:
                        // Show error message
                        ShowSnackbar(Snackbar.Make(wEx.Message, SnackbarDuration.Long));
                        break;
                }
            });
        }

        public Task<bool> OnBackRequested()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e?.Parameter is WeatherPageArgs args)
            {
                location = args.Location;
                WeatherView = args.WeatherNowView;
                AlertsView = args.AlertsView;
            }

            if (location == null)
                location = await Settings.GetHomeData().ConfigureAwait(true);
            if (WeatherView == null)
                WeatherView = new WeatherNowViewModel(Dispatcher);
            if (AlertsView == null)
                AlertsView = new WeatherAlertsViewModel();

            if (WeatherView?.IsValid != true)
            {
                new WeatherDataLoader(location)
                    .LoadWeatherData(new WeatherRequest.Builder()
                        .ForceLoadSavedData()
                        .SetErrorListener(this)
                        .Build())
                        .ContinueWith(async (t) =>
                        {
                            if (t.IsCompletedSuccessfully)
                            {
                                await Dispatcher.RunOnUIThread(() =>
                                {
                                    WeatherView.UpdateView(t.Result);
                                    AlertsView?.UpdateAlerts(location);
                                });
                            }
                        });
            }

            AlertsView?.UpdateAlerts(location);
        }

        private void StackControl_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Item is WeatherAlertViewModel)
            {
                var container = args.ItemContainer;
                var headerToggle = VisualTreeHelperExtensions.FindChild<ToggleButton>(container, "AlertHeader");

                if (headerToggle != null)
                {
                    headerToggle.IsChecked = false;
                }
            }
        }
    }
}