#if false //ANDROID || IOS || MACCATALYST
using Microsoft.Maui.Controls.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;
using Microsoft.Maui.Dispatching;
using SimpleWeather.Helpers;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Utils;

namespace SimpleWeather.NET.Radar.RainViewer
{
    public class RainViewerViewProvider : MapTileRadarViewProvider, IDisposable
    {
        private const string MapsURL = "https://api.rainviewer.com/public/weather-maps.json";
        private const string URLTemplate = "{host}{path}/256/{z}/{x}/{y}/1/1_1.png";

        private readonly List<RadarFrame> AvailableRadarFrames;
        private readonly Dictionary<long, MapElement> RadarLayers;

        private Map _mapControl = null;

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

        public RainViewerViewProvider(Border container) : base(container)
        {
            AvailableRadarFrames = new List<RadarFrame>();
            RadarLayers = new Dictionary<long, MapElement>();
            cts = new CancellationTokenSource();

            API_ID = RadarProvider.RadarProviders.RainViewer.GetStringValue();
        }

        public override async void UpdateMap(Map mapControl)
        {
            this._mapControl = mapControl;

            new Microsoft.Maui.Controls.Maps.()
            {
                
            }

            await GetRadarFrames();
        }

        private async Task GetRadarFrames()
        {
            var HttpClient = SharedModule.Instance.WebClient;

            try
            {
                RefreshToken();
                var token = cts.Token;

                using var response = await HttpClient.GetAsync(new Uri(MapsURL), token);
                await response.CheckForErrors(API_ID, 5000);
                response.EnsureSuccessStatusCode();

                token.ThrowIfCancellationRequested();

                var stream = await response.Content.ReadAsStreamAsync();
                var root = await JSONParser.DeserializerAsync<Rootobject>(stream);

                token.ThrowIfCancellationRequested();

                ProcessingFrames = true;

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

                if (root?.radar != null)
                {
                    if (root.radar?.past?.Count > 0)
                    {
                        root.radar.past.RemoveAll(t => t == null);
                        AvailableRadarFrames.AddRange(
                            root.radar.past.Select(f => new RadarFrame(f.time, root.host, f.path))
                            );
                    }

                    if (root.radar?.nowcast?.Count > 0)
                    {
                        root.radar.nowcast.RemoveAll(t => t == null);
                        AvailableRadarFrames.AddRange(
                            root.radar.nowcast.Select(f => new RadarFrame(f.time, root.host, f.path))
                            );
                    }
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
                        var lastPastFramePosition = (root?.radar?.past?.Count ?? 0) - 1;
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

            if (!RadarLayers.ContainsKey(mapFrame.TimeStamp))
            {
                var layer = new TileLayer(CreateTileSource(mapFrame))
                {
                    Opacity = 0
                };
                layer.Attribution.Enabled = false;
                layer.Attribution.MarginY = 18;
                layer.Attribution.PaddingX = 5;
                RadarLayers[mapFrame.TimeStamp] = layer;
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
            var currentTimeStamp = currentFrame.TimeStamp;

            var nextFrame = AvailableRadarFrames[position];
            var nextTimeStamp = nextFrame.TimeStamp;

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
            _mapControl.RefreshGraphics();

            UpdateToolbar(position, nextFrame);
        }

        private void UpdateToolbar(int position)
        {
            UpdateToolbar(position, AvailableRadarFrames[position]);
        }

        private void UpdateToolbar(int position, RadarFrame mapFrame)
        {
            RadarMapContainer.UpdateTimestamp(AnimationPosition, AvailableRadarFrames[AnimationPosition].TimeStamp);
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

        private void RadarMapContainer_OnPlayAnimation(object sender, EventArgs e)
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

        private void RadarMapContainer_OnPauseAnimation(object sender, EventArgs e)
        {
            AnimationTimer?.Stop();
        }

        private void RefreshToken()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
        }

        private static readonly BruTile.Attribution RainViewerAttribution = new("RainViewer", "https://www.rainviewer.com/api.html");

        private static HttpTileSource CreateTileSource(RadarFrame? mapFrame)
        {
            string uri;
            if (mapFrame != null)
            {
                uri = URLTemplate.Replace("{host}", mapFrame.Host).Replace("{path}", mapFrame.Path);
            }
            else
            {
                uri = "about:blank";
            }

            return new HttpTileSource(new GlobalSphericalMercator(yAxis: BruTile.YAxis.OSM, minZoomLevel: 6, maxZoomLevel: 6, name: "RainViewer"),
                uri, name: RainViewerAttribution.Text,
                persistentCache: new FileCache(Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, "RainViewer"), "tile.png"),
                attribution: RainViewerAttribution, userAgent: Constants.GetUserAgentString());
        }

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