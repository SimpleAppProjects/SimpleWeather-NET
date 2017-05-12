using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.Controls
{
    public class LocationPanelView
    {
        public string LocationName { get; set; }
        public string CurrTemp { get; set; }
        public string WeatherIcon { get; set; }
        public KeyValuePair<int, object> Pair { get; set; }

        // Background
        public Brush Background { get; set; }

        public bool ShowLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                isLoading = value;
                IsEnabled = !value;
            }
        }
        public bool IsEnabled { get; set; }
        private bool isLoading = false;

        public LocationPanelView()
        {
            ShowLoading = true;
        }

        #region Yahoo Weather
        public LocationPanelView(WeatherYahoo.Weather weather)
        {
            ShowLoading = true;
            setWeather(weather);
        }

        public void setWeather(WeatherYahoo.Weather weather)
        {
            // Update background
            Background = WeatherUtils.GetBackground(weather);

            LocationName = weather.location.description;
            CurrTemp = weather.condition.temp + "º";
            WeatherIcon = WeatherUtils.GetWeatherIcon(int.Parse(weather.condition.code));

            ShowLoading = false;
        }
        #endregion

        #region WeatherUnderground
        public LocationPanelView(WeatherUnderground.Weather weather)
        {
            ShowLoading = true;
            setWeather(weather);
        }

        public void setWeather(WeatherUnderground.Weather weather)
        {
            // Update background
            Background = WeatherUtils.GetBackground(weather);

            LocationName = weather.location.full_name;
            CurrTemp = (Settings.Unit == "F" ?
                Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º";
            WeatherIcon = WeatherUtils.GetWeatherIcon(weather.condition.icon_url);

            ShowLoading = false;
        }
        #endregion
    }
}
