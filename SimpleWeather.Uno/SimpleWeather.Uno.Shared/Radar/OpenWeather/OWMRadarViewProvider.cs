using SimpleWeather.Weather_API.Keys;
using System;
using System.Globalization;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace SimpleWeather.UWP.Radar.OpenWeather
{
    public class OWMRadarViewProvider : MapTileRadarViewProvider
    {
        private MapTileSource TileSource;

        public OWMRadarViewProvider(Border container) : base(container)
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

        private void CachingHttpMapTileDataSource_UriRequested(HttpMapTileDataSource sender, MapTileUriRequestedEventArgs args)
        {
            // Get the custom Uri.
            args.Request.Uri = GetTileUri(args.X, args.Y, args.ZoomLevel);
        }

        private Uri GetTileUri(int x, int y, int zoom)
        {
            return new Uri(string.Format(CultureInfo.InvariantCulture,
                "https://tile.openweathermap.org/map/precipitation_new/{0}/{1}/{2}.png?appid={3}",
                zoom, x, y, APIKeys.GetOWMKey()));
        }
    }
}
