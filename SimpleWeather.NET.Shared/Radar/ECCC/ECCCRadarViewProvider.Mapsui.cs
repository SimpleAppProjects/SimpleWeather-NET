#if !__IOS__
using System.Globalization;
using System.Net.Http.Headers;
using System.Xml;
using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Tiling.Fetcher;
using Mapsui.Tiling.Layers;
using Mapsui.Tiling.Rendering;
using SimpleWeather.Helpers;
using SimpleWeather.NET.MapsUi;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Utils;
using HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment;
using VerticalAlignment = Mapsui.Widgets.VerticalAlignment;
using TileLayer = SimpleWeather.NET.MapsUi.TileLayer;


#if WINDOWS
using MapControl = Mapsui.UI.WinUI.MapControl;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Mapsui.UI.Maui;
using MapControl = Mapsui.UI.Maui.MapControl;
using Microsoft.Maui.Dispatching;
#endif

namespace SimpleWeather.NET.Radar.ECCC
{
    public partial class ECCCRadarViewProvider : MapTileRadarViewProvider, IDisposable
    {
        private readonly List<RadarFrame> AvailableRadarFrames;
        private readonly Dictionary<string, TileLayer> RadarLayers;

        private MapControl _mapControl = null;

        private int AnimationPosition = 0;
#if WINDOWS
        private DispatcherTimer AnimationTimer;
#else
        private IDispatcherTimer AnimationTimer;
#endif

        private bool ProcessingFrames = false;
        private CancellationTokenSource cts;

        private bool disposedValue;

        private readonly string API_ID;

        public ECCCRadarViewProvider(Border container) : base(container)
        {
            AvailableRadarFrames = new List<RadarFrame>();
            RadarLayers = new Dictionary<string, TileLayer>();
            cts = new CancellationTokenSource();

            API_ID = RadarProvider.RadarProviders.ECCC.GetStringValue();
        }

        public override void UpdateRadarView()
        {
            base.UpdateRadarView();

#if WINDOWS
            RadarMapContainer.ToolbarVisibility =
                InteractionsEnabled() && ExtrasService.IsAtLeastProEnabled() ? Visibility.Visible : Visibility.Collapsed;
#else
            RadarMapContainer.IsToolbarVisible = InteractionsEnabled() && ExtrasService.IsAtLeastProEnabled();
#endif
        }

        public override async void UpdateMap(MapControl mapControl)
        {
            base.UpdateMap(mapControl);

            this._mapControl = mapControl;

            await GetRadarFrames();
        }

        private async Task GetRadarFrames()
        {
            var HttpClient = SharedModule.Instance.WebClient;

            try
            {
                RefreshToken();
                var token = cts.Token;

                using var response = await HttpClient.GetAsync(new Uri("https://geo.weather.gc.ca/geomet/?lang=en&service=WMS&version=1.3.0&request=GetCapabilities&LAYERS=Radar_1km_SfcPrecipType"), token);
                await response.CheckForErrors(API_ID, 5000);
                response.EnsureSuccessStatusCode();

                token.ThrowIfCancellationRequested();

                var stream = await response.Content.ReadAsStreamAsync();

                // XPath: /WMS_Capabilities/Capability/Layer/Layer/Layer/Layer/Dimension
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(stream);

                var node = xmlDoc.GetElementsByTagName("Dimension")?.Item(0);
                var dimensions = node?.InnerText?.Split('/');
                if (dimensions?.Length != 3) throw new InvalidOperationException();

                token.ThrowIfCancellationRequested();

                ProcessingFrames = true;

                // 3 hour window / interval - 6 minutes
                var start = DateTimeOffset.ParseExact(dimensions[0], DateTimeUtils.ISO8601_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                var end = DateTimeOffset.ParseExact(dimensions[1], DateTimeUtils.ISO8601_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                var interval = XmlConvert.ToTimeSpan(dimensions[2]);

                // Remove added tile layers
                var layersToRemove = RadarLayers.Values.ToList();
                RadarLayers.Clear();
                layersToRemove.ForEach(layer =>
                {
#if WINDOWS
                    _mapControl?.DispatcherQueue?.TryEnqueue(() =>
#else
                    _mapControl?.Dispatcher?.Dispatch(() =>
#endif
                    {
                        _mapControl?.Map?.Layers?.Remove(layer);
                    });
                });

                if (token.IsCancellationRequested)
                {
                    ProcessingFrames = false;
                    return;
                }

                AvailableRadarFrames.Clear();
                AnimationPosition = 0;

                var current = start;
                var nowIndex = -1;

                while (current <= end)
                {
                    AvailableRadarFrames.Add(new RadarFrame(current.UtcDateTime.ToISO8601Format()));

                    if (current == end)
                    {
                        nowIndex = AvailableRadarFrames.Count - 1;
                    }

                    current = current.Add(interval.Multiply(2));
                }

                ProcessingFrames = false;

#if WINDOWS
                RadarMapContainer?.DispatcherQueue?.TryEnqueue(() =>
#else
                RadarMapContainer?.Dispatcher?.Dispatch(() =>
#endif
                {
                    if (IsViewAlive)
                    {
                        var lastPastFramePosition = nowIndex;
                        ShowFrame(lastPastFramePosition);
                    }
                });
            }
            catch (OperationCanceledException)
            {
                // ignore.
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex);
            }
            finally
            {
                ProcessingFrames = false;
            }
        }

        private void AddLayer(RadarFrame mapFrame)
        {
            if (ProcessingFrames) return;

            if (!RadarLayers.ContainsKey(mapFrame.timestamp))
            {
                var layer = new TileLayer(CreateTileSource(mapFrame), dataFetchStrategy: new MinimalDataFetchStrategy(), renderFetchStrategy: new MinimalRenderFetchStrategy())
                {
                    Opacity = 0
                };
                layer.Attribution.Enabled = false;
                layer.Attribution.VerticalAlignment = VerticalAlignment.Bottom;
                layer.Attribution.HorizontalAlignment = HorizontalAlignment.Right;
                layer.Attribution.Margin = new MRect(10);
                layer.Attribution.Padding = new MRect(4);

                RadarLayers[mapFrame.timestamp] = layer;
                _mapControl.Map.Layers.Add(layer);
            }

            RadarMapContainer?.UpdateSeekbarRange(0, AvailableRadarFrames.Count - 1);
        }

        private void ChangeRadarPosition(int pos, bool preloadOnly = false)
        {
            if (ProcessingFrames) return;

            var position = pos;
            while (position >= AvailableRadarFrames.Count)
            {
                position -= AvailableRadarFrames.Count;
            }
            while (position < 0)
            {
                position += AvailableRadarFrames.Count;
            }

            if (!AvailableRadarFrames.Any() || AnimationPosition >= AvailableRadarFrames.Count || position >= AvailableRadarFrames.Count)
            {
                return;
            }

            var currentFrame = AvailableRadarFrames[AnimationPosition];
            var currentTimeStamp = currentFrame.timestamp;

            var nextFrame = AvailableRadarFrames[position];
            var nextTimeStamp = nextFrame.timestamp;

            AddLayer(nextFrame);

            if (preloadOnly)
            {
                return;
            }

            AnimationPosition = position;

            if (RadarLayers.TryGetValue(currentTimeStamp, out TileLayer currentLayer))
            {
                if (currentLayer != null)
                {
                    currentLayer.Opacity = 0;
                    currentLayer.Attribution.Enabled = false;
                }
            }
            var nextLayer = RadarLayers[nextTimeStamp];
            if (nextLayer != null)
            {
                nextLayer.Opacity = 1;
                nextLayer.Attribution.Enabled = true;
            }
            _mapControl.ForceUpdate();

            UpdateToolbar(position, nextFrame);
        }

        private void UpdateToolbar(int position)
        {
            UpdateToolbar(position, AvailableRadarFrames[position]);
        }

        private void UpdateToolbar(int position, RadarFrame mapFrame)
        {
            var ts = DateTimeOffset.ParseExact(mapFrame.timestamp, DateTimeUtils.ISO8601_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            RadarMapContainer.UpdateTimestamp(position, ts.LocalDateTime);
        }

        /**
         * Check availability and show particular frame position from the timestamps list
         */
        private void ShowFrame(int nextPosition)
        {
            if (ProcessingFrames) return;

            var preloadingDirection = nextPosition - AnimationPosition > 0 ? 1 : -1;

            ChangeRadarPosition(nextPosition);

            // preload next next frame (typically, +1 frame)
            // if don't do that, the animation will be blinking at the first loop
            ChangeRadarPosition(nextPosition + preloadingDirection, true);
        }

        public override void OnDestroyView()
        {
            AnimationTimer?.Stop();
            base.OnDestroyView();
            AvailableRadarFrames?.Clear();
        }

        protected override void OnPlayRadarAnimation()
        {
            if (AnimationTimer == null)
            {
#if WINDOWS
                AnimationTimer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds(500)
                };
#else
                AnimationTimer = RadarMapContainer.Dispatcher.CreateTimer();
                AnimationTimer.Interval = TimeSpan.FromMilliseconds(500);
#endif
                AnimationTimer.Tick += (s, ev) =>
                {
                    // Update toolbar
                    if (IsViewAlive)
                    {
                        ShowFrame(AnimationPosition + 1);
                    }
                    else
                    {
                        AnimationTimer?.Stop();
                    }
                };
            }
            AnimationTimer?.Start();
        }

        protected override void OnPauseRadarAnimation()
        {
            AnimationTimer?.Stop();
        }

        private void RefreshToken()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
        }

        private static readonly Attribution ECCCAttribution = new("Environment and Climate Change Canada (ECCC)", "https://geo.weather.gc.ca");

        private HttpTileSource CreateTileSource(RadarFrame? mapFrame)
        {
            return new CustomHttpTileSource(new GlobalSphericalMercator(yAxis: YAxis.OSM, minZoomLevel: (int)MIN_ZOOM_LEVEL, maxZoomLevel: (int)MAX_ZOOM_LEVEL, name: "ECCC"),
                new ECCCTileProvider(mapFrame), name: this.GetType().Name,
                configureHttpRequestMessage: request =>
                {
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromMinutes(30)
                    };
                },
                persistentCache: new FileCache(Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, "ECCC"), "tile.png"),
                attribution: ECCCAttribution);
        }

        private class ECCCTileProvider(RadarFrame mapFrame) : IUrlBuilder
        {
            public Uri GetUrl(TileInfo info)
            {
                var zoom = info.Index.Level;
                var x = info.Index.Col;
                var y = info.Index.Row;

                if (mapFrame != null)
                {
                    var bbox = BoundingBox.FromTile(x, y, zoom);

                    return "https://geo.weather.gc.ca/geomet".ToUriBuilderEx()
                        .AppendQueryParameter("SERVICE", "WMS")
                        .AppendQueryParameter("VERSION", "1.3.0")
                        .AppendQueryParameter("REQUEST", "GetMap")
                        .AppendQueryParameter("BBOX", bbox.ToString())
                        .AppendQueryParameter("CRS", "EPSG:4326")
                        .AppendQueryParameter("WIDTH", "256")
                        .AppendQueryParameter("HEIGHT", "256")
                        .AppendQueryParameter("LAYERS", "Radar_1km_SfcPrecipType")
                        .AppendQueryParameter("FORMAT", "image/png")
                        .AppendQueryParameter("TIME", mapFrame.timestamp) // ex) 2019-06-21T12:00:00Z
                        .BuildUri();
                }

                return null;
            }
        }

        private record BoundingBox(double xMin, double yMin, double xMax, double yMax)
        {
            public override string ToString()
            {
                return $"{yMin},{xMin},{yMax},{xMax}";
            }

            public static BoundingBox FromTile(int x, int y, int zoom)
            {
                return new BoundingBox(
                    yMin: tiley2lat(y + 1, zoom),
                    yMax: tiley2lat(y, zoom),
                    xMin: tilex2long(x, zoom),
                    xMax: tilex2long(x + 1, zoom)
                );
            }

            // Source: https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Common_programming_languages
            public static double tilex2long(int x, int z)
            {
                return x / (double)(1 << z) * 360.0 - 180;
            }

            public static double tiley2lat(int y, int z)
            {
                double n = Math.PI - 2.0 * Math.PI * y / (double)(1 << z);
                return 180.0 / Math.PI * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));
            }
        };

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cts?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
#endif