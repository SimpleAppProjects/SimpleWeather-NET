using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SimpleWeather.Controls
{
    public partial class LocationQueryViewModel
    {
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
    }
}