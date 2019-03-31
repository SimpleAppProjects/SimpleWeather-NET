using System;
using SimpleWeather.WeatherUnderground;
using SimpleWeather.WeatherYahoo;
using SimpleWeather.OpenWeather;
using System.Collections.Generic;
using SimpleWeather.UWP;

namespace SimpleWeather.Controls
{
    public class LocationQueryViewModel
    {
        public string LocationName { get; set; }
        public string LocationCountry { get; set; }
        public string LocationQuery { get; set; }

        public double LocationLat { get; set; }
        public double LocationLong { get; set; }

        public string LocationTZ_Long { get; set; }

        public LocationQueryViewModel()
        {
            LocationName = App.ResLoader.GetString("Error_NoResults");
            LocationCountry = string.Empty;
            LocationQuery = string.Empty;
        }

        #region WeatherUnderground
        public LocationQueryViewModel(WeatherUnderground.AC_RESULT location)
        {
            SetLocation(location);
        }

        public void SetLocation(WeatherUnderground.AC_RESULT location)
        {
            LocationName = location.name;
            LocationCountry = location.c;
            LocationQuery = location.l;

            LocationLat = double.Parse(location.lat);
            LocationLong = double.Parse(location.lon);

            LocationTZ_Long = location.tz;
        }

        public LocationQueryViewModel(WeatherUnderground.location location)
        {
            SetLocation(location);
        }

        public void SetLocation(WeatherUnderground.location location)
        {
            LocationName = string.Format("{0}, {1}", location.city, location.state);
            LocationCountry = location.country;
            LocationQuery = location.query;

            LocationLat = double.Parse(location.lat);
            LocationLong = double.Parse(location.lon);

            LocationTZ_Long = location.tz_unix;
        }
        #endregion

        #region Yahoo Weather
        public LocationQueryViewModel(WeatherYahoo.AC_RESULT location)
        {
            SetLocation(location);
        }

        public void SetLocation(WeatherYahoo.AC_RESULT location)
        {
            LocationName = location.name;
            LocationCountry = location.c;
            LocationQuery = string.Format("lat={0}&lon={1}", location.lat, location.lon);

            LocationLat = double.Parse(location.lat);
            LocationLong = double.Parse(location.lon);

            LocationTZ_Long = location.tz;
        }

        public LocationQueryViewModel(WeatherYahoo.location location)
        {
            SetLocation(location);
        }

        public void SetLocation(WeatherYahoo.location location)
        {
            LocationName = string.Format("{0}, {1}", location.city, location.state);
            LocationCountry = location.country;
            LocationQuery = string.Format("lat={0}&lon={1}", location.lat, location.lon);

            LocationLat = double.Parse(location.lat);
            LocationLong = double.Parse(location.lon);

            LocationTZ_Long = location.tz_unix;
        }
        #endregion

        #region OpenWeatherMap|Met.No
        public LocationQueryViewModel(OpenWeather.AC_RESULT location)
        {
            SetLocation(location);
        }

        public void SetLocation(OpenWeather.AC_RESULT location)
        {
            LocationName = location.name;
            LocationCountry = location.c;
            LocationQuery = string.Format("lat={0}&lon={1}", location.lat, location.lon);

            LocationLat = double.Parse(location.lat);
            LocationLong = double.Parse(location.lon);

            LocationTZ_Long = location.tz;
        }

        public LocationQueryViewModel(OpenWeather.location location)
        {
            SetLocation(location);
        }

        public void SetLocation(OpenWeather.location location)
        {
            LocationName = string.Format("{0}, {1}", location.city, location.state);
            LocationCountry = location.country;
            LocationQuery = string.Format("lat={0}&lon={1}", location.lat, location.lon);

            LocationLat = double.Parse(location.lat);
            LocationLong = double.Parse(location.lon);

            LocationTZ_Long = location.tz_unix;
        }
        #endregion

        public LocationQueryViewModel(HERE.Suggestion location)
        {
            SetLocation(location);
        }

        public void SetLocation(HERE.Suggestion location)
        {
            string town, region;

            // Try to get district name or fallback to city name
            if (!String.IsNullOrEmpty(location.address.district))
                town = location.address.district;
            else
                town = location.address.city;

            // Try to get district name or fallback to city name
            if (!String.IsNullOrEmpty(location.address.state))
                region = location.address.state;
            else
                region = location.address.country;

            if (!String.IsNullOrEmpty(location.address.county)
                    && !(location.address.county.Equals(region) || location.address.county.Equals(town)))
                LocationName = string.Format("{0}, {1}, {2}", town, location.address.county, region);
            else
                LocationName = string.Format("{0}, {1}", town, region);

            LocationCountry = location.countryCode;
            LocationQuery = location.locationId;

            LocationLat = -1;
            LocationLong = -1;

            LocationTZ_Long = null;
        }

        public LocationQueryViewModel(HERE.Result location)
        {
            SetLocation(location);
        }

        public void SetLocation(HERE.Result location)
        {
            string country = null, town = null, region = null;

            if (location.location.address.additionalData != null)
            {
                foreach(HERE.Additionaldata item in location.location.address.additionalData)
                {
                    if ("Country2".Equals(item.key))
                        country = item.value;
                    else if ("StateName".Equals(item.key))
                        region = item.value;

                    if (country != null && region != null)
                        break;
                }
            }

            // Try to get district name or fallback to city name
            if (!String.IsNullOrEmpty(location.location.address.district))
                town = location.location.address.district;
            else
                town = location.location.address.city;

            if (String.IsNullOrEmpty(region))
                region = location.location.address.state;

            if (String.IsNullOrEmpty(region))
                region = location.location.address.county;

            if (String.IsNullOrEmpty(country))
                region = location.location.address.country;

            if (!String.IsNullOrEmpty(location.location.address.county)
                    && !(location.location.address.county.Equals(region) || location.location.address.county.Equals(town)))
                LocationName = string.Format("{0}, {1}, {2}", town, location.location.address.county, region);
            else
                LocationName = string.Format("{0}, {1}", town, region);

            LocationCountry = country;
            LocationQuery = string.Format("latitude={0}&longitude={1}",
                location.location.displayPosition.latitude, location.location.displayPosition.longitude);

            LocationLat = location.location.displayPosition.latitude;
            LocationLong = location.location.displayPosition.longitude;

            LocationTZ_Long = location.location.adminInfo.timeZone.id;
        }

        public override bool Equals(object obj)
        {
            var model = obj as LocationQueryViewModel;
            return model != null &&
                   LocationName == model.LocationName &&
                   LocationCountry == model.LocationCountry/* &&
                   LocationQuery == model.LocationQuery*/;
        }

        public override int GetHashCode()
        {
            var hashCode = 879653843;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocationName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocationCountry);
            //hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocationQuery);
            return hashCode;
        }
    }
}
