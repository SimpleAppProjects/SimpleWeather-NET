using BingMapsRESTToolkit;
using SimpleWeather.LocationData;
using SimpleWeather.WeatherData;
using System;
using System.Text;

namespace SimpleWeather.Weather_API.Bing
{
    public static class BingMapsLocationProviderExtensions
    {
        /* Bing AutoComplete */
        public static LocationQuery CreateLocationModel(this WeatherLocationProviderImpl _, AutoSuggestPlaceResource place, string weatherAPI)
        {
            if (place?.EntityAddress == null)
                return null;

            var model = new LocationQuery();

            string town, region;

            // Try to get district name or fallback to city name
            if (!String.IsNullOrEmpty(place.Name))
                town = place.Name;
            else
                town = place.EntityAddress.Locality;

            // Try to get district name or fallback to city name
            if (!String.IsNullOrEmpty(place.EntityAddress.AdminDistrict))
                region = place.EntityAddress.AdminDistrict;
            else
                region = place.EntityAddress.AdminDistrict2;

            if (String.IsNullOrEmpty(region) || Equals(town, region))
            {
                region = place.EntityAddress.CountryRegion;
            }

            if (!Equals(region, place.EntityAddress.CountryRegion))
            {
                if (!String.IsNullOrEmpty(place.EntityAddress.AdminDistrict2) &&
                    !(place.EntityAddress.AdminDistrict2.Equals(region) || place.EntityAddress.AdminDistrict2.Equals(town)))
                    model.LocationName = string.Format("{0}, {1}, {2}", town, place.EntityAddress.AdminDistrict2, region);
                else
                    model.LocationName = string.Format("{0}, {1}", town, region);
            }
            else if (!String.IsNullOrEmpty(place.EntityAddress.AdminDistrict2) &&
                !(place.EntityAddress.AdminDistrict2.Equals(region) || place.EntityAddress.AdminDistrict2.Equals(town)))
            {
                model.LocationName = string.Format("{0}, {1}, {2}", town, place.EntityAddress.AdminDistrict2, region);
            }
            else
            {
                model.LocationName = string.Format("{0}, {1}", town, region);
            }

            model.LocationCountry = place.EntityAddress.CountryRegionIso2;

            StringBuilder sb = new StringBuilder();
            sb.Append(place.EntityAddress.Locality).Append(", ");
            if (!String.IsNullOrWhiteSpace(place.EntityAddress.AdminDistrict2))
                sb.Append(place.EntityAddress.AdminDistrict2).Append(", ");
            sb.Append(place.EntityAddress.AdminDistrict).Append(", ");
            sb.Append(place.EntityAddress.CountryRegion);

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

        public static LocationQuery CreateLocationModel(this WeatherLocationProviderImpl _, BingMapsRESTToolkit.Location result, String weatherAPI)
        {
            if (result == null)
                return null;

            var model = new LocationQuery();

            string town, region;

            // Try to get district name or fallback to city name
            if (!String.IsNullOrEmpty(result.Name))
                town = result.Name;
            else
                town = result.Address.Locality;

            // Try to get district name or fallback to city name
            if (!String.IsNullOrEmpty(result.Address.AdminDistrict))
                region = result.Address.AdminDistrict;
            else
                region = result.Address.AdminDistrict2;

            if (String.IsNullOrEmpty(region) || Equals(town, region))
            {
                region = result.Address.CountryRegion;
            }

            if (!Equals(region, result.Address.CountryRegion))
            {
                if (!String.IsNullOrEmpty(result.Address.AdminDistrict2) &&
                    !(result.Address.AdminDistrict2.Equals(region) || result.Address.AdminDistrict2.Equals(town)))
                    model.LocationName = string.Format("{0}, {1}, {2}", town, result.Address.AdminDistrict2, region);
                else
                    model.LocationName = string.Format("{0}, {1}", town, region);
            }
            else if (!String.IsNullOrEmpty(result.Address.AdminDistrict2) &&
                !(result.Address.AdminDistrict2.Equals(region) || result.Address.AdminDistrict2.Equals(town)))
            {
                model.LocationName = string.Format("{0}, {1}, {2}", town, result.Address.AdminDistrict2, region);
            }
            else
            {
                model.LocationName = string.Format("{0}, {1}", town, region);
            }

            model.LocationCountry = result.Address.CountryRegionIso2;

            model.LocationLat = result.Point.Coordinates[0];
            model.LocationLong = result.Point.Coordinates[1];

            model.LocationTZLong = null;

            model.LocationSource = WeatherAPI.BingMaps;

            model.UpdateWeatherSource(weatherAPI);

            return model;
        }
    }
}