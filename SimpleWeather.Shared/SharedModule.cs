using CacheCow.Client;
using CacheCow.Client.FileCacheStore;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
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
#if WINDOWS_UWP
using Windows.ApplicationModel.Resources;
#endif
using static SimpleWeather.CommonActionChangedEventArgs;

namespace SimpleWeather
{
    public sealed partial class SharedModule : IDisposable
    {
        private readonly ResourceLoader ResourceLoader;
        private readonly ServiceCollection ServiceCollection = new();

        public event CommonActionChangedEventHandler OnCommonActionChanged;

        private readonly static Lazy<SharedModule> lazy = new(() =>
        {
            return new SharedModule();
        });

        private SharedModule()
        {
#if WINDOWS_UWP
            ResourceLoader = GetResourceLoader();
#endif

            ConfigureServices(ServiceCollection);
        }

        public void Initialize()
        {
            Logger.Init();
        }

        public static SharedModule Instance => lazy.Value;

#if WINDOWS_UWP
        public ResourceLoader ResLoader => ResourceLoader;
#endif

        public HttpClient WebClient => httpClientLazy.Value;

        public WeatherIconsManager WeatherIconsManager => wimLazy.Value;

        public IServiceProvider Services => Ioc.Default;

        public void RequestAction(string Action, IDictionary<string, object> Bundle = null)
        {
            OnCommonActionChanged?.Invoke(null,
                new CommonActionChangedEventArgs(Action, Bundle));
        }

#if WINDOWS_UWP
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
#endif

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IExtrasService, DefaultExtrasServiceImpl>();
            serviceCollection.AddSingleton<ImageDataHelperImpl, ImageDataHelperDefault>();
            serviceCollection.AddSingleton<IRemoteConfigService>(DI.Utils.RemoteConfigService);
#if WINDOWS_UWP
            serviceCollection.AddSingleton(ResLoader);
#endif
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
#if WINDOWS_UWP
                Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path,
#endif
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