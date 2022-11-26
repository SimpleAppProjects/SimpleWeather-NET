#if true
using SimpleWeather.LocationData;
using SimpleWeather.WeatherData;
using System;

namespace SimpleWeather.Weather_API.HERE
{
    public static partial class HERELocationProviderExtensions
    {
        /* HERE Autocomplete */
        public static LocationQuery CreateLocationModel(this HERELocationProvider _, HERE.Suggestion location, String weatherAPI)
        {
            if (location?.address == null)
                return null;

            var model = new LocationQuery();

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
                model.LocationName = string.Format("{0}, {1}, {2}", town, location.address.county, region);
            }
            else if (!String.IsNullOrWhiteSpace(town) && !String.IsNullOrWhiteSpace(region))
            {
                if (Equals(town, region))
                {
                    model.LocationName = string.Format("{0}, {1}", town, country);
                }
                else
                {
                    model.LocationName = string.Format("{0}, {1}", town, region);
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(town) || String.IsNullOrWhiteSpace(region))
                {
                    if (String.IsNullOrWhiteSpace(town))
                    {
                        model.LocationName = string.Format("{0}, {1}", region, country);
                    }
                    else
                    {
                        model.LocationName = string.Format("{0}, {1}", town, country);
                    }
                }
            }

            model.LocationCountry = location.countryCode;
            model.Location_Query = location.locationId;

            model.LocationLat = -1;
            model.LocationLong = -1;

            model.LocationTZLong = null;

            model.LocationSource = WeatherAPI.Here;
            model.WeatherSource = weatherAPI;

            return model;
        }

        /* HERE Geocoder */
        public static LocationQuery CreateLocationModel(this HERELocationProvider _, HERE.Result location, String weatherAPI)
        {
            if (location?.location?.address == null)
                return null;

            var model = new LocationQuery();

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
                model.LocationName = string.Format("{0}, {1}, {2}", town, location.location.address.county, region);
            }
            else if (!String.IsNullOrWhiteSpace(town) && !String.IsNullOrWhiteSpace(region))
            {
                if (Equals(town, region))
                {
                    model.LocationName = string.Format("{0}, {1}", town, country);
                }
                else
                {
                    model.LocationName = string.Format("{0}, {1}", town, region);
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(town) || String.IsNullOrWhiteSpace(region))
                {
                    if (!String.IsNullOrWhiteSpace(location.location.address.label))
                    {
                        model.LocationName = location.location.address.label;

                        if (model.LocationName?.Contains(", " + country) == true)
                        {
                            model.LocationName = model.LocationName.Replace(", " + country, "");
                        }
                    }
                    else
                    {
                        if (String.IsNullOrWhiteSpace(town))
                        {
                            model.LocationName = string.Format("{0}, {1}", region, country);
                        }
                        else
                        {
                            model.LocationName = string.Format("{0}, {1}", town, country);
                        }
                    }
                }
            }

            model.LocationCountry = countryCode;

            model.LocationLat = location.location.displayPosition.latitude;
            model.LocationLong = location.location.displayPosition.longitude;

            model.LocationTZLong = location.location.adminInfo.timeZone.id;

            model.LocationSource = WeatherAPI.Here;

            model.UpdateWeatherSource(weatherAPI);

            return model;
        }
    }
}
#endif