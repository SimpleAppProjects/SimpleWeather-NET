using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
        public static String GetWeatherIcon(string icon)
        {
            string WeatherIcon = string.Empty;

            /* WeatherUnderground */
            if (icon.Contains("nt_clear") || icon.Contains("nt_mostlysunny") 
                || icon.Contains("nt_partlysunny") || icon.Contains("nt_sunny"))
                WeatherIcon = "\uf02e";
            else if (icon.Contains("nt_mostlycloudy") || icon.Contains("nt_partlycloudy")
                || icon.Contains("nt_cloudy"))
                WeatherIcon = "\uf031";
            else if (icon.Contains("mostlysunny") || icon.Contains("partlysunny"))
                WeatherIcon = "\uf00d";
            else if (icon.Contains("mostlycloudy") || icon.Contains("partlycloudy"))
                WeatherIcon = "\uf002";
            else if (icon.Contains("flurries"))
                WeatherIcon = "\uf064";
            else if (icon.Contains("hazy"))
                WeatherIcon = "\uf0b6";
            else if (icon.Contains("fog"))
                WeatherIcon = "\uf014";
            else if (icon.Contains("rain"))
                WeatherIcon = "\uf01a";
            else if (icon.Contains("sleet"))
                WeatherIcon = "\uf0b5";
            else if (icon.Contains("snow"))
                WeatherIcon = "\uf01b";
            else if (icon.Contains("tstorms"))
                WeatherIcon = "\uf01e";
            else if (icon.Contains("cloudy"))
                WeatherIcon = "\uf002";
            else if (icon.Contains("clear") || icon.Contains("sunny"))
                WeatherIcon = "\uf00d";

            /* Yahoo Weather */
            if (String.IsNullOrWhiteSpace(WeatherIcon) && int.TryParse(icon, out int code))
            {
                switch (code)
                {
                    case 0: // Tornado
                        WeatherIcon = "\uf056";
                        break;
                    case 1: // Tropical Storm
                    case 37:
                    case 38: // Scattered Thunderstorms/showers
                    case 39:
                    case 45:
                    case 47:
                        WeatherIcon = "\uf00e";
                        break;
                    case 2: // Hurricane
                        WeatherIcon = "\uf073";
                        break;
                    case 3:
                    case 4: // Scattered Thunderstorms
                        WeatherIcon = "\uf01e";
                        break;
                    case 5: // Mixed Rain/Snow
                    case 6: // Mixed Rain/Sleet
                    case 7: // Mixed Snow/Sleet
                    case 18: // Sleet
                    case 35: // Mixed Rain/Hail
                        WeatherIcon = "\uf017";
                        break;
                    case 8: // Freezing Drizzle
                    case 10: // Freezing Rain
                    case 17: // Hail
                        WeatherIcon = "\uf015";
                        break;
                    case 9: // Drizzle
                    case 11: // Showers
                    case 12:
                    case 40: // Scattered Showers
                        WeatherIcon = "\uf01a";
                        break;
                    case 13: // Snow Flurries
                    case 14: // Light Snow Showers
                    case 16: // Snow
                    case 42: // Scattered Snow Showers
                    case 46: // Snow Showers
                        WeatherIcon = "\uf01b";
                        break;
                    case 15: // Blowing Snow
                    case 41: // Heavy Snow
                    case 43:
                        WeatherIcon = "\uf064";
                        break;
                    case 19: // Dust
                        WeatherIcon = "\uf063";
                        break;
                    case 20: // Foggy
                        WeatherIcon = "\uf014";
                        break;
                    case 21: // Haze
                        WeatherIcon = "\uf021";
                        break;
                    case 22: // Smoky
                        WeatherIcon = "\uf062";
                        break;
                    case 23: // Blustery
                    case 24: // Windy
                        WeatherIcon = "\uf050";
                        break;
                    case 25: // Cold
                        WeatherIcon = "\uf076";
                        break;
                    case 26: // Cloudy
                        WeatherIcon = "\uf013";
                        break;
                    case 27: // Mostly Cloudy (Night)
                    case 29: // Partly Cloudy (Night)
                        WeatherIcon = "\uf031";
                        break;
                    case 28: // Mostly Cloudy (Day)
                    case 30: // Partly Cloudy (Day)
                        WeatherIcon = "\uf002";
                        break;
                    case 31: // Clear (Night)
                        WeatherIcon = "\uf02e";
                        break;
                    case 32: // Sunny
                        WeatherIcon = "\uf00d";
                        break;
                    case 33: // Fair (Night)
                        WeatherIcon = "\uf083";
                        break;
                    case 34: // Fair (Day)
                    case 44: // Partly Cloudy
                        WeatherIcon = "\uf00c";
                        break;
                    case 36: // HOT
                        WeatherIcon = "\uf072";
                        break;
                    case 3200: // Not Available
                    default:
                        WeatherIcon = "\uf07b";
                        break;
                }
            }

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                WeatherIcon = "\uf07b";
            }

            return WeatherIcon;
        }

        public static bool IsNight(Weather weather)
        {
            TimeSpan sunrise = weather.astronomy.sunrise.TimeOfDay;
            TimeSpan sunset = weather.astronomy.sunset.TimeOfDay;
            TimeSpan now = DateTimeOffset.UtcNow.ToOffset(weather.location.tz_offset).TimeOfDay;

            // Determine whether its night using sunset/rise times
            if (now < sunrise || now > sunset)
                return true;
            else
                return false;
        }

        public static String GetLastBuildDate(Weather weather)
        {
            String date;

            if (weather.update_time.DayOfWeek == DateTime.Today.DayOfWeek)
            {
                date = string.Format("Updated at {0}", weather.update_time.ToString("t"));
            }
            else
                date = string.Format("Updated on {0} {1}", weather.update_time.ToString("ddd"), weather.update_time.ToString("t"));

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
            UNKNOWN = -1,
            SUCCESS,
            NOWEATHER,
            NETWORKERROR,
            INVALIDAPIKEY,
            QUERYNOTFOUND,
        }
    }
}
