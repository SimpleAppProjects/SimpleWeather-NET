using SimpleWeather.Weather_API.Keys;
using Windows.UI.Xaml.Controls.Maps;

namespace SimpleWeather.UWP.Radar
{
    public class MapControlCreator
    {
        private static MapControlCreator _instance;

        public static MapControlCreator Instance => _instance ??= new MapControlCreator();

        private MapControl _mapControl;

        public MapControl Map => _mapControl ??= CreateMapControl();

        private static MapControl CreateMapControl()
        {
            return new MapControl()
            {
                MapServiceToken = APIKeys.GetBingMapsKey(),
                LandmarksVisible = true,
                PedestrianFeaturesVisible = false,
                TransitFeaturesVisible = false,
                ZoomLevel = 6,
                PanInteractionMode = MapPanInteractionMode.Disabled,
                RotateInteractionMode = MapInteractionMode.Disabled,
                TiltInteractionMode = MapInteractionMode.Disabled,
                ZoomInteractionMode = MapInteractionMode.Disabled,
            };
        }

        public void RemoveMapControl()
        {
            _mapControl?.TileSources?.Clear();
            _mapControl = null;
        }
    }
}
