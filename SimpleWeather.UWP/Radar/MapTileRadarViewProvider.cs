using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Extras;
using SimpleWeather.Utils;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace SimpleWeather.UWP.Radar
{
    public abstract class MapTileRadarViewProvider : RadarViewProvider
    {
        private MapControl mapControl;
        private WeatherUtils.Coordinate locationCoords;
        private MapIcon locationMarkerIcon;
        protected RadarToolbar RadarMapContainer { get; private set; }

        protected bool IsAnimationAvailable { get; }

        private readonly IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();

        public MapTileRadarViewProvider(Border container) : base(container)
        {
            IsAnimationAvailable = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7);
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
                RadarContainer.Child = (RadarMapContainer = new RadarToolbar());
                RadarMapContainer.MapContainerChild = mapControl;
            }

            switch (RadarContainer.ActualTheme)
            {
                default:
                case Windows.UI.Xaml.ElementTheme.Default:
                    mapControl.StyleSheet = MapStyleSheet.RoadLight();
                    break;
                case Windows.UI.Xaml.ElementTheme.Light:
                    mapControl.StyleSheet = MapStyleSheet.RoadLight();
                    break;
                case Windows.UI.Xaml.ElementTheme.Dark:
                    mapControl.StyleSheet = MapStyleSheet.RoadDark();
                    break;
            }

            if (locationMarkerIcon == null)
            {
                locationMarkerIcon = new MapIcon()
                {
                    NormalizedAnchorPoint = new Point(0.5, 1.0),
                    ZIndex = 0,
                    Visible = false
                };

                mapControl.Layers.Add(new MapElementsLayer()
                {
                    ZIndex = 1,
                    Visible = true,
                    MapElements = new List<MapElement>
                    {
                        locationMarkerIcon
                    }
                });
            }

            if (MapCameraPosition is Geopoint point)
            {
                mapControl.Center = point;
                locationMarkerIcon.Location = point;
            }
            locationMarkerIcon.Visible = InteractionsEnabled();

            mapControl.PanInteractionMode = InteractionsEnabled() ? MapPanInteractionMode.Auto : MapPanInteractionMode.Disabled;

            RadarMapContainer.ToolbarVisibility =
                InteractionsEnabled() && IsAnimationAvailable && ExtrasService.IsEnabled() ? Visibility.Visible : Visibility.Collapsed;

            UpdateMap(mapControl);
        }

        public abstract void UpdateMap(MapControl mapControl);

        public override void OnDestroyView()
        {
            RadarContainer.Child = null;
            if (RadarMapContainer != null)
            {
                RadarMapContainer.MapContainerChild = null;
                RadarMapContainer = null;
            }
            mapControl?.TileSources.Clear();
            mapControl = null;
        }

        private MapControl CreateMapControl()
        {
            var mapControl = MapControlCreator.Instance.Map;

            if (locationCoords != null)
            {
                mapControl.Center = new Geopoint(
                    new BasicGeoposition()
                    {
                        Latitude = locationCoords.Latitude,
                        Longitude = locationCoords.Longitude
                    }
                );
                mapControl.ZoomLevel = 6;
            }

            return mapControl;
        }

        protected Geopoint MapCameraPosition
        {
            get
            {
                if (locationCoords != null)
                {
                    return new Geopoint(
                        new BasicGeoposition()
                        {
                            Latitude = locationCoords.Latitude,
                            Longitude = locationCoords.Longitude
                        }
                    );
                }

                return null;
            }
        }
    }
}
