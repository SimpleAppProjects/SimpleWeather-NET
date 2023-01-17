using BruTile.Cache;
using Mapsui.Tiling;
using Mapsui.UI.WinUI;
using SimpleWeather.Helpers;
using System.IO;

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
            var mapControl = new MapControl();

            if (OpenStreetMap.DefaultCache == null)
            {
                OpenStreetMap.DefaultCache = new FileCache(
                    Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, "OpenStreepMap"), "tile.png");
            }

            mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer(Constants.GetUserAgentString())); // Default map layer
            mapControl.Map.ZoomLock = true;
            mapControl.Map.RotationLock = true;
            mapControl.Map.PanLock = true;
            mapControl.Navigator.ZoomTo(6d.ToMapsuiResolution());
            return mapControl;
        }

        public void RemoveMapControl()
        {
            _mapControl?.Map?.Layers?.Clear();
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