using System;
using SimpleWeather.WeatherUnderground;
using SimpleWeather.WeatherYahoo;

namespace SimpleWeather.Controls
{
    public class LocationQueryViewModel
    {
        public string LocationName { get; set; }
        public string LocationCountry { get; set; }
        public string LocationQuery { get; set; }

        public LocationQueryViewModel()
        {
            LocationName = "No results found";
            LocationCountry = string.Empty;
            LocationQuery = string.Empty;
        }

        #region WeatherUnderground
        public LocationQueryViewModel(AC_RESULT location)
        {
            setLocation(location);
        }

        public void setLocation(AC_RESULT location)
        {
            LocationName = location.name;
            LocationCountry = location.c;
            LocationQuery = location.l;
        }

        public LocationQueryViewModel(location location)
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
        public LocationQueryViewModel(place location)
        {
            setLocation(location);
        }

        public void setLocation(place location)
        {
            string town, region;

            if ((location.placeTypeName.Value == "Zip Code"
                || location.placeTypeName.Value == "Postal Code"))
            {
                town = location.name;

                if (location.locality1 != null
                    && !String.IsNullOrEmpty(location.locality1.Value))
                    town += " - " + location.locality1.Value;
            }
            else
            {
                if (location.locality1 != null
                    && !String.IsNullOrEmpty(location.locality1.Value))
                    town = location.locality1.Value;
                else
                    town = location.name;
            }

            if (location.admin1 != null
                && !String.IsNullOrEmpty(location.admin1.Value))
                region = location.admin1.Value;
            else if (location.admin2 != null
                && !String.IsNullOrEmpty(location.admin2.Value))
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
