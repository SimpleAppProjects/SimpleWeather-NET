namespace SimpleWeather.LocationData
{
    public sealed class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Location() { }

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
