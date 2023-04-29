#if __IOS__
using CoreLocation;
using SimpleWeather.LocationData;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Weather_API.Maui
{
    public static partial class MauiLocationProviderExtensions
    {
        /* CLGeocoder AutoComplete Query */
        public static LocationQuery CreateLocationModel(this MauiLocationProvider _, CLPlacemark result, string weatherAPI)
        {
            if (result == null || result?.Location?.Coordinate.Latitude == null || result?.Location?.Coordinate.Longitude == null)
                return null;

            var model = new LocationQuery();

            string town, region;

            if (!string.IsNullOrWhiteSpace(result.SubLocality))
            {
                town = result.SubLocality;
            }
            else if (!string.IsNullOrWhiteSpace(result.Locality))
            {
                town = result.Locality;
            }
            else
            {
                town = result.SubAdministrativeArea;
            }

            if (!string.IsNullOrWhiteSpace(result.AdministrativeArea))
            {
                region = result.AdministrativeArea;
            }
            else
            {
                region = result.Country;
            }

            if (region != null && town != region)
            {
                if (town != null)
                {
                    model.LocationName = $"{town}, {region}";
                }
                else
                {
                    if (region != result.Country)
                    {
                        model.LocationName = $"{region}, {result.Country}";
                    }
                    else
                    {
                        model.LocationName = region;
                    }
                }
            }
            else
            {
                if (town != null)
                {
                    model.LocationName = $"{town}, {result.Country}";
                }
                else
                {
                    model.LocationName = result.Country;
                }
            }

            model.LocationRegion = result.SubAdministrativeArea;

            model.LocationLat = result.Location.Coordinate.Latitude;
            model.LocationLong = result.Location.Coordinate.Longitude;

            model.LocationCountry = result.IsoCountryCode;

            model.LocationTZLong = result.TimeZone?.Name;

            model.LocationSource = WeatherAPI.Apple;

            model.UpdateWeatherSource(weatherAPI);

            return model;
        }
    }
}
#endif