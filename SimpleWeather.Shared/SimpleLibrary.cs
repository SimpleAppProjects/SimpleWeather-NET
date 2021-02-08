using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using static SimpleWeather.CommonActionChangedEventArgs;

namespace SimpleWeather
{
    public sealed partial class SimpleLibrary : IDisposable
    {
        private ResourceLoader ResourceLoader;
        private HttpClient HttpWebClient;

        public static event CommonActionChangedEventHandler OnCommonActionChanged;

        private static SimpleLibrary sSimpleLib;

        private SimpleLibrary()
        {
            ResourceLoader = GetResourceLoader();
        }

        public static ResourceLoader ResLoader
        {
            get
            {
                Init();
                return sSimpleLib.ResourceLoader;
            }
        }

        public static HttpClient WebClient
        {
            get
            {
                Init();
                return sSimpleLib.GetHttpClient();
            }
        }

        private static void Init()
        {
            if (sSimpleLib == null)
                sSimpleLib = new SimpleLibrary();
        }

        public static void RequestAction(string Action, IDictionary<String, String> Bundle = null)
        {
            OnCommonActionChanged?.Invoke(null, 
                new CommonActionChangedEventArgs(Action, Bundle));
        }

        private ResourceLoader GetResourceLoader()
        {
            if (Windows.UI.Core.CoreWindow.GetForCurrentThread() != null)
            {
                return ResourceLoader.GetForCurrentView();
            }
            else
            {
                return ResourceLoader.GetForViewIndependentUse();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Client will be disposed")]
        private HttpClient GetHttpClient()
        {
            if (HttpWebClient == null)
            {
                var handler = new HttpBaseProtocolFilter()
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = true,
                    AllowUI = false
                };
                HttpWebClient = new HttpClient(handler);

                var version = string.Format("v{0}.{1}.{2}",
                    Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

                HttpWebClient.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue("SimpleWeather (thewizrd.dev@gmail.com)", version));
            }

            return HttpWebClient;
        }

        public void Dispose()
        {
            HttpWebClient.Dispose();
        }
    }
}