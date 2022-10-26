using SimpleWeather.WeatherData;

namespace SimpleWeather.Location
{
    public partial class LocationQuery
    {
        public LocationQuery(AccuWeather.GeopositionRootobject result, string weatherAPI)
        {
            SetLocation(result, weatherAPI);
        }

        public void SetLocation(AccuWeather.GeopositionRootobject result, string weatherAPI)
        {
            LocationName = $"{result.LocalizedName}, {result.AdministrativeArea.LocalizedName}";
            LocationCountry = result.Country.ID;

            Location_Query = result.Key;

            LocationLat = (double)result.GeoPosition.Latitude;
            LocationLong = (double)result.GeoPosition.Longitude;

            LocationTZLong = result.TimeZone.Name;

            LocationSource = WeatherAPI.AccuWeather;
            WeatherSource = WeatherAPI.AccuWeather;
        }

        public static LocationQuery CreateLocationModel(AccuWeather.GeopositionRootobject result, LocationQuery oldModel)
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