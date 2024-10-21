//#if !(ANDROID || IOS || MACCATALYST)
using BruTile.Predefined;
using BruTile.Web;
using CacheCow.Client.Headers;
using Mapsui.Tiling.Fetcher;
using Mapsui.Tiling.Layers;
using Mapsui.Tiling.Rendering;
#if WINDOWS
using MapControl = Mapsui.UI.WinUI.MapControl;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Mapsui.UI.Maui;
using MapControl = Mapsui.UI.Maui.MapControl;
using Microsoft.Maui.Dispatching;
#endif
using SimpleWeather.Utils;
using System.Net.Http.Headers;
using BruTile;

namespace SimpleWeather.NET.Radar.NWS
{
    public partial class NWSRadarViewProvider : MapTileRadarViewProvider, IDisposable
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

        public NWSRadarViewProvider(Border container) : base(container)
        {
            AvailableRadarFrames = new List<RadarFrame>();
            RadarLayers = new Dictionary<string, TileLayer>();
            cts = new CancellationTokenSource();
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

        public override void UpdateMap(MapControl mapControl)
        {
            base.UpdateMap(mapControl);

            this._mapControl = mapControl;

            GetRadarFrames();
        }

        private void GetRadarFrames()
        {
            ProcessingFrames = true;

            RefreshToken();
            var token = cts.Token;

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

            var now = DateTimeOffset.UtcNow.Trim(TimeSpan.TicksPerMinute);
            var minute = now.Minute;
            now = now.AddMinutes(-(minute % 10));

            var start = now.AddHours(-2);
            var end = now;

            var current = start;
            var nowIndex = -1;

            while (current <= end)
            {
                AvailableRadarFrames.Add(new RadarFrame(current.ToUnixTimeMilliseconds().ToInvariantString()));

                if (current == now)
                {
                    nowIndex = AvailableRadarFrames.Count - 1;
                }

                current = current.AddMinutes(10);
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
                layer.Attribution.MarginY = 18;
                layer.Attribution.PaddingX = 5;
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
            var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(mapFrame.timestamp));
            RadarMapContainer.UpdateTimestamp(position, dateTime.LocalDateTime);
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

        private static readonly BruTile.Attribution NWSAttribution = new("National Weather Service (NOAA)", "https://radar.weather.gov/");

        private HttpTileSource CreateTileSource(RadarFrame? mapFrame)
        {
            return new HttpTileSource(new GlobalSphericalMercator(yAxis: BruTile.YAxis.OSM, minZoomLevel: (int)MIN_ZOOM_LEVEL, maxZoomLevel: (int)MAX_ZOOM_LEVEL, name: "NWS"),
                new NWSTileProvider(mapFrame), name: NWSAttribution.Text,
                tileFetcher: FetchTileAsync,
                attribution: NWSAttribution, userAgent: Constants.GetUserAgentString());
        }

        private async Task<byte[]> FetchTileAsync(Uri arg)
        {
            byte[] arr = null;

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, arg);
                request.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    MaxAge = TimeSpan.FromMinutes(30)
                };

                using var response = await WebClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

#if DEBUG
                var cacheHeader = response.Headers.GetCacheCowHeader();
                if (cacheHeader?.RetrievedFromCache == true)
                {
                    Logger.WriteLine(LoggerLevel.Debug, $"{nameof(NWSRadarViewProvider)}: tile fetched from cache");
                }
                else
                {
                    Logger.WriteLine(LoggerLevel.Debug, $"{nameof(NWSRadarViewProvider)}: tile fetched from web");
                }
#endif

                arr = await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex);
            }

            return arr;
        }

        private class NWSTileProvider(RadarFrame mapFrame) : IRequest
        {
            public Uri GetUri(TileInfo info)
            {
                var zoom = info.Index.Level;
                var x = info.Index.Col;
                var y = info.Index.Row;

                if (mapFrame != null)
                {
                    var bbox = BoundingBox.FromTile(x, y, zoom);

                    return "https://mapservices.weather.noaa.gov/eventdriven/rest/services/radar/radar_base_reflectivity_time/ImageServer/exportImage".ToUriBuilderEx()
                        .AppendQueryParameter("bbox", bbox.ToString())
                        .AppendQueryParameter("bboxSR", "4326")
                        .AppendQueryParameter("size", "256,256")
                        .AppendQueryParameter("time", mapFrame.timestamp)
                        .AppendQueryParameter("format", "png")
                        .AppendQueryParameter("f", "image")
                        .BuildUri();
                }

                return null;
            }
        }

        private record BoundingBox(double xMin, double yMin, double xMax, double yMax)
        {
            public override string ToString()
            {
                return $"{xMin},{yMin},{xMax},{yMax}";
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
//#endif