using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Preferences;
using SimpleWeather.Weather_API.WeatherData;
using System.Collections.Generic;

namespace SimpleWeather.Common.Controls
{
    public class BaseForecastItemViewModel
    {
        protected readonly WeatherProviderManager wm = Ioc.Default.GetService<WeatherProviderManager>();
        protected readonly SettingsManager SettingsMgr = Ioc.Default.GetService<SettingsManager>();

        public string WeatherIcon { get; set; }
        public string Date { get; set; }
        public string ShortDate { get; set; }
        public string LongDate { get; set; }
        public string Condition { get; set; }
        public string HiTemp { get; set; }
        public string WindDir { get; set; }
        public int WindDirection { get; set; }
        public string WindSpeed { get; set; }

        public IDictionary<WeatherDetailsType, DetailItemViewModel> DetailExtras { get; private set; }

        public BaseForecastItemViewModel()
        {
            int capacity = System.Enum.GetNames(typeof(WeatherDetailsType)).Length;
            DetailExtras = new DetailsMap<WeatherDetailsType, DetailItemViewModel>(capacity);
        }
    }
}