#if __IOS__
using Microsoft.Maui.Controls.Maps;
using SimpleWeather.Maui.Maps;
using SimpleWeather.Maui.Radar;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace SimpleWeather.NET.Radar
{
    public abstract partial class MapTileRadarViewProvider : RadarViewProvider
    {
        private Pin locationMarker;

        public override void UpdateRadarView()
        {
            if (mapControl == null || RadarMapContainer == null)
            {
                mapControl = CreateMapControl();
                RadarContainer.Content = (RadarMapContainer = new RadarToolbar());
                RadarMapContainer.MapContainerChild = mapControl;
                RadarMapContainer.OnPlayAnimation += RadarMapContainer_OnPlayAnimation;
                RadarMapContainer.OnPauseAnimation += RadarMapContainer_OnPauseAnimation;
            }

            IsViewAlive = true;

            if (locationMarker == null)
            {
                mapControl.Pins.Add(locationMarker = new Pin
                {
                    Location = new Location(),
                    Label = string.Empty,
                    Type = PinType.Generic
                });
            }

            if (MapCameraPosition is { } point)
            {
                mapControl.SetCameraPosition(point);
                locationMarker.Location = new Location(point.Latitude, point.Longitude);
            }

            mapControl.IsScrollEnabled = InteractionsEnabled();
            mapControl.IsZoomEnabled = InteractionsEnabled() && ExtrasService.IsAtLeastProEnabled();

            UpdateMap(mapControl);
        }

        public virtual partial void UpdateMap(Map mapControl)
        {
            mapControl.IsScrollEnabled = InteractionsEnabled();
            mapControl.IsZoomEnabled = InteractionsEnabled() && ExtrasService.IsAtLeastProEnabled();
        }

        private Map CreateMapControl()
        {
            var mapControl = MapControlCreator.Instance.Map;
            mapControl.IsScrollEnabled = InteractionsEnabled();
            mapControl.IsZoomEnabled = InteractionsEnabled() && ExtrasService.IsAtLeastProEnabled();
            if (locationCoords != null)
            {
                mapControl.SetCameraPosition(new CameraPosition(locationCoords.Latitude, locationCoords.Longitude, (nuint)DEFAULT_ZOOM_LEVEL));
            }

            return mapControl;
        }

        protected CameraPosition MapCameraPosition
        {
            get
            {
                if (locationCoords != null)
                {
                    return new CameraPosition(locationCoords.Latitude, locationCoords.Longitude, (nuint)DEFAULT_ZOOM_LEVEL);
                }

                return null;
            }
        }
    }
}
#endif