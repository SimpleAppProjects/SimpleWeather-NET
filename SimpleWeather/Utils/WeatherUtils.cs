using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace SimpleWeather.Utils
{
    public static class WeatherUtils
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
            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                switch (int.Parse(icon))
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
                        WeatherIcon = "\uf077";
                        break;
                }
            }

            return WeatherIcon;
        }

        public static void SetBackground(ImageBrush bg, Weather weather)
        {
            Uri imgURI = null;

            // Apply background based on weather condition
            /* WeatherUnderground */
            switch (weather.condition.icon)
            {
                case "cloudy":
                case "mostlycloudy":
                    if (isNight(weather))
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/MostlyCloudy-Night.jpg");
                    else
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/MostlyCloudy-Day.jpg");
                    break;
                case "mostlysunny":
                case "partlysunny":
                case "partlycloudy":
                    if (isNight(weather))
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/PartlyCloudy-Night.jpg");
                    else
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/PartlyCloudy-Day.jpg");
                    break;
                case "chancerain":
                case "chancesleat":
                case "rain":
                case "sleat":
                    imgURI = new Uri("ms-appx:///Assets/Backgrounds/RainySky.jpg");
                    break;
                case "chanceflurries":
                case "chancesnow":
                case "flurries":
                case "snow":
                    imgURI = new Uri("ms-appx:///Assets/Backgrounds/Snow.jpg");
                    break;
                case "chancetstorms":
                case "tstorms":
                    imgURI = new Uri("ms-appx:///Assets/Backgrounds/StormySky.jpg");
                    break;
                case "hazy":
                    imgURI = new Uri("ms-appx:///Assets/Backgrounds/FoggySky.jpg");
                    break;
                case "sunny":
                case "clear":
                case "unknown":
                    // Set background based using sunset/rise times
                    if (isNight(weather))
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/NightSky.jpg");
                    else
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/DaySky.jpg");
                    break;
            }

            /* Yahoo Weather */
            if (imgURI == null)
            {
                switch (int.Parse(weather.condition.icon))
                {
                    // Night
                    case 31:
                    case 33:
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/NightSky.jpg");
                        break;
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
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/RainySky.jpg");
                        break;
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
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/StormySky.jpg");
                        break;
                    // Dust
                    case 19:
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/Dust.jpg");
                        break;
                    // Foggy / Haze
                    case 20:
                    case 21:
                    case 22:
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/FoggySky.jpg");
                        break;
                    // Snow / Snow Showers/Storm
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 41:
                    case 42:
                    case 43:
                    case 46:
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/Snow.jpg");
                        break;
                    /* Ambigious weather conditions */
                    // (Mostly) Cloudy
                    case 28:
                    case 26:
                    case 27:
                        if (isNight(weather))
                            imgURI = new Uri("ms-appx:///Assets/Backgrounds/MostlyCloudy-Night.jpg");
                        else
                            imgURI = new Uri("ms-appx:///Assets/Backgrounds/MostlyCloudy-Day.jpg");
                        break;
                    // Partly Cloudy
                    case 44:
                    case 29:
                    case 30:
                        if (isNight(weather))
                            imgURI = new Uri("ms-appx:///Assets/Backgrounds/PartlyCloudy-Night.jpg");
                        else
                            imgURI = new Uri("ms-appx:///Assets/Backgrounds/PartlyCloudy-Day.jpg");
                        break;
                    default:
                        // Set background based using sunset/rise times
                        if (isNight(weather))
                            imgURI = new Uri("ms-appx:///Assets/Backgrounds/NightSky.jpg");
                        else
                            imgURI = new Uri("ms-appx:///Assets/Backgrounds/DaySky.jpg");
                        break;
                }
            }

            if (bg != null && bg.ImageSource != null)
            {
                BitmapImage bmp = bg.ImageSource as BitmapImage;

                if (bmp != null && bmp.UriSource == imgURI)
                {
                    System.Diagnostics.Debug.WriteLine("Skipping...");
                    return;
                }
            }

            BitmapImage img = new BitmapImage(imgURI);
            img.CreateOptions = BitmapCreateOptions.None;
            img.DecodePixelWidth = 960;
            bg.ImageSource = img;
        }

        public static bool isNight(Weather weather)
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
                date = string.Format("Updated on {0}", weather.update_time.ToString("ddd t"));

            return date;
        }

        [DataContract]
        public class Coordinate
        {
            public double Latitude { get => lat; set { setLat(value); } }
            public double Longitude { get => _long; set { setLong(value); } }

            [DataMember]
            private double lat = 0;
            [DataMember]
            private double _long = 0;

            private void setLat(double value) { lat = value; }

            private void setLong(double value) { _long = value; }

            public Coordinate(string coordinatePair)
            {
                setCoordinate(coordinatePair);
            }

            public Coordinate(double latitude, double longitude)
            {
                lat = latitude;
                _long = longitude;
            }

            public void setCoordinate(string coordinatePair)
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
