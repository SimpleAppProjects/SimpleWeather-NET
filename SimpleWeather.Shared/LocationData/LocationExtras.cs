namespace SimpleWeather.LocationData
{
    public static class LocationExtras
    {
        public static Location ToLocation(this LocationData locationData)
        {
            return new Location(locationData.latitude, locationData.longitude);
        }

#if WINUI
        public static Location ToLocation(this Windows.Devices.Geolocation.Geoposition geoposition)
        {
            return new Location(geoposition.Coordinate.Point.Position.Latitude, geoposition.Coordinate.Point.Position.Longitude);
        }
#else
        public static Location ToLocation(this Microsoft.Maui.Devices.Sensors.Location geoposition)
        {
            return new Location(geoposition.Latitude, geoposition.Longitude);
        }
#endif
    }
}
