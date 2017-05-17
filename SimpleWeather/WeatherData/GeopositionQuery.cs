using SimpleWeather.Utils;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace SimpleWeather.WeatherData
{
    public static class GeopositionQuery
    {
        public static async Task<Controls.LocationQueryView> getLocation(Geoposition geoPos)
        {
            Controls.LocationQueryView location = null;

            if (Settings.API == "WUnderground")
            {
                WeatherUnderground.location geoLocation = await WeatherUnderground.GeopositionQuery.getLocation(geoPos);

                if (geoLocation != null && !String.IsNullOrWhiteSpace(geoLocation.query))
                    location = new Controls.LocationQueryView(geoLocation);
                else
                    location = new Controls.LocationQueryView();
            }
            else
            {
                WeatherYahoo.place geoLocation = await WeatherYahoo.GeopositionQuery.getLocation(geoPos);

                if (geoLocation != null && !String.IsNullOrWhiteSpace(geoLocation.woeid))
                    location = new Controls.LocationQueryView(geoLocation);
                else
                    location = new Controls.LocationQueryView();
            }

            return location;
        }
    }
}