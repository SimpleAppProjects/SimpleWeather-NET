#if WINDOWS
using Mapsui.UI.WinUI;
//#elif ANDROID || IOS || MACCATALYST
//using MapControl = Microsoft.Maui.Controls.Maps.Map;
#else
using Mapsui.UI.Maui;
#endif

namespace SimpleWeather.NET.Radar
{
    public class MapControlCreator
    {
        private static MapControlCreator _instance;

        public static MapControlCreator Instance => _instance ??= new MapControlCreator();

#if __IOS__
        public MapControl Map => CreateMapControl();
#else
        private MapControl _mapControl;

        public MapControl Map => _mapControl ??= CreateMapControl();
#endif

        private static MapControl CreateMapControl()
        {
#if MACCATALYST
            MapControl.UseGPU = false;
#endif

            var mapControl = new MapControl()
            {
#if false //ANDROID || IOS || MACCATALYST
                MapType = Microsoft.Maui.Maps.MapType.Hybrid
#else
                Map = new Mapsui.Map()
                {
                    CRS = "EPSG:3857"
                },
#endif
            };

#if false //ANDROID || IOS || MACCATALYST
            mapControl.IsZoomEnabled = false;
            mapControl.IsScrollEnabled = false;
            mapControl.IsTrafficEnabled = false;
#else
            mapControl.Map.Navigator.ZoomLock = false;
            mapControl.Map.Navigator.PanLock = false;
            mapControl.Map.Navigator.RotationLock = true;
            mapControl.Map.Navigator.ZoomTo(6d.ToMapsuiResolution());
#endif
            return mapControl;
        }

        public void RemoveMapControl()
        {
#if !__IOS__
#if false //ANDROID || IOS || MACCATALYST
            _mapControl?.MapElements?.Clear();
            _mapControl?.Pins?.Clear();
#else
            _mapControl?.Map?.Layers?.Clear();
            _mapControl?.Dispose();
#endif
            _mapControl = null;
#endif
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