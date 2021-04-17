using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherDetailsPage : CustomPage, IBackRequestedPage, IWeatherErrorListener
    {
        private LocationData location { get; set; }
        public WeatherNowViewModel WeatherView { get; set; }
        public ForecastsListViewModel ForecastsView { get; set; }
        public bool IsHourly { get; set; }

        public static readonly DependencyProperty ForecastsProperty =
            DependencyProperty.Register("Forecasts", typeof(object),
            typeof(WeatherDetailsPage), new PropertyMetadata(null));

        public object Forecasts
        {
            get { return GetValue(ForecastsProperty); }
            set
            {
                SetValue(ForecastsProperty, value);
            }
        }

        public WeatherDetailsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("Label_Forecast/Header");
            AnalyticsLogger.LogEvent("WeatherDetailsPage");
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

        /// <summary>
        /// OnNavigatedTo
        /// </summary>
        /// <param name="e"></param>
        /// <exception cref="WeatherException">Ignore.</exception>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        /// <exception cref="InvalidOperationException">Ignore.</exception>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e?.Parameter is DetailsPageArgs args)
            {
                location = args.Location;
                IsHourly = args.IsHourly;

                WeatherView = Shell.Instance.GetViewModel<WeatherNowViewModel>();

                if (ForecastsView == null)
                    ForecastsView = new ForecastsListViewModel();

                Task.Run(async () =>
                {
                    if (location == null)
                        location = await Settings.GetHomeData();

                    ForecastsView.UpdateForecasts(location);
                }).ContinueWith(async (t) =>
                {
                    await Dispatcher.RunOnUIThread(() =>
                    {
                        if (IsHourly)
                        {
                            SetBinding(ForecastsProperty, new Binding()
                            {
                                Mode = BindingMode.OneWay,
                                Source = ForecastsView.HourlyForecasts
                            });
                        }
                        else
                        {
                            SetBinding(ForecastsProperty, new Binding()
                            {
                                Mode = BindingMode.OneWay,
                                Source = ForecastsView.Forecasts
                            });
                        }

                        if (WeatherView?.IsValid == false)
                        {
                            new WeatherDataLoader(location)
                                .LoadWeatherData(new WeatherRequest.Builder()
                                    .ForceLoadSavedData()
                                    .SetErrorListener(this)
                                    .Build())
                                    .ContinueWith((t2) =>
                                    {
                                        WeatherView.UpdateView(t2.Result);
                                    });
                        }

                        // Scroll item into view
                        void contentChangedListener(ListViewBase sender, ContainerContentChangingEventArgs cccEvArgs)
                        {
                            ListControl.ContainerContentChanging -= contentChangedListener;

                            void layoutUpdateListener(object s, object layoutEvArgs)
                            {
                                ListControl.LayoutUpdated -= layoutUpdateListener;

                                if (args.ScrollToPosition > 0 && ListControl.Items?.Count > args.ScrollToPosition)
                                {
                                    ListControl.ScrollIntoView(ListControl.Items[args.ScrollToPosition], ScrollIntoViewAlignment.Leading);
                                }
                            };

                            ListControl.LayoutUpdated += layoutUpdateListener;
                        };
                        ListControl.ContainerContentChanging += contentChangedListener;
                    });
                });
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void ListControl_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Item is BaseForecastItemViewModel)
            {
                var container = args.ItemContainer;
                var headerToggle = VisualTreeHelperExtensions.FindChild<ToggleButton>(container, "DetailHeader");

                if (headerToggle != null)
                {
                    headerToggle.IsChecked = false;
                }
            }
        }
    }
}