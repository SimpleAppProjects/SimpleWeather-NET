#if WINDOWS
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
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
                RadarContainer.Child = webview = CreateWebView();
            }

            webview.NavigationStarting -= RadarWebView_NavigationStarting;
            try
            {
                webview.Source = RadarURL;
            }
            catch (Exception e)
            {
                // System.Exception: The remote procedure call failed
                Logger.WriteLine(LoggerLevel.Error, e);
            }
            webview.NavigationStarting += RadarWebView_NavigationStarting;
        }

        private async void Webview_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            if (!InteractionsEnabled())
            {
                var disableInteractions = /*new string[] { */"var style = document.createElement('style'); style.innerHTML = '* { pointer-events: none !important; overscroll-behavior: none !important; overflow: hidden !important; } body { pointer-events: all !important; overscroll-behavior: none !important; overflow: hidden !important; }'; document.head.appendChild(style);"/* }*/;
                try
                {
                    await sender.ExecuteScriptAsync(/*"eval", */disableInteractions);
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e);
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
            if (RadarContainer?.Child is WebView2 webview)
            {
                webview.CoreWebView2?.Stop();
                webview.NavigationStarting -= RadarWebView_NavigationStarting;
                try
                {
                    webview.Source = new Uri(BLANK_PAGE_URL);
                }
                catch (Exception e)
                {
                    // System.Exception: The remote procedure call failed
                    Logger.WriteLine(LoggerLevel.Error, e);
                }
                webview.NavigationStarting += RadarWebView_NavigationStarting;
                RadarContainer.Child = null;
                webview = null;
            }
        }

        private WebView2 GetRadarWebView()
        {
            return RadarContainer?.Child as WebView2;
        }

        private WebView2 CreateWebView()
        {
            WebView2 webview = webview = new WebView2()
            {
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch
            };

            webview.CoreProcessFailed += (sender, e) =>
            {
                if (RadarContainer == null) return;
                var newWebView = CreateWebView();
                RadarContainer.Child = newWebView;
                UpdateRadarView();
            };

            webview.NavigationCompleted += Webview_NavigationCompleted;
            webview.CoreWebView2Initialized += Webview_CoreWebView2Initialized;
            webview.EnsureCoreWebView2Async();

            return webview;
        }

        private void Webview_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            if (args.Exception != null)
            {
                Logger.WriteLine(LoggerLevel.Error, args.Exception);
            }

            sender.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            sender.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            sender.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            sender.CoreWebView2.Settings.AreDevToolsEnabled = false;
            sender.CoreWebView2.Settings.AreHostObjectsAllowed = false;
            sender.CoreWebView2.Settings.HiddenPdfToolbarItems = CoreWebView2PdfToolbarItems.None;
            sender.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            sender.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
            sender.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
            sender.CoreWebView2.Settings.IsPinchZoomEnabled = false;
            sender.CoreWebView2.Settings.IsScriptEnabled = true;
            sender.CoreWebView2.Settings.IsStatusBarEnabled = false;
            sender.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
            sender.CoreWebView2.Settings.IsWebMessageEnabled = false;
            sender.CoreWebView2.Settings.IsZoomControlEnabled = false;

            UpdateRadarView();
        }

        private void RadarWebView_NavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            var newUri = new Uri(args.Uri);
            var defaultUri = new Uri(EARTHWINDMAP_DEFAULT_URL);

            // Cancel all navigation
            if (Equals(args.Uri, BLANK_PAGE_URL) && newUri.Host != defaultUri.Host)
            {
                args.Cancel = true;
            }
        }
    }
}
#endif