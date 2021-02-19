using SimpleWeather.Keys;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Metadata;
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
                InteractionsEnabled() && IsAnimationAvailable && Extras.ExtrasLibrary.IsEnabled() ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;

            UpdateMap(mapControl);
        }

        public abstract void UpdateMap(MapControl mapControl);

        public override void OnDestroyView()
        {
            RadarContainer.Child = null;
            mapControl?.TileSources.Clear();
            mapControl = null;
        }

        private MapControl CreateMapControl()
        {
            var mapControl = new MapControl()
            {
                MapServiceToken = APIKeys.GetBingMapsKey(),
                LandmarksVisible = true,
                PedestrianFeaturesVisible = false,
                TransitFeaturesVisible = false,
                ZoomLevel = 6,
                PanInteractionMode = MapPanInteractionMode.Disabled,
                RotateInteractionMode = MapInteractionMode.Disabled,
                TiltInteractionMode = MapInteractionMode.Disabled,
                ZoomInteractionMode = MapInteractionMode.Disabled,
            };

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
