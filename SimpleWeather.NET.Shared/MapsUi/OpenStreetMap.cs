#if !__IOS__
using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using CommunityToolkit.Mvvm.DependencyInjection;
using Mapsui.Tiling.Layers;
using SimpleWeather.Helpers;
using System.Net.Http.Headers;

namespace SimpleWeather.NET.MapsUi
{
    public static class OpenStreetMap
    {
        private static readonly Attribution _openStreetMapAttribution = new Attribution("© OpenStreetMap contributors", "https://www.openstreetmap.org/copyright");

        public static TileLayer CreateTileLayer()
        {
            return new TileLayer(CreateTileSource(), httpClient: Ioc.Default.GetService<HttpClient>())
            {
                Name = "Root"
            };
        }

        private static HttpTileSource CreateTileSource()
        {
            return new CustomHttpTileSource(
                new GlobalSphericalMercator(name: "OpenStreetMap", minZoomLevel: 2, maxZoomLevel: 18),
                uri: "https://tile.openstreetmap.org/{z}/{x}/{y}.png",
                name: "OpenStreetMap", attribution: _openStreetMapAttribution,
                persistentCache: new FileCache(
                            Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, "OpenStreepMap"), "tile.png"),
                configureHttpRequestMessage: request =>
                {
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(30)
                    };
                }
            );
        }
    }
}
#endif