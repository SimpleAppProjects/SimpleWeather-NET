﻿//#if !(ANDROID || IOS || MACCATALYST)
using BruTile.Cache;
using Mapsui.Tiling;
#if !DEBUG
#if WINDOWS
using Microsoft.UI.Xaml;
#endif
using Mapsui.Layers;
using SimpleWeather.Helpers;
using SimpleWeather.NET.MapsUi;
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
                    OpenStreetMap.DefaultCache ??= new FileCache(
                            Path.Combine(SimpleWeather.Helpers.ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, "OpenStreepMap"), "tile.png");

                    mapControl?.Map?.Layers?.Insert(0, OpenStreetMap.CreateTileLayer(Constants.GetUserAgentString())); // Default map layer
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

                    mapLayer = await BingMaps.CreateBingMapsLayer(imageryType, isDarkMode, userAgent: Constants.GetUserAgentString());
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