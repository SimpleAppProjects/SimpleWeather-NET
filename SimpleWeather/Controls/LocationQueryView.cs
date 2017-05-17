using SimpleWeather.WeatherUnderground;
using SimpleWeather.WeatherYahoo;

namespace SimpleWeather.Controls
{
    public class LocationQueryView
    {
        public string LocationName { get; set; }
        public string LocationCountry { get; set; }
        public string LocationQuery { get; set; }

        public LocationQueryView()
        {
            LocationName = "No results found";
            LocationCountry = string.Empty;
            LocationQuery = string.Empty;
        }

        #region WeatherUnderground
        public LocationQueryView(AC_RESULT location)
        {
            setLocation(location);
        }

        public void setLocation(AC_RESULT location)
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
        #endregion

        #region Yahoo Weather
        public LocationQueryView(place location)
        {
            setLocation(location);
        }

        public void setLocation(place location)
        {
            string town, region;

            if (location.locality1 != null)
                town = location.locality1.Value;
            else
                town = location.name;

            if (location.admin1 != null)
                region = location.admin1.Value;
            else if (location.admin2 != null)
                region = location.admin2.Value;
            else
                region = location.country.Value;

            LocationName = string.Format("{0}, {1}", town, region);
            LocationCountry = location.country.code;
            LocationQuery = location.woeid;
        }
        #endregion
    }
}
