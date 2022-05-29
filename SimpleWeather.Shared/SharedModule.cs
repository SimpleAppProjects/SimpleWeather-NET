using CacheCow.Client;
using CacheCow.Client.FileCacheStore;
using Microsoft.Extensions.DependencyInjection;
using SimpleWeather.Extras;
using SimpleWeather.HttpClientExtensions;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData.Images;
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
    public sealed partial class SharedModule : IDisposable
    {
        private ResourceLoader ResourceLoader;
        private ServiceCollection ServiceCollection = new();

        public event CommonActionChangedEventHandler OnCommonActionChanged;

        private readonly static Lazy<SharedModule> lazy = new(() =>
        {
            return new SharedModule();
        });

        private SharedModule()
        {
            ResourceLoader = GetResourceLoader();

            Logger.Init();

            ConfigureServices(ServiceCollection);
        }

        public static SharedModule Instance => lazy.Value;

        public ResourceLoader ResLoader => ResourceLoader;

        public HttpClient WebClient => httpClientLazy.Value;

        public WeatherIconsManager WeatherIconsManager => wimLazy.Value;

        public IServiceProvider Services { get; private set; }

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

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IExtrasService, DefaultExtrasServiceImpl>();
            serviceCollection.AddSingleton<ImageDataHelperImpl, ImageDataHelperDefault>();
        }

        public IServiceCollection GetServiceCollection()
        {
            return ServiceCollection;
        }

        public void BuildServiceProvider()
        {
            if (Services == null)
            {
                Services = ServiceCollection.BuildServiceProvider();
            }
        }

        public void Initialize()
        {
            Task.Run(async () =>
            {
                await DataMigrations.PerformVersionMigrations(Settings.GetWeatherDBConnection(), Settings.GetLocationDBConnection());
            }).Wait();
        }

        private readonly Lazy<HttpClient> httpClientLazy = new(() =>
        {
            var CacheRoot = System.IO.Path.Combine(
                Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path,
                "CacheCow");

            return ClientExtensions.CreateClient(new RemoveHeaderDelagatingCacheStore(new FileStore(CacheRoot) { MinExpiry = TimeSpan.FromDays(7) }), handler: new CacheFilter());
        });

        private readonly Lazy<WeatherIconsManager> wimLazy = new(() =>
        {
            return new WeatherIconsManager();
        });

        public void Dispose()
        {
            if (httpClientLazy.IsValueCreated)
            {
                httpClientLazy.Value.Dispose();
            }
        }
    }
}