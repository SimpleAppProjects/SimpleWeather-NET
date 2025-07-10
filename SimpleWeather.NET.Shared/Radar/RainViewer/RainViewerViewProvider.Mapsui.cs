#if !__IOS__
using System.Net.Http.Headers;
using BruTile;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Tiling.Fetcher;
using Mapsui.Tiling.Layers;
using Mapsui.Tiling.Rendering;
using SimpleWeather.NET.MapsUi;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Utils;
using HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment;
using VerticalAlignment = Mapsui.Widgets.VerticalAlignment;
using Microsoft.UI.Xaml.Controls;
using TileLayer = SimpleWeather.NET.MapsUi.TileLayer;

#if WINDOWS
using MapControl = Mapsui.UI.WinUI.MapControl;
using Microsoft.UI.Xaml;
#else
using Mapsui.UI.Maui;
using MapControl = Mapsui.UI.Maui.MapControl;
using Microsoft.Maui.Dispatching;
#endif

namespace SimpleWeather.NET.Radar.RainViewer
{
    public partial class RainViewerViewProvider : MapTileRadarViewProvider, IDisposable
    {
        private const string MapsURL = "https://api.rainviewer.com/public/weather-maps.json";
        private const string URLTemplate = "{host}{path}/256/{z}/{x}/{y}/1/1_1.png";

        private readonly List<RadarFrame> AvailableRadarFrames;
        private readonly Dictionary<long, TileLayer> RadarLayers;

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

        public RainViewerViewProvider(Border container) : base(container)
        {
            AvailableRadarFrames = new List<RadarFrame>();
            RadarLayers = new Dictionary<long, TileLayer>();
            cts = new CancellationTokenSource();

            API_ID = RadarProvider.RadarProviders.RainViewer.GetStringValue();
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
                var layer = new TileLayer(CreateTileSource(mapFrame), dataFetchStrategy: new MinimalDataFetchStrategy(), renderFetchStrategy: new MinimalRenderFetchStrategy())
                {
                    Opacity = 0
                };
                layer.Attribution.Enabled = false;
                layer.Attribution.VerticalAlignment = VerticalAlignment.Bottom;
                layer.Attribution.HorizontalAlignment = HorizontalAlignment.Right;
                layer.Attribution.Margin = new MRect(10);
                layer.Attribution.Padding = new MRect(4);

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
            _mapControl.ForceUpdate();

            UpdateToolbar(position, nextFrame);
        }

        private void UpdateToolbar(int position)
        {
            UpdateToolbar(position, AvailableRadarFrames[position]);
        }

        private void UpdateToolbar(int position, RadarFrame mapFrame)
        {
            RadarMapContainer.UpdateTimestamp(position, mapFrame.TimeStamp);
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

        private static readonly Attribution RainViewerAttribution = new("RainViewer", "https://www.rainviewer.com/api.html");

        private HttpTileSource CreateTileSource(RadarFrame? mapFrame)
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

            return new CustomHttpTileSource(new GlobalSphericalMercator(yAxis: YAxis.OSM, minZoomLevel: (int)MIN_ZOOM_LEVEL, maxZoomLevel: (int)MAX_ZOOM_LEVEL, name: "RainViewer"),
                uri, name: this.GetType().Name,
                configureHttpRequestMessage: request =>
                {
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(2)
                    };
                },
                attribution: RainViewerAttribution);
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