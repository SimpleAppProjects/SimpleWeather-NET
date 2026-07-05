#if __IOS__
using Foundation;
using MapKit;
using SimpleWeather.Maui.Maps;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Utils;
using System.Globalization;
using System.Xml;
using MapControl = Microsoft.Maui.Controls.Maps.Map;
using TileLayer = SimpleWeather.Maui.Maps.ICustomTileOverlay;

namespace SimpleWeather.NET.Radar.WeatherApi
{
    public partial class WeatherApiRadarViewProvider : MapTileRadarViewProvider, IDisposable
    {
        private const float DEFAULT_TRANSPARENCY = 0.5f;

        private readonly List<RadarFrame> AvailableRadarFrames;
        private readonly Dictionary<DateTimeOffset, TileLayer> RadarLayers;

        private MapControl _mapControl = null;

        private int AnimationPosition = 0;
        private IDispatcherTimer AnimationTimer;

        private bool ProcessingFrames = false;
        private CancellationTokenSource cts;

        private bool disposedValue;

        private readonly string API_ID;

        public WeatherApiRadarViewProvider(Border container) : base(container)
        {
            AvailableRadarFrames = new List<RadarFrame>();
            RadarLayers = new Dictionary<DateTimeOffset, TileLayer>();
            cts = new CancellationTokenSource();

            API_ID = RadarProvider.RadarProviders.WeatherApi.GetStringValue();
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

            var now = DateTimeOffset.UtcNow.Trim(TimeSpan.TicksPerHour);
            var start = now;
            var end = now.AddHours(12);

            var current = start;
            var nowIndex = -1;

            while (current <= end)
            {
                AvailableRadarFrames.Add(new RadarFrame(current));

                if (current == end)
                {
                    nowIndex = AvailableRadarFrames.Count - 1;
                }

                current = current.AddHours(1);
            }

            ProcessingFrames = false;

            RadarMapContainer?.Dispatcher?.Dispatch(() =>
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
                var layer = new WeatherApiTileProvider(GetUrlTemplate(mapFrame))
                {
                    Alpha = 0,
                    MinimumZ = (int)MIN_ZOOM_LEVEL,
                    MaximumZ = 6,
                };

                RadarLayers[mapFrame.timestamp] = layer;
                _mapControl.AddOverlay(layer);
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
                    currentLayer.Alpha = 0;
                }
            }
            var nextLayer = RadarLayers[nextTimeStamp];
            if (nextLayer != null)
            {
                nextLayer.Alpha = DEFAULT_TRANSPARENCY;
            }

            UpdateToolbar(position, nextFrame);
        }

        private void UpdateToolbar(int position)
        {
            UpdateToolbar(position, AvailableRadarFrames[position]);
        }

        private void UpdateToolbar(int position, RadarFrame mapFrame)
        {
            var ts = mapFrame.timestamp.ToLocalTime();
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

        private string GetUrlTemplate(RadarFrame mapFrame)
        {
            var date = mapFrame.timestamp.Date.ToString("yyyyMMdd");
            var hour = mapFrame.timestamp.Hour.ToString("D2");

            return $"https://weathermaps.weatherapi.com/precip/tiles/{date}{hour}/{{z}}/{{x}}/{{y}}.png";
        }

        private class WeatherApiTileProvider(string urlTemplate) : CustomWmsTileOverlay("WeatherApiTileProvider", urlTemplate, cacheTimeSeconds: 60 * 30)
        {
            public override NSUrl URLForTilePath(MKTileOverlayPath path)
            {
                var url = URLTemplate!.Replace("{z}", $"{path.Z.ToInt32()}")
                    .Replace("{x}", $"{path.X.ToInt32()}")
                    .Replace("{y}", $"{path.Y.ToInt32()}")
                    .ToUriBuilderEx()
                    .BuildUri();

                return url;
            }
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