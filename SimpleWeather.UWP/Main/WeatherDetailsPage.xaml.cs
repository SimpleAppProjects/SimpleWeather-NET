using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherDetailsPage : CustomPage, IBackRequestedPage, IWeatherLoadedListener, IWeatherErrorListener
    {
        private LocationData location { get; set; }
        public WeatherNowViewModel WeatherView { get; set; }
        public bool IsHourly { get; set; }

        public WeatherDetailsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("Label_Forecast/Header");
        }

        public void OnWeatherLoaded(LocationData location, Weather weather)
        {
            AsyncTask.RunOnUIThread(() =>
            {
                WeatherView.UpdateView(weather);
            });
        }

        public void OnWeatherError(WeatherException wEx)
        {
            AsyncTask.RunOnUIThread(() =>
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
            var tcs = new TaskCompletionSource<bool>();

            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                tcs.SetResult(true);
                return tcs.Task;
            }

            tcs.SetResult(false);
            return tcs.Task;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e?.Parameter is DetailsPageArgs args)
            {
                location = args.Location;
                WeatherView = args.WeatherNowView;
                IsHourly = args.IsHourly;

                if (location == null)
                    location = Settings.HomeData;
                if (WeatherView == null)
                    WeatherView = new WeatherNowViewModel();

                if (IsHourly)
                    ListControl.ItemsSource = WeatherView.HourlyForecasts;
                else
                    ListControl.ItemsSource = WeatherView.Forecasts;

                if (WeatherView?.IsValid == false)
                {
                    await new WeatherDataLoader(location, this, this)
                        .LoadWeatherData(new WeatherRequest.Builder()
                            .LoadAlerts()
                            .ForceLoadSavedData()
                            .Build());
                }

                this.Loaded += async (sender, ev) =>
                {
                    if (args.ScrollToPosition <= ListControl.Items.Count)
                    {
                        var item = ListControl.Items[args.ScrollToPosition];
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            ListControl.ScrollIntoView(item, ScrollIntoViewAlignment.Leading);
                        });
                    }
                };
            }
        }
    }
}