//#if !(ANDROID || IOS || MACCATALYST)
using CommunityToolkit.Mvvm.DependencyInjection;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
#if WINDOWS
using CommunityToolkit.WinUI;
using Mapsui.UI.WinUI;
using MapControl = Mapsui.UI.WinUI.MapControl;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Mapsui.UI.Maui;
using SimpleWeather.Maui.Radar;
using MapControl = Mapsui.UI.Maui.MapControl;
#endif
using SimpleWeather.Extras;
using SimpleWeather.NET.Utils;
using SimpleWeather.Utils;

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
                mapControl.Map.Layers.LayerAdded += Layers_LayerAdded;
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
                    Features = new[]
                    {
                        new PointFeature(MapCameraPosition)
                    },
                    Style = Mapsui.Styles.SymbolStyles.CreatePinStyle(pinColor: Mapsui.Styles.Color.FromString("#FF4500"), symbolScale: 0.75d)
                };
                mapControl.Map.Layers.LayerAdded -= Layers_LayerAdded;
                mapControl.Map.Layers.Add(markerLayer);
                mapControl.Map.Layers.LayerAdded += Layers_LayerAdded;
            }

            if (MapCameraPosition?.X != 0 && MapCameraPosition?.Y != 0)
            {
#if WINDOWS
                mapControl.DispatcherQueue.EnqueueAsync(mapControl.NavigateHome);
#else
                mapControl.Dispatcher.Dispatch(mapControl.NavigateHome);
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
            mapControl.Map.Navigator.ZoomLock = !(InteractionsEnabled() && ExtrasService.IsEnabled());

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

        private void Layers_LayerAdded(ILayer layer)
        {
            // Make sure marker layer is always on top
            if (mapControl?.Map != null && markerLayer != null && layer != markerLayer)
            {
                mapControl.Map.Layers.LayerAdded -= Layers_LayerAdded;
                mapControl.Map.Layers.Remove(markerLayer);
                mapControl.Map.Layers.Add(markerLayer);
                mapControl.Map.Layers.LayerAdded += Layers_LayerAdded;
            }
        }

        public virtual void UpdateMap(MapControl mapControl)
        {
            mapControl?.Map?.Navigator?.Apply(n =>
            {
                n.PanLock = !InteractionsEnabled();
                n.ZoomLock = !(InteractionsEnabled() && ExtrasService.IsEnabled());
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
            mapControl.Map.Home = n =>
            {
                n.ZoomLock = false;
                n.PanLock = false;
                n.CenterOnAndZoomTo(MapCameraPosition, DEFAULT_ZOOM_LEVEL.ToMapsuiResolution());
                n.ZoomLock = !(InteractionsEnabled() && ExtrasService.IsEnabled());
                n.PanLock = !InteractionsEnabled();
            };
            mapControl.Map.Navigator.OverrideZoomBounds = new MMinMax(MIN_ZOOM_LEVEL.ToMapsuiResolution(), MAX_ZOOM_LEVEL.ToMapsuiResolution());
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
    }
}
//#endif