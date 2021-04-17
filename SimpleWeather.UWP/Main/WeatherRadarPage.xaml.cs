using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Radar;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherRadarPage : CustomPage
    {
        private RadarViewProvider radarViewProvider;
        private WeatherNowViewModel WeatherView;

        public WeatherRadarPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("Label_Radar/Header");
            AnalyticsLogger.LogEvent("WeatherRadarPage");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            WeatherView = Shell.Instance.GetViewModel<WeatherNowViewModel>();
            radarViewProvider?.UpdateCoordinates(WeatherView.LocationCoord, true);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            radarViewProvider?.OnDestroyView();
        }

        private void RadarWebViewContainer_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            radarViewProvider = RadarProvider.GetRadarViewProvider(RadarWebViewContainer);
            radarViewProvider.EnableInteractions(true);
            if (WeatherView.LocationCoord != null)
            {
                radarViewProvider.UpdateCoordinates(WeatherView.LocationCoord, true);
            }
        }
    }
}