using SimpleWeather.Utils;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Radar;
using SimpleWeather.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherRadarPage : Page, ICommandBarPage
    {
        public String CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        private RadarViewProvider radarViewProvider;
        private WeatherNowViewModel WNowViewModel { get; } = Shell.Instance.GetViewModel<WeatherNowViewModel>();

        public WeatherRadarPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("label_radar");
            PrimaryCommands = new List<ICommandBarElement>()
            {
                new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Refresh),
                    Label = App.ResLoader.GetString("action_refresh"),
                    Tag = "refresh"
                }
            };
            GetRefreshBtn().Tapped += RefreshButton_Click;

            AnalyticsLogger.LogEvent("WeatherRadarPage");
        }

        private AppBarButton GetRefreshBtn()
        {
            return PrimaryCommands.LastOrDefault() as AppBarButton;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            WNowViewModel.Weather?.Let(it =>
            {
                radarViewProvider?.UpdateCoordinates(it.LocationCoord, true);
            });
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            radarViewProvider?.OnDestroyView();
            AnalyticsLogger.LogEvent("WeatherRadarPage: OnNavigatingFrom");
            base.OnNavigatingFrom(e);
        }

        private void RadarWebViewContainer_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (radarViewProvider == null)
            {
                radarViewProvider = RadarProvider.GetRadarViewProvider(RadarWebViewContainer);
            }
            radarViewProvider.EnableInteractions(true);
            WNowViewModel.Weather?.Let(it =>
            {
                radarViewProvider?.UpdateCoordinates(it.LocationCoord, true);
            });
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            WNowViewModel.Weather?.Let(it =>
            {
                radarViewProvider?.UpdateCoordinates(it.LocationCoord, true);
            });
        }
    }
}