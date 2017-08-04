using SimpleWeather.Utils;
using System;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public static class GeopositionQuery
    {
#if WINDOWS_UWP
        public static async Task<Controls.LocationQueryViewModel> GetLocation(Windows.Devices.Geolocation.Geoposition geoPos)
#elif __ANDROID__
        public static async Task<Controls.LocationQueryViewModel> GetLocation(Android.Locations.Location geoPos)
#endif
        {
            Controls.LocationQueryViewModel location = null;

            if (Settings.API == Settings.API_WUnderground)
            {
                // Create model from geo-location query
                WeatherUnderground.location geoLocation = 
                    await WeatherUnderground.GeopositionQuery.GetLocation(new WeatherUtils.Coordinate(geoPos));

                if (geoLocation != null && !String.IsNullOrWhiteSpace(geoLocation.query))
                    location = new Controls.LocationQueryViewModel(geoLocation);
                else
                    location = new Controls.LocationQueryViewModel();
            }
            else
            {
                // Create model from geo-location query
                WeatherYahoo.place geoLocation = await WeatherYahoo.GeopositionQuery.GetLocation(new WeatherUtils.Coordinate(geoPos));

                if (geoLocation != null && !String.IsNullOrWhiteSpace(geoLocation.woeid))
                    location = new Controls.LocationQueryViewModel(geoLocation);
                else
                    location = new Controls.LocationQueryViewModel();
            }

            return location;
        }

        public static async Task<Controls.LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coordinate)
        {
            Controls.LocationQueryViewModel location = null;

            if (Settings.API == Settings.API_WUnderground)
            {
                // Create model from geo-location query
                WeatherUnderground.location geoLocation =
                    await WeatherUnderground.GeopositionQuery.GetLocation(coordinate);

                if (geoLocation != null && !String.IsNullOrWhiteSpace(geoLocation.query))
                    location = new Controls.LocationQueryViewModel(geoLocation);
                else
                    location = new Controls.LocationQueryViewModel();
            }
            else
            {
                // Create model from geo-location query
                WeatherYahoo.place geoLocation = await WeatherYahoo.GeopositionQuery.GetLocation(coordinate);

                if (geoLocation != null && !String.IsNullOrWhiteSpace(geoLocation.woeid))
                    location = new Controls.LocationQueryViewModel(geoLocation);
                else
                    location = new Controls.LocationQueryViewModel();
            }

            return location;
        }
    }
}