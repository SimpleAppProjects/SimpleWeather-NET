using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Navigation;
using SimpleWeather.Common.Controls;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.NET.Helpers;
using SimpleWeather.NET.ViewModels;
using SimpleWeather.Utils;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.NET.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherAQIPage : ViewModelPage, ICommandBarPage, ISnackbarPage, IBackRequestedPage
    {
        public String CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        private LocationData.LocationData locationData { get; set; }
        public WeatherNowViewModel WNowViewModel { get; } = Shell.Instance.GetViewModel<WeatherNowViewModel>();
        public AirQualityForecastViewModel AQIView { get; private set; }

        public WeatherAQIPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // CommandBar
            CommandBarLabel = App.Current.ResLoader.GetString("label_airquality");
            AnalyticsLogger.LogEvent("WeatherAQIPage");
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

            AQIView = this.GetViewModel<AirQualityForecastViewModel>();
            AQIView.PropertyChanged += AQIView_PropertyChanged;

            if (e?.Parameter is WeatherPageArgs args)
            {
                locationData = args.Location;
            }
            else
            {
                WNowViewModel.PropertyChanged += WNowViewModel_PropertyChanged;
            }

            Initialize();
        }

        private void AQIView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AQIView.AQIGraphData) || e.PropertyName == nameof(AQIView.AQIForecastData))
            {
                if (AQIView.AQIGraphData != null || AQIView.AQIForecastData != null)
                {
                    LoadingRing.IsActive = false;
                }
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

        private void Initialize()
        {
            if (locationData == null)
            {
                locationData = WNowViewModel.UiState.LocationData;
            }

            if (locationData != null)
            {
                AQIView.UpdateForecasts(locationData);
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                var windowWidth = MainWindow.Current.Bounds.Width;

                AQIContainer.SetBinding(ItemsControl.ItemsSourceProperty, new Binding()
                {
                    Mode = BindingMode.OneWay,
                    Source = AQIView,
                    Path = new PropertyPath(windowWidth >= 691 ? nameof(AQIView.AQIForecastData) : nameof(AQIView.AQIGraphData)),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
            });
        }
    }

    public class AQIDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement element && VisualTreeHelperExtensions.GetParent<ItemsControl>(element) is FrameworkElement elementParent)
            {
                if (item is BarGraphData)
                {
                    return elementParent.Resources["AQIGraphTemplate"] as DataTemplate;
                }
                else if (item is AirQualityViewModel)
                {
                    return elementParent.Resources["AQIForecastTemplate"] as DataTemplate;
                }
            }

            return null;
        }
    }
}
