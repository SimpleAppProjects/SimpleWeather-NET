using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace SimpleWeather.UWP.Radar.RainViewer
{
    public class RainViewerViewProvider : MapTileRadarViewProvider, IDisposable
    {
        private const string MapsURL = "https://api.rainviewer.com/public/weather-maps.json";
        private const string URLTemplate = "{host}{path}/256/{zoomlevel}/{x}/{y}/1/1_1.png";

        private List<RadarFrame> AvailableRadarFrames;
        private MapTileSource TileSource;

        private DispatcherTimer AnimationTimer;
        private int AnimationPosition = 0;
        private CancellationTokenSource cts;
        private bool disposedValue;

        private readonly string API_ID;

        public RainViewerViewProvider(Border container) : base(container)
        {
            AvailableRadarFrames = new List<RadarFrame>();
            cts = new CancellationTokenSource();

            API_ID = RadarProvider.RadarProviders.RainViewer.GetStringValue();
        }

        public override async void UpdateMap(MapControl mapControl)
        {
            if (TileSource == null)
            {
                var dataSrc = new HttpMapTileDataSource()
                {
                    AllowCaching = true
                };
                dataSrc.UriRequested += CachingHttpMapTileDataSource_UriRequested;
                TileSource = new MapTileSource(dataSrc);
                if (IsAnimationAvailable)
                {
                    TileSource.FrameCount = AvailableRadarFrames.Count;
                    TileSource.FrameDuration = TimeSpan.FromMilliseconds(500);
                    TileSource.AutoPlay = false;

                    RadarMapContainer.OnPlayAnimation += RadarMapContainer_OnPlayAnimation;
                    RadarMapContainer.OnPauseAnimation += RadarMapContainer_OnPauseAnimation;
                };
                mapControl.TileSources.Add(TileSource);
            }

            await GetRadarFrames();

            await mapControl.Dispatcher.RunOnUIThread(() =>
            {
                if (IsAnimationAvailable)
                {
                    if (TileSource != null)
                    {
                        TileSource.FrameCount = InteractionsEnabled() && AvailableRadarFrames.Count > 0 ? AvailableRadarFrames.Count : 0;
                    }
                    RadarMapContainer?.UpdateSeekbarRange(0, (TileSource?.FrameCount ?? 0) - 1);
                    AnimationTimer?.Stop();
                    TileSource?.Stop();
                    AnimationPosition = 0;
                }
            });
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
                    Interval = TimeSpan.FromMilliseconds(490)
                };
                AnimationTimer.Tick += (s, ev) =>
                {
                    // Update toolbar
                    RadarMapContainer.Dispatcher.LaunchOnUIThread(() =>
                    {
                        if (TileSource.FrameCount > 0)
                        {
                            if (TileSource.AnimationState == MapTileAnimationState.Stopped)
                            {
                                TileSource?.Play();
                                RadarMapContainer.UpdateTimestamp(AnimationPosition = 0, AvailableRadarFrames.LastOrDefault()?.TimeStamp ?? 0);
                            }
                            else
                            {
                                AnimationPosition = (AnimationPosition + 1) % TileSource.FrameCount;
                                if (AnimationPosition <= 0)
                                {
                                    TileSource?.Stop();
                                    AnimationPosition = -1;
                                    RadarMapContainer.UpdateTimestamp(0, AvailableRadarFrames.LastOrDefault()?.TimeStamp ?? 0);
                                }
                                else
                                {
                                    RadarMapContainer.UpdateTimestamp(AnimationPosition, AvailableRadarFrames[AnimationPosition].TimeStamp);
                                }
                            }
                        }
                    });
                };
            }
            TileSource?.Play();
            AnimationTimer?.Start();
        }

        private void RadarMapContainer_OnPauseAnimation(object sender, EventArgs e)
        {
            TileSource?.Pause();
            AnimationTimer?.Stop();
        }

        private void CachingHttpMapTileDataSource_UriRequested(HttpMapTileDataSource sender, MapTileUriRequestedEventArgs args)
        {
            RadarFrame mapFrame = null;
            if (AvailableRadarFrames?.Count > 0 && args.FrameIndex < AvailableRadarFrames.Count)
            {
                if (InteractionsEnabled() && IsAnimationAvailable)
                {
                    mapFrame = AvailableRadarFrames[args.FrameIndex];
                }
                else
                {
                    mapFrame = AvailableRadarFrames.LastOrDefault();
                }
            }

            // Get the custom Uri.
            if (mapFrame != null)
            {
                args.Request.Uri = new Uri(URLTemplate.Replace("{host}", mapFrame.Host).Replace("{path}", mapFrame.Path));
            }
            else
            {
                args.Request.Uri = new Uri("about:blank");
            }
        }

        private async Task GetRadarFrames()
        {
            var HttpClient = SharedModule.Instance.WebClient;

            try
            {
                RefreshToken();
                using (var response = await HttpClient.GetAsync(new Uri(MapsURL), cts.Token))
                {
                    await response.CheckForErrors(API_ID);
                    response.EnsureSuccessStatusCode();

                    var stream = await response.Content.ReadAsStreamAsync();
                    var root = await JSONParser.DeserializerAsync<Rootobject>(stream);

                    AvailableRadarFrames.Clear();

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
                }
            }
            catch (OperationCanceledException)
            {
                // ignore.
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex);
            }
        }

        private void RefreshToken()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
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
