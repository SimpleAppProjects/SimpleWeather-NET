using SimpleWeather.WeatherData;
using System;
using System.Text;

namespace SimpleWeather.Location
{
    public partial class LocationQuery
    {
        /* Bing AutoComplete */
        public LocationQuery(Bing.Address address, String weatherAPI)
        {
            SetLocation(address, weatherAPI);
        }

        /* Bing AutoComplete */
        public void SetLocation(Bing.Address address, String weatherAPI)
        {
            if (address == null)
                return;

            string town, region;

            // Try to get district name or fallback to city name
            if (!String.IsNullOrEmpty(address.name))
                town = address.name;
            else
                town = address.locality;

            // Try to get district name or fallback to city name
            if (!String.IsNullOrEmpty(address.adminDistrict))
                region = address.adminDistrict;
            else
                region = address.adminDistrict2;

            if (String.IsNullOrEmpty(region) || Equals(town, region))
            {
                region = address.countryRegion;
            }

            if (!Equals(region, address.countryRegion))
            {
                if (!String.IsNullOrEmpty(address.adminDistrict2) &&
                    !(address.adminDistrict2.Equals(region) || address.adminDistrict2.Equals(town)))
                    LocationName = string.Format("{0}, {1}, {2}", town, address.adminDistrict2, region);
                else
                    LocationName = string.Format("{0}, {1}", town, region);
            }
            else if (!String.IsNullOrEmpty(address.adminDistrict2) &&
                !(address.adminDistrict2.Equals(region) || address.adminDistrict2.Equals(town)))
            {
                LocationName = string.Format("{0}, {1}, {2}", town, address.adminDistrict2, region);
            }
            else
            {
                LocationName = string.Format("{0}, {1}", town, region);
            }

            LocationCountry = address.countryRegionIso2;

            StringBuilder sb = new StringBuilder();
            sb.Append(address.locality).Append(", ");
            if (!String.IsNullOrWhiteSpace(address.adminDistrict2))
                sb.Append(address.adminDistrict2).Append(", ");
            sb.Append(address.adminDistrict).Append(", ");
            sb.Append(address.countryRegion);

            Location_Query = sb.ToString();

            LocationLat = -1;
            LocationLong = -1;

            LocationTZLong = null;

            LocationSource = WeatherAPI.BingMaps;
            WeatherSource = weatherAPI;
        }

        /* Bing Geocoder */
        public LocationQuery(Windows.Services.Maps.MapLocation result, String weatherAPI)
        {
            SetLocation(result, weatherAPI);
        }

        /* Bing Geocoder */
        private void SetLocation(Windows.Services.Maps.MapLocation result, String weatherAPI)
        {
            if (result?.Address == null)
                return;

            string town, region;

            // Try to get district name or fallback to city name
            if (!String.IsNullOrEmpty(result.Address.Neighborhood))
                town = result.Address.Neighborhood;
            else
                town = result.Address.Town;

            // Try to get district name or fallback to city name
            if (!String.IsNullOrEmpty(result.Address.District))
                region = result.Address.District;
            else
                region = result.Address.Country;

            if (String.IsNullOrEmpty(region) || Equals(town, region))
            {
                region = result.Address.Country;
            }

            if (!Equals(region, result.Address.Country))
            {
                if (!String.IsNullOrEmpty(result.Address.Region) &&
                    !(result.Address.Region.Equals(region) || result.Address.Region.Equals(town)))
                    LocationName = string.Format("{0}, {1}, {2}", town, region, result.Address.Region);
                else
                    LocationName = string.Format("{0}, {1}", town, region);
            }
            else if (!String.IsNullOrEmpty(result.Address.Region) &&
                !(result.Address.Region.Equals(region) || result.Address.Region.Equals(town)))
            {
                LocationName = string.Format("{0}, {1}, {2}", town, result.Address.Region, region);
            }
            else
            {
                LocationName = string.Format("{0}, {1}", town, region);
            }

            LocationCountry = result.Address.CountryCode;

            LocationLat = result.Point.Position.Latitude;
            LocationLong = result.Point.Position.Longitude;

            LocationTZLong = null;

            LocationSource = WeatherAPI.BingMaps;
            WeatherSource = weatherAPI;

            UpdateLocationQuery();
        }
    }
}