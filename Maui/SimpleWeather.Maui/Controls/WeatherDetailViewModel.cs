using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SimpleWeather.Common.Controls;
using SimpleWeather.ComponentModel;

namespace SimpleWeather.Maui.Controls
{
    [Bindable(true)]
    public partial class WeatherDetailViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string date;
        [ObservableProperty]
        private string icon;
        [ObservableProperty]
        private string hiLo;
        [ObservableProperty]
        private string condition;
        [ObservableProperty]
        private string conditionLongDesc;
        [ObservableProperty]
        private ObservableCollection<DetailItemViewModel> extras;
        [ObservableProperty]
        private bool hasExtras;

        internal WeatherDetailViewModel()
        {
        }

        public void SetForecast(ForecastItemViewModel forecastViewModel)
        {
            Date = forecastViewModel.Date;
            Icon = forecastViewModel.WeatherIcon;
            HiLo = $"{forecastViewModel.HiTemp} / {forecastViewModel.LoTemp}";
            Condition = forecastViewModel.Condition;
            ConditionLongDesc = forecastViewModel.ConditionLong;
            Extras = new ObservableCollection<DetailItemViewModel>(forecastViewModel.DetailExtras.Values);

            HasExtras = Extras?.Count > 0 || !String.IsNullOrWhiteSpace(ConditionLongDesc);
        }

        public void SetForecast(HourlyForecastItemViewModel hrforecastViewModel)
        {
            Date = hrforecastViewModel.Date;
            Icon = hrforecastViewModel.WeatherIcon;
            HiLo = hrforecastViewModel.HiTemp;
            Condition = hrforecastViewModel.Condition;
            ConditionLongDesc = null;
            Extras = new ObservableCollection<DetailItemViewModel>(hrforecastViewModel.DetailExtras.Values);

            HasExtras = Extras?.Count > 0 || !String.IsNullOrWhiteSpace(ConditionLongDesc);
        }
    }
}
