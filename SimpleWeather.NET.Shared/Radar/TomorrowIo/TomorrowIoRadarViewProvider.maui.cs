#if __IOS__
using System.Globalization;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Maui.Maps;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.WeatherData;
using MapControl = Microsoft.Maui.Controls.Maps.Map;
using TileLayer = SimpleWeather.Maui.Maps.ICustomTileOverlay;

namespace SimpleWeather.NET.Radar.TomorrowIo
{
    public partial class TomorrowIoRadarViewProvider : MapTileRadarViewProvider, IDisposable
    {
        private readonly List<RadarFrame> AvailableRadarFrames;
        private readonly Dictionary<string, TileLayer> RadarLayers;

        private MapControl _mapControl = null;

        private int AnimationPosition = 0;
        private IDispatcherTimer AnimationTimer;

        private bool ProcessingFrames = false;
        private CancellationTokenSource cts;

        private bool disposedValue;

        private readonly string API_ID;

        public TomorrowIoRadarViewProvider(Border container) : base(container)
        {
            AvailableRadarFrames = new List<RadarFrame>();
            RadarLayers = new Dictionary<string, TileLayer>();
            cts = new CancellationTokenSource();

            API_ID = RadarProvider.RadarProviders.TomorrowIo.GetStringValue();
        }

        public override void UpdateRadarView()
        {
            base.UpdateRadarView();
            RadarMapContainer.IsToolbarVisible = InteractionsEnabled()/* && ExtrasService.IsEnabled()*/;
        }

        public override void UpdateMap(MapControl mapControl)
        {
            base.UpdateMap(mapControl);

            this._mapControl = mapControl;

            mapControl.IsScrollEnabled = InteractionsEnabled();
            mapControl.IsZoomEnabled = InteractionsEnabled();

            GetRadarFrames();
        }

        private void GetRadarFrames()
        {
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

            AvailableRadarFrames.Clear();
            AnimationPosition = 0;

            var now = DateTimeOffset.UtcNow.Trim(TimeSpan.TicksPerMinute);
            // Trim minute
            now = now.AddMinutes(-now.Minute);

            var start = now.AddHours(-2);
            var end = now.AddHours(2);

            var current = start;
            var nowIndex = -1;

            while (current <= end)
            {
                AvailableRadarFrames.Add(new(current.UtcDateTime.ToISO8601Format()));

                if (current == now)
                {
                    nowIndex = AvailableRadarFrames.Count - 1;
                }

                current = current.AddMinutes(10);
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

            ProcessingFrames = false;
        }

        private void AddLayer(RadarFrame mapFrame)
        {
            if (ProcessingFrames) return;

            if (!RadarLayers.ContainsKey(mapFrame.timestamp))
            {
                var layer = new CustomTileOverlay("TomorrowIoRadarTileProvider", GetUrlTemplate(mapFrame), cacheTimeSeconds: 60 * 30)
                {
                    Alpha = 0,
                    MinimumZ = (int)MIN_ZOOM_LEVEL,
                    MaximumZ = (int)MAX_ZOOM_LEVEL,
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

            if (AvailableRadarFrames.Count == 0 || AnimationPosition >= AvailableRadarFrames.Count || position >= AvailableRadarFrames.Count)
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
            var SettingsManager = Ioc.Default.GetService<SettingsManager>();
            var key = SettingsManager.APIKeys[WeatherAPI.TomorrowIo];

            if (string.IsNullOrWhiteSpace(key))
                key = APIKeys.GetTomorrowIOKey();

            return
                $"https://api.tomorrow.io/v4/map/tile/{{z}}/{{x}}/{{y}}/precipitationIntensity/{mapFrame.timestamp}.png?apikey={key}";
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

    internal record RadarFrame(string timestamp);
}
#endif