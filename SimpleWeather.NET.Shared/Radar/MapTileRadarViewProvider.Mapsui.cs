#if !__IOS__
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using SimpleWeather.Utils;
using Color = Mapsui.Styles.Color;
using SkiaSharp;

#if WINDOWS
using CommunityToolkit.WinUI;
using MapControl = Mapsui.UI.WinUI.MapControl;
#else
using SimpleWeather.Maui.Radar;
using MapControl = Mapsui.UI.Maui.MapControl;
#endif

namespace SimpleWeather.NET.Radar
{
    public abstract partial class MapTileRadarViewProvider : RadarViewProvider
    {
        private MemoryLayer markerLayer;

        public override void UpdateRadarView()
        {
            if (mapControl == null || RadarMapContainer == null)
            {
                mapControl = CreateMapControl();
                mapControl.Map.Layers.Changed += Layers_Changed;
#if WINDOWS
                RadarContainer.Child = (RadarMapContainer = new RadarToolbar());
#else
                RadarContainer.Content = (RadarMapContainer = new RadarToolbar());
#endif
                RadarMapContainer.MapContainerChild = mapControl;
                RadarMapContainer.OnPlayAnimation += RadarMapContainer_OnPlayAnimation;
                RadarMapContainer.OnPauseAnimation += RadarMapContainer_OnPauseAnimation;
            }

            IsViewAlive = true;

            UpdateMapLayer();

            if (markerLayer == null)
            {
                markerLayer = new MemoryLayer("Point")
                {
                    Features =
                    [
                        new PointFeature(MapCameraPosition)
                    ],
                    Style = ImageStyles.CreatePinStyle(fillColor: Color.FromString("#FF4500"), symbolScale: 0.75d)
                };
                mapControl.Map.Layers.Changed -= Layers_Changed;
                mapControl.Map.Layers.Add(markerLayer);
                mapControl.Map.Layers.Changed += Layers_Changed;
            }

            if (MapCameraPosition?.X != 0 && MapCameraPosition?.Y != 0)
            {
#if WINDOWS
                mapControl.DispatcherQueue.EnqueueAsync(() => mapControl?.Let(NavigateHome));
#else
                mapControl.Dispatcher.Dispatch(() => mapControl?.Let(NavigateHome));
#endif
                if (markerLayer.Features.FirstOrDefault() is PointFeature markerFeature)
                {
                    markerFeature.Point.X = MapCameraPosition.X;
                    markerFeature.Point.Y = MapCameraPosition.Y;
                    markerLayer.DataHasChanged();
                }
            }
            markerLayer.Opacity = 1;

            mapControl.Map.Navigator.PanLock = !InteractionsEnabled();
            mapControl.Map.Navigator.ZoomLock = !(InteractionsEnabled() && ExtrasService.IsAtLeastProEnabled());

            UpdateMap(mapControl);
        }

        private void Layers_Changed(object _, LayerCollectionChangedEventArgs e)
        {
            // Make sure marker layer is always on top
            if (mapControl?.Map != null && markerLayer != null && e.AddedLayers.Any() && !e.AddedLayers.Contains(markerLayer))
            {
                mapControl.Map.Layers.Changed -= Layers_Changed;
                mapControl.Map.Layers.Remove(markerLayer);
                mapControl.Map.Layers.Add(markerLayer);
                mapControl.Map.Layers.Changed += Layers_Changed;
            }
        }

        public virtual partial void UpdateMap(MapControl mapControl)
        {
            mapControl?.Map?.Navigator?.Apply(n =>
            {
                n.PanLock = !InteractionsEnabled();
                n.ZoomLock = !(InteractionsEnabled() && ExtrasService.IsAtLeastProEnabled());
            });
        }

        private MapControl CreateMapControl()
        {
            var mapControl = MapControlCreator.Instance.Map;
            mapControl.Map.Navigator.OverrideZoomBounds = new MMinMax(MIN_ZOOM_LEVEL.ToMapsuiResolution(), MAX_ZOOM_LEVEL.ToMapsuiResolution());
            mapControl.Map.RefreshGraphicsRequest += (s, e) =>
            {
                int count = 0;
                double maxHeight = 1;
                double previousWidth = 1;

                var rootLayers = mapControl?.Map.Layers?.Where(layer => layer.Name?.StartsWith("Root") == true);
                rootLayers?.ForEachIndexed((index, layer) =>
                {
                    layer.Attribution?.Let(attribution =>
                    {
                        attribution.VerticalAlignment = Mapsui.Widgets.VerticalAlignment.Bottom;
                        attribution.HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment.Right;

                        if (!string.IsNullOrWhiteSpace(attribution.Text))
                        {
                            using var textPaint = new SKFont(SKTypeface.Default, (float)attribution.TextSize) { Edging = SKFontEdging.SubpixelAntialias };
                            textPaint.MeasureText(attribution.Text, out SKRect textBounds);

                            maxHeight = Math.Max(maxHeight, textBounds.Height);

                            attribution.Height = textBounds.Height;
                            attribution.Margin = new MRect(previousWidth + (4 * count), 5);
                            attribution.Padding = new MRect(8);

                            previousWidth += textBounds.Width;
                            count++;
                        }
                    });
                });

                count = 0;

                mapControl?.Map?.Layers?.Except(rootLayers)?.ForEach(layer =>
                {
                    layer.Attribution?.Let(attribution =>
                    {
                        attribution.VerticalAlignment = Mapsui.Widgets.VerticalAlignment.Bottom;
                        attribution.HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment.Right;

                        if (!string.IsNullOrWhiteSpace(attribution.Text))
                        {
                            using var textPaint = new SKFont(SKTypeface.Default, (float)attribution.TextSize) { Edging = SKFontEdging.SubpixelAntialias };
                            textPaint.MeasureText(attribution.Text, out SKRect textBounds);

                            attribution.Margin = new MRect(attribution.Margin.Width, attribution.Margin.Height + maxHeight + (textBounds.Height * (count + 1)) / 2);
                            attribution.Padding = new MRect(8);
                            attribution.BackColor = Color.Transparent;
#if WINDOWS
                            attribution.TextColor = Color.LightSkyBlue;
#endif
                            count++;
                        }
                    });
                });
            };

            NavigateHome(mapControl);

            return mapControl;
        }

        protected MPoint MapCameraPosition
        {
            get
            {
                if (locationCoords != null)
                {
                    return SphericalMercator.FromLonLat(
                        lat: locationCoords.Latitude,
                        lon: locationCoords.Longitude
                    ).ToMPoint();
                }

                return SphericalMercator.FromLonLat(new MPoint());
            }
        }

        private void NavigateHome(MapControl mapControl)
        {
            mapControl.Map.Navigator.ZoomLock = false;
            mapControl.Map.Navigator.PanLock = false;
            mapControl.Map.Navigator.CenterOnAndZoomTo(MapCameraPosition, DEFAULT_ZOOM_LEVEL.ToMapsuiResolution());
            mapControl.Map.Navigator.ZoomLock = !(InteractionsEnabled() && ExtrasService.IsAtLeastProEnabled());
            mapControl.Map.Navigator.PanLock = !InteractionsEnabled();
        }
    }
}
#endif