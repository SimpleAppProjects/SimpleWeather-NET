using CacheCow.Client;
using CacheCow.Client.FileCacheStore;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using SimpleWeather.Extras;
using SimpleWeather.HttpClientExtensions;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
using SimpleWeather.RemoteConfig;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData.Images;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Windows.ApplicationModel.Resources;
using static SimpleWeather.CommonActionChangedEventArgs;

namespace SimpleWeather
{
    public sealed partial class SharedModule : IDisposable
    {
        private readonly ResourceLoader _resourceLoader;
        private readonly ServiceCollection ServiceCollection = new();

        public event CommonActionChangedEventHandler OnCommonActionChanged;

        private readonly static Lazy<SharedModule> lazy = new(() =>
        {
            return new SharedModule();
        });

        private SharedModule()
        {
            _resourceLoader = GetResourceLoader();

            ConfigureServices(ServiceCollection);
        }

        public void Initialize()
        {
            Logger.Init();
        }

        public static SharedModule Instance => lazy.Value;

        public ResourceLoader ResLoader => _resourceLoader;

        public HttpClient WebClient => httpClientLazy.Value;

        public WeatherIconsManager WeatherIconsManager => wimLazy.Value;

        public IServiceProvider Services => Ioc.Default;

        public readonly DispatcherQueue DispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public void RequestAction(string Action, IDictionary<string, object> Bundle = null)
        {
            OnCommonActionChanged?.Invoke(null,
                new CommonActionChangedEventArgs(Action, Bundle));
        }

        private ResourceLoader GetResourceLoader()
        {
            return ResourceLoader.GetForViewIndependentUse();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IExtrasService, DefaultExtrasServiceImpl>();
            serviceCollection.AddSingleton<ImageDataHelperImpl, ImageDataHelperDefault>();
            serviceCollection.AddSingleton<IRemoteConfigService>(DI.Utils.RemoteConfigService);
            serviceCollection.AddSingleton(ResLoader);
            serviceCollection.AddSingleton<SettingsManager>(DI.Utils.SettingsManager);
        }

        public IServiceCollection GetServiceCollection()
        {
            return ServiceCollection;
        }

        public void BuildServiceProvider()
        {
            Ioc.Default.ConfigureServices(ServiceCollection.BuildServiceProvider());
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