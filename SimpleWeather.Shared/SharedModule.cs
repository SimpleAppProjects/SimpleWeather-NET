using CacheCow.Client;
using CacheCow.Client.FileCacheStore;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
#if WINUI
using Microsoft.UI.Dispatching;
#else
using Microsoft.Maui.Dispatching;
#endif
using SimpleWeather.Extras;
using SimpleWeather.Helpers;
using SimpleWeather.HttpClientExtensions;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
using SimpleWeather.RemoteConfig;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData.Images;
using System;
using System.Collections.Generic;
using System.Net.Http;
using static SimpleWeather.CommonActionChangedEventArgs;

namespace SimpleWeather
{
    public sealed partial class SharedModule : IDisposable
    {
        private readonly ServiceCollection ServiceCollection = new();

        public event CommonActionChangedEventHandler OnCommonActionChanged;

        private readonly static Lazy<SharedModule> lazy = new(() =>
        {
            return new SharedModule();
        });

        private SharedModule()
        {
            ConfigureServices(ServiceCollection);
        }

        public void Initialize()
        {
            Logger.Init();
        }

        public static SharedModule Instance => lazy.Value;

        public HttpClient WebClient => httpClientLazy.Value;

        public WeatherIconsManager WeatherIconsManager => wimLazy.Value;

        public IServiceProvider Services => Ioc.Default;

#if WINUI
        public readonly DispatcherQueue DispatcherQueue = DispatcherQueue.GetForCurrentThread();
#else
        public readonly IDispatcher Dispatcher = Microsoft.Maui.Dispatching.Dispatcher.GetForCurrentThread();
#endif

        public void RequestAction(string Action, IDictionary<string, object> Bundle = null)
        {
            OnCommonActionChanged?.Invoke(null,
                new CommonActionChangedEventArgs(Action, Bundle));
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IExtrasService, DefaultExtrasServiceImpl>();
            serviceCollection.AddSingleton<ImageDataHelperImpl, ImageDataHelperDefault>();
            serviceCollection.AddSingleton<IRemoteConfigService>(DI.Utils.RemoteConfigService);
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
#if WINUI
            var CacheRoot = System.IO.Path.Combine(
                Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path,
                "CacheCow");
#else
            var CacheRoot = System.IO.Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), "CacheCow");
#endif

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