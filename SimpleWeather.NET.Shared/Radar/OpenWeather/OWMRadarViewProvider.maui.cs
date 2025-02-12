#if __IOS__
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Maui.Maps;
using SimpleWeather.Preferences;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.WeatherData;
using MapControl = Microsoft.Maui.Controls.Maps.Map;
using TileLayer = SimpleWeather.Maui.Maps.ICustomTileOverlay;

namespace SimpleWeather.NET.Radar.OpenWeather
{
    public partial class OWMRadarViewProvider : MapTileRadarViewProvider
    {
        private TileLayer TileLayer;

        public OWMRadarViewProvider(Border container) : base(container)
        {
        }

        public override void UpdateRadarView()
        {
            base.UpdateRadarView();
            RadarMapContainer.IsToolbarVisible = InteractionsEnabled() && ExtrasService.IsAtLeastProEnabled();
        }

        public override void UpdateMap(MapControl mapControl)
        {
            base.UpdateMap(mapControl);

            if (TileLayer == null)
            {
                TileLayer = new CustomTileOverlay("OWMTileProvider", GetUrlTemplate(), cacheTimeSeconds: 60 * 15)
                {
                    MinimumZ = (int)MIN_ZOOM_LEVEL,
                    MaximumZ = (int)MAX_ZOOM_LEVEL,
                };
                mapControl.AddOverlay(TileLayer);
            }
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            TileLayer = null;
        }

        private string GetUrlTemplate()
        { 
            var SettingsManager = Ioc.Default.GetService<SettingsManager>();
            var key = SettingsManager.APIKeys[WeatherAPI.OpenWeatherMap];

            if (string.IsNullOrWhiteSpace(key))
                key = APIKeys.GetOWMKey();

            return $"https://tile.openweathermap.org/map/precipitation_new/{{z}}/{{x}}/{{y}}.png?appid={key}";
        }
    }
}
#endif