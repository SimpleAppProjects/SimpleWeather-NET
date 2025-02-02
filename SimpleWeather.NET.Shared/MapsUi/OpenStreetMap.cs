using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui.Tiling.Layers;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace SimpleWeather.NET.MapsUi
{
    public static class OpenStreetMap
    {
        private static readonly Attribution _openStreetMapAttribution = new Attribution("© OpenStreetMap contributors", "https://www.openstreetmap.org/copyright");

        public static TileLayer CreateTileLayer()
        {
            return new TileLayer(CreateTileSource())
            {
                Name = "Root"
            };
        }

        private static HttpTileSource CreateTileSource()
        {
            return new CustomHttpTileSource(
                new GlobalSphericalMercator(),
                uri: "https://tile.openstreetmap.org/{z}/{x}/{y}.png",
                name: "OpenStreetMap", attribution: _openStreetMapAttribution,
                persistentCache: new FileCache(
                            Path.Combine(SimpleWeather.Helpers.ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, "OpenStreepMap"), "tile.png"),
                configureHttpRequestMessage: request =>
                {
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(7)
                    };
                }
            );
        }
    }
}
