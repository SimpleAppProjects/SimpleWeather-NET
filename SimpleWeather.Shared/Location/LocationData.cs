using SimpleWeather.Utils;

namespace SimpleWeather.Location
{
    public partial class LocationData
    {
        public static LocationData BuildEmptyGPSLocation()
        {
            string weatherSource = null;

            if (Settings.IsLoaded())
            {
                weatherSource = Settings.API;
            }

            return LocationQuery.BuildEmptyModel(weatherSource).ToLocationData(new Location());
        }

        public LocationData()
        {
            if (Settings.IsLoaded())
            {
                weatherSource = Settings.API;
            }
        }

        public LocationData(WeatherData.Weather weather)
        {
            query = weather.query;
            name = weather.location.name;
            latitude = weather.location.latitude.GetValueOrDefault();
            longitude = weather.location.longitude.GetValueOrDefault();
            tz_long = weather.location.tz_long;
            weatherSource = weather.source;
        }
    }
}