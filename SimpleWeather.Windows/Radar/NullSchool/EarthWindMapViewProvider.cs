﻿#if WINDOWS
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
                webview.CoreWebView2?.Navigate(RadarURL.ToString());
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
                    webview.CoreWebView2?.Navigate("about:blank");
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
            WebView2 webview = webview = new WebView2();

            if (Windows.Foundation.Metadata.ApiInformation.IsEventPresent("Microsoft.UI.Xaml.Controls.WebView", "SeparateProcessLost"))
            {
                webview.CoreProcessFailed += (sender, e) =>
                {
                    if (RadarContainer == null) return;
                    var newWebView = CreateWebView();
                    RadarContainer.Child = newWebView;
                    UpdateRadarView();
                };
            }

            webview.NavigationCompleted += Webview_NavigationCompleted;

            webview.CoreWebView2Initialized += Webview_CoreWebView2Initialized;
            webview.EnsureCoreWebView2Async();

            return webview;
        }

        private void Webview_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            UpdateRadarView();
        }

        private void RadarWebView_NavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            // Cancel all navigation
            args.Cancel = true;
        }
    }
}
#endif