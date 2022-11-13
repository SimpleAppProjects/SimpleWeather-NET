using SimpleWeather.LocationData;
using SimpleWeather.WeatherData;
using System;
using System.Text;

namespace SimpleWeather.Weather_API.Bing
{
    public static class BingMapsLocationProviderExtensions
    {
        /* Bing AutoComplete */
        public static LocationQuery CreateLocationModel(this WeatherLocationProviderImpl _, Address address, string weatherAPI)
        {
            if (address == null)
                return null;

            var model = new LocationQuery();

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
                    model.LocationName = string.Format("{0}, {1}, {2}", town, address.adminDistrict2, region);
                else
                    model.LocationName = string.Format("{0}, {1}", town, region);
            }
            else if (!String.IsNullOrEmpty(address.adminDistrict2) &&
                !(address.adminDistrict2.Equals(region) || address.adminDistrict2.Equals(town)))
            {
                model.LocationName = string.Format("{0}, {1}, {2}", town, address.adminDistrict2, region);
            }
            else
            {
                model.LocationName = string.Format("{0}, {1}", town, region);
            }

            model.LocationCountry = address.countryRegionIso2;

            StringBuilder sb = new StringBuilder();
            sb.Append(address.locality).Append(", ");
            if (!String.IsNullOrWhiteSpace(address.adminDistrict2))
                sb.Append(address.adminDistrict2).Append(", ");
            sb.Append(address.adminDistrict).Append(", ");
            sb.Append(address.countryRegion);

            model.Location_Query = sb.ToString();

            model.LocationLat = -1;
            model.LocationLong = -1;

            model.LocationTZLong = null;

            model.LocationSource = WeatherAPI.BingMaps;
            model.WeatherSource = weatherAPI;

            return model;
        }

#if WINDOWS_UWP
        public static LocationQuery CreateLocationModel(this WeatherLocationProviderImpl _, Windows.Services.Maps.MapLocation result, String weatherAPI)
        {
            if (result?.Address == null)
                return null;

            var model = new LocationQuery();

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
                    model.LocationName = string.Format("{0}, {1}, {2}", town, region, result.Address.Region);
                else
                    model.LocationName = string.Format("{0}, {1}", town, region);
            }
            else if (!String.IsNullOrEmpty(result.Address.Region) &&
                !(result.Address.Region.Equals(region) || result.Address.Region.Equals(town)))
            {
                model.LocationName = string.Format("{0}, {1}, {2}", town, result.Address.Region, region);
            }
            else
            {
                model.LocationName = string.Format("{0}, {1}", town, region);
            }

            model.LocationCountry = result.Address.CountryCode;

            model.LocationLat = result.Point.Position.Latitude;
            model.LocationLong = result.Point.Position.Longitude;

            model.LocationTZLong = null;

            model.LocationSource = WeatherAPI.BingMaps;

            model.UpdateWeatherSource(weatherAPI);

            return model;
        }
#endif
    }
}