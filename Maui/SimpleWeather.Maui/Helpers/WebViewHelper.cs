using Microsoft.Maui.Handlers;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Helpers
{
    public static class WebViewHelper
    {
        public static void DisableInteractions(this WebView view)
        {
            view.IsEnabled = false;
            view.GestureRecognizers.Clear();
            var platformView = view.GetPlatformView();

            if (platformView == null)
            {
                Logger.WriteLine(LoggerLevel.Warn, "DisableInteractions: Webview not available");
                return;
            }

#if __ANDROID__
            platformView.SetOnClickListener(null);
            platformView.SetOnTouchListener(new NoTouchListener());
            platformView.Settings.SetSupportZoom(false);
            platformView.Settings.DisplayZoomControls = false;
#elif IOS || MACCATALYST
            platformView.UserInteractionEnabled = false;
            platformView.MultipleTouchEnabled = false;
#endif
        }

        public static void EnableInteractions(this WebView view)
        {
            view.IsEnabled = true;
            var platformView = view.GetPlatformView();

            if (platformView == null)
            {
                Logger.WriteLine(LoggerLevel.Warn, "EnableInteractions: Webview not available");
                return;
            }

#if __ANDROID__
            platformView.SetOnTouchListener(null);
            platformView.Settings.SetSupportZoom(true);
            platformView.Settings.DisplayZoomControls = true;
#elif IOS || MACCATALYST
            platformView.UserInteractionEnabled = true;
            platformView.MultipleTouchEnabled = true;
#endif
        }

        public static void RestrictWebView(this WebView view)
        {
            var platformView = view.GetPlatformView();

            if (platformView == null)
            {
                Logger.WriteLine(LoggerLevel.Warn, "RestrictWebView: Webview not available");
                return;
            }

#if __ANDROID__
            platformView.Settings.JavaScriptEnabled = false;
            platformView.Settings.AllowContentAccess = false;
            platformView.Settings.AllowFileAccess = false;
            platformView.Settings.AllowFileAccessFromFileURLs = false;
            platformView.Settings.SetGeolocationEnabled(false);
            if (OperatingSystem.IsAndroidVersionAtLeast(26)) // O
            {
                platformView.Settings.SafeBrowsingEnabled = true;
            }
#elif IOS || MACCATALYST
            if (OperatingSystem.IsIOSVersionAtLeast(14) || OperatingSystem.IsMacCatalystVersionAtLeast(14))
            {
                if (platformView.Configuration.DefaultWebpagePreferences == null)
                {
                    platformView.Configuration.DefaultWebpagePreferences = new WebKit.WKWebpagePreferences();
                }
                platformView.Configuration.DefaultWebpagePreferences.AllowsContentJavaScript = false;
            }
            else
            {
                platformView.Configuration.Preferences.JavaScriptEnabled = false;
            }
            platformView.Configuration.AllowsInlineMediaPlayback = false;
            platformView.Configuration.AllowsAirPlayForMediaPlayback = false;
            platformView.Configuration.AllowsPictureInPictureMediaPlayback = false;
            platformView.Configuration.MediaTypesRequiringUserActionForPlayback = WebKit.WKAudiovisualMediaTypes.All;
#endif
        }

        public static void EnableJS(this WebView view, bool enabled)
        {
            var platformView = view.GetPlatformView();

            if (platformView == null)
            {
                Logger.WriteLine(LoggerLevel.Warn, "EnableJS: Webview not available");
                return;
            }

#if __ANDROID__
            platformView.Settings.JavaScriptEnabled = enabled;
#elif IOS || MACCATALYST
            if (OperatingSystem.IsIOSVersionAtLeast(14) || OperatingSystem.IsMacCatalystVersionAtLeast(14))
            {
                if (platformView.Configuration.DefaultWebpagePreferences == null)
                {
                    platformView.Configuration.DefaultWebpagePreferences = new WebKit.WKWebpagePreferences();
                }
                platformView.Configuration.DefaultWebpagePreferences.AllowsContentJavaScript = enabled;
            }
            else
            {
                platformView.Configuration.Preferences.JavaScriptEnabled = enabled;
            }
#endif
        }

        public static void ForceReload(this WebView view, Uri url)
        {
            view.Dispatcher.Dispatch(() =>
            {
                view.Source = new HtmlWebViewSource()
                {
                    Html = "<html><body style=\"background-color: black;\"></body></html>",
                };
            });
            view.Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(1), () =>
            {
                view.Source = url;
            });
        }

        public static void LoadBlank(this WebView view)
        {
            view.Dispatcher.Dispatch(() =>
            {
                view.Source = new HtmlWebViewSource()
                {
                    Html = "<html><body style=\"background-color: black;\"></body></html>",
                };
            });
        }

        public static void DispatchLoadUrl(this WebView view, Uri uri)
        {
            view.Dispatcher.Dispatch(() =>
            {
                if (view.Source is not UrlWebViewSource urlSrc || !Equals(urlSrc.Url, uri.ToString()))
                {
                    view.Source = uri;
                }
            });
        }

#if __ANDROID__
        public static Android.Webkit.WebView GetPlatformView(this WebView view)
#elif IOS || MACCATALYST
        public static WebKit.WKWebView GetPlatformView(this WebView view)
#else
        public static object GetPlatformView(this WebView view)
#endif
        {
            if (view.Handler is IWebViewHandler webViewHandler)
            {
                return webViewHandler.PlatformView;
            }

            return null;
        }

#if ANDROID
        private class NoTouchListener : Java.Lang.Object, Android.Views.View.IOnTouchListener
        {
            public bool OnTouch(Android.Views.View v, Android.Views.MotionEvent e)
            {
                return true;
            }
        }
#endif
    }
}
