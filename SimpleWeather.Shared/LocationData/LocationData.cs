using SimpleWeather.Preferences;
using SimpleWeather.WeatherData;

namespace SimpleWeather.LocationData
{
    public partial class LocationData
    {
        public static LocationData BuildEmptyGPSLocation()
        {
            string weatherSource = null;

            if (SettingsManager.IsLoaded)
            {
                var SettingsManager = DI.Utils.SettingsManager;
                weatherSource = SettingsManager.API;
            }

            return LocationQuery.BuildEmptyModel(weatherSource).ToLocationData(new Location());
        }

        public LocationData()
        {
            if (SettingsManager.IsLoaded)
            {
                var SettingsManager = DI.Utils.SettingsManager;
                weatherSource = SettingsManager.API;
            }
        }

        public LocationData(Weather weather)
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