#if !__IOS__
using System.Globalization;
using System.Net.Http.Headers;
using System.Xml;
using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Tiling.Fetcher;
using Mapsui.Tiling.Layers;
using Mapsui.Tiling.Rendering;
using SimpleWeather.Helpers;
using SimpleWeather.NET.MapsUi;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Utils;
using HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment;
using VerticalAlignment = Mapsui.Widgets.VerticalAlignment;
using CommunityToolkit.Mvvm.DependencyInjection;
using RTools_NTS.Util;
using NodaTime;

#if WINDOWS
using MapControl = Mapsui.UI.WinUI.MapControl;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Mapsui.UI.Maui;
using MapControl = Mapsui.UI.Maui.MapControl;
using Microsoft.Maui.Dispatching;
#endif

namespace SimpleWeather.NET.Radar.WeatherApi
{
    public partial class WeatherApiRadarViewProvider : MapTileRadarViewProvider, IDisposable
    {
        private const float DEFAULT_TRANSPARENCY = 0.5f;

        private readonly List<RadarFrame> AvailableRadarFrames;
        private readonly Dictionary<DateTimeOffset, TileLayer> RadarLayers;

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

#if WINDOWS
            RadarMapContainer.ToolbarVisibility =
                InteractionsEnabled() ? Visibility.Visible : Visibility.Collapsed;
#else
            RadarMapContainer.IsToolbarVisible = InteractionsEnabled();
#endif
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
                var layer = new TileLayer(CreateTileSource(mapFrame), dataFetchStrategy: new MinimalDataFetchStrategy(), renderFetchStrategy: new MinimalRenderFetchStrategy(), httpClient: Ioc.Default.GetService<HttpClient>())
                {
                    Opacity = 0
                };
                layer.Attribution.Enabled = false;
                layer.Attribution.VerticalAlignment = VerticalAlignment.Bottom;
                layer.Attribution.HorizontalAlignment = HorizontalAlignment.Right;
                layer.Attribution.Margin = new MRect(10);
                layer.Attribution.Padding = new MRect(4);

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
                nextLayer.Opacity = DEFAULT_TRANSPARENCY;
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

        protected override void OnSliderValueChanged(double value, bool fromUser)
        {
            if (fromUser)
            {
                AnimationTimer?.Stop();
                ShowFrame((int)value);
            }
        }

        private void RefreshToken()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
        }

        private static readonly Attribution WeatherApiAttribution = new("WeatherAPI.com", "https://weatherapi.com");

        private HttpTileSource CreateTileSource(RadarFrame? mapFrame)
        {
            return new CustomHttpTileSource(new GlobalSphericalMercator(yAxis: YAxis.OSM, minZoomLevel: (int)MIN_ZOOM_LEVEL, maxZoomLevel: 6, name: "WeatherApi"),
                new WeatherApiTileProvider(mapFrame), name: this.GetType().Name,
                configureHttpRequestMessage: request =>
                {
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromMinutes(30)
                    };
                },
                persistentCache: new FileCache(Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, "WeatherApi"), "tile.png"),
                attribution: WeatherApiAttribution);
        }

        private class WeatherApiTileProvider(RadarFrame? mapFrame) : IUrlBuilder
        {
            public Uri GetUrl(TileInfo info)
            {
                var zoom = info.Index.Level;
                var x = info.Index.Col;
                var y = info.Index.Row;

                if (mapFrame != null)
                {
                    var date = mapFrame.timestamp.Date.ToString("yyyyMMdd");
                    var hour = mapFrame.timestamp.Hour.ToString("D2");
                    var url = $"https://weathermaps.weatherapi.com/precip/tiles/{date}{hour}/{zoom}/{x}/{y}.png";

                    return new Uri(url);
                }

                return null;
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