using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SimpleWeather.Controls
{
    public partial class LocationQueryViewModel
    {
        public string LocationName { get; set; }
        public string LocationRegion { get; set; }
        public string LocationCountry { get; set; }
        public string LocationQuery { get; set; }

        public double LocationLat { get; set; }
        public double LocationLong { get; set; }

        public string LocationTZLong { get; set; }

        public string LocationSource { get; set; }
        public string WeatherSource { get; set; }

        public string LocationRegionText
        {
            get
            {
                if (String.IsNullOrWhiteSpace(LocationRegion))
                {
                    return LocationCountry;
                }
                else
                {
                    return LocationRegion + ", " + LocationCountry;
                }
            }
        }

        public LocationQueryViewModel()
        {
            LocationName = SimpleLibrary.GetInstance().ResLoader.GetString("Error_NoResults");
            LocationCountry = string.Empty;
            LocationQuery = string.Empty;
        }

        public LocationQueryViewModel(Location.LocationData data)
        {
            LocationQuery = data.query;
            LocationName = data.name;
            LocationLat = data.latitude;
            LocationLong = data.longitude;
            LocationTZLong = data.tz_long;
            WeatherSource = data.weatherSource;
            LocationSource = data.locationSource;
            LocationCountry = data.country_code;
        }

        public bool IsEmpty => String.IsNullOrEmpty(LocationCountry) && String.IsNullOrEmpty(LocationQuery);

        private void UpdateLocationQuery()
        {
            if (WeatherAPI.Here.Equals(WeatherSource))
            {
                LocationQuery = String.Format(CultureInfo.InvariantCulture, "latitude={0:0.####}&longitude={1:0.####}", LocationLat, LocationLong);
            }
            else if (WeatherAPI.WeatherUnlocked.Equals(WeatherSource) || WeatherAPI.WeatherApi.Equals(WeatherSource))
            {
                LocationQuery = String.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}", LocationLat, LocationLong);
            }
            else
            {
                LocationQuery = String.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", LocationLat, LocationLong);
            }
        }

        public void UpdateWeatherSource(string API)
        {
            WeatherSource = API;
            UpdateLocationQuery();
        }

        public override bool Equals(object obj)
        {
            var model = obj as LocationQueryViewModel;
            return model != null &&
                   LocationName == model.LocationName &&
                   LocationCountry == model.LocationCountry &&
                   LocationRegion == model.LocationRegion;
        }

        public override int GetHashCode()
        {
            var hashCode = 879653843;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocationName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocationCountry);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocationRegion);
            return hashCode;
        }
    }
}