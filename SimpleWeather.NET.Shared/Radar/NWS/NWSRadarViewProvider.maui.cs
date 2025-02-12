#if __IOS__
using Foundation;
using MapKit;
using SimpleWeather.Maui.Maps;
using SimpleWeather.Utils;
using MapControl = Microsoft.Maui.Controls.Maps.Map;
using TileLayer = SimpleWeather.Maui.Maps.ICustomTileOverlay;

namespace SimpleWeather.NET.Radar.NWS
{
    public partial class NWSRadarViewProvider : MapTileRadarViewProvider, IDisposable
    {
        private readonly List<RadarFrame> AvailableRadarFrames;
        private readonly Dictionary<string, TileLayer> RadarLayers;

        private MapControl _mapControl = null;

        private int AnimationPosition = 0;
        private IDispatcherTimer AnimationTimer;

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
            RadarMapContainer.IsToolbarVisible = InteractionsEnabled() && ExtrasService.IsAtLeastProEnabled();
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
                var layer = new NWSTileProvider(GetUrlTemplate(mapFrame))
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
            var url =
                "https://mapservices.weather.noaa.gov/eventdriven/rest/services/radar/radar_base_reflectivity_time/ImageServer/exportImage"
                    .ToUriBuilderEx()
                    .AppendQueryParameter("bbox", "{bboxXMin},{bboxYMin},{bboxXMax},{bboxYMax}", encode: false)
                    .AppendQueryParameter("bboxSR", "4326")
                    .AppendQueryParameter("size", "256,256")
                    .AppendQueryParameter("time", mapFrame.timestamp)
                    .AppendQueryParameter("format", "png")
                    .AppendQueryParameter("f", "image")
                    .ToString();
            
            return url;
        }
        
        private class NWSTileProvider(string urlTemplate) : CustomWmsTileOverlay("NWSTileProvider", urlTemplate, cacheTimeSeconds: 60 * 30)
        {
            public override NSUrl URLForTilePath(MKTileOverlayPath path)
            {
                var bbox = BoundingBox.FromTile(path.X.ToInt32(), path.Y.ToInt32(), path.Z.ToInt32());

                var url = URLTemplate!.Replace("{bboxXMin}", $"{bbox.xMin}")
                    .Replace("{bboxYMin}", $"{bbox.yMin}")
                    .Replace("{bboxXMax}", $"{bbox.xMax}")
                    .Replace("{bboxYMax}", $"{bbox.yMax}")
                    .ToUriBuilderEx()
                    .BuildUri();
            
                return url;
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
#endif