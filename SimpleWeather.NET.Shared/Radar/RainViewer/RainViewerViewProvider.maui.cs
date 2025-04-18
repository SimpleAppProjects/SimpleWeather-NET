﻿#if __IOS__
using SimpleWeather.Maui.Maps;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Utils;
using MapControl = Microsoft.Maui.Controls.Maps.Map;
using TileLayer = SimpleWeather.Maui.Maps.ICustomTileOverlay;

namespace SimpleWeather.NET.Radar.RainViewer
{
    public partial class RainViewerViewProvider : MapTileRadarViewProvider, IDisposable
    {
        private const string MapsURL = "https://api.rainviewer.com/public/weather-maps.json";

        private readonly List<RadarFrame> AvailableRadarFrames;
        private readonly Dictionary<long, TileLayer> RadarLayers;

        private MapControl _mapControl = null;

        private int AnimationPosition = 0;
        private IDispatcherTimer AnimationTimer;

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
            RadarMapContainer.IsToolbarVisible = InteractionsEnabled() && ExtrasService.IsAtLeastProEnabled();
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
                    _mapControl?.Dispatcher?.Dispatch(() =>
                    {
                        _mapControl?.RemoveOverlay(layer);
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

                RadarMapContainer?.Dispatcher?.Dispatch(() =>
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
                var layer = new CustomTileOverlay("RainViewerTileProvider", GetUrlTemplate(mapFrame), cacheTimeSeconds: 172800) // 48hrs
                {
                    Alpha = 0,
                    CanReplaceMapContent = false,
                    MinimumZ = (int)MIN_ZOOM_LEVEL,
                    MaximumZ = (int)MAX_ZOOM_LEVEL,
                };

                RadarLayers[mapFrame.TimeStamp] = layer;
                _mapControl?.Dispatcher?.Dispatch(() =>
                {
                    _mapControl?.AddOverlay(layer);
                });
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
                    currentLayer.Alpha = 0;
                }
            }
            var nextLayer = RadarLayers[nextTimeStamp];
            if (nextLayer != null)
            {
                nextLayer.Alpha = 1;
            }

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
                AnimationTimer = RadarMapContainer.Dispatcher.CreateTimer();
                AnimationTimer.Interval = TimeSpan.FromMilliseconds(500);
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

        private string GetUrlTemplate(RadarFrame mapFrame)
        {
            return $"{mapFrame.Host}{mapFrame.Path}/256/{{z}}/{{x}}/{{y}}/1/1_1.png";
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