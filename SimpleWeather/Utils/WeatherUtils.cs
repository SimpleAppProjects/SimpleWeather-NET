#if __ANDROID__
using Android.App;
#endif
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
            DateTime update_time = weather.update_time.DateTime;
            String timeformat = update_time.ToString("t", culture);

#if __ANDROID__
            if (Android.Text.Format.DateFormat.Is24HourFormat(Application.Context))
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
                prefix = Application.Context.GetString(Droid.Resource.String.update_prefix_day);
#endif
                date = string.Format("{0} {1}", prefix, timeformat);
            }
            else
            {
#if WINDOWS_UWP
                prefix = UWP.App.ResLoader.GetString("Update_Prefix");
#elif __ANDROID__
                prefix = Application.Context.GetString(Droid.Resource.String.update_prefix);
#endif
                date = string.Format("{0} {1} {2}",
                    prefix, update_time.ToString("ddd", culture), timeformat);
            }

            return date;
        }

        public static String GetFeelsLikeTemp(String temp_f, String wind_mph, String humidity_percent)
        {
            String feelslikeTemp = temp_f;

            if (float.TryParse(temp_f, out float temp))
            {
                if (temp < 50 && float.TryParse(wind_mph, out float windmph))
                {
                    feelslikeTemp = CalculateWindChill(temp, windmph).ToString();
                }
                else if (temp > 80 && int.TryParse(humidity_percent, out int humidity))
                {
                    feelslikeTemp = CalculateHeatIndex(temp, humidity).ToString();
                }
                else
                    feelslikeTemp = temp_f;
            }

            return feelslikeTemp;
        }

        public static float CalculateWindChill(float temp_f, float wind_mph)
        {
            if (temp_f < 50)
                return (float)(35.74f + (0.6215f * temp_f) - (35.75f * Math.Pow(wind_mph, 0.16f)) + (0.4275f * temp_f * Math.Pow(wind_mph, 0.16f)));
            else
                return temp_f;
        }

        public static double CalculateHeatIndex(float temp_f, int humidity)
        {
            if (temp_f > 80)
            {
                double HI = -42.379
                            + (2.04901523 * temp_f)
                            + (10.14333127 * humidity)
                            - (0.22475541 * temp_f * humidity)
                            - (0.00683783 * Math.Pow(temp_f, 2))
                            - (0.05481717 * Math.Pow(humidity, 2))
                            + (0.00122874 * Math.Pow(temp_f, 2) * humidity)
                            + (0.00085282 * temp_f * Math.Pow(humidity, 2))
                            - (0.00000199 * Math.Pow(temp_f, 2) * Math.Pow(humidity, 2));

                if (humidity < 13 && (temp_f > 80 && temp_f < 112))
                {
                    double adj = ((13 - humidity) / 4) * Math.Sqrt((17 - Math.Abs(temp_f - 95)) / 17);
                    HI -= adj;
                }
                else if (humidity > 85 && (temp_f > 80 && temp_f < 87))
                {
                    double adj = ((humidity - 85) / 10) * ((87 - temp_f) / 5);
                    HI += adj;
                }

                if (HI > 80 && HI > temp_f)
                    return HI;
                else
                    return temp_f;
            }
            else
                return temp_f;
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
                return string.Format(CultureInfo.InvariantCulture, "{0},{1}", lat, _long);
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
