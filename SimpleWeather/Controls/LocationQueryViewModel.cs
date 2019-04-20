using System;
using SimpleWeather.WeatherUnderground;
using SimpleWeather.WeatherYahoo;
using SimpleWeather.OpenWeather;
using System.Collections.Generic;
using SimpleWeather.UWP;
using SimpleWeather.WeatherData;
using SimpleWeather.HERE;
using System.Globalization;
using SimpleWeather.Bing;
using System.Text;
using Windows.Services.Maps;

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

        public string LocationSource { get; set; }
        public string WeatherSource { get; set; }

        public LocationQueryViewModel()
        {
            LocationName = App.ResLoader.GetString("Error_NoResults");
            LocationCountry = string.Empty;
            LocationQuery = string.Empty;
        }

        #region WeatherUnderground
        public LocationQueryViewModel(AC_RESULT location, String WeatherAPI)
        {
            SetLocation(location, WeatherAPI);
        }

        public void SetLocation(AC_RESULT location, String weatherAPI)
        {
            LocationName = location.name;
            LocationCountry = location.c;

            if (WeatherAPI.WeatherUnderground.Equals(weatherAPI))
            {
                LocationQuery = location.l;
            }
            else if (WeatherAPI.Here.Equals(weatherAPI))
            {
                LocationQuery = String.Format("latitude={0}&longitude={1}", location.lat, location.lon);
            }
            else
            {
                LocationQuery = String.Format("lat={0}&lon={1}", location.lat, location.lon);
            }

            LocationLat = double.Parse(location.lat);
            LocationLong = double.Parse(location.lon);

            LocationTZ_Long = location.tz;

            LocationSource = WeatherAPI.WeatherUnderground;
            WeatherSource = weatherAPI;
        }

        public LocationQueryViewModel(location location, String weatherAPI)
        {
            SetLocation(location, weatherAPI);
        }

        public void SetLocation(location location, String weatherAPI)
        {
            LocationName = string.Format("{0}, {1}", location.city, location.state);
            LocationCountry = location.country;
            if (WeatherAPI.WeatherUnderground.Equals(weatherAPI))
            {
                LocationQuery = location.query;
            }
            else if (WeatherAPI.Here.Equals(weatherAPI))
            {
                LocationQuery = String.Format("latitude={0}&longitude={1}", location.lat, location.lon);
            }
            else
            {
                LocationQuery = String.Format("lat={0}&lon={1}", location.lat, location.lon);
            }

            LocationLat = double.Parse(location.lat);
            LocationLong = double.Parse(location.lon);

            LocationTZ_Long = location.tz_unix;

            LocationSource = WeatherAPI.WeatherUnderground;
            WeatherSource = weatherAPI;
        }
        #endregion

        public LocationQueryViewModel(Suggestion location, String weatherAPI)
        {
            SetLocation(location, weatherAPI);
        }

        public void SetLocation(Suggestion location, String weatherAPI)
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

            LocationSource = WeatherAPI.Here;
            WeatherSource = weatherAPI;
        }

        public LocationQueryViewModel(Result location, String weatherAPI)
        {
            SetLocation(location, weatherAPI);
        }

        public void SetLocation(Result location, String weatherAPI)
        {
            string country = null, town = null, region = null;

            if (location.location.address.additionalData != null)
            {
                foreach(Additionaldata item in location.location.address.additionalData)
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

            LocationLat = location.location.displayPosition.latitude;
            LocationLong = location.location.displayPosition.longitude;

            LocationTZ_Long = location.location.adminInfo.timeZone.id;

            LocationSource = WeatherAPI.Here;
            WeatherSource = weatherAPI;

            UpdateLocationQuery();
        }

        public LocationQueryViewModel(Bing.Address address, String weatherAPI)
        {
            SetLocation(address, weatherAPI);
        }

        public void SetLocation(Bing.Address address, String weatherAPI)
        {
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

            LocationTZ_Long = null;

            LocationSource = WeatherAPI.BingMaps;
            WeatherSource = weatherAPI;
        }

        public LocationQueryViewModel(MapLocation result, String weatherAPI)
        {
            SetLocation(result, weatherAPI);
        }

        private void SetLocation(MapLocation result, String weatherAPI)
        {
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

            LocationTZ_Long = null;

            LocationSource = WeatherAPI.BingMaps;
            WeatherSource = weatherAPI;

            UpdateLocationQuery();
        }

        private void UpdateLocationQuery()
        {
            if (WeatherAPI.WeatherUnderground.Equals(WeatherSource))
            {
                LocationQuery = String.Format("/q/{0},{1}", LocationLat.ToString(CultureInfo.InvariantCulture), LocationLong.ToString(CultureInfo.InvariantCulture));
            }
            else if (WeatherAPI.Here.Equals(WeatherSource))
            {
                LocationQuery = String.Format("latitude={0}&longitude={1}", LocationLat.ToString(CultureInfo.InvariantCulture), LocationLong.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                LocationQuery = String.Format("lat={0}&lon={1}", LocationLat.ToString(CultureInfo.InvariantCulture), LocationLong.ToString(CultureInfo.InvariantCulture));
            }
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
