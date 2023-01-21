using BruTile.Cache;
using Mapsui.Tiling;
using System.IO;
using System.Linq;
#if !DEBUG
using Microsoft.UI.Xaml;
using SimpleWeather.Helpers;
using SimpleWeather.NET.MapsUi;
#endif

namespace SimpleWeather.NET.Radar
{
    public partial class MapTileRadarViewProvider
    {
        private void UpdateMapLayer()
        {
            RadarContainer.DispatcherQueue.TryEnqueue(async () =>
            {
#if DEBUG
                if (mapControl.Map.Layers.FindLayer("Root").FirstOrDefault() is null)
                {
                    OpenStreetMap.DefaultCache ??= new FileCache(
                            Path.Combine(SimpleWeather.Helpers.ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, "OpenStreepMap"), "tile.png");

                    mapControl?.Map?.Layers?.Insert(0, OpenStreetMap.CreateTileLayer(Constants.GetUserAgentString())); // Default map layer
                }
#else
                if (mapControl.Map.Layers.FindLayer("Root").FirstOrDefault() is null)
                {
                    mapControl?.Map?.Layers?.Insert(0, await BingMaps.CreateBingCanvasGrayLayer(Constants.GetUserAgentString())); // Default map layer
                }
                /*
                bool changeMap = false;

                if (mapControl.Map.Layers.FindLayer("Root").FirstOrDefault() is ILayer mapLayer)
                {
                    if (!Equals(mapLayer.Tag, RadarContainer.ActualTheme))
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
                    mapControl?.Map?.Layers?.Insert(0, await BingMaps.CreateBingRoadsDynamicLayer(
                        RadarContainer.ActualTheme == ElementTheme.Dark, Constants.GetUserAgentString()
                        ));
                }
                */
#endif
            });
        }
    }
}
