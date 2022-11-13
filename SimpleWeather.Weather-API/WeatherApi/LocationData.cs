using SimpleWeather.LocationData;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Weather_API.WeatherApi
{
    public static partial class WeatherApiLocationProviderExtensions
    {
        /* WeatherAPI AutoComplete Query */
        public static LocationQuery CreateLocationModel(this WeatherApiLocationProvider _, LocationItem result, string weatherAPI)
        {
            if (result == null)
                return null;

            var model = new LocationQuery();

            bool isUSorCA = LocationUtils.IsUSorCanada(result.country);

            string name = result.name;
            if (isUSorCA)
            {
                name = name.ReplaceFirst(string.Format(", {0}", result.country), "");
            }
            else
            {
                name = name.ReplaceFirst(string.Format(", {0}, {1}", result.region, result.country), string.Format(", {0}", result.country));
                model.LocationRegion = result.region;
            }

            model.LocationName = name;
            model.LocationCountry = result.country;
            model.Location_Query = result.id.ToInvariantString();

            model.LocationLat = result.lat;
            model.LocationLong = result.lon;

            model.LocationTZLong = null;

            model.LocationSource = WeatherAPI.WeatherApi;

            model.UpdateWeatherSource(weatherAPI);

            return model;
        }
    }
}