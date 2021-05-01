using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace SimpleWeather.UWP.Radar.NullSchool
{
    public class EarthWindMapViewProvider : RadarViewProvider
    {
        public const string EARTHWINDMAP_DEFAULT_URL = "https://earth.nullschool.net/#current/wind/surface/level/overlay=precip_3hr";
        public const string EARTHWINDMAP_URL_FORMAT = EARTHWINDMAP_DEFAULT_URL + "/orthographic={0:0.####},{1:0.####},3000";

        private Uri RadarURL;

        public EarthWindMapViewProvider(Border container) : base(container) { }

        public override void UpdateCoordinates(WeatherUtils.Coordinate coordinates, bool updateView = false)
        {
            RadarURL = new Uri(string.Format(CultureInfo.InvariantCulture, EARTHWINDMAP_URL_FORMAT, coordinates.Longitude, coordinates.Latitude));
            if (updateView) UpdateRadarView();
        }

        public override void UpdateRadarView()
        {
            var webview = GetRadarWebView();

            if (webview == null)
            {
                RadarContainer.Child = webview = CreateWebView();
            }

            webview.NavigationStarting -= RadarWebView_NavigationStarting;
            webview.Navigate(RadarURL);
            webview.NavigationStarting += RadarWebView_NavigationStarting;
        }

        private async void Webview_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (!InteractionsEnabled())
            {
                var disableInteractions = new string[] { "var style = document.createElement('style'); style.innerHTML = '* { pointer-events: none !important; overscroll-behavior: none !important; overflow: hidden !important; } body { pointer-events: all !important; overscroll-behavior: none !important; overflow: hidden !important; }'; document.head.appendChild(style);" };
                try
                {
                    await sender.InvokeScriptAsync("eval", disableInteractions);
                }
                catch (Exception e)
                {
                    //Logger.WriteLine(LoggerLevel.Error, e);
                }
            }

            sender.IsHitTestVisible = InteractionsEnabled();
            sender.IsDoubleTapEnabled = InteractionsEnabled();
            sender.IsHoldingEnabled = InteractionsEnabled();
            sender.IsRightTapEnabled = InteractionsEnabled();
            sender.IsTapEnabled = InteractionsEnabled();
        }

        public override void OnDestroyView()
        {
            // Destroy webview if we leave
            if (RadarContainer?.Child is WebView webview)
            {
                webview.Stop();
                webview.NavigationStarting -= RadarWebView_NavigationStarting;
                webview.Navigate(new Uri("about:blank"));
                webview.NavigationStarting += RadarWebView_NavigationStarting;
                RadarContainer.Child = null;
                webview = null;
            }
        }

        private WebView GetRadarWebView()
        {
            return RadarContainer?.Child as WebView;
        }

        private WebView CreateWebView()
        {
            WebView webview = null;

            // Windows 1803+
            if (Windows.Foundation.Metadata.ApiInformation.IsEnumNamedValuePresent("Windows.UI.Xaml.Controls.WebViewExecutionMode", "SeparateProcess"))
            {
                try
                {
                    // NOTE: Potential managed code exception; don't know why
                    webview = new WebView(WebViewExecutionMode.SeparateProcess);
                }
                catch (Exception)
                {
                    webview = null;
                }
            }

            if (webview == null)
                webview = new WebView(WebViewExecutionMode.SeparateThread);

            if (Windows.Foundation.Metadata.ApiInformation.IsEventPresent("Windows.UI.Xaml.Controls.WebView", "SeparateProcessLost"))
            {
                webview.SeparateProcessLost += (sender, e) =>
                {
                    if (RadarContainer == null) return;
                    var newWebView = CreateWebView();
                    RadarContainer.Child = newWebView;
                    UpdateRadarView();
                };
            }

            webview.NavigationCompleted += Webview_NavigationCompleted;

            return webview;
        }

        private void RadarWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            // Cancel all navigation
            args.Cancel = true;
        }
    }
}
