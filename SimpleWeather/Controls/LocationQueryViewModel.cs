using System;
using SimpleWeather.WeatherUnderground;
using SimpleWeather.WeatherYahoo;
using SimpleWeather.OpenWeather;
#if WINDOWS_UWP
using SimpleWeather.UWP;
#elif __ANDROID__
using SimpleWeather.Droid;
#endif

namespace SimpleWeather.Controls
{
    public class LocationQueryViewModel
    {
        public string LocationName { get; set; }
        public string LocationCountry { get; set; }
        public string LocationQuery { get; set; }

        public TimeSpan LocationTZ_Offset { get; set; }
        public string LocationTZ_Short { get; set; }

        public LocationQueryViewModel()
        {
#if WINDOWS_UWP
            LocationName = App.ResLoader.GetString("Error_NoResults");
#elif __ANDROID__
            LocationName = App.Context.GetString(Resource.String.error_noresults);
#else
            LocationName = "No results found";
#endif
            LocationCountry = string.Empty;
            LocationQuery = string.Empty;
        }

#region WeatherUnderground
        public LocationQueryViewModel(WeatherUnderground.AC_RESULT location)
        {
            SetLocation(location);
        }

        public void SetLocation(WeatherUnderground.AC_RESULT location)
        {
            LocationName = location.name;
            LocationCountry = location.c;
            LocationQuery = location.l;
        }

        public LocationQueryViewModel(WeatherUnderground.location location)
        {
            SetLocation(location);
        }

        public void SetLocation(WeatherUnderground.location location)
        {
            LocationName = string.Format("{0}, {1}", location.city, location.state);
            LocationCountry = location.country;
            LocationQuery = location.query;
        }
        #endregion

        #region Yahoo Weather
        public LocationQueryViewModel(place location)
        {
            SetLocation(location);
        }

        public void SetLocation(place location)
        {
            string town, region;

            // If location type is ZipCode append it to location name
            if ((location.placeTypeName.Value == "Zip Code"
                || location.placeTypeName.Value == "Postal Code"))
            {
                town = location.name;

                if (location.locality1 != null
                    && !String.IsNullOrEmpty(location.locality1.Value))
                    town += " - " + location.locality1.Value;
            }
            else
            {
                if (location.locality1 != null
                    && !String.IsNullOrEmpty(location.locality1.Value))
                    town = location.locality1.Value;
                else
                    town = location.name;
            }

            // Try to get region name or fallback to country name
            if (location.admin1 != null
                && !String.IsNullOrEmpty(location.admin1.Value))
                region = location.admin1.Value;
            else if (location.admin2 != null
                && !String.IsNullOrEmpty(location.admin2.Value))
                region = location.admin2.Value;
            else
                region = location.country.Value;

            LocationName = string.Format("{0}, {1}", town, region);
            LocationCountry = location.country.code;
            LocationQuery = location.woeid;
        }
        #endregion

        #region OpenWeatherMap
        public LocationQueryViewModel(OpenWeather.AC_RESULT location)
        {
            SetLocation(location);
        }

        public void SetLocation(OpenWeather.AC_RESULT location)
        {
            LocationName = location.name;
            LocationCountry = location.c;
            LocationQuery = string.Format("lat={0}&lon={1}", location.lat, location.lon);

            var tz = location.tz;
            var nodaTZ = NodaTime.DateTimeZoneProviders.Tzdb[tz];
            LocationTZ_Offset = nodaTZ.GetUtcOffset(NodaTime.SystemClock.Instance.GetCurrentInstant()).ToTimeSpan();
            LocationTZ_Short = location.tzs;
        }

        public LocationQueryViewModel(OpenWeather.location location)
        {
            SetLocation(location);
        }

        public void SetLocation(OpenWeather.location location)
        {
            LocationName = string.Format("{0}, {1}", location.city, location.state);
            LocationCountry = location.country;
            LocationQuery = string.Format("lat={0}&lon={1}", location.lat, location.lon);

            var tz = location.tz_unix;
            var nodaTZ = NodaTime.DateTimeZoneProviders.Tzdb[tz];
            LocationTZ_Offset = nodaTZ.GetUtcOffset(NodaTime.SystemClock.Instance.GetCurrentInstant()).ToTimeSpan();
            LocationTZ_Short = location.tz_short;
        }
        #endregion
    }
}
