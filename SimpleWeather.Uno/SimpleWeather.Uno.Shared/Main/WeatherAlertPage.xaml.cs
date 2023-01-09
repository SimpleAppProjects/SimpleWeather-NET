using SimpleWeather.Common.Controls;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.Utils;
using SimpleWeather.Uno.Controls;
using SimpleWeather.Uno.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.Uno.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherAlertPage : Page, ICommandBarPage, ISnackbarPage, IBackRequestedPage
    {
        public String CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        private LocationData.LocationData locationData { get; set; }
        public WeatherNowViewModel WNowViewModel { get; } = Shell.Instance.GetViewModel<WeatherNowViewModel>();
        public WeatherAlertsViewModel AlertsView { get; } = Shell.Instance.GetViewModel<WeatherAlertsViewModel>();

        public WeatherAlertPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // CommandBar
            CommandBarLabel = App.Current.ResLoader.GetString("title_fragment_alerts");
            AnalyticsLogger.LogEvent("WeatherAlertPage");
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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

        private void WNowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
                AlertsView.UpdateAlerts(locationData);
            }
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