namespace SimpleWeather.Location
{
    public static class LocationDataExtras
    {
        public static LocationData ToLocationData(this LocationQuery locQuery, LocationType type = LocationType.Search)
        {
            return new LocationData()
            {
                query = locQuery.Location_Query,
                name = locQuery.LocationName,
                latitude = locQuery.LocationLat,
                longitude = locQuery.LocationLong,
                tz_long = locQuery.LocationTZLong,
                weatherSource = locQuery.WeatherSource,
                locationSource = locQuery.LocationSource,
                locationType = type
            };
        }

        public static LocationData ToLocationData(this LocationQuery locQuery, Location location)
        {
            return new LocationData()
            {
                query = locQuery.Location_Query,
                name = locQuery.LocationName,
                latitude = location.Latitude,
                longitude = location.Longitude,
                tz_long = locQuery.LocationTZLong,
                locationType = LocationType.GPS,
                weatherSource = locQuery.WeatherSource,
                locationSource = locQuery.LocationSource,
            };
        }
    }
}
