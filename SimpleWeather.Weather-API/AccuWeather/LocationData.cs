using SimpleWeather.LocationData;
using SimpleWeather.Weather_API.WeatherApi;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Weather_API.AccuWeather
{
    internal static partial class AccuWeatherExtensions
    {
        public static LocationQuery CreateLocationModel(this AccuWeatherLocationProvider _, GeopositionRootobject result)
        {
            return new LocationQuery()
            {
                LocationName = $"{result.LocalizedName}, {result.AdministrativeArea.LocalizedName}",
                LocationCountry = result.Country.ID,

                Location_Query = result.Key,

                LocationLat = (double)result.GeoPosition.Latitude,
                LocationLong = (double)result.GeoPosition.Longitude,

                LocationTZLong = result.TimeZone.Name,

                LocationSource = WeatherAPI.AccuWeather,
                WeatherSource = WeatherAPI.AccuWeather
            };
        }

        public static LocationQuery CreateLocationModel(this AccuWeatherLocationProvider _, GeopositionRootobject result, LocationQuery oldModel)
        {
            var newModel = oldModel.Clone();

            newModel.Location_Query = result.Key;
            newModel.LocationTZLong = oldModel.LocationTZLong ?? result.TimeZone?.Name;
            newModel.LocationCountry = oldModel.LocationCountry ?? result.Country?.ID;
            newModel.LocationSource = WeatherAPI.AccuWeather;
            newModel.WeatherSource = WeatherAPI.AccuWeather;

            return newModel;
        }
    }
}