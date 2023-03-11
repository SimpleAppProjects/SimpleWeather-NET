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
        private string condition;
        [ObservableProperty]
        private string conditionLongDesc;
        [ObservableProperty]
        private string poPChance;
        [ObservableProperty]
        private string cloudiness;
        [ObservableProperty]
        private string windSpeed;
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
            Condition = String.Format(CultureInfo.InvariantCulture, "{0} | {1} - {2}",
                forecastViewModel.HiTemp, forecastViewModel.LoTemp, forecastViewModel.Condition);
            ConditionLongDesc = forecastViewModel.ConditionLong;
            Extras ??= new ObservableCollection<DetailItemViewModel>();

            PoPChance = Cloudiness = WindSpeed = null;

            Extras.Clear();
            foreach (DetailItemViewModel detailItem in forecastViewModel.DetailExtras.Values)
            {
                if (detailItem.DetailsType == WeatherDetailsType.PoPChance)
                {
                    PoPChance = detailItem.Value;
                    continue;
                }
                else if (detailItem.DetailsType == WeatherDetailsType.PoPCloudiness)
                {
                    Cloudiness = detailItem.Value;
                    continue;
                }
                else if (detailItem.DetailsType == WeatherDetailsType.WindSpeed)
                {
                    WindSpeed = detailItem.Value;
                    continue;
                }

                Extras.Add(detailItem);
            }

            HasExtras = Extras?.Count > 0 || !String.IsNullOrWhiteSpace(ConditionLongDesc);
        }

        public void SetForecast(HourlyForecastItemViewModel hrforecastViewModel)
        {
            Date = hrforecastViewModel.Date;
            Icon = hrforecastViewModel.WeatherIcon;
            Condition = String.Format(CultureInfo.InvariantCulture, "{0} - {1}",
                hrforecastViewModel.HiTemp, hrforecastViewModel.Condition);
            Extras ??= new ObservableCollection<DetailItemViewModel>();

            PoPChance = Cloudiness = WindSpeed = null;

            Extras.Clear();
            foreach (DetailItemViewModel detailItem in hrforecastViewModel.DetailExtras.Values)
            {
                if (detailItem.DetailsType == WeatherDetailsType.PoPChance)
                {
                    PoPChance = detailItem.Value;
                    continue;
                }
                else if (detailItem.DetailsType == WeatherDetailsType.PoPCloudiness)
                {
                    Cloudiness = detailItem.Value;
                    continue;
                }
                else if (detailItem.DetailsType == WeatherDetailsType.WindSpeed)
                {
                    WindSpeed = detailItem.Value;
                    continue;
                }

                Extras.Add(detailItem);
            }

            HasExtras = Extras?.Count > 0 || !String.IsNullOrWhiteSpace(ConditionLongDesc);
        }
    }
}
