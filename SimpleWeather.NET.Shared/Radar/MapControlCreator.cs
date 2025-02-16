#if __IOS__
using Microsoft.Maui.Maps;
using MapControl = Microsoft.Maui.Controls.Maps.Map;
#else
using Mapsui;
using Mapsui.UI.WinUI;
using Mapsui.Widgets.ButtonWidgets;
using Mapsui.Widgets.InfoWidgets;
using SimpleWeather.NET.MapsUi;
using SimpleWeather.Utils;
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
#if MACCATALYST
            //MapControl.UseGPU = false;
#endif

#if !__IOS__
            LoggingWidget.ShowLoggingInMap = ShowLoggingInMap.No;
#endif

            var mapControl = new MapControl()
            {
#if __IOS__
                MapType = MapType.Street,
#else
                Map = new Map()
                {
                    CRS = "EPSG:3857"
                },
#endif
            };

#if __IOS__
            mapControl.IsZoomEnabled = false;
            mapControl.IsScrollEnabled = false;
            mapControl.IsTrafficEnabled = false;
#else
            mapControl.Map.Navigator.ZoomLock = false;
            mapControl.Map.Navigator.PanLock = false;
            mapControl.Map.Navigator.RotationLock = true;
            mapControl.Map.Navigator.ZoomTo(6d.ToMapsuiResolution());

            Mapsui.Logging.Logger.LogDelegate = (loggerLevel, message, exception) =>
            {
                Logger.WriteLine(loggerLevel switch
                {
                    Mapsui.Logging.LogLevel.Error => LoggerLevel.Error,
                    Mapsui.Logging.LogLevel.Warning => LoggerLevel.Warn,
                    Mapsui.Logging.LogLevel.Information => LoggerLevel.Info,
                    Mapsui.Logging.LogLevel.Debug or Mapsui.Logging.LogLevel.Trace => LoggerLevel.Debug,
                    _ => LoggerLevel.Info,
                }, exception, message);
            };
#endif
            return mapControl;
        }

        public void RemoveMapControl()
        {
#if __IOS__
            _mapControl?.MapElements?.Clear();
            _mapControl?.Pins?.Clear();
#else
            _mapControl?.Map?.Layers?.Clear();
            _mapControl?.Dispose();
#endif
            _mapControl = null;
        }
    }

#if !__IOS__
    public static class ZoomLevelExtensions
    {
        /// <summary>
        /// Convert zoom level (as described at https://wiki.openstreetmap.org/wiki/Zoom_levels) into a Mapsui resolution
        /// </summary>
        /// <param name="zoomLevel">Zoom level</param>
        /// <returns>Resolution in Mapsui format</returns>
        public static double ToMapsuiResolution(this double zoomLevel)
        {
            if (zoomLevel is < 0 or > 30)
                return 0;

            return 156543.03392 / Math.Pow(2, zoomLevel);
        }
    }
#endif
}