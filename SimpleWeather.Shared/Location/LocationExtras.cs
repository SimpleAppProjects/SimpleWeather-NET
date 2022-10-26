using Windows.Devices.Geolocation;

namespace SimpleWeather.Location
{
    public static class LocationExtras
    {
        public static Location ToLocation(this LocationData locationData)
        {
            return new Location(locationData.latitude, locationData.longitude);
        }

        public static Location ToLocation(this Geoposition geoposition)
        {
            return new Location(geoposition.Coordinate.Point.Position.Latitude, geoposition.Coordinate.Point.Position.Longitude);
        }
    }
}
