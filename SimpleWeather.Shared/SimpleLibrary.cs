using CacheCow.Client;
using CacheCow.Client.FileCacheStore;
using SimpleWeather.HttpClientExtensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
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

            // Register default icon providers
            ResetIconProviders();
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

        public void RequestAction(string Action, IDictionary<string, object> Bundle = null)
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
                var CacheRoot = System.IO.Path.Combine(
                    Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path,
                    "CacheCow");
                HttpWebClient = ClientExtensions.CreateClient(new FileStore(CacheRoot) { MinExpiry = TimeSpan.FromDays(7) }, handler: new CacheFilter());
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
            IconProviders.Clear();
            foreach (var provider in Icons.WeatherIconsManager.DefaultIcons)
            {
                IconProviders.Add(provider.Key, provider.Value);
            }
        }

        public void Dispose()
        {
            HttpWebClient.Dispose();
        }
    }
}