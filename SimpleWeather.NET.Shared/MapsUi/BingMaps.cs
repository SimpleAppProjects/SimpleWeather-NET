using BingMapsRESTToolkit;
using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Providers.Wfs.Utilities;
using Mapsui.Tiling.Layers;
using SimpleWeather.Helpers;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using System.Net.Http.Headers;

namespace SimpleWeather.NET.MapsUi
{
    public static class BingMaps
    {
        private static readonly Dictionary<ImageryType, ImageryMetadata> ImageryMetadataCache = new();

        public static async Task<TileLayer> CreateBingMapsLayer(ImageryType imageryType = ImageryType.RoadOnDemand, bool isDarkMode = false)
        {
            var dynamicTileSource = await GetBingMapsTileSource(imageryType, isDarkMode);
            TileLayer tileLayer;

            if (dynamicTileSource != null)
            {
                tileLayer = new TileLayer(dynamicTileSource)
                {
                    Name = "Root"
                };
            }
            else
            {
                tileLayer = new TileLayer(
                    tileSource: KnownTileSources.Create(KnownTileSource.BingRoads, APIKeys.GetBingMapsKey(), new FileCache(
                        Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, nameof(KnownTileSource.BingRoads)), "tile.png")
                    ))
                {
                    Name = "Root",
                };
            }

            tileLayer.Attribution.VerticalAlignment = Mapsui.Widgets.VerticalAlignment.Bottom;
            tileLayer.Attribution.HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment.Right;

            return tileLayer;
        }

        public static async Task<ITileSource> GetBingMapsTileSource(ImageryType imageryType = ImageryType.RoadOnDemand, bool isDarkMode = false)
        {
            var key = APIKeys.GetBingMapsKey();
            var culture = LocaleUtils.GetLocale();

            try
            {
                var metadata = ImageryMetadataCache.GetValueOrDefault(imageryType);

                if (metadata == null)
                {
                    using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);

                    var request = new ImageryMetadataRequest()
                    {
                        BingMapsKey = key,
                        Culture = culture.Name,
                        ImagerySet = imageryType,
                        IncludeImageryProviders = true,
                        UseHTTPS = true,
                    };

                    var response = await request.Execute().WaitAsync(cts.Token);

                    ImageryMetadataCache[imageryType] = metadata = response.StatusCode switch
                    {
                        // OK
                        200 or 201 or 202 => response?.ResourceSets?.FirstOrDefault()?.Resources?.FirstOrDefault() as ImageryMetadata,
                        _ => null,
                    };
                }

                if (metadata != null)
                {
                    // Format tile url
                    var tileUrl = metadata.ImageUrl.ReplaceFirst("http:", "https:")
                        .ReplaceFirst("{subdomain}", "{s}")
                        .AppendQuery("token={k}");

                    if (isDarkMode)
                    {
                        tileUrl = tileUrl.AppendQuery(BingMapsCanvasDarkStyleQuery);
                    }

                    Attribution attribution;
                    if (metadata?.ImageryProviders?.FirstOrDefault() is ImageryProvider provider)
                    {
                        attribution = new Attribution(provider.Attribution);
                    }
                    else
                    {
                        attribution = new Attribution("© Microsoft");
                    }

                    string name = $"Bing{imageryType}";

                    return new CustomHttpTileSource(
                        tileSchema: new GlobalSphericalMercator(Math.Max(1, metadata.ZoomMin), Math.Min(19, metadata.ZoomMax)),
                        new BasicUrlBuilder(urlFormatter: tileUrl, serverNodes: metadata.ImageUrlSubdomains, apiKey: key), attribution: attribution,
                        name: name,
                        persistentCache: new FileCache(
                            Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, name), "tile.png"),
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
            catch (Exception ex)
            {
                Logger.Debug("BingMaps", ex);
            }

            return null;
        }

        private const string BingMapsCanvasDarkStyleQuery = "st=g|lc:FF353F54_me|lbc:CCFFFFFF;loc:40000000_rd|fc:66FF7903;sc:66FF7903_wt|fc:FF162439;lbc:FF567D98;loc:33162439";
    }
}
