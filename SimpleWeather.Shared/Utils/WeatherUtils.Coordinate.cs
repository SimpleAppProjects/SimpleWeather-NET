using SimpleWeather.Location;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace SimpleWeather.Utils
{
    public partial class WeatherUtils
    {
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
        public class Coordinate
        {
            public double Latitude { get => lat; set { SetLat(value); } }
            public double Longitude { get => _long; set { SetLong(value); } }

            private double lat = 0;
            private double _long = 0;

            private void SetLat(double value) { lat = value; }

            private void SetLong(double value) { _long = value; }

            public Coordinate(string coordinatePair)
            {
                SetCoordinate(coordinatePair);
            }

            public Coordinate(double latitude, double longitude)
            {
                lat = latitude;
                _long = longitude;
            }

            public Coordinate(Windows.Devices.Geolocation.Geoposition geoPos)
            {
                if (geoPos is null)
                {
                    throw new ArgumentNullException(nameof(geoPos));
                }

                lat = geoPos.Coordinate.Point.Position.Latitude;
                _long = geoPos.Coordinate.Point.Position.Longitude;
            }

            public Coordinate(LocationData location)
            {
                if (location is null)
                {
                    throw new ArgumentNullException(nameof(location));
                }

                lat = location.latitude;
                _long = location.longitude;
            }

            public void SetCoordinate(string coordinatePair)
            {
                if (string.IsNullOrWhiteSpace(coordinatePair))
                {
                    throw new ArgumentException("Invalid coordinate pair", nameof(coordinatePair));
                }

                string[] coord = coordinatePair.Split(',');
                lat = double.Parse(coord[0]);
                _long = double.Parse(coord[1]);
            }

            public void SetCoordinate(double latitude, double longitude)
            {
                lat = latitude;
                _long = longitude;
            }

            public bool IsValid => Latitude != 0 || Longitude != 0;

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "{0},{1}",
                    lat.ToString(CultureInfo.InvariantCulture), _long.ToString(CultureInfo.InvariantCulture));
            }

            public override bool Equals(object obj)
            {
                return obj is Coordinate coordinate &&
                       lat == coordinate.lat &&
                       _long == coordinate._long;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(lat, _long);
            }
        }
    }
}