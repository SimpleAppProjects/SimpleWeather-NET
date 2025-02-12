#if !__IOS__
using BruTile;
using BruTile.Cache;
using BruTile.Web;
using CacheCow.Client.Headers;
using SimpleWeather.HttpClientExtensions;
using SimpleWeather.Utils;

namespace SimpleWeather.NET.MapsUi
{
    internal class CustomHttpTileSource(ITileSchema tileSchema, IUrlBuilder urlBuilder, string name = null, IPersistentCache<byte[]> persistentCache = null, Attribution? attribution = null, Action<HttpRequestMessage> configureHttpRequestMessage = null) : HttpTileSource(tileSchema, urlBuilder, name, persistentCache, attribution, configureHttpRequestMessage)
    {
        public CustomHttpTileSource(ITileSchema tileSchema, string uri, string name = null, IPersistentCache<byte[]> persistentCache = null, Attribution? attribution = null, Action<HttpRequestMessage> configureHttpRequestMessage = null) : this(tileSchema, new BasicUrlBuilder(uri), name, persistentCache, attribution, configureHttpRequestMessage)
        {
        }

        public override async Task<byte[]?> GetTileAsync(HttpClient httpClient, TileInfo tileInfo, CancellationToken? cancellationToken = null)
        {
            var bytes = PersistentCache.Find(tileInfo.Index);

            if (bytes != null)
                return bytes;

            bytes = await GetTileAsync(httpClient, tileInfo, cancellationToken ?? CancellationToken.None);

            if (bytes != null)
                PersistentCache.Add(tileInfo.Index, bytes);

            return bytes;
        }

        private async Task<byte[]?> GetTileAsync(HttpClient httpClient, TileInfo tileInfo, CancellationToken cancellationToken)
        {
            try
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, GetUrl(tileInfo));

                if (ConfigureHttpRequestMessage is not null)
                    ConfigureHttpRequestMessage(requestMessage);

                requestMessage.Headers.UserAgent.AddAppUserAgent();

                using var response = await httpClient.SendAsync(requestMessage, cancellationToken);
                response.EnsureSuccessStatusCode();

#if DEBUG
                var cacheHeader = response.Headers.GetCacheCowHeader();
                if (cacheHeader?.RetrievedFromCache == true)
                {
                    Logger.Debug(this.Name, "tile fetched from cache");
                }
                else
                {
                    Logger.Debug(this.Name, "tile fetched from web");
                }
#endif

                return await response.Content.ReadAsByteArrayAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.Error(this.Name, ex);
            }

            return null;
        }
    }
}
#endif