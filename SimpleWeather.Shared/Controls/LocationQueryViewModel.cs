using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SimpleWeather.Controls
{
    public class LocationQueryViewModel
    {
        public string LocationName { get; set; }
        public string LocationRegion { get; set; }
        public string LocationCountry { get; set; }
        public string LocationQuery { get; set; }

        public double LocationLat { get; set; }
        public double LocationLong { get; set; }

        public string LocationTZLong { get; set; }

        public string LocationSource { get; set; }
        public string WeatherSource { get; set; }

        public string LocationRegionText
        {
            get
            {
                if (String.IsNullOrWhiteSpace(LocationRegion))
                {
                    return LocationCountry;
                }
                else
                {
                    return LocationRegion + ", " + LocationCountry;
                }
            }
        }

        public LocationQueryViewModel()
        {
            LocationName = SimpleLibrary.GetInstance().ResLoader.GetString("Error_NoResults");
            LocationCountry = string.Empty;
            LocationQuery = string.Empty;
        }

        public LocationQueryViewModel(Location.LocationData data)
        {
            LocationQuery = data.query;
            LocationName = data.name;
            LocationLat = data.latitude;
            LocationLong = data.longitude;
            LocationTZLong = data.tz_long;
            WeatherSource = data.weatherSource;
            LocationSource = data.locationSource;
            LocationCountry = data.country_code;
        }

        public bool IsEmpty => String.IsNullOrEmpty(LocationCountry) && String.IsNullOrEmpty(LocationQuery);

        /* HERE Autocomplete */
        public LocationQueryViewModel(HERE.Suggestion location, String weatherAPI)
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
            LocationQuery = location.locationId;

            LocationLat = -1;
            LocationLong = -1;

            LocationTZLong = null;

            LocationSource = WeatherAPI.Here;
            WeatherSource = weatherAPI;
        }

        /* HERE Geocoder */
        public LocationQueryViewModel(HERE.Result location, String weatherAPI)
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

                        if ((bool)LocationName?.Contains(", " + country))
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

        /* Bing AutoComplete */
        public LocationQueryViewModel(Bing.Address address, String weatherAPI)
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

            if (String.IsNullOrEmpty(region) || town.Equals(region))
            {
                region = address.countryRegion;
            }

            if (!region.Equals(address.countryRegion))
            {
                if (!String.IsNullOrEmpty(address.adminDistrict2)
                        && !(address.adminDistrict2.Equals(region) || address.adminDistrict2.Equals(town)))
                    LocationName = string.Format("{0}, {1}, {2}", town, address.adminDistrict2, region);
                else
                    LocationName = string.Format("{0}, {1}", town, region);
            }
            else if (!String.IsNullOrEmpty(address.adminDistrict2)
                    && !(address.adminDistrict2.Equals(region) || address.adminDistrict2.Equals(town)))
                LocationName = string.Format("{0}, {1}, {2}", town, address.adminDistrict2, region);
            else
                LocationName = string.Format("{0}, {1}", town, region);

            LocationCountry = address.countryRegionIso2;

            StringBuilder sb = new StringBuilder();
            sb.Append(address.locality).Append(", ");
            if (!String.IsNullOrWhiteSpace(address.adminDistrict2))
                sb.Append(address.adminDistrict2).Append(", ");
            sb.Append(address.adminDistrict).Append(", ");
            sb.Append(address.countryRegion);

            LocationQuery = sb.ToString();

            LocationLat = -1;
            LocationLong = -1;

            LocationTZLong = null;

            LocationSource = WeatherAPI.BingMaps;
            WeatherSource = weatherAPI;
        }

        /* Bing Geocoder */
        public LocationQueryViewModel(Windows.Services.Maps.MapLocation result, String weatherAPI)
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

            if (String.IsNullOrEmpty(region) || town.Equals(region))
            {
                region = result.Address.Country;
            }

            if (!region.Equals(result.Address.Country))
            {
                if (!String.IsNullOrEmpty(result.Address.Region)
                        && !(result.Address.Region.Equals(region) || result.Address.Region.Equals(town)))
                    LocationName = string.Format("{0}, {1}, {2}", town, region, result.Address.Region);
                else
                    LocationName = string.Format("{0}, {1}", town, region);
            }
            else if (!String.IsNullOrEmpty(result.Address.Region)
                    && !(result.Address.Region.Equals(region) || result.Address.Region.Equals(town)))
                LocationName = string.Format("{0}, {1}, {2}", town, result.Address.Region, region);
            else
                LocationName = string.Format("{0}, {1}", town, region);

            LocationCountry = result.Address.CountryCode;

            LocationLat = result.Point.Position.Latitude;
            LocationLong = result.Point.Position.Longitude;

            LocationTZLong = null;

            LocationSource = WeatherAPI.BingMaps;
            WeatherSource = weatherAPI;

            UpdateLocationQuery();
        }

        /* WeatherAPI AutoComplete Query */
        public LocationQueryViewModel(WeatherApi.LocationItem result, String weatherAPI)
        {
            SetLocation(result, weatherAPI);
        }

        /* WeatherAPI AutoComplete Query */
        private void SetLocation(WeatherApi.LocationItem result, String weatherAPI)
        {
            if (result == null)
                return;

            bool isUSorCA = LocationUtils.IsUSorCanada(result.country);

            string name = result.name;
            if (isUSorCA)
            {
                name = name.ReplaceFirst(string.Format(", {0}", result.country), "");
            }
            else
            {
                name = name.ReplaceFirst(string.Format(", {0}, {1}", result.region, result.country), string.Format(", {0}", result.country));
                LocationRegion = result.region;
            }

            LocationName = name;
            LocationCountry = result.country;
            LocationQuery = result.id.ToInvariantString();

            LocationLat = result.lat;
            LocationLong = result.lon;

            LocationTZLong = null;

            LocationSource = WeatherAPI.WeatherApi;
            WeatherSource = weatherAPI;

            UpdateLocationQuery();
        }

        private void UpdateLocationQuery()
        {
            if (WeatherAPI.Here.Equals(WeatherSource))
            {
                LocationQuery = String.Format(CultureInfo.InvariantCulture, "latitude={0:0.####}&longitude={1:0.####}", LocationLat, LocationLong);
            }
            else if (WeatherAPI.WeatherUnlocked.Equals(WeatherSource))
            {
                LocationQuery = String.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}", LocationLat, LocationLong);
            }
            else
            {
                LocationQuery = String.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", LocationLat, LocationLong);
            }
        }

        public void UpdateWeatherSource(string API)
        {
            WeatherSource = API;
            UpdateLocationQuery();
        }

        public override bool Equals(object obj)
        {
            var model = obj as LocationQueryViewModel;
            return model != null &&
                   LocationName == model.LocationName &&
                   LocationCountry == model.LocationCountry &&
                   LocationRegion == model.LocationRegion;
        }

        public override int GetHashCode()
        {
            var hashCode = 879653843;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocationName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocationCountry);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocationRegion);
            return hashCode;
        }
    }
}