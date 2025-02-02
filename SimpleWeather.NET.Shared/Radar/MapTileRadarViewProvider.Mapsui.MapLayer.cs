//#if !(ANDROID || IOS || MACCATALYST)
using BruTile.Cache;
using Mapsui;
using SimpleWeather.NET.MapsUi;
#if !DEBUG
#if WINDOWS
using Microsoft.UI.Xaml;
#endif
using Mapsui.Layers;
#endif

namespace SimpleWeather.NET.Radar
{
    public partial class MapTileRadarViewProvider
    {
        private void UpdateMapLayer()
        {
#if WINDOWS
            RadarContainer.DispatcherQueue.TryEnqueue(async () =>
#else
            RadarContainer.Dispatcher.Dispatch(async () =>
#endif
            {
#if DEBUG
                if (mapControl.Map.Layers.FindLayer("Root").FirstOrDefault() is null)
                {
                    var tileLayer = OpenStreetMap.CreateTileLayer();
                    tileLayer.Attribution.VerticalAlignment = Mapsui.Widgets.VerticalAlignment.Bottom;
                    tileLayer.Attribution.HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment.Right;

                    mapControl?.Map?.Layers?.Insert(0, tileLayer); // Default map layer
                }
#else
                bool changeMap = false;

                if (mapControl.Map.Layers.FindLayer("Root").FirstOrDefault() is ILayer mapLayer)
                {
#if WINDOWS
                    if (!Equals(mapLayer.Tag, RadarContainer.ActualTheme))
#else
                    if (!Equals(mapLayer.Tag, RadarContainer.ClassId))
#endif
                    {
                        mapControl.Map.Layers.Remove(mapLayer);
                        changeMap = true;
                    }
                }
                else
                {
                    changeMap = true;
                }

                if (changeMap)
                {
#if WINDOWS
                    var isDarkMode = RadarContainer.ActualTheme == ElementTheme.Dark;
#else
                    var isDarkMode = Equals(RadarContainer.ClassId, "dark");
#endif

                    var imageryType = isDarkMode switch
                    {
                        true => BingMapsRESTToolkit.ImageryType.CanvasDark,
                        false => BingMapsRESTToolkit.ImageryType.CanvasLight
                    };

                    mapLayer = await BingMaps.CreateBingMapsLayer(imageryType, isDarkMode);
#if WINDOWS
                    mapLayer.Tag = RadarContainer.ActualTheme;
#else
                    mapLayer.Tag = RadarContainer.ClassId;
#endif

                    mapControl?.Map?.Layers?.Insert(0, mapLayer);
                }
#endif
                });
        }
    }
}
//#endif