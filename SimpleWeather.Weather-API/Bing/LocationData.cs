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
        public static LocationQuery CreateLocationModel(this WeatherLocationProviderImpl _, AutosuggestEntityResource place, string weatherAPI)
        {
            if (place?.Address == null)
                return null;

            var model = new LocationQuery();

            if (!string.IsNullOrWhiteSpace(place?.Name))
            {
                model.LocationName = place.Name;

                if (!string.IsNullOrWhiteSpace(place?.Address?.AdminDistrict) && !model.LocationName.EndsWith(place.Address.AdminDistrict))
                {
                    model.LocationName = $"{model.LocationName}, {place.Address.AdminDistrict}";
                }
            }
            else if (!string.IsNullOrWhiteSpace(place?.Address?.FormattedAddress))
            {
                model.LocationName = place.Address.FormattedAddress;
            }
            else
            {
                string town, region;

                // Try to get district name or fallback to city name
                if (!String.IsNullOrEmpty(place?.Address?.Neighborhood))
                    town = place.Address.Neighborhood;
                else
                    town = place.Address.Locality;

                // Try to get district name or fallback to city name
                if (!String.IsNullOrEmpty(place.Address.AdminDistrict))
                    region = place.Address.AdminDistrict;
                else
                    region = place.Address.AdminDistrict2;

                if (String.IsNullOrEmpty(region) || Equals(town, region))
                {
                    region = place.Address.CountryRegion;
                }

                if (!Equals(region, place.Address.CountryRegion))
                {
                    if (!String.IsNullOrEmpty(place.Address.AdminDistrict2) &&
                        !(place.Address.AdminDistrict2.Equals(region) || place.Address.AdminDistrict2.Equals(town)))
                        model.LocationName = string.Format("{0}, {1}, {2}", town, place.Address.AdminDistrict2, region);
                    else
                        model.LocationName = string.Format("{0}, {1}", town, region);
                }
                else if (!String.IsNullOrEmpty(place.Address.AdminDistrict2) &&
                    !(place.Address.AdminDistrict2.Equals(region) || place.Address.AdminDistrict2.Equals(town)))
                {
                    model.LocationName = string.Format("{0}, {1}, {2}", town, place.Address.AdminDistrict2, region);
                }
                else
                {
                    model.LocationName = string.Format("{0}, {1}", town, region);
                }
            }

            model.LocationCountry = place.Address.CountryRegionIso2;

            StringBuilder sb = new StringBuilder();
            sb.Append(place.Address.Locality).Append(", ");
            if (!String.IsNullOrWhiteSpace(place.Address.AdminDistrict2))
                sb.Append(place.Address.AdminDistrict2).Append(", ");
            sb.Append(place.Address.AdminDistrict).Append(", ");
            sb.Append(place.Address.CountryRegion);

            model.Location_Query = sb.ToString();

            model.LocationLat = -1;
            model.LocationLong = -1;

            model.LocationTZLong = null;

            model.LocationSource = WeatherAPI.BingMaps;
            model.WeatherSource = weatherAPI;

            return model;
        }

#if WINDOWS
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
            if (result?.Address == null)
                return null;

            var model = new LocationQuery();

            if (!string.IsNullOrWhiteSpace(result?.Name) || !string.IsNullOrWhiteSpace(result?.Address?.Neighborhood))
            {
                model.LocationName = result?.Address?.Neighborhood ?? result?.Address?.Locality ?? result.Name;

                if (!string.IsNullOrWhiteSpace(result?.Address?.AdminDistrict) && !model.LocationName.EndsWith(result.Address.AdminDistrict) && !model.LocationName.EndsWith(result.Address.CountryRegion))
                {
                    model.LocationName = $"{model.LocationName}, {result.Address.AdminDistrict}";
                }
            }
            else if (!string.IsNullOrWhiteSpace(result?.Address?.FormattedAddress))
            {
                model.LocationName = result.Address.FormattedAddress;
            }
            else
            {
                string town, region;

                // Try to get district name or fallback to city name
                if (!String.IsNullOrEmpty(result?.Address?.Neighborhood))
                    town = result.Address.Neighborhood;
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