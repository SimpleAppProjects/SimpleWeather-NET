using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
        public static String GetLastBuildDate(Weather weather)
        {
#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
            var culture = new System.Globalization.CultureInfo(userlang);
#else
            var culture = System.Globalization.CultureInfo.CurrentCulture;
#endif

            String date;
            String prefix;
            DateTime update_time = weather.update_time.DateTime.Add(weather.location.tz_offset);
            String timeformat = update_time.ToString("t", culture);

#if __ANDROID__
            if (Android.Text.Format.DateFormat.Is24HourFormat(Droid.App.Context))
                timeformat = update_time.ToString("HH:mm");
            else
                timeformat = update_time.ToString("h:mm tt");
#endif

            timeformat = string.Format("{0} {1}", timeformat, weather.location.tz_short);

            if (update_time.DayOfWeek == DateTime.Now.DayOfWeek)
            {
#if WINDOWS_UWP
                prefix = UWP.App.ResLoader.GetString("Update_PrefixDay");
#elif __ANDROID__
                prefix = Droid.App.Context.GetString(Droid.Resource.String.update_prefix_day);
#endif
                date = string.Format("{0} {1}", prefix, timeformat);
            }
            else
            {
#if WINDOWS_UWP
                prefix = UWP.App.ResLoader.GetString("Update_Prefix");
#elif __ANDROID__
                prefix = Droid.App.Context.GetString(Droid.Resource.String.update_prefix);
#endif
                date = string.Format("{0} {1} {2}",
                    prefix, update_time.ToString("ddd", culture), timeformat);
            }

            return date;
        }

        public class Coordinate
        {
            public double Latitude { get => lat; set { SetLat(value); } }
            public double Longitude { get => _long; set { SetLong(value); } }

            private double lat = 0;
            private double _long = 0;

            private void SetLat(double value) { lat = value; }

            private void SetLong(double value) { _long = value; }

            public Coordinate(string coordinatePair)
            {
                SetCoordinate(coordinatePair);
            }

            public Coordinate(double latitude, double longitude)
            {
                lat = latitude;
                _long = longitude;
            }

#if WINDOWS_UWP
            public Coordinate(Windows.Devices.Geolocation.Geoposition geoPos)
            {
                lat = geoPos.Coordinate.Point.Position.Latitude;
                _long = geoPos.Coordinate.Point.Position.Longitude;
            }
#endif

#if __ANDROID__
            public Coordinate(Android.Locations.Location location)
            {
                lat = location.Latitude;
                _long = location.Longitude;
            }
#endif

            public void SetCoordinate(string coordinatePair)
            {
                string[] coord = coordinatePair.Split(',');
                lat = double.Parse(coord[0]);
                _long = double.Parse(coord[1]);
            }

            public override string ToString()
            {
                return "(" + lat + ", " + _long + ")";
            }
        }

        public enum ErrorStatus
        {
            Unknown = -1,
            Success,
            NoWeather,
            NetworkError,
            InvalidAPIKey,
            QueryNotFound,
        }
    }
}
