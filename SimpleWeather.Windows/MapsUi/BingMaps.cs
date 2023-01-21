using BingMapsRESTToolkit;
using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui.Providers.Wfs.Utilities;
using Mapsui.Tiling.Layers;
using SimpleWeather.Helpers;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.NET.MapsUi
{
    public static class BingMaps
    {
        private static readonly Dictionary<ImageryType, ImageryMetadata> ImageryMetadataCache = new();

        public static TileLayer CreateBingRoadsLayer(string? userAgent = null)
        {
            return new TileLayer(
                tileSource: KnownTileSources.Create(KnownTileSource.BingRoads, APIKeys.GetBingMapsKey(), new FileCache(
                    Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, nameof(KnownTileSource.BingRoads)), "tile.png")
                ))
            {
                Name = "Root"
            };
        }

        public static TileLayer CreateBingAerialLayer(string? userAgent = null)
        {
            return new TileLayer(
                tileSource: KnownTileSources.Create(KnownTileSource.BingAerial, APIKeys.GetBingMapsKey(), new FileCache(
                    Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, nameof(KnownTileSource.BingAerial)), "tile.png")
                ))
            {
                Name = "Root"
            };
        }

        public static async Task<TileLayer> CreateBingRoadsOnDemandLayer(bool isDarkMode = false, string? userAgent = null)
        {
            var dynamicTileSource = await GetBingMapsRoadOnDemandTileSource(isDarkMode, userAgent);

            if (dynamicTileSource != null)
            {
                return new TileLayer(dynamicTileSource)
                {
                    Name = "Root"
                };
            }
            else
            {
                return new TileLayer(
                    tileSource: KnownTileSources.Create(KnownTileSource.BingRoads, APIKeys.GetBingMapsKey(), new FileCache(
                        Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, nameof(KnownTileSource.BingRoads)), "tile.png")
                    ))
                {
                    Name = "Root"
                };
            }
        }

        public static async Task<TileLayer> CreateBingCanvasGrayLayer(string? userAgent = null)
        {
            var dynamicTileSource = await GetBingMapsCanvasGrayTileSource(userAgent);

            if (dynamicTileSource != null)
            {
                return new TileLayer(dynamicTileSource)
                {
                    Name = "Root"
                };
            }
            else
            {
                return new TileLayer(
                    tileSource: KnownTileSources.Create(KnownTileSource.BingRoads, APIKeys.GetBingMapsKey(), new FileCache(
                        Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, nameof(KnownTileSource.BingRoads)), "tile.png")
                    ))
                {
                    Name = "Root"
                };
            }
        }

        public static async Task<ITileSource> GetBingMapsCanvasGrayTileSource(string? userAgent = null)
        {
            var key = APIKeys.GetBingMapsKey();
            var culture = CultureUtils.UserCulture;

            try
            {
                var metadata = ImageryMetadataCache.GetValueOrDefault(ImageryType.CanvasGray);

                if (metadata == null)
                {
                    using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);

                    var request = new ImageryMetadataRequest()
                    {
                        BingMapsKey = key,
                        Culture = culture.Name,
                        ImagerySet = ImageryType.CanvasGray,
                        IncludeImageryProviders = true,
                        UseHTTPS = true,
                    };

                    var response =
#if WINDOWS || NETSTANDARD2_0
                        await request.Execute().AsAsyncOperation().AsTask(cts.Token);
#else
                    await request.Execute().WaitAsync(cts.Token);
#endif
                    ImageryMetadataCache[ImageryType.CanvasGray] = metadata = response.StatusCode switch
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

                    Attribution attribution;
                    if (metadata?.ImageryProviders?.FirstOrDefault() is ImageryProvider provider)
                    {
                        attribution = new Attribution(provider.Attribution);
                    }
                    else
                    {
                        attribution = new Attribution("© Microsoft");
                    }

                    return new HttpTileSource(
                        tileSchema: new GlobalSphericalMercator(Math.Max(1, metadata.ZoomMin), Math.Min(19, metadata.ZoomMax)),
                        urlFormatter: tileUrl, serverNodes: metadata.ImageUrlSubdomains, apiKey: key, attribution: attribution,
                        userAgent: userAgent, name: "BingCanvasGray",
                        persistentCache: new FileCache(
                            Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, "BingCanvasGray"), "tile.png")
                        );
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public static async Task<ITileSource> GetBingMapsRoadOnDemandTileSource(bool isDarkMode = false, string? userAgent = null)
        {
            var key = APIKeys.GetBingMapsKey();
            var culture = CultureUtils.UserCulture;

            try
            {
                var metadata = ImageryMetadataCache.GetValueOrDefault(ImageryType.RoadOnDemand);

                if (metadata == null)
                {
                    using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);

                    var request = new ImageryMetadataRequest()
                    {
                        BingMapsKey = key,
                        Culture = culture.Name,
                        ImagerySet = ImageryType.RoadOnDemand,
                        IncludeImageryProviders = true,
                        UseHTTPS = true,
                    };

                    var response =
#if WINDOWS || NETSTANDARD2_0
                        await request.Execute().AsAsyncOperation().AsTask(cts.Token);
#else
                    await request.Execute().WaitAsync(cts.Token);
#endif
                    ImageryMetadataCache[ImageryType.RoadOnDemand] = metadata = response.StatusCode switch
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
                        tileUrl = tileUrl.AppendQuery(BingMapsRoadDarkStyleQuery);
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

                    string name = isDarkMode switch
                    {
                        true => "BingRoadOnDemandDark",
                        false => "BingRoadOnDemand"
                    };

                    return new HttpTileSource(
                        tileSchema: new GlobalSphericalMercator(Math.Max(1, metadata.ZoomMin), Math.Min(19, metadata.ZoomMax)),
                        urlFormatter: tileUrl, serverNodes: metadata.ImageUrlSubdomains, apiKey: key, attribution: attribution,
                        userAgent: userAgent, name: name,
                        persistentCache: new FileCache(
                            Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, name), "tile.png")
                        );
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        private const string BingMapsRoadDarkStyleQuery = "&st=g|lc:FF0B334D_me|lbc:FFFFFFFF;loc:FF000000_ar|fc:FF115166_pt|fc:FF000000;ic:FF0C4152;sc:FF0C4152_pl|boc:00000000;bsc:FF144B53_str|fc:FF115166_trs|fc:FF000000;sc:FF000000_rl|fc:FF000000;sc:FF146474_ard|fc:FF000000;sc:FF995002_cah|fc:FF000000;sc:FF995002_hg|fc:FF000000;sc:FF995002_mr|fc:FF000000;sc:FF995002_wt|fc:FF021019";
    }
}
