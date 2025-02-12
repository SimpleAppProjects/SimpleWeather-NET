#if __IOS__
using System.Globalization;
using System.Xml;
using Foundation;
using MapKit;
using SimpleWeather.Maui.Maps;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Utils;
using MapControl = Microsoft.Maui.Controls.Maps.Map;
using TileLayer = SimpleWeather.Maui.Maps.ICustomTileOverlay;

namespace SimpleWeather.NET.Radar.ECCC
{
    public partial class ECCCRadarViewProvider : MapTileRadarViewProvider, IDisposable
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

                RadarMapContainer?.Dispatcher?.Dispatch(() =>
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
                var layer = new ECCCTileProvider(GetUrlTemplate(mapFrame))
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

        private string GetUrlTemplate(RadarFrame mapFrame)
        {
            var url = "https://geo.weather.gc.ca/geomet".ToUriBuilderEx()
                .AppendQueryParameter("SERVICE", "WMS")
                .AppendQueryParameter("VERSION", "1.3.0")
                .AppendQueryParameter("REQUEST", "GetMap")
                .AppendQueryParameter("BBOX", "{bboxYMin},{bboxXMin},{bboxYMax},{bboxXMax}", encode: false)
                .AppendQueryParameter("CRS", "EPSG:4326")
                .AppendQueryParameter("WIDTH", "256")
                .AppendQueryParameter("HEIGHT", "256")
                .AppendQueryParameter("LAYERS", "Radar_1km_SfcPrecipType")
                .AppendQueryParameter("FORMAT", "image/png")
                .AppendQueryParameter("TIME", mapFrame.timestamp) // ex) 2019-06-21T12:00:00Z
                .ToString();
            
            return url;
        }
        
        private class ECCCTileProvider(string urlTemplate) : CustomWmsTileOverlay("ECCCTileProvider", urlTemplate, cacheTimeSeconds: 60 * 30)
        {
            public override NSUrl URLForTilePath(MKTileOverlayPath path)
            {
                var bbox = BoundingBox.FromTile(path.X.ToInt32(), path.Y.ToInt32(), path.Z.ToInt32());
                
                var url = URLTemplate!.Replace("{bboxYMin}", $"{bbox.yMin}")
                    .Replace("{bboxXMin}", $"{bbox.xMin}")
                    .Replace("{bboxYMax}", $"{bbox.yMax}")
                    .Replace("{bboxXMax}", $"{bbox.xMax}")
                    .ToUriBuilderEx()
                    .BuildUri();
            
                return url;
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