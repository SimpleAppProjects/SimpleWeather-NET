using SimpleWeather.WeatherData;
using System.Collections.Generic;

namespace SimpleWeather.Controls
{
    public class BaseForecastItemViewModel
    {
        protected readonly WeatherManager wm = WeatherManager.GetInstance();

        public string WeatherIcon { get; set; }
        public string Date { get; set; }
        public string ShortDate { get; set; }
        public string Condition { get; set; }
        public string HiTemp { get; set; }
        public string WindDir { get; set; }
        public int WindDirection { get; set; }
        public string WindSpeed { get; set; }

        public IDictionary<WeatherDetailsType, DetailItemViewModel> DetailExtras { get; private set; }

        public BaseForecastItemViewModel()
        {
            int capacity = System.Enum.GetNames(typeof(WeatherDetailsType)).Length;
            DetailExtras = new Dictionary<WeatherDetailsType, DetailItemViewModel>(capacity);
        }
    }
}