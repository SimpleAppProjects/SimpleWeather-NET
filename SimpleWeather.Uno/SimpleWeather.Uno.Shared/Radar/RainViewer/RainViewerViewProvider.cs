using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui.Tiling.Layers;
using Mapsui.UI.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleWeather.Helpers;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.Uno.Radar.RainViewer
{
    public class RainViewerViewProvider : MapTileRadarViewProvider, IDisposable
    {
        private const string MapsURL = "https://api.rainviewer.com/public/weather-maps.json";
        private const string URLTemplate = "{host}{path}/256/{z}/{x}/{y}/1/1_1.png";

        private readonly List<RadarFrame> AvailableRadarFrames;
        private readonly Dictionary<long, TileLayer> RadarLayers;
        private HttpTileSource TileSource;

        private MapControl _mapControl = null;

        private int AnimationPosition = 0;
        private DispatcherTimer AnimationTimer;

        private bool ProcessingFrames = false;
        private CancellationTokenSource cts;

        private bool disposedValue;

        private readonly string API_ID;

        public RainViewerViewProvider(Border container) : base(container)
        {
            AvailableRadarFrames = new List<RadarFrame>();
            RadarLayers = new Dictionary<long, TileLayer>();
            cts = new CancellationTokenSource();

            API_ID = RadarProvider.RadarProviders.RainViewer.GetStringValue();
        }

        public override async void UpdateMap(MapControl mapControl)
        {
            this._mapControl = mapControl;

            if (TileSource == null)
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
                    _mapControl?.DispatcherQueue?.TryEnqueue(() =>
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

                RadarMapContainer.DispatcherQueue.TryEnqueue(() =>
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
                RadarLayers[mapFrame.TimeStamp] = layer;
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
                    currentLayer.Opacity = 1;
                }
            }
            var nextLayer = RadarLayers[nextTimeStamp];
            if (nextLayer != null)
            {
                nextLayer.Opacity = 0;
            }

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
            TileSource = null;
        }

        private void RadarMapContainer_OnPlayAnimation(object sender, EventArgs e)
        {
            if (AnimationTimer == null)
            {
                AnimationTimer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds(500)
                };
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

            return new HttpTileSource(new GlobalSphericalMercator(6, 6),
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

    internal sealed class RadarFrame
    {
        public String Host { get; }
        public String Path { get; }
        public long TimeStamp { get; }

        public RadarFrame(long timeStamp, string host, string path)
        {
            Host = host;
            Path = path;
            TimeStamp = timeStamp;
        }
    }
}
