#if false //ANDROID || IOS || MACCATALYST
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Maui.Controls.Maps;
using SimpleWeather.Maui.Radar;
using SimpleWeather.Extras;
using SimpleWeather.NET.Utils;
using SimpleWeather.Utils;
using Map = Microsoft.Maui.Controls.Maps.Map;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Platform;

namespace SimpleWeather.NET.Radar
{
    public abstract partial class MapTileRadarViewProvider : RadarViewProvider
    {
        private Map mapControl;
        private WeatherUtils.Coordinate locationCoords;
        private Pin locationMarker;
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
#if WINDOWS
                RadarContainer.Child = (RadarMapContainer = new RadarToolbar());
#else
                RadarContainer.Content = (RadarMapContainer = new RadarToolbar());
#endif
                RadarMapContainer.MapContainerChild = mapControl;
            }

            IsViewAlive = true;

            if (locationMarker == null)
            {
                locationMarker = new Pin()
                {
                    Location = MapCameraPosition.Center,
                    Type = PinType.Place
                };

                locationMarker.HandlerChanged += LocationMarker_HandlerChanged;

                mapControl.Pins.Add(locationMarker);
            }

            if (MapCameraPosition is MapSpan point)
            {
                mapControl.MoveToRegion(point.WithZoom(6d));
                locationMarker.Location = point.Center;
            }

            UpdateLocationMarkerVisibility(locationMarker, InteractionsEnabled());

            mapControl.IsScrollEnabled = InteractionsEnabled();

#if WINDOWS
            RadarMapContainer.ToolbarVisibility =
                InteractionsEnabled() && ExtrasService.IsEnabled() ? Visibility.Visible : Visibility.Collapsed;
#else
            RadarMapContainer.IsToolbarVisible = InteractionsEnabled() && ExtrasService.IsEnabled();
#endif

            UpdateMap(mapControl);
        }

        private void UpdateLocationMarkerVisibility(Pin locationMarker, bool isVisible)
        {
            if (locationMarker?.Handler is IPlatformViewHandler markerHandler)
            {
#if ANDROID
                if (markerHandler.PlatformView is Android.Views.View markerView)
                {
                    markerView.Visibility = isVisible ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Invisible;
                }
#elif IOS || MACCATALYST
                if (markerHandler.PlatformView is UIKit.UIView markerView)
                {
                    markerView.Hidden = !isVisible;
                }
#endif
            }
        }

        private void LocationMarker_HandlerChanged(object sender, EventArgs e)
        {
            UpdateLocationMarkerVisibility(sender as Pin, InteractionsEnabled());
        }

        public abstract void UpdateMap(Map mapControl);

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
                RadarMapContainer.MapContainerChild = null;
                RadarMapContainer = null;
            }
            mapControl?.MapElements?.Clear();
            mapControl?.Pins?.Clear();
            mapControl = null;
        }

        private Map CreateMapControl()
        {
            var mapControl = MapControlCreator.Instance.Map;
            mapControl.IsScrollEnabled = false;

            mapControl.IsZoomEnabled = true;
            mapControl.MoveToRegion(MapCameraPosition.WithZoom(6d));
            mapControl.IsZoomEnabled = false;

            return mapControl;
        }

        protected MapSpan MapCameraPosition
        {
            get
            {
                if (locationCoords != null)
                {
                    return new MapSpan(
                        new Location(locationCoords.Latitude, locationCoords.Longitude),
                        0.01d, 0.01d);
                }

                return new MapSpan(new Location(), 0.01d, 0.01d);
            }
        }
    }
}
#endif