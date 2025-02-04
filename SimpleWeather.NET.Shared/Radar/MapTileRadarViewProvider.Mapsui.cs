//#if !(ANDROID || IOS || MACCATALYST)

using CommunityToolkit.Mvvm.DependencyInjection;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using SimpleWeather.Extras;
using SimpleWeather.Utils;
using Color = Mapsui.Styles.Color;
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
        protected const double MIN_ZOOM_LEVEL = 2d;
        protected const double MAX_ZOOM_LEVEL = 18d;
        protected const double DEFAULT_ZOOM_LEVEL = 6d;

        private MapControl mapControl;
        private WeatherUtils.Coordinate locationCoords;
        private MemoryLayer markerLayer;
        protected RadarToolbar RadarMapContainer { get; private set; }

        protected readonly IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();

        protected bool IsViewAlive { get; private set; }

        public MapTileRadarViewProvider(Border container) : base(container)
        {
        }

        public override void UpdateCoordinates(WeatherUtils.Coordinate coordinates, bool updateView = false)
        {
            locationCoords = coordinates;
            if (updateView) UpdateRadarView();
        }

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
                    Style = SymbolStyles.CreatePinStyle(fillColor: Color.FromString("#FF4500"), symbolScale: 0.75d)
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

        private void RadarMapContainer_OnPlayAnimation(object sender, EventArgs e)
        {
            OnPlayRadarAnimation();
        }

        private void RadarMapContainer_OnPauseAnimation(object sender, EventArgs e)
        {
            OnPauseRadarAnimation();
        }

        protected virtual void OnPlayRadarAnimation() { }
        protected virtual void OnPauseRadarAnimation() { }

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

        public virtual void UpdateMap(MapControl mapControl)
        {
            mapControl?.Map?.Navigator?.Apply(n =>
            {
                n.PanLock = !InteractionsEnabled();
                n.ZoomLock = !(InteractionsEnabled() && ExtrasService.IsAtLeastProEnabled());
            });
        }

        public override void OnDestroyView()
        {
            IsViewAlive = false;
#if WINDOWS
            RadarContainer.Child = null;
#else
            RadarContainer.Content = null;
#endif
            if (RadarMapContainer != null)
            {
                RadarMapContainer.OnPauseAnimation -= RadarMapContainer_OnPauseAnimation;
                RadarMapContainer.OnPlayAnimation -= RadarMapContainer_OnPlayAnimation;
                RadarMapContainer.MapContainerChild = null;
                RadarMapContainer = null;
            }
            mapControl?.Map?.Layers?.Clear();
            mapControl = null;
        }

        private MapControl CreateMapControl()
        {
            var mapControl = MapControlCreator.Instance.Map;
            mapControl.Map.Navigator.OverrideZoomBounds = new MMinMax(MIN_ZOOM_LEVEL.ToMapsuiResolution(), MAX_ZOOM_LEVEL.ToMapsuiResolution());
            mapControl.Map.RefreshGraphicsRequest += (s, e) =>
            {
                int count = 0;
                mapControl?.Map?.Layers?.ForEach(layer =>
                {
                    layer.Attribution?.Let(attribution =>
                    {
                        attribution.Margin = new MRect(attribution.Margin.Width, attribution.Margin.Height + (20 * count));
                        count++;
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
//#endif