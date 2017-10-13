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
            if (icon.Contains("nt_mostlycloudy") || icon.Contains("nt_partlysunny") || icon.Contains("nt_cloudy"))
                WeatherIcon = "\uf031";
            else if (icon.Contains("nt_partlycloudy") || icon.Contains("nt_mostlysunny"))
                WeatherIcon = "\uf083";
            else if (icon.Contains("nt_clear") || icon.Contains("nt_sunny")|| icon.Contains("nt_unknown"))
                WeatherIcon = "\uf02e";
            else if (icon.Contains("chancerain"))
                WeatherIcon = "\uf019";
            else if (icon.Contains("clear") || icon.Contains("sunny"))
                WeatherIcon = "\uf00d";
            else if (icon.Contains("cloudy"))
                WeatherIcon = "\uf002";
            else if (icon.Contains("flurries"))
                WeatherIcon = "\uf064";
            else if (icon.Contains("fog"))
                WeatherIcon = "\uf014";
            else if (icon.Contains("hazy"))
                WeatherIcon = "\uf021";
            else if (icon.Contains("sleet") || icon.Contains("sleat"))
                WeatherIcon = "\uf0b5";
            else if (icon.Contains("rain"))
                WeatherIcon = "\uf01a";
            else if (icon.Contains("snow"))
                WeatherIcon = "\uf01b";
            else if (icon.Contains("tstorms"))
                WeatherIcon = "\uf01e";
            else if (icon.Contains("unknown"))
                WeatherIcon = "\uf00d";
            else if (icon.Contains("nt_"))
                WeatherIcon = "\uf02e";

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

        public static String GetWeatherIconURI(string icon)
        {
#if WINDOWS_UWP
            string baseuri = "ms-appx:///Assets/WeatherIcons/png/";
#elif __ANDROID__
            string baseuri = "weathericons/png/";
#endif
            string fileIcon = string.Empty;

            if (int.TryParse(icon, out int code))
                fileIcon = string.Format("yahoo-{0}.png", code);
            else
            {
                if (icon.Contains("nt_mostlycloudy") || icon.Contains("nt_partlysunny") || icon.Contains("nt_cloudy"))
                    fileIcon = "yahoo-27.png";
                else if (icon.Contains("nt_partlycloudy") || icon.Contains("nt_mostlysunny"))
                    fileIcon = "yahoo-33.png";
                else if (icon.Contains("nt_clear") || icon.Contains("nt_sunny") || icon.Contains("nt_unknown"))
                    fileIcon = "yahoo-31.png";
                else if (icon.Contains("chancerain"))
                    fileIcon = "wu-chancerain.png";
                else if (icon.Contains("clear") || icon.Contains("sunny"))
                    fileIcon = "wu-clear.png";
                else if (icon.Contains("cloudy"))
                    fileIcon = "wu-cloudy.png";
                else if (icon.Contains("flurries"))
                    fileIcon = "wu-flurries.png";
                else if (icon.Contains("fog"))
                    fileIcon = "yahoo-20.png";
                else if (icon.Contains("hazy"))
                    fileIcon = "yahoo-21.png";
                else if (icon.Contains("sleet") || icon.Contains("sleat"))
                    fileIcon = "wu-sleat.png";
                else if (icon.Contains("rain"))
                    fileIcon = "wu-rain.png";
                else if (icon.Contains("snow"))
                    fileIcon = "wu-snow.png";
                else if (icon.Contains("tstorms"))
                    fileIcon = "wu-tstorms.png";
                else if (icon.Contains("unknown"))
                    fileIcon = "wu-unknown.png";
                else if (icon.Contains("nt_"))
                    fileIcon = "yahoo-31.png";
            }

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "na.png";
            }

            return baseuri + fileIcon;
        }

        public static bool IsNight(Weather weather)
        {
            bool isNight = false;

            if (weather.condition.icon.StartsWith("nt_"))
                isNight = true;
            else if (int.TryParse(weather.condition.icon, out int code))
            {
                switch (code)
                {
                    case 27: // Mostly Cloudy (Night)
                    case 29: // Partly Cloudy (Night)
                    case 31: // Clear (Night)
                    case 33: // Fair (Night)
                        isNight = true;
                        break;
                }
            }

            if (!isNight)
            {
                // Fallback to sunset/rise time just in case
                TimeSpan sunrise = weather.astronomy.sunrise.TimeOfDay;
                TimeSpan sunset = weather.astronomy.sunset.TimeOfDay;
                TimeSpan now = DateTimeOffset.UtcNow.ToOffset(weather.location.tz_offset).TimeOfDay;

                // Determine whether its night using sunset/rise times
                if (now < sunrise || now > sunset)
                    isNight = true;
            }

            return isNight;
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

#if WINDOWS_UWP
        public static Windows.UI.Color GetWeatherBackgroundColor(Weather weather)
        {
#elif __ANDROID__
        public static Android.Graphics.Color GetWeatherBackgroundColor(Weather weather)
        {
#endif
            byte[] rgb = null;
            String icon = weather.condition.icon;

            // Apply background based on weather condition
            /* WeatherUnderground */
            if (icon.Contains("mostly") || icon.Contains("partly") ||
                icon.Contains("cloudy"))
            {
                if (IsNight(weather))
                {
                    // Add night background plus cloudiness
                    rgb = new byte[3] { 16, 37, 67 };
                }
                else
                {
                    // add day bg + cloudiness
                    rgb = new byte[3] { 119, 148, 196 };
                }
            }
            else if (icon.Contains("rain") || icon.Contains("sleet") || icon.Contains("sleat") || 
                     icon.Contains("flurries") || icon.Contains("snow") || icon.Contains("tstorms"))
            {
                // lighter than night color + cloudiness
                rgb = new byte[3] { 53, 67, 116 };
            }
            else if (icon.Contains("hazy") || icon.Contains("fog"))
            {
                // add haziness
                rgb = new byte[3] { 143, 163, 196 };
            }
            else if (icon.Contains("clear") || icon.Contains("sunny"))
            {
                // Set background based using sunset/rise times
                if (IsNight(weather))
                {
                    // Night background
                    rgb = new byte[3] { 26, 36, 74 };
                }
                else
                {
                    // set day bg
                    rgb = new byte[3] { 72, 116, 191 };
                }
            }

            /* Yahoo Weather */
            if (rgb == null && int.TryParse(icon, out int code))
            {
                switch (code)
                {
                    // Rain 
                    case 9:
                    case 11:
                    case 12:
                    case 40:
                    // (Mixed) Rain/Snow/Sleet
                    case 5:
                    case 6:
                    case 7:
                    case 18:
                    // Hail / Freezing Rain
                    case 8:
                    case 10:
                    case 17:
                    case 35:
                    // Snow / Snow Showers/Storm
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 41:
                    case 42:
                    case 43:
                    case 46:
                    // Tornado / Hurricane / Thunderstorm / Tropical Storm
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 37:
                    case 38:
                    case 39:
                    case 45:
                    case 47:
                        // lighter than night color + cloudiness
                        rgb = new byte[3] { 53, 67, 116 };
                        break;
                    // Dust
                    case 19:
                    // Foggy / Haze
                    case 20:
                    case 21:
                    case 22:
                        // add haziness
                        rgb = new byte[3] { 143, 163, 196 };
                        break;
                    // Night
                    case 31:
                    case 33:
                        // Night background
                        rgb = new byte[3] { 26, 36, 74 };
                        break;
                    /* Ambigious weather conditions */
                    // (Mostly) Cloudy
                    case 28:
                    case 26:
                    case 27:
                    // Partly Cloudy
                    case 44:
                    case 29:
                    case 30:
                        if (IsNight(weather))
                        {
                            // Add night background plus cloudiness
                            rgb = new byte[3] { 16, 37, 67 };
                        }
                        else
                        {
                            // add day bg + cloudiness
                            rgb = new byte[3] { 119, 148, 196 };
                        }
                        break;
                    case 3200:
                    default:
                        // Set background based using sunset/rise times
                        if (IsNight(weather))
                        {
                            // Night background
                            rgb = new byte[3] { 26, 36, 74 };
                        }
                        else
                        {
                            // set day bg
                            rgb = new byte[3] { 72, 116, 191 };
                        }
                        break;
                }
            }

            // Just in case
            if (rgb == null)
            {
                // Set background based using sunset/rise times
                if (IsNight(weather))
                {
                    // Night background
                    rgb = new byte[3] { 26, 36, 74 };
                }
                else
                {
                    // set day bg
                    rgb = new byte[3] { 72, 116, 191 };
                }
            }

#if WINDOWS_UWP
            return Windows.UI.Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);
#elif __ANDROID__
            return new Android.Graphics.Color(rgb[0], rgb[1], rgb[2]);
#endif
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
