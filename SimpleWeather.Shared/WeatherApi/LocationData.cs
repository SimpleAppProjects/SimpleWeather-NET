using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;

namespace SimpleWeather.Location
{
    public partial class LocationQuery
    {
        /* WeatherAPI AutoComplete Query */
        public LocationQuery(WeatherApi.LocationItem result, String weatherAPI)
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
            Location_Query = result.id.ToInvariantString();

            LocationLat = result.lat;
            LocationLong = result.lon;

            LocationTZLong = null;

            LocationSource = WeatherAPI.WeatherApi;
            WeatherSource = weatherAPI;

            UpdateLocationQuery();
        }
    }
}