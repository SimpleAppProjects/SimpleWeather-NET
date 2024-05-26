using BruTile.Predefined;
using BruTile.Web;
using CacheCow.Client.Headers;
using CommunityToolkit.Mvvm.DependencyInjection;
using Mapsui.Tiling.Fetcher;
using Mapsui.Tiling.Layers;
using Mapsui.Tiling.Rendering;
#if WINDOWS
using Mapsui.UI.WinUI;
using MapControl = Mapsui.UI.WinUI.MapControl;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleWeather.Helpers;
#else
using Mapsui.UI.Maui;
using MapControl = Mapsui.UI.Maui.MapControl;
using Microsoft.Maui.Dispatching;
#endif
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using System.Globalization;
using System.Net.Http.Headers;

namespace SimpleWeather.NET.Radar.TomorrowIo
{
    public partial class TomorrowIoRadarViewProvider : MapTileRadarViewProvider, IDisposable
    {
        private const string URLTemplate = "https://api.tomorrow.io/v4/map/tile/{z}/{x}/{y}/precipitationIntensity/{timestamp}.png?apikey={k}";

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

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

#if WINDOWS
            RadarMapContainer.ToolbarVisibility =
                InteractionsEnabled()/* && ExtrasService.IsEnabled()*/ ? Visibility.Visible : Visibility.Collapsed;
#else
            RadarMapContainer.IsToolbarVisible = InteractionsEnabled()/* && ExtrasService.IsEnabled()*/;
#endif
        }

        public override void UpdateMap(MapControl mapControl)
        {
            base.UpdateMap(mapControl);

            this._mapControl = mapControl;

            mapControl.Map.Navigator.PanLock = !InteractionsEnabled();
            mapControl.Map.Navigator.ZoomLock = !InteractionsEnabled();

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
#if WINDOWS
                _mapControl?.DispatcherQueue?.TryEnqueue(() =>
#else
                _mapControl?.Dispatcher?.Dispatch(() =>
#endif
                {
                    _mapControl?.Map?.Layers?.Remove(layer);
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

            ProcessingFrames = false;
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

        private static readonly BruTile.Attribution TomorrowIoAttribution = new("Tomorrow.io", "https://www.tomorrow.io/weather-api/");

        private HttpTileSource CreateTileSource(RadarFrame? mapFrame)
        {
            string uri;
            if (mapFrame != null)
            {
                uri = URLTemplate.Replace("{timestamp}", mapFrame.timestamp);
            }
            else
            {
                uri = "about:blank";
            }

            return new HttpTileSource(new GlobalSphericalMercator(yAxis: BruTile.YAxis.OSM, minZoomLevel: (int)MIN_ZOOM_LEVEL, maxZoomLevel: (int)MAX_ZOOM_LEVEL, name: "TomorrowIo"),
                uri, apiKey: GetKey(), name: TomorrowIoAttribution.Text,
                tileFetcher: FetchTileAsync,
                attribution: TomorrowIoAttribution, userAgent: Constants.GetUserAgentString());
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
                    Logger.WriteLine(LoggerLevel.Debug, $"{nameof(TomorrowIoRadarViewProvider)}: tile fetched from cache");
                }
                else
                {
                    Logger.WriteLine(LoggerLevel.Debug, $"{nameof(TomorrowIoRadarViewProvider)}: tile fetched from web");
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

        private string GetKey()
        {
            var key = SettingsManager.APIKeys[WeatherData.WeatherAPI.TomorrowIo];

            if (string.IsNullOrWhiteSpace(key))
                return APIKeys.GetTomorrowIOKey();

            return key;
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