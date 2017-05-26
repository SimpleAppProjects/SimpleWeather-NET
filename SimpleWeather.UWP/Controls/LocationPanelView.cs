using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.UWP.Controls
{
    public class LocationPanelView
    {
        public string LocationName { get; set; }
        public string CurrTemp { get; set; }
        public string WeatherIcon { get; set; }
        public KeyValuePair<int, string> Pair { get; set; }

        // Background
        public ImageBrush Background { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsLoading { get; set; }

        public LocationPanelView()
        {
            ShowLoading(true);
        }

        public LocationPanelView(Weather weather)
        {
            ShowLoading(true);
            setWeather(weather);
        }

        public void ShowLoading(bool show)
        {
            IsEnabled = show ? false : true;
            IsLoading = show ? true : false;
        }

        public void setWeather(Weather weather)
        {
            // Update background
            if (Background == null)
            {
                Background = new ImageBrush();
                Background.Stretch = Stretch.UniformToFill;
                Background.AlignmentX = AlignmentX.Center;
            }
            WeatherUtils.SetBackground(Background, weather);

            LocationName = weather.location.name;
            CurrTemp = (Settings.Unit == "F" ?
                Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º";
            WeatherIcon = WeatherUtils.GetWeatherIcon(weather.condition.icon);

            ShowLoading(false);
        }
    }
}
