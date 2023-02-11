using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui.Tiling.Layers;
#if WINDOWS
using Mapsui.UI.WinUI;
using Microsoft.UI.Xaml.Controls;
#else
using Mapsui.UI.Maui;
#endif
using SimpleWeather.Helpers;
using SimpleWeather.Weather_API.Keys;

namespace SimpleWeather.NET.Radar.OpenWeather
{
    public class OWMRadarViewProvider : MapTileRadarViewProvider
    {
        private HttpTileSource TileSource;

        public OWMRadarViewProvider(Border container) : base(container)
        {
        }

        public override void UpdateMap(MapControl mapControl)
        {
            if (TileSource == null)
            {
                TileSource = CreateTileSource();
                mapControl.Map.Layers.Add(new TileLayer(TileSource));
            }
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            TileSource = null;
        }

        private static readonly BruTile.Attribution OWMAttribution = new("OpenWeatherMap", "https://openweathermap.org/");

        private static HttpTileSource CreateTileSource()
        {
            return new HttpTileSource(new GlobalSphericalMercator(),
                "https://tile.openweathermap.org/map/precipitation_new/{z}/{x}/{y}.png?appid={k}",
                apiKey: APIKeys.GetOWMKey(), name: OWMAttribution.Text,
                persistentCache: new FileCache(Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, "OpenWeatherMap"), "tile.png"),
                attribution: OWMAttribution, userAgent: Constants.GetUserAgentString());
        }
    }
}
