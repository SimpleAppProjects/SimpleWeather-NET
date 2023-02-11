#if WINDOWS
using Mapsui.UI.WinUI;
#else
using Mapsui.UI.Maui;
#endif

namespace SimpleWeather.NET.Radar
{
    public class MapControlCreator
    {
        private static MapControlCreator _instance;

        public static MapControlCreator Instance => _instance ??= new MapControlCreator();

        private MapControl _mapControl;

        public MapControl Map => _mapControl ??= CreateMapControl();

        private static MapControl CreateMapControl()
        {
            var mapControl = new MapControl()
            {
                Map = new Mapsui.Map()
                {
                    CRS = "EPSG:3857"
                }
            };

            mapControl.Map.ZoomLock = false;
            mapControl.Map.PanLock = false;
            mapControl.Map.RotationLock = true;
            mapControl.Navigator.ZoomTo(6d.ToMapsuiResolution());
            return mapControl;
        }

        public void RemoveMapControl()
        {
            _mapControl?.Map?.Layers?.Clear();
            _mapControl?.Dispose();
            _mapControl = null;
        }
    }

    public static class ZoomLevelExtensions
    {
        /// <summary>
        /// Convert zoom level (as described at https://wiki.openstreetmap.org/wiki/Zoom_levels) into a Mapsui resolution
        /// </summary>
        /// <param name="zoomLevel">Zoom level</param>
        /// <returns>Resolution in Mapsui format</returns>
        public static double ToMapsuiResolution(this double zoomLevel)
        {
            if (zoomLevel < 0 || zoomLevel > 30)
                return 0;

            return 156543.03392 / System.Math.Pow(2, zoomLevel);
        }
    }
}