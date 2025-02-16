using SimpleWeather.LocationData;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Weather_API.Radar
{
    public static partial class RadarLocationProviderExtensions
    {
        /* AutoComplete Query */
        public static LocationQuery CreateLocationModel(this RadarLocationProvider _, Address address, string weatherAPI)
        {
            if (address == null)
                return null;

            var model = new LocationQuery
            {
                LocationName = address.formattedAddress,
                LocationCountry = address.countryCode,

                LocationRegion = address.county ?? address.city ?? address.state,

                LocationLat = address.latitude,
                LocationLong = address.longitude,

                LocationTZLong = null,

                LocationSource = WeatherAPI.Radar
            };

            if (address.countryCode != null && model.LocationName.EndsWith(address.countryCode))
            {
                model.LocationName = model.LocationName.RemoveSuffix(address.countryCode).TrimEnd();
            }

            model.UpdateWeatherSource(weatherAPI);

            return model;
        }

        /* Reverse Geocode Query */
        public static LocationQuery CreateLocationModel(this RadarLocationProvider _, GeocodeAddress address, string weatherAPI)
        {
            if (address == null)
                return null;

            var model = new LocationQuery
            {
                LocationName = address.formattedAddress,
                LocationCountry = address.countryCode,
                LocationRegion = address.county ?? address.city ?? address.state,

                LocationLat = address.latitude,
                LocationLong = address.longitude,

                LocationTZLong = address?.timeZone?.id,

                LocationSource = WeatherAPI.Radar
            };

            if (address.countryCode != null && model.LocationName.EndsWith(address.countryCode))
            {
                model.LocationName = model.LocationName.RemoveSuffix(address.countryCode).TrimEnd();
            }

            model.UpdateWeatherSource(weatherAPI);

            return model;
        }
    }
}