#if HAS_UNO
using Microsoft.UI.Xaml.Controls;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Uno.Radar.NullSchool
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

#if !HAS_UNO_SKIA
            webview.NavigationStarting -= RadarWebView_NavigationStarting;
            try
            {
                webview.Navigate(RadarURL);
            }
            catch (Exception e)
            {
                // System.Exception: The remote procedure call failed
                Logger.WriteLine(LoggerLevel.Error, e);
            }
            webview.NavigationStarting += RadarWebView_NavigationStarting;
#endif
        }

        private async void Webview_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
#if !HAS_UNO_SKIA
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
#endif

            sender.IsHitTestVisible = InteractionsEnabled();
#if !HAS_UNO_SKIA
            sender.IsDoubleTapEnabled = InteractionsEnabled();
            sender.IsHoldingEnabled = InteractionsEnabled();
            sender.IsRightTapEnabled = InteractionsEnabled();
            sender.IsTapEnabled = InteractionsEnabled();
#endif
        }

        public override void OnDestroyView()
        {
            // Destroy webview if we leave
            if (RadarContainer?.Child is WebView webview)
            {
#if !HAS_UNO_SKIA
                webview.Stop();
                webview.NavigationStarting -= RadarWebView_NavigationStarting;
                try
                {
                    webview.Navigate(new Uri("about:blank"));
                }
                catch (Exception e)
                {
                    // System.Exception: The remote procedure call failed
                    Logger.WriteLine(LoggerLevel.Error, e);
                }
                webview.NavigationStarting += RadarWebView_NavigationStarting;
#endif
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
            WebView webview = new WebView();
#if !HAS_UNO_SKIA
            webview.NavigationCompleted += Webview_NavigationCompleted;
#endif
            return webview;
        }

        private void RadarWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            // Cancel all navigation
            args.Cancel = true;
        }
    }
}
#endif