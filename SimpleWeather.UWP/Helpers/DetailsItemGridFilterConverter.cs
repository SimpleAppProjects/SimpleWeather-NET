using SimpleWeather.Controls;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using FeatureSettings = SimpleWeather.UWP.Utils.FeatureSettings;

namespace SimpleWeather.UWP.Helpers
{
    public class DetailsItemGridFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is IEnumerable<DetailItemViewModel> detailItems
                ? detailItems.WhereNot(it => ((it.DetailsType == WeatherDetailsType.Sunrise || it.DetailsType == WeatherDetailsType.Sunset) && FeatureSettings.SunPhase) ||
                        ((it.DetailsType == WeatherDetailsType.Moonrise || it.DetailsType == WeatherDetailsType.Moonset || it.DetailsType == WeatherDetailsType.MoonPhase) && FeatureSettings.MoonPhase))
                : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
