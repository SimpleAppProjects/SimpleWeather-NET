#if !__IOS__
using SimpleWeather.NET.MapsUi;
using HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment;
using VerticalAlignment = Mapsui.Widgets.VerticalAlignment;
using Mapsui.Layers;
using Windows.Storage;
using Mapsui.Styles;
using Mapsui.Extensions;
using Mapsui.Widgets.ButtonWidgets;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.RemoteConfig;
using WApi = SimpleWeather.WeatherData.WeatherAPI;
using Mapsui;



#if WINDOWS
using Microsoft.UI.Xaml;
#endif
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
#if false
                if (mapControl.Map.Layers.FindLayer("Root").FirstOrDefault() is null)
                {
                    var tileLayer = OpenStreetMap.CreateTileLayer();
                    tileLayer.Attribution.VerticalAlignment = VerticalAlignment.Bottom;
                    tileLayer.Attribution.HorizontalAlignment = HorizontalAlignment.Right;

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
                        mapControl.Map.Layers.Remove(layer => layer.Name?.StartsWith("Root") == true);
                        mapControl.Map.Widgets.Clear();
                        changeMap = true;
                    }
                }
                else
                {
                    changeMap = true;
                }

                if (changeMap)
                {
                    var remoteConfigSvc = Ioc.Default.GetService<IRemoteConfigService>();

#if WINDOWS
                    var isDarkMode = RadarContainer.ActualTheme == ElementTheme.Dark;
#else
                    var isDarkMode = Equals(RadarContainer.ClassId, "dark");
#endif

                    string baseMapLayerProvider;

                    if (remoteConfigSvc.IsProviderEnabled(WApi.MapBox))
                    {
                        baseMapLayerProvider = WApi.MapBox;
                    }
                    else if (remoteConfigSvc.IsProviderEnabled(WApi.OpenStreetMap))
                    {
                        baseMapLayerProvider = WApi.OpenStreetMap;
                    }
                    else
                    {
                        baseMapLayerProvider = null;
                    }

                    if (baseMapLayerProvider == WApi.MapBox)
                    {
                        mapLayer = MapBox.CreateMapBoxLayer(isDarkMode);
#if WINDOWS
                        mapLayer.Tag = RadarContainer.ActualTheme;
#else
                        mapLayer.Tag = RadarContainer.ClassId;
#endif

                        if (mapLayer.Attribution != null)
                        {
                            mapLayer.Attribution.HorizontalAlignment = HorizontalAlignment.Right;
                            mapLayer.Attribution.VerticalAlignment = VerticalAlignment.Bottom;
                            mapLayer.Attribution.BackColor = Color.Transparent;
                            mapLayer.Attribution.TextColor = Color.LightSkyBlue;
                            mapLayer.Attribution.Margin = new(0);
                            mapLayer.Attribution.Padding = new(0);
                        }

                        mapControl?.Map?.Layers?.Insert(0, mapLayer);
                        await MapBox.CreateLayersAndWidgets(mapControl);
                    }
                    else if (baseMapLayerProvider == WApi.OpenStreetMap)
                    {
                        mapLayer = OpenStreetMap.CreateTileLayer();
#if WINDOWS
                        mapLayer.Tag = RadarContainer.ActualTheme;
#else
                        mapLayer.Tag = RadarContainer.ClassId;
#endif

                        if (mapLayer.Attribution != null)
                        {
                            mapLayer.Attribution.HorizontalAlignment = HorizontalAlignment.Right;
                            mapLayer.Attribution.VerticalAlignment = VerticalAlignment.Bottom;
                            mapLayer.Attribution.BackColor = Color.Transparent;
                            mapLayer.Attribution.TextColor = Color.LightSkyBlue;
                            mapLayer.Attribution.Margin = new(0);
                            mapLayer.Attribution.Padding = new(0);
                        }

                        mapControl?.Map?.Layers?.Insert(0, mapLayer);
                    }
                }
#endif
            });
        }
    }
}
#endif