using SimpleWeather.ComponentModel;
using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.ViewModels;
using SimpleWeather.ViewModels;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public sealed partial class WeatherDetailsPage : ViewModelPage, ICommandBarPage, ISnackbarPage, IBackRequestedPage
    {
        public String CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        private LocationData locationData { get; set; }
        public WeatherNowViewModel WNowViewModel { get; } = Shell.Instance.GetViewModel<WeatherNowViewModel>();
        public ForecastsListViewModel ForecastsView { get; private set; }
        public bool IsHourly { get; private set; }

        private readonly WeatherManager wm = WeatherManager.GetInstance();

        public object Forecasts
        {
            get { return GetValue(ForecastsProperty); }
            set
            {
                SetValue(ForecastsProperty, value);
            }
        }

        public static readonly DependencyProperty ForecastsProperty =
            DependencyProperty.Register("Forecasts", typeof(object),
            typeof(WeatherDetailsPage), new PropertyMetadata(null));

        public WeatherDetailsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("label_forecast");
            AnalyticsLogger.LogEvent("WeatherDetailsPage");
        }

        private void OnWeatherError(WeatherException wEx)
        {
            Dispatcher.LaunchOnUIThread(() =>
            {
                switch (wEx.ErrorStatus)
                {
                    case WeatherUtils.ErrorStatus.NetworkError:
                    case WeatherUtils.ErrorStatus.NoWeather:
                        // Show error message and prompt to refresh
                        ShowSnackbar(Snackbar.MakeError(wEx.Message, SnackbarDuration.Long));
                        break;

                    case WeatherUtils.ErrorStatus.QueryNotFound:
                        if (locationData?.country_code?.Let(it => !wm.IsRegionSupported(it)) == true)
                        {
                            ShowSnackbar(Snackbar.MakeError(App.ResLoader.GetString("error_message_weather_region_unsupported"), SnackbarDuration.Long));
                        }
                        else
                        {
                            ShowSnackbar(Snackbar.MakeError(wEx.Message, SnackbarDuration.Long));
                        }
                        break;

                    default:
                        // Show error message
                        ShowSnackbar(Snackbar.MakeError(wEx.Message, SnackbarDuration.Long));
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

        public void ShowSnackbar(Snackbar snackbar)
        {
            Shell.Instance?.ShowSnackbar(snackbar);
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

            ForecastsView = this.GetViewModel<ForecastsListViewModel>();

            if (e?.Parameter is DetailsPageArgs args)
            {
                locationData = args.Location;
                IsHourly = args.IsHourly;

                Initialize(args.ScrollToPosition);
            }
            else
            {
                WNowViewModel.PropertyChanged += WNowViewModel_PropertyChanged;
                Initialize();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            WNowViewModel.PropertyChanged -= WNowViewModel_PropertyChanged;
        }

        private void WNowViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WNowViewModel.UiState))
            {
                locationData = WNowViewModel.UiState.LocationData;
                Initialize();
            }
        }

        private void Initialize(int scrollToPosition = 0)
        {
            if (locationData == null)
            {
                locationData = WNowViewModel.UiState.LocationData;
            }

            if (locationData != null)
            {
                ForecastsView.UpdateForecasts(locationData);
            }

            Dispatcher.LaunchOnUIThread(() =>
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

                // Scroll item into view
                void contentChangedListener(ListViewBase sender, ContainerContentChangingEventArgs cccEvArgs)
                {
                    ListControl.ContainerContentChanging -= contentChangedListener;

                    void layoutUpdateListener(object s, object layoutEvArgs)
                    {
                        ListControl.LayoutUpdated -= layoutUpdateListener;

                        if (scrollToPosition > 0 && ListControl.Items?.Count > scrollToPosition)
                        {
                            ListControl.ScrollIntoView(ListControl.Items[scrollToPosition], ScrollIntoViewAlignment.Leading);
                        }
                    };

                    ListControl.LayoutUpdated += layoutUpdateListener;
                };

                ListControl.ContainerContentChanging += contentChangedListener;
            });
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