using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
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
        private Uri RadarURI { get; set; }
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

            if (e?.Parameter is Uri radarUri)
            {
                RadarURI = radarUri;
                NavigateToRadarURL();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (RadarWebViewContainer?.Child is WebView webview)
            {
                webview.Stop();
                webview.NavigationStarting -= RadarWebView_NavigationStarting;
                webview.Navigate(new Uri("about:blank"));
                webview.NavigationStarting += RadarWebView_NavigationStarting;
                RadarWebViewContainer.Child = null;
                webview = null;
            }
        }

        private void NavigateToRadarURL()
        {
            if (RadarURI == null) return;

            if (!(RadarWebViewContainer.Child is WebView webview))
            {
                webview = CreateWebView();
                RadarWebViewContainer.Child = webview;
            }

            webview.NavigationStarting -= RadarWebView_NavigationStarting;
            webview.Navigate(RadarURI);
            webview.NavigationStarting += RadarWebView_NavigationStarting;
        }

        private WebView CreateWebView()
        {
            WebView webview;
            if (Windows.Foundation.Metadata.ApiInformation.IsEnumNamedValuePresent("Windows.UI.Xaml.Controls.WebViewExecutionMode", "SeparateProcess"))
                webview = new WebView(WebViewExecutionMode.SeparateProcess);
            else
                webview = new WebView(WebViewExecutionMode.SeparateThread);

            if (Windows.Foundation.Metadata.ApiInformation.IsEventPresent("Windows.UI.Xaml.Controls.WebView", "SeparateProcessLost"))
            {
                webview.SeparateProcessLost += (sender, e) =>
                {
                    if (RadarWebViewContainer == null) return;
                    var newWebView = CreateWebView();
                    RadarWebViewContainer.Child = newWebView;
                    NavigateToRadarURL();
                };
            }

            webview.ContentLoading += (s, e) =>
            {
                LoadingRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
            };

            webview.DOMContentLoaded += (s, e) =>
            {
                LoadingRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            };

            return webview;
        }

        private void RadarWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            // Cancel all navigation
            args.Cancel = true;
        }
    }
}