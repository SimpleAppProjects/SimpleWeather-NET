using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SimpleWeather.Common.Controls;
using SimpleWeather.ComponentModel;

namespace SimpleWeather.Maui.Controls
{
    [Bindable(true)]
    public partial class WeatherDetailExtraViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string date;
        [ObservableProperty]
        private string icon;
        [ObservableProperty]
        private string loTemp;
        [ObservableProperty]
        private string hiTemp;
        [ObservableProperty]
        private string condition;
        [ObservableProperty]
        private string conditionLongDesc;
        [ObservableProperty]
        private ObservableCollection<DetailItemViewModel> extras;
        [ObservableProperty]
        private bool hasExtras;

        internal WeatherDetailExtraViewModel()
        {
        }

        public void SetForecast(ForecastItemViewModel forecastViewModel)
        {
            Date = forecastViewModel.LongDate;
            Icon = forecastViewModel.WeatherIcon;
            HiTemp = forecastViewModel.HiTemp;
            LoTemp = forecastViewModel.LoTemp;
            Condition = forecastViewModel.Condition;
            ConditionLongDesc = forecastViewModel.ConditionLong;
            Extras ??= new ObservableCollection<DetailItemViewModel>();

            Extras.Clear();
            foreach (DetailItemViewModel detailItem in forecastViewModel.DetailExtras.Values)
            {
                Extras.Add(detailItem);
            }

            HasExtras = Extras?.Count > 0 || !String.IsNullOrWhiteSpace(ConditionLongDesc);
        }

        public void SetForecast(HourlyForecastItemViewModel hrforecastViewModel)
        {
            Date = hrforecastViewModel.Date;
            Icon = hrforecastViewModel.WeatherIcon;
            HiTemp = hrforecastViewModel.HiTemp;
            LoTemp = null;
            Condition = hrforecastViewModel.Condition;
            ConditionLongDesc = null;
            Extras ??= new ObservableCollection<DetailItemViewModel>();

            Extras.Clear();
            foreach (DetailItemViewModel detailItem in hrforecastViewModel.DetailExtras.Values)
            {
                Extras.Add(detailItem);
            }

            HasExtras = Extras?.Count > 0 || !String.IsNullOrWhiteSpace(ConditionLongDesc);
        }
    }
}
