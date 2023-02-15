#if !WINDOWS
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Utils;
using System;
using System.Globalization;

namespace SimpleWeather.NET.Radar.NullSchool
{
    public class EarthWindMapViewProvider : RadarViewProvider
    {
        public const string EARTHWINDMAP_DEFAULT_URL = "https://earth.nullschool.net/#current/wind/surface/level/overlay=precip_3hr";
        public const string EARTHWINDMAP_URL_FORMAT = EARTHWINDMAP_DEFAULT_URL + "/orthographic={0:0.####},{1:0.####},3000";
        private const string BLANK_PAGE_URL = "about:blank";

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
                RadarContainer.Content = webview = CreateWebView();
            }

            if (InteractionsEnabled())
            {
                webview.EnableInteractions();
            }
            else
            {
                webview.DisableInteractions();
            }

            webview.Navigating -= RadarWebView_Navigating;
            try
            {
                webview.Source = RadarURL;
            }
            catch (Exception e)
            {
                // System.Exception: The remote procedure call failed
                Logger.WriteLine(LoggerLevel.Error, e);
            }
            webview.Navigating += RadarWebView_Navigating;
        }

        private async void Webview_NavigationCompleted(object sender, WebNavigatedEventArgs args)
        {
            var webview = sender as WebView;

            if (InteractionsEnabled())
            {
                webview.EnableInteractions();
            }
            else
            {
                webview.DisableInteractions();

                // Fallback
                var disableInteractions = "var style = document.createElement('style'); style.innerHTML = '* { pointer-events: none !important; overscroll-behavior: none !important; overflow: hidden !important; } body { pointer-events: all !important; overscroll-behavior: none !important; overflow: hidden !important; }'; document.head.appendChild(style);";
                try
                {
                    await webview.EvaluateJavaScriptAsync(disableInteractions);
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e);
                }
            }
        }

        public override void OnDestroyView()
        {
            // Destroy webview if we leave
            if (RadarContainer?.Content is WebView webview)
            {
                webview.Navigating -= RadarWebView_Navigating;
                try
                {
                    webview.LoadBlank();
                }
                catch (Exception e)
                {
                    // System.Exception: The remote procedure call failed
                    Logger.WriteLine(LoggerLevel.Error, e);
                }
                webview.Navigating += RadarWebView_Navigating;
                RadarContainer.Content = null;
#if ANDROID
                webview.GetPlatformView()?.Destroy();
#elif IOS || MACCATALYST
                webview.GetPlatformView()?.StopLoading();
#endif
                webview = null;
            }
        }

        private WebView GetRadarWebView()
        {
            return RadarContainer?.Content as WebView;
        }

        private WebView CreateWebView()
        {
            WebView webview = webview = new WebView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            webview.Navigated += Webview_NavigationCompleted;
            webview.HandlerChanged += (s, e) =>
            {
                webview.RestrictWebView();
                webview.EnableJS(true);
                UpdateRadarView();
            };

            return webview;
        }

        private void RadarWebView_Navigating(object sender, WebNavigatingEventArgs args)
        {
            var newUri = new Uri(args.Url);
            var defaultUri = new Uri(EARTHWINDMAP_DEFAULT_URL);

            // Cancel all navigation
            if (Equals(args.Url, BLANK_PAGE_URL) && newUri.Host != defaultUri.Host)
            {
                args.Cancel = true;
            }
        }
    }
}
#endif