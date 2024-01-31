using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using SimpleWeather.Common.Controls;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Helpers;
using SimpleWeather.NET.ViewModels;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.NET.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherDetailsPage : ViewModelPage, ICommandBarPage, ISnackbarPage, IBackRequestedPage
    {
        public String CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        private LocationData.LocationData locationData { get; set; }
        public WeatherNowViewModel WNowViewModel { get; } = Shell.Instance.GetViewModel<WeatherNowViewModel>();
        public ForecastsListViewModel ForecastsView { get; private set; }
        public bool IsHourly { get; private set; }

        private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;

        public WeatherDetailsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // CommandBar
            CommandBarLabel = App.Current.ResLoader.GetString("label_forecast");
            AnalyticsLogger.LogEvent("WeatherDetailsPage");
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

            DispatcherQueue.TryEnqueue(() =>
            {
                ForecastsView.SelectForecast(IsHourly);

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

            args.RegisterUpdateCallback(ListControl_Phase1);
            args.Handled = true;
        }

        private void ListControl_Phase1(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.RegisterUpdateCallback(ListControl_Phase2);
        }

        private void ListControl_Phase2(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase == 2 && args.ItemContainer?.ContentTemplateRoot is WeatherDetailPanel panel)
            {
                panel.DataContext = args.Item;
            }
            args.RegisterUpdateCallback(ListControl_Phase3);
        }

        private void ListControl_Phase3(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            // Phase 3, icon update time
            if (args.Phase == 3 && args.ItemContainer?.ContentTemplateRoot is WeatherDetailPanel panel && args.Item is BaseForecastItemViewModel model)
            {
                panel.WeatherIcon = model.WeatherIcon;
            }
        }
    }
}