using SimpleWeather.WeatherUnderground;

namespace SimpleWeather.Controls
{
    public class LocationQueryView
    {
        public string LocationName { get; set; }
        public string LocationCountry { get; set; }
        public string LocationQuery { get; set; }

        public LocationQueryView()
        {
            LocationName = string.Empty;
            LocationCountry = string.Empty;
            LocationQuery = string.Empty;
        }

        public LocationQueryView(AC_Location location)
        {
            setLocation(location);
        }

        public void setLocation(AC_Location location)
        {
            LocationName = location.name;
            LocationCountry = location.c;
            LocationQuery = location.l;
        }

        public LocationQueryView(location location)
        {
            setLocation(location);
        }

        public void setLocation(location location)
        {
            LocationName = string.Format("{0}, {1}", location.city, location.state);
            LocationCountry = location.country;
            LocationQuery = location.query;
        }
    }
}
