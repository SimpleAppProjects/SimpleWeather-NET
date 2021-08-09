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
        public LocationQueryViewModel(AccuWeather.GeopositionRootobject result, string weatherAPI)
        {
            SetLocation(result, weatherAPI);
        }

        public void SetLocation(AccuWeather.GeopositionRootobject result, string weatherAPI)
        {
            LocationName = $"{result.LocalizedName}, {result.AdministrativeArea.LocalizedName}";
            LocationCountry = result.Country.ID;

            LocationQuery = result.Key;

            LocationLat = (double)result.GeoPosition.Latitude;
            LocationLong = (double)result.GeoPosition.Longitude;

            LocationTZLong = result.TimeZone.Name;

            LocationSource = WeatherAPI.AccuWeather;
            WeatherSource = WeatherAPI.AccuWeather;
        }

        public static LocationQueryViewModel CreateLocationModel(AccuWeather.GeopositionRootobject result, LocationQueryViewModel oldModel)
        {
            var newModel = oldModel.Clone();

            newModel.LocationQuery = result.Key;
            newModel.LocationTZLong = oldModel.LocationTZLong ?? result.TimeZone?.Name;
            newModel.LocationCountry = oldModel.LocationCountry ?? result.Country?.ID;
            newModel.LocationSource = WeatherAPI.AccuWeather;
            newModel.WeatherSource = WeatherAPI.AccuWeather;

            return newModel;
        }
    }
}