using SimpleWeather.Utils;
using System;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public static class GeopositionQuery
    {
#if WINDOWS_UWP
        public static async Task<Controls.LocationQueryViewModel> getLocation(Windows.Devices.Geolocation.Geoposition geoPos)
#elif __ANDROID__
        public static async Task<Controls.LocationQueryViewModel> getLocation(Android.Locations.Location geoPos)
#endif
        {
            Controls.LocationQueryViewModel location = null;

            if (Settings.API == "WUnderground")
            {
                WeatherUnderground.location geoLocation = 
                    await WeatherUnderground.GeopositionQuery.getLocation(new WeatherUtils.Coordinate(geoPos));

                if (geoLocation != null && !String.IsNullOrWhiteSpace(geoLocation.query))
                    location = new Controls.LocationQueryViewModel(geoLocation);
                else
                    location = new Controls.LocationQueryViewModel();
            }
            else
            {
                WeatherYahoo.place geoLocation = await WeatherYahoo.GeopositionQuery.getLocation(new WeatherUtils.Coordinate(geoPos));

                if (geoLocation != null && !String.IsNullOrWhiteSpace(geoLocation.woeid))
                    location = new Controls.LocationQueryViewModel(geoLocation);
                else
                    location = new Controls.LocationQueryViewModel();
            }

            return location;
        }
    }
}