using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private OrderedDictionary IconProviders;

        public event CommonActionChangedEventHandler OnCommonActionChanged;

        private static SimpleLibrary sSimpleLib;

        private SimpleLibrary()
        {
            ResourceLoader = GetResourceLoader();
            IconProviders = new OrderedDictionary();

            // Register default icon provider
            RegisterIconProvider(new Icons.WeatherIconsProvider());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static SimpleLibrary GetInstance()
        {
            if (sSimpleLib == null)
                sSimpleLib = new SimpleLibrary();

            return sSimpleLib;
        }

        public ResourceLoader ResLoader
        {
            get
            {
                return sSimpleLib.ResourceLoader;
            }
        }

        public HttpClient WebClient
        {
            get
            {
                return sSimpleLib.GetHttpClient();
            }
        }

        public void RequestAction(string Action, IDictionary<String, String> Bundle = null)
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

        public void RegisterIconProvider(Icons.WeatherIconProvider provider)
        {
            if (!IconProviders.Contains(provider.Key))
                IconProviders.Add(provider.Key, provider);
        }

        public Icons.WeatherIconProvider GetIconProvider(string key)
        {
            Icons.WeatherIconProvider provider = IconProviders[key] as Icons.WeatherIconProvider;
            if (provider == null)
            {
                // Can't find the provider for this key; fallback to default/first available
                if (IconProviders.Count > 0)
                {
                    provider = IconProviders[0] as Icons.WeatherIconProvider;
                }
                else
                {
                    RegisterIconProvider(provider = new Icons.WeatherIconsProvider());
                }
            }
            return provider;
        }

        public OrderedDictionary GetIconProviders()
        {
            return IconProviders.AsReadOnly();
        }

        public void ResetIconProviders()
        {
            Icons.WeatherIconProvider defaultProvider = null;
            if (IconProviders.Contains(Icons.WeatherIconsProvider.KEY))
            {
                defaultProvider = IconProviders[Icons.WeatherIconsProvider.KEY] as Icons.WeatherIconProvider;
            }

            IconProviders.Clear();
            
            if (defaultProvider != null)
            {
                IconProviders.Add(Icons.WeatherIconsProvider.KEY, defaultProvider);
            }
            else
            {
                RegisterIconProvider(new Icons.WeatherIconsProvider());
            }
        }

        public void Dispose()
        {
            HttpWebClient.Dispose();
        }
    }
}