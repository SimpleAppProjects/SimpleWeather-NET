using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace SimpleWeather
{
    public class WeatherNowView
    {
        public string Location { get; set; }
        public string UpdateDate { get; set; }

        // Current Condition
        public string CurTemp { get; set; }
        public string CurCondition { get; set; }
        public string WeatherIcon { get; set; }

        // Weather Details
        public string Humidity { get; set; }
        public string Pressure { get; set; }
        public string PressureUnit { get; set; }
        public string Visibility { get; set; }
        public string VisibilityUnit { get; set; }
        public string Chill { get; set; }
        public string Speed { get; set; }
        public string SpeedUnit { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }

        // Forecast
        public ObservableCollection<ForecastItemView> Forecasts { get; set; }

        // Background
        public ImageBrush Background { get; set; }

        public WeatherNowView()
        {
        }

        public WeatherNowView(Weather weather)
        {
            updateView(weather);
        }

        private void updateView(Weather weather)
        {
            // Update background
            updateBg(weather);

            // Location
            Location = weather.location.description;

            // Date Updated
            UpdateDate = updateLastBuildDate(weather);

            // Update Current Condition
            CurTemp = weather.condition.temp +
                (weather.units.temperature == "F" ? "\uf045" : "\uf03c");
            CurCondition = weather.condition.text;
            updateWeatherIcon(int.Parse(weather.condition.code));

            // WeatherDetails
            // Astronomy
            Sunrise = weather.astronomy.sunrise;
            Sunset = weather.astronomy.sunset;
            // Wind
            Chill = (weather.units.temperature == "F" ? weather.wind.chill : ConversionMethods.FtoC(weather.wind.chill)) + "º";
            Speed = weather.wind.speed;
            SpeedUnit = weather.units.speed;

            // Atmosphere
            Humidity = weather.atmosphere.humidity;
            Pressure = (weather.units.temperature == "F" ?
                weather.atmosphere.pressure : Math.Round(double.Parse(weather.atmosphere.pressure)).ToString());
            PressureUnit = weather.units.pressure;
            Visibility = weather.atmosphere.visibility;
            VisibilityUnit = weather.units.distance;

            // Add UI elements
            Forecasts = new ObservableCollection<ForecastItemView>();
            foreach (Forecast forecast in weather.forecasts)
            {
                ForecastItemView forecastView = new ForecastItemView(forecast);
                Forecasts.Add(forecastView);
            }
        }

        private void updateWeatherIcon(int weatherCode)
        {
            switch (weatherCode)
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

        private String updateLastBuildDate(Weather weather)
        {
            String date;

            // ex. "2016-08-22T04:53:07Z"
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime updateTime = DateTime.ParseExact(weather.lastBuildDate,
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", provider).ToLocalTime();

            if (updateTime.DayOfWeek == DateTime.Today.DayOfWeek)
            {
                date = "Updated at " + updateTime.ToString("t");
            }
            else
                date = "Updated on " + updateTime.ToString("ddd") + " " + updateTime.ToString("t");

            return date;
        }

        private void updateBg(Weather weather)
        {
            ImageBrush bg = new ImageBrush();
            bg.Stretch = Stretch.UniformToFill;
            bg.AlignmentX = AlignmentX.Right;

            // Apply background based on weather condition
            switch (int.Parse(weather.condition.code))
            {
                // Night
                case 31:
                case 33:
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/NightSky.jpg"));
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
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/RainySky.jpg"));
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
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/StormySky.jpg"));
                    break;
                // Dust
                case 19:
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/Dust.jpg"));
                    break;
                // Foggy / Haze
                case 20:
                case 21:
                case 22:
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/FoggySky.jpg"));
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
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/Snow.jpg"));
                    break;
                /* Ambigious weather conditions */
                // (Mostly) Cloudy
                case 28:
                case 26:
                case 27:
                    if (isNight(weather))
                        bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/MostlyCloudy-Night.jpg"));
                    else
                        bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/MostlyCloudy-Day.jpg"));
                    break;
                // Partly Cloudy
                case 44:
                case 29:
                case 30:
                    if (isNight(weather))
                        bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/PartlyCloudy-Night.jpg"));
                    else
                        bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/PartlyCloudy-Day.jpg"));
                    break;
            }

            if (bg.ImageSource == null)
            {
                // Set background based using sunset/rise times
                if (isNight(weather))
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/NightSky.jpg"));
                else
                    bg.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/DaySky.jpg"));
            }

            Background = bg;
        }

        private bool isNight(Weather weather)
        {
            // Determine whether its night using sunset/rise times
            if (DateTime.Now < DateTime.Parse(weather.astronomy.sunrise)
                    || DateTime.Now > DateTime.Parse(weather.astronomy.sunset))
                return true;
            else
                return false;
        }
    }
}
