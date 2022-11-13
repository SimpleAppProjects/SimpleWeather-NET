#if WINDOWS_UWP
using Windows.Devices.Geolocation;
#endif

namespace SimpleWeather.LocationData
{
    public static class LocationExtras
    {
        public static Location ToLocation(this LocationData locationData)
        {
            return new Location(locationData.latitude, locationData.longitude);
        }

#if WINDOWS_UWP
        public static Location ToLocation(this Geoposition geoposition)
        {
            return new Location(geoposition.Coordinate.Point.Position.Latitude, geoposition.Coordinate.Point.Position.Longitude);
        }
#endif
    }
}
