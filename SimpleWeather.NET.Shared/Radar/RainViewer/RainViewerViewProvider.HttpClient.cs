using CacheCow.Client;
using CacheCow.Client.FileCacheStore;
using SimpleWeather.Helpers;
using SimpleWeather.HttpClientExtensions;

namespace SimpleWeather.NET.Radar.RainViewer
{
    public partial class RainViewerViewProvider
    {
        private HttpClient WebClient => httpClientLazy.Value;

        private readonly Lazy<HttpClient> httpClientLazy = new(() =>
        {
#if WINUI
            var CacheRoot = System.IO.Path.Combine(
                Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path,
                "TileCache", "RainViewer");
#else
            var CacheRoot = System.IO.Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), "TileCache", "RainViewer");
#endif

            return ClientExtensions.CreateClient(new RemoveHeaderDelagatingCacheStore(new FileStore(CacheRoot) { MinExpiry = TimeSpan.FromDays(7) }), handler: new CacheFilter());
        });
    }
}