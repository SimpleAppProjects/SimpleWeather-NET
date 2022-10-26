using SimpleWeather.WeatherData;
using System;

namespace SimpleWeather.Location
{
    public partial class LocationQuery
    {
        /* HERE Autocomplete */
        public LocationQuery(HERE.Suggestion location, String weatherAPI)
        {
            SetLocation(location, weatherAPI);
        }

        /* HERE Autocomplete */
        public void SetLocation(HERE.Suggestion location, String weatherAPI)
        {
            if (location?.address == null)
                return;

            string town, region, country;

            // Try to get district name or fallback to city name
            town = location.address.district;

            if (String.IsNullOrEmpty(town))
                town = location.address.city;

            // Try to get district name or fallback to city name
            region = location.address.state;

            if (String.IsNullOrEmpty(location.address.state) && !Equals(town, location.address.county))
                region = location.address.county;

            country = location.address.country;

            if (!String.IsNullOrWhiteSpace(town) && !String.IsNullOrWhiteSpace(region) &&
                !String.IsNullOrEmpty(location.address.county)
                && !(Equals(location.address.county, region) ||
                Equals(location.address.county, town)))
            {
                LocationName = string.Format("{0}, {1}, {2}", town, location.address.county, region);
            }
            else if (!String.IsNullOrWhiteSpace(town) && !String.IsNullOrWhiteSpace(region))
            {
                if (Equals(town, region))
                {
                    LocationName = string.Format("{0}, {1}", town, country);
                }
                else
                {
                    LocationName = string.Format("{0}, {1}", town, region);
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(town) || String.IsNullOrWhiteSpace(region))
                {
                    if (String.IsNullOrWhiteSpace(town))
                    {
                        LocationName = string.Format("{0}, {1}", region, country);
                    }
                    else
                    {
                        LocationName = string.Format("{0}, {1}", town, country);
                    }
                }
            }

            LocationCountry = location.countryCode;
            Location_Query = location.locationId;

            LocationLat = -1;
            LocationLong = -1;

            LocationTZLong = null;

            LocationSource = WeatherAPI.Here;
            WeatherSource = weatherAPI;
        }

        /* HERE Geocoder */
        public LocationQuery(HERE.Result location, String weatherAPI)
        {
            SetLocation(location, weatherAPI);
        }

        /* HERE Geocoder */
        public void SetLocation(HERE.Result location, String weatherAPI)
        {
            if (location?.location?.address == null)
                return;

            string country = null, countryCode = null, town = null, region = null;

            if (location.location.address.additionalData != null)
            {
                foreach (HERE.Additionaldata item in location.location.address.additionalData)
                {
                    if ("Country2".Equals(item.key))
                        countryCode = item.value;
                    else if ("CountryName".Equals(item.key))
                        country = item.value;

                    if (countryCode != null && country != null)
                        break;
                }
            }

            // Try to get district name or fallback to city name
            town = location.location.address.district;

            if (String.IsNullOrEmpty(town))
                town = location.location.address.city;

            region = location.location.address.state;

            if (String.IsNullOrEmpty(region) && !Equals(town, location.location.address.county))
                region = location.location.address.county;

            if (String.IsNullOrEmpty(country))
                country = location.location.address.country;

            if (String.IsNullOrEmpty(countryCode))
                countryCode = location.location.address.country;

            if (!String.IsNullOrWhiteSpace(town) && !String.IsNullOrWhiteSpace(region) &&
                !String.IsNullOrEmpty(location.location.address.county)
                && !(Equals(location.location.address.county, region) ||
                Equals(location.location.address.county, town)))
            {
                LocationName = string.Format("{0}, {1}, {2}", town, location.location.address.county, region);
            }
            else if (!String.IsNullOrWhiteSpace(town) && !String.IsNullOrWhiteSpace(region))
            {
                if (Equals(town, region))
                {
                    LocationName = string.Format("{0}, {1}", town, country);
                }
                else
                {
                    LocationName = string.Format("{0}, {1}", town, region);
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(town) || String.IsNullOrWhiteSpace(region))
                {
                    if (!String.IsNullOrWhiteSpace(location.location.address.label))
                    {
                        LocationName = location.location.address.label;

                        if (LocationName?.Contains(", " + country) == true)
                        {
                            LocationName = LocationName.Replace(", " + country, "");
                        }
                    }
                    else
                    {
                        if (String.IsNullOrWhiteSpace(town))
                        {
                            LocationName = string.Format("{0}, {1}", region, country);
                        }
                        else
                        {
                            LocationName = string.Format("{0}, {1}", town, country);
                        }
                    }
                }
            }

            LocationCountry = countryCode;

            LocationLat = location.location.displayPosition.latitude;
            LocationLong = location.location.displayPosition.longitude;

            LocationTZLong = location.location.adminInfo.timeZone.id;

            LocationSource = WeatherAPI.Here;
            WeatherSource = weatherAPI;

            UpdateLocationQuery();
        }
    }
}