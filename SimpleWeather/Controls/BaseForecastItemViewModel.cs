using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Collections.Generic;

namespace SimpleWeather.Controls
{
    public class BaseForecastItemViewModel
    {
        protected WeatherManager wm;

        public string WeatherIcon { get; set; }
        public string Date { get; set; }
        public string ShortDate { get; set; }
        public string Condition { get; set; }
        public string HiTemp { get; set; }
        public string PoP { get; set; }
        public string WindDir { get; set; }
        public int WindDirection { get; set; }
        public string WindSpeed { get; set; }

        public List<DetailItemViewModel> DetailExtras { get; set; }

        public BaseForecastItemViewModel()
        {
            wm = WeatherManager.GetInstance();
            DetailExtras = new List<DetailItemViewModel>();
        }
    }
}
