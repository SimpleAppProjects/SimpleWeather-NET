using SimpleWeather.Keys;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace SimpleWeather.UWP.Radar
{
    public class RainViewerViewProvider : MapTileRadarViewProvider
    {
        private const string URLTemplate = "https://tilecache.rainviewer.com/v2/radar/{timestamp}/256/{zoomlevel}/{x}/{y}/1/1_1.png";

        private List<long> AvailableTimestamps;
        private MapTileSource TileSource;

        private DispatcherTimer AnimationTimer;
        private int AnimationPosition = 0;

        public RainViewerViewProvider(Border container) : base(container)
        {
            AvailableTimestamps = new List<long>();
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
                TileSource = new MapTileSource(dataSrc) 
                {
                    FrameCount = AvailableTimestamps.Count,
                    FrameDuration = TimeSpan.FromMilliseconds(500),
                    AutoPlay = false
                };
                mapControl.TileSources.Add(TileSource);
                RadarMapContainer.OnPlayAnimation += RadarMapContainer_OnPlayAnimation;
                RadarMapContainer.OnPauseAnimation += RadarMapContainer_OnPauseAnimation;
            }

            await RefreshTimestamps().ContinueWith((t) => 
            {
                TileSource.FrameCount = AvailableTimestamps.Count;
                RadarMapContainer.UpdateSeekbarRange(0, TileSource.FrameCount - 1);
                AnimationTimer?.Stop();
                TileSource?.Stop();
                AnimationPosition = 0;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public override void OnDestroyView()
        {
            AnimationTimer?.Stop();
            base.OnDestroyView();
            AvailableTimestamps?.Clear();
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
                    RadarMapContainer.Dispatcher.RunOnUIThread(() =>
                    {
                        if (TileSource.FrameCount > 0)
                        {
                            if (TileSource.AnimationState == MapTileAnimationState.Stopped)
                            {
                                TileSource?.Play();
                                RadarMapContainer.UpdateTimestamp(AnimationPosition = 0, AvailableTimestamps[0]);
                            }
                            else
                            {
                                AnimationPosition = (AnimationPosition + 1) % TileSource.FrameCount;
                                if (AnimationPosition <= 0)
                                {
                                    TileSource?.Stop();
                                    AnimationPosition = -1;
                                    RadarMapContainer.UpdateTimestamp(0, AvailableTimestamps[0]);
                                }
                                else
                                {
                                    RadarMapContainer.UpdateTimestamp(AnimationPosition, AvailableTimestamps[AnimationPosition]);
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
            var currentTimeStamp = 0L;
            if (AvailableTimestamps?.Count > 0 && args.FrameIndex < AvailableTimestamps.Count)
            {
                currentTimeStamp = AvailableTimestamps[args.FrameIndex];
            }

            // Get the custom Uri.
            if (currentTimeStamp > 0)
            {
                args.Request.Uri = new Uri(URLTemplate.Replace("{timestamp}", AvailableTimestamps[args.FrameIndex].ToInvariantString()));
            }
            else
            {
                args.Request.Uri = new Uri("about:blank");
            }
        }

        private async Task RefreshTimestamps()
        {
            var HttpClient = SimpleLibrary.WebClient;

            try
            {
                using (var response = await HttpClient.GetAsync(new Uri("https://api.rainviewer.com/public/maps.json")))
                {
                    var stream = await response.Content.ReadAsInputStreamAsync();
                    var root = JSONParser.Deserializer<List<long>>(stream.AsStreamForRead());

                    AvailableTimestamps.Clear();
                    AvailableTimestamps.AddRange(root);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex);
            }
        }
    }
}
