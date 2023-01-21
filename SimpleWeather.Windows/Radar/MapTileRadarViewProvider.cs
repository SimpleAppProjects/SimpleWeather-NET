using CommunityToolkit.Mvvm.DependencyInjection;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.UI.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleWeather.Extras;
using SimpleWeather.NET.Utils;
using SimpleWeather.Utils;
using System.Linq;

namespace SimpleWeather.NET.Radar
{
    public abstract partial class MapTileRadarViewProvider : RadarViewProvider
    {
        private MapControl mapControl;
        private WeatherUtils.Coordinate locationCoords;
        private MemoryLayer markerLayer;
        protected RadarToolbar RadarMapContainer { get; private set; }

        private readonly IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();

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
                RadarContainer.Child = (RadarMapContainer = new RadarToolbar());
                RadarMapContainer.MapContainerChild = mapControl;
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
                mapControl.NavigateHome();
                if (markerLayer.Features.FirstOrDefault() is PointFeature markerFeature)
                {
                    markerFeature.Point.X = MapCameraPosition.X;
                    markerFeature.Point.Y = MapCameraPosition.Y;
                    markerLayer.DataHasChanged();
                }
            }
            markerLayer.Opacity = InteractionsEnabled() ? 1 : 0;

            mapControl.Map.PanLock = !InteractionsEnabled();

            RadarMapContainer.ToolbarVisibility =
                InteractionsEnabled() && ExtrasService.IsEnabled() ? Visibility.Visible : Visibility.Collapsed;

            UpdateMap(mapControl);
        }

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

        public abstract void UpdateMap(MapControl mapControl);

        public override void OnDestroyView()
        {
            IsViewAlive = false;
            RadarContainer.Child = null;
            if (RadarMapContainer != null)
            {
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
                mapControl.Map.ZoomLock = false;
                mapControl.Map.PanLock = false;
                n.NavigateTo(MapCameraPosition, 6d.ToMapsuiResolution());
                mapControl.Map.ZoomLock = true;
                mapControl.Map.PanLock = !InteractionsEnabled();
            };
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