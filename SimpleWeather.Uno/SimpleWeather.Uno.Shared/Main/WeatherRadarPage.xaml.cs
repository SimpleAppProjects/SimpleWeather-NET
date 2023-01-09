using SimpleWeather.Common.ViewModels;
using SimpleWeather.Utils;
using SimpleWeather.Uno.Helpers;
using SimpleWeather.Uno.Radar;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.Uno.Main
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
            CommandBarLabel = App.Current.ResLoader.GetString("label_radar");
            PrimaryCommands = new List<ICommandBarElement>()
            {
                new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Refresh),
                    Label = App.Current.ResLoader.GetString("action_refresh"),
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

        private void RadarWebViewContainer_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
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