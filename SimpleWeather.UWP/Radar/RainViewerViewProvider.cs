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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace SimpleWeather.UWP.Radar
{
    public class RainViewerViewProvider : MapTileRadarViewProvider
    {
        private MapTileSource TileSource;

        public RainViewerViewProvider(Border container) : base(container)
        {
        }

        public override void UpdateMap(MapControl mapControl)
        {
            if (TileSource == null)
            {
                var dataSrc = new HttpMapTileDataSource()
                {
                    AllowCaching = true
                };
                dataSrc.UriRequested += CachingHttpMapTileDataSource_UriRequested;
                TileSource = new MapTileSource(dataSrc);
                mapControl.TileSources.Add(TileSource);
            }
        }

        private List<long> AvailableTimestamps;

        private async void CachingHttpMapTileDataSource_UriRequested(HttpMapTileDataSource sender, MapTileUriRequestedEventArgs args)
        {
            // Get a deferral to do something asynchronously.
            // Omit this line if you don't have to do something asynchronously.
            var deferral = args.Request.GetDeferral();

            // Get the custom Uri.
            var uri = await GetTileUri(args.X, args.Y, args.ZoomLevel);

            // Specify the Uri in the Uri property of the MapTileUriRequest.
            args.Request.Uri = uri;

            // Notify the app that the custom Uri is ready.
            // Omit this line also if you don't have to do something asynchronously.
            deferral.Complete();
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

                    AvailableTimestamps = new List<long>(root);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex);
            }
        }

        private async Task<Uri> GetTileUri(int x, int y, int zoom)
        {
            // Check timestamps
            long lastTimestamp = -1;
            if (AvailableTimestamps?.Count > 0)
            {
                lastTimestamp = AvailableTimestamps.LastOrDefault();
                if (lastTimestamp > 0)
                {
                    var now = DateTimeOffset.UtcNow;
                    var lastTime = DateTimeOffset.FromUnixTimeSeconds(lastTimestamp);
                    if ((now - lastTime).TotalMinutes > 15)
                    {
                        await RefreshTimestamps();
                    }
                }
            }
            else
            {
                await RefreshTimestamps();
            }
            lastTimestamp = AvailableTimestamps.LastOrDefault();

            if (lastTimestamp > 0)
            {
                return new Uri(String.Format(CultureInfo.InvariantCulture,
                    "https://tilecache.rainviewer.com/v2/radar/{0}/256/{1}/{2}/{3}/1/1_1.png",
                    lastTimestamp, zoom, x, y));
            }

            return null;
        }
    }
}
