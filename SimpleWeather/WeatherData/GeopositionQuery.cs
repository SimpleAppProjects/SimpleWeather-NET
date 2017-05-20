using SimpleWeather.Utils;
using System;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public static class GeopositionQuery
    {
#if WINDOWS_UWP
        public static async Task<Controls.LocationQueryView> getLocation(Windows.Devices.Geolocation.Geoposition geoPos)
#elif __ANDROID__
        public static async Task<Controls.LocationQueryView> getLocation(Android.Locations.Location geoPos)
#endif
        {
            Controls.LocationQueryView location = null;

            if (Settings.API == "WUnderground")
            {
                WeatherUnderground.location geoLocation = 
                    await WeatherUnderground.GeopositionQuery.getLocation(new WeatherUtils.Coordinate(geoPos));

                if (geoLocation != null && !String.IsNullOrWhiteSpace(geoLocation.query))
                    location = new Controls.LocationQueryView(geoLocation);
                else
                    location = new Controls.LocationQueryView();
            }
            else
            {
                WeatherYahoo.place geoLocation = await WeatherYahoo.GeopositionQuery.getLocation(new WeatherUtils.Coordinate(geoPos));

                if (geoLocation != null && !String.IsNullOrWhiteSpace(geoLocation.woeid))
                    location = new Controls.LocationQueryView(geoLocation);
                else
                    location = new Controls.LocationQueryView();
            }

            return location;
        }
    }
}