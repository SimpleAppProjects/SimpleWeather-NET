using SimpleWeather.Common.ViewModels;
using SimpleWeather.Utils;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.NET.Helpers;
using SimpleWeather.NET.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.NET.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherChartsPage : ViewModelPage, ICommandBarPage, ISnackbarPage, IBackRequestedPage
    {
        public String CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        private LocationData.LocationData locationData { get; set; }
        public WeatherNowViewModel WNowViewModel { get; } = Shell.Instance.GetViewModel<WeatherNowViewModel>();
        public ChartsViewModel ChartsView { get; private set; }

        public WeatherChartsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // CommandBar
            CommandBarLabel = App.Current.ResLoader.GetString("label_forecast");
            AnalyticsLogger.LogEvent("WeatherChartsPage");
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

            ChartsView = this.GetViewModel<ChartsViewModel>();

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
                ChartsView.UpdateForecasts(locationData);
            }
        }
    }

    public class ChartsDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement element && VisualTreeHelperExtensions.GetParent<ItemsControl>(element) is FrameworkElement elementParent)
            {
                if (item is LineViewData)
                {
                    return elementParent.Resources["LineViewDataTemplate"] as DataTemplate;
                }
                else if (item is BarGraphData)
                {
                    return elementParent.Resources["BarChartDataTemplate"] as DataTemplate;
                }
            }

            return null;
        }
    }
}
