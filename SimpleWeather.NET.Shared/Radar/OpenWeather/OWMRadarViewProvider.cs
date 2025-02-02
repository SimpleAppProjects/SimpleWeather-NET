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
#else
using Mapsui.UI.Maui;
using MapControl = Mapsui.UI.Maui.MapControl;
#endif
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using System.Net.Http.Headers;
using SimpleWeather.NET.MapsUi;

namespace SimpleWeather.NET.Radar.OpenWeather
{
    public partial class OWMRadarViewProvider : MapTileRadarViewProvider
    {
        private HttpTileSource TileSource;
        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public OWMRadarViewProvider(Border container) : base(container)
        {
        }

        public override void UpdateRadarView()
        {
            base.UpdateRadarView();

#if WINDOWS
            RadarMapContainer.ToolbarVisibility =
                InteractionsEnabled() && ExtrasService.IsAtLeastProEnabled() ? Visibility.Visible : Visibility.Collapsed;
#else
            RadarMapContainer.IsToolbarVisible = InteractionsEnabled() && ExtrasService.IsAtLeastProEnabled();
#endif
        }

        public override void UpdateMap(MapControl mapControl)
        {
            base.UpdateMap(mapControl);

            if (TileSource == null)
            {
                TileSource = CreateTileSource();
                mapControl.Map.Layers.Add(new TileLayer(TileSource, dataFetchStrategy: new MinimalDataFetchStrategy(), renderFetchStrategy: new MinimalRenderFetchStrategy()));
            }
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            TileSource = null;
        }

        private static readonly BruTile.Attribution OWMAttribution = new("OpenWeatherMap", "https://openweathermap.org/");

        private HttpTileSource CreateTileSource()
        {
            return new CustomHttpTileSource(new GlobalSphericalMercator(yAxis: BruTile.YAxis.OSM, minZoomLevel: (int)MIN_ZOOM_LEVEL, maxZoomLevel: (int)MAX_ZOOM_LEVEL, name: "OWMRadar"),
                new BasicUrlBuilder("https://tile.openweathermap.org/map/precipitation_new/{z}/{x}/{y}.png?appid={k}", apiKey: GetKey()),
                name: this.GetType().Name,
                configureHttpRequestMessage: request =>
                {
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromMinutes(15)
                    };
                },
                attribution: OWMAttribution);
        }

        private string GetKey()
        {
            var key = SettingsManager.APIKeys[WeatherData.WeatherAPI.OpenWeatherMap];

            if (string.IsNullOrWhiteSpace(key))
                return APIKeys.GetOWMKey();

            return key;
        }
    }
}
